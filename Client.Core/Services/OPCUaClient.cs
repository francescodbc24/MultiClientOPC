using Client.Core.Exceptions;
using Client.Core.Interfaces;
using Client.Core.Models;
using Client.Core.Services.Enums;
using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Core.Services
{
	public class OPCUaClient : IUAClient
	{

		Session session;
		Subscription subscription;
		Browser browser;

		private const int SESSION_TIMEOUT = 60000;
		private const int KEEP_ALIVE_INTERVAL = 2000;
		private const int PUBLISHING_INTERVAL = 100;
		private const int KEEP_ALIVE_COUNT = 32;     //Il KEEP_ALIVE_COUNT * PUBLISHING_INTERVAL dovrebbe essere più grande del KEEP_ALIVE_INTERVAL
		private const int LIFETIME_COUNT = 100; //valore minimo del server a seconda del PUBLISH INTERVAL

		public string Ip { get ; set ; }
		public int Port { get; set; }
		public string Name { get; set; }

		public IList<IItem> Items => items;
		private IList<IItem> items = new List<IItem>();

		private List<BrowserItem> itemsFound = new List<BrowserItem>();

		public OPCUaClient()
		{

		}

		#region Connection

		public async void Connect()
		{
			try
			{

			await ConnectAsync();
			}
			catch(Exception ex)
			{
				//throw ex;
			}

		}

	
		public async Task ConnectAsync()
		{
			try
			{
				if (Ip == null)
					throw new ArgumentNullException(nameof(Ip));
				if (Port == 0)
					throw new ArgumentNullException(nameof(Port));

				await CreateSession(new Uri($"opc.tcp://{Ip}:{Port}"));
				//await CreateSession(new Uri($"opc.tcp://desktop-dtk52ou:51210/UA/SampleServer"));
				//await CreateSession(new Uri($"opc.tcp://127.0.0.1:53530/OPCUA/SimulationServer"));
				CreateSubscription();

				if (session != null && subscription != null)
				{
					session.AddSubscription(subscription);
					subscription.Create();

				}

			}catch(Exception ex)
			{
				throw ex;
			}
			
		}

		public bool isConnect()
		{
			return false;
			//TODO not implement
			//return (subscription == ServerState.Running);
		}

		public void Disconnect()
		{
			try
			{
				if (session != null && session.Connected)
				{
					//this.session.KeepAlive -= SessionOnKeepAlive;
					this.session.Close();
					this.session.Dispose();
				}

				session = null;
				subscription.Dispose();
				browser = null;
				items.Clear();
			}
			catch
			{ }
		}

		public async Task CreateSession(Uri uri)
		{
			var appConfig = new ApplicationConfiguration();
			appConfig.ApplicationName = "Client";
			appConfig.ApplicationType = ApplicationType.Client;
			appConfig.SecurityConfiguration.ApplicationCertificate = new CertificateIdentifier();
			appConfig.ClientConfiguration = new Opc.Ua.ClientConfiguration();
			await appConfig.Validate(ApplicationType.Client);

			var discoveryClient = DiscoveryClient.Create(uri, null);

			var endPoints = discoveryClient.GetEndpoints(null);

			EndpointDescription endPoint = null;

			foreach (EndpointDescription endpointDescription in endPoints)
			{
				if (endpointDescription.SecurityMode == MessageSecurityMode.None)
				{
					endPoint = endpointDescription;
					break;
				}
			}

			endPoint.EndpointUrl = uri.ToString();

			var sc = new StringCollection();
			sc.Add(uri.ToString());
			endPoint.Server.DiscoveryUrls = sc;

			var confEndPoint = new ConfiguredEndpoint(null, endPoint);

			session = await Session.Create(appConfig, confEndPoint, true, "", SESSION_TIMEOUT, null, null);

			session.KeepAliveInterval = KEEP_ALIVE_INTERVAL;

			//session.KeepAlive += SessionOnKeepAlive;
			//session.Notification += OnNotification;
		}

		public void CreateSubscription()
		{
			subscription = new Subscription(session.DefaultSubscription);
			subscription.PublishingInterval = PUBLISHING_INTERVAL;
			subscription.KeepAliveCount = KEEP_ALIVE_COUNT;
			subscription.LifetimeCount = LIFETIME_COUNT;
			subscription.MinLifetimeInterval = 0;
		}

		#endregion

		#region Browsing

		public List<BrowserItem> BrowseItems()
		{
			if (itemsFound.Count == 0)
				Browse();

			return itemsFound;
		}

		private void Browse()
		{
			browser = new Browser(session);
			browser.ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences;

			INode node = browser.Session.NodeCache.Find(Objects.ObjectsFolder);

			RecursiveBrowse((NodeId)node.NodeId);
		}

		private void RecursiveBrowse(NodeId nodeId, string root = "")
		{
			ReferenceDescriptionCollection refs = browser.Browse(nodeId);
			HashSet<ReferenceDescription> refDesList = new HashSet<ReferenceDescription>(refs.Where(x => x.NodeClass == NodeClass.Variable));

				itemsFound.AddRange(refDesList.Select(x => new BrowserItem { ItemID= x.NodeId.ToString(), Name=x.DisplayName.Text,Path=root } ));

			refs.RemoveAll(x => refDesList.Contains(x));

			foreach (ReferenceDescription referenceDescription in refs)
			{

				if (referenceDescription.BrowseName.NamespaceIndex == 0)
				{
					continue;
				}
				
					RecursiveBrowse((NodeId)referenceDescription.NodeId,root + "." +referenceDescription.DisplayName.Text);
				
			}
		}

		#endregion

		#region Crud

		public IItem CreateItem(string itemName, ItemType type = ItemType.None)
		{			
			var ItemExist = Items.FirstOrDefault(x => x.ItemID == itemName);
			if (ItemExist == null)
			{
				OPCUaItem newUAItem = new OPCUaItem(itemName);				
				Items.Add(newUAItem);						
				subscription.AddItem(newUAItem.MonitorItem);
				subscription.ApplyChanges();
				return newUAItem;
			}
			//TODO Return Exceptioons
			return null;

		}
		public void RemoveItem(string ItemID, ItemType type = ItemType.None)
		{					
				try
				{
				var item= items.SingleOrDefault(x => x.ItemID == ItemID);
					if (item != null)
					{					
						subscription.RemoveItem((item as OPCUaItem).MonitorItem);
						subscription.ApplyChanges();
						Items.Remove(item);
					}
				}
				catch
				{ }
		}
		public void WriteItem(string ItemID, object value)
		{
			var item = items.SingleOrDefault(x => x.ItemID == ItemID);
			if (item == null)
				throw new ServerException("The item don't exits in the current subscription");
			item.Write(value);
		}
		public object ReadItem(string ItemID)
		{
			var item = items.SingleOrDefault(x => x.ItemID == ItemID);
			if (item == null)
				throw new ServerException("The item don't exits in the current subscription");
			item.Read();
			return item.Value;
		}
		public void RemoveAll()
		{
			foreach (var item in items.ToList())
			{
				RemoveItem(item.ItemID);
			}
		}
		
		#endregion

		
		List<BrowserItem> IClient.BrowseItems()
		{
			var list = BrowseItems();
			return list;
		}

	}
}
