using Client.Core.Exceptions;
using Client.Core.Interfaces;
using Client.Core.Services.Enums;
using Client.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Da;
using TitaniumAS.Opc.Client.Da.Browsing;

namespace Client.Core.Services
{
	public class OPCDAClient : IClient
	{

		public OPCDAClient() {}
		public OPCDAClient(string name)
		{
			this.Name = name;
		}
		public OPCDAClient(string name,string ip):this(name)
		{
			this.Ip = ip;
		}

		private OpcDaGroup group;
		private OpcDaServer Server;
		private string _ip= "127.0.0.1"; //Default Localhost
		private string _name;
		private IList<IItem> _items=new List<IItem>();

		public string Ip { get => _ip; set => _ip = value; }
		public int Port { get; set; }
		public string Name { get => _name; set => _name = value; }
		public IList<IItem> Items { get => _items; }


		#region Connection
		public void Connect()
		{
			if (String.IsNullOrEmpty(this.Name))
				throw new ArgumentNullException(nameof(this.Name));
			Uri url = UrlBuilder.Build(this.Name);
			Server = new OpcDaServer(url);

			// Connect to the server first.
			Server.Connect();			
			group = Server.AddGroup("MyGroup");
			group.UpdateRate = TimeSpan.FromMilliseconds(100); // ValuesChanged won't be triggered if zero
			group.ValuesChanged += Group_ValuesChanged;
			group.IsActive = true;

		}		

		public async Task ConnectAsync()
		{
			await Task.Run(() => Connect());
		}


		public void Disconnect()
		{
			this.Items.Clear();
			Server.Disconnect();
			Server.Dispose();
		}
		public bool isConnect()
		{
			if (Server != null)
				return Server.IsConnected;

			return false;
		}
		#endregion

		#region Events
		private void Group_ValuesChanged(object sender, OpcDaItemValuesChangedEventArgs e)
		{
			foreach (var item in e.Values)
			{
				//Find the item changed and raise the event
				var itemResult = Items.SingleOrDefault(x => x.ItemID == item.Item.ItemId);
					if(itemResult != null)
				{
					itemResult.OnUpdated(item.Value);
				}
			}
		
		}
		
		#endregion
		
		#region Create/Remove Operations

		public IItem CreateItem(string itemName, ItemType type = ItemType.None)
		{
			if (String.IsNullOrEmpty(itemName))
				throw new ArgumentNullException(nameof(itemName));

			if (group.Items.SingleOrDefault(x => x.ItemId == itemName) != null)
				throw new ServerException("The item exist.");

			var definition = new OpcDaItemDefinition
			{
				ItemId = itemName,
				IsActive = true
			};
			OpcDaItemDefinition[] definitions = { definition };
			OpcDaItemResult[] results = group.AddItems(definitions);
			var item = new OPCDAItem(results[0]);
			Items.Add(item);
			return item;

		}
		public void WriteItem(string ItemID, object value)
		{
			if (string.IsNullOrEmpty(ItemID))
				throw new ArgumentNullException(nameof(ItemID));
			if (value == null)
				throw new ArgumentNullException(nameof(value));
			var resultGroup = group.Items.Where(x => x.ItemId == ItemID);
			group.Write(resultGroup.ToList(), new object[] { value});

		}
		public object ReadItem(string ItemID)
		{
			if (string.IsNullOrEmpty(ItemID))
				throw new ArgumentNullException(nameof(ItemID));
			var resultGroup = group.Items.Where(x => x.ItemId == ItemID);
			var result = group.Read(resultGroup.ToList(), OpcDaDataSource.Cache);
			return result[0].Value;
		}
		public void RemoveItem(string ItemID, ItemType type = ItemType.None)
		{
			var resultGroup = group.Items.Where(x => x.ItemId == ItemID);
			if(resultGroup != null)
				group.RemoveItems(resultGroup.ToList());

			var resultitems = Items.SingleOrDefault(x => x.ItemID == ItemID);
			if (resultitems != null)
				Items.Remove(resultitems);

		}
		public void RemoveAll()
		{
			foreach (var item in Items.ToList())
			{
				RemoveItem(item.ItemID);
			}
		}

		#endregion
	

		/* new functions */
		public static List<String> getAvaliableServers(string host= null)
		{
			List<String> ListOfServers = new List<string>(); 
			var x = new OpcServerEnumeratorAuto();
			var ServersResult = x.Enumerate("127.0.0.1", OpcServerCategory.OpcDaServer20, OpcServerCategory.OpcDaServer10, OpcServerCategory.OpcDaServer30);
			foreach (var server in ServersResult)
			{
				ListOfServers.Add(server.ProgId);
			}
			return ListOfServers;
		}
		private List<string> ListItems = new List<string>();
		private List<BrowserItem> ListItems2 = new List<BrowserItem>();
		private List<BrowserItem> BrowseChildren(IOpcDaBrowser browser, string itemId = null, int indent = 0)
		{
			OpcDaBrowseElement[] elements = browser.GetElements(itemId);
			// Output elements.
			foreach (OpcDaBrowseElement element in elements)
			{
				if (element.IsItem)
				{
					ListItems.Add(element.ItemId);
					ListItems2.Add(new BrowserItem
					{
						ItemID = element.ItemId,
						Name = element.Name,
						Path = element.ItemId.Replace(element.Name, "")
					});
				}
				// Skip elements without children.
				if (!element.HasChildren)
					continue;
				// Output children of the element.
				BrowseChildren(browser, element.ItemId, indent + 2);
			}

			return ListItems2;
		}

		public List<BrowserItem> BrowseItems()
		{
			TitaniumAS.Opc.Client.OpcConfiguration.BatchSize = int.MaxValue;
			var browser = new OpcDaBrowserAuto(Server);
			return BrowseChildren(browser);
		}

		

	}
}
