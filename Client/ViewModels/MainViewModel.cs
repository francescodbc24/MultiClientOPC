using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Client.Models;
using Client.Core.Interfaces;
using Microsoft.Win32;
using Client.Core.Services;
using Client.Core.Services.Enums;
using Client.Models.Common;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Client.Core.Exceptions;
using Client.Windows;
using Client.Messaging;
using System.Windows.Threading;
using log4net;
using System.Reflection;
using Client.Core.Models;

namespace Client.ViewModels
{
	public class MainViewModel : BaseModel
	{
		private IClient Client;
		private IFileService _fileManager;
		private ILog _Logger= LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		

		public MainViewModel()
		{

			ListOfItemsActive = new ObservableCollection<IItemView>();

			//When the file type change update the fileManager
			this.PropertyChanged += (sender, args) =>
			{
				if(args.PropertyName== nameof(FileType))
				{
					_fileManager = FileContainer.Instance.Resolve(fileType);
				}
			};

			//When close the Main Windows Disconnect the servers
			Messenger.Default.Register<CloseWindow>(this, async m =>
			{
				if (Client != null && Client.isConnect())
					await ClickButtonConnect(null);

			});
		}


		private ObservableCollection<TreeViewItem> _treeViewItems;

		public ObservableCollection<TreeViewItem> TreeViewItems
		{
			get { return _treeViewItems; }
			set { _treeViewItems = value; OnPropertyChange(); }
		}

		#region UA


		#endregion


		#region ModBus
		private List<ClientType> _listClients;

			public List<ClientType> ListClients
			{
				get {
					var list = new List<ClientType>();
					foreach (ClientType item in Enum.GetValues(typeof(ClientType)))
						list.Add(item);
					return list;
					}
				private set { _listClients = value; OnPropertyChange(); }
			}

			private ClientType _selectedClient;

			public ClientType SelectedClient
			{
				get { return _selectedClient; }
				set { _selectedClient = value; OnPropertyChange(); }
			}

			private ItemType _modBusType;

			public ItemType ModBusType
			{
				get { return _modBusType; }
				set { _modBusType = value; OnPropertyChange(); }
			}

			private string _modbusAddress;
			public string ModbusAddress
			{
				get { return _modbusAddress; }
				set { _modbusAddress = value; OnPropertyChange(); }
			}

			/*ADD Item Modbus Click.*/
			private RelayCommand _addModbusCommand;
			public ICommand AddModbusCommand
			{
				get
				{
					if (_addModbusCommand == null)
					{
						_addModbusCommand = new RelayCommand(AddModbus);
					}
					return _addModbusCommand;
				}
			}
			public void AddModbus(object parameter)
			{
			try
			{
				if (parameter == null) { Messenger.Default.Send(new Messaging.ShowMessage("You have to insert the address.")); return; }
				if (ModBusType == ItemType.None) { Messenger.Default.Send(new Messaging.ShowMessage("You have to select a type.")); return; }
				IItem Item = Client.CreateItem(parameter.ToString(), ModBusType);
				if (Item != null)
				{
					ListOfItemsActive.Add(new ItemViewModel(Item));				
				}
				else
				{
					Messenger.Default.Send(new Messaging.ShowMessage("The item is already in the list."));
				}
			}
			catch (Exception ex)
			{
				Messenger.Default.Send(new Messaging.ShowMessage(ex.Message));
				_Logger.Error("Error trying to adding an item \n "+ ex.Message );
			}
				

			}
		
		/*ADD RADIO button event.*/
		private RelayCommand _modBusTypeRadioButtonCommand;
		public ICommand ModBusTypeRadioButtonCommand
		{
			get
			{
				if (_modBusTypeRadioButtonCommand == null)
				{
					_modBusTypeRadioButtonCommand = new RelayCommand(ModBusTypeRadioButton);
				}
				return _modBusTypeRadioButtonCommand;
			}
		}
		public void ModBusTypeRadioButton(object parameter)
		{
			if (parameter == null) return;
			switch (parameter.ToString())
			{
				case nameof(ItemType.Coils):
					ModBusType = ItemType.Coils;
					break;
				case nameof(ItemType.DiscreteInputs):
					ModBusType = ItemType.DiscreteInputs;
					break;
				case nameof(ItemType.HoldingRegister):
					ModBusType = ItemType.HoldingRegister;
					break;
				case nameof(ItemType.InputRegister):
					ModBusType = ItemType.InputRegister;
					break;
				
			}
		}


		


		#endregion


	
		#region OPCDA

		private ObservableCollection<BrowserItem> _browseList;		

		public ObservableCollection<BrowserItem> BrowseList
		{
			get { return _browseList; }
			set { _browseList = value; OnPropertyChange(); }
		}

		private List<string> _listServers;

		public List<string> ListServers
		{
			get { return _listServers = OPCDAClient.getAvaliableServers(); }
			set { _listServers = value; OnPropertyChange(); }
		}

		private string _serverselected;

		public string ServerSelected
		{
			get { return _serverselected; }
			set { _serverselected = value; OnPropertyChange(); }
		}

		private List<BrowserItem> ListItemsInServer;


		private Dictionary<string, List<BrowserItem>> _TreeListItems;
		public Dictionary<string, List<BrowserItem>> TreeListItems
		{
			get { return _TreeListItems; }
			set
			{
				_TreeListItems = value;
				OnPropertyChange();
			}
		}
		private string _TextTable;
		public string TextTable {
			get
			{
				return _TextTable;
			}
			set
			{
				_TextTable = value;
				OnPropertyChange();
			}
}
		private ObservableCollection<IItemView> _listOfItemsActive;
		public ObservableCollection<IItemView> ListOfItemsActive
		{
			get {

				return _listOfItemsActive;
			}
			set {
				_listOfItemsActive = value;
				OnPropertyChange();
			}
		}


		/*ADD Item Button Double Click.*/
		private RelayCommand _DoubleClick;
		public ICommand DoubleClick
		{
			get
			{
				if (_DoubleClick == null)
				{
					_DoubleClick = new RelayCommand(AddItem);
				}
				return _DoubleClick;
			}
		}
		public void AddItem(object parameter)
		{
			try
			{
				BrowserItem item = parameter as BrowserItem;
				//if(!(parameter is string)) { return; }
				//IItem oPCItem = Server.CreateItem(parameter.ToString());
				IItem oPCItem = Client.CreateItem(item.ItemID);
				if (oPCItem != null)
				{
					ListOfItemsActive.Add(new ItemViewModel(oPCItem));
				}
				else
				{
					Messenger.Default.Send(new Messaging.ShowMessage("The item is already in the list."));
				}
			}
			catch (Exception ex)
			{
				_Logger.Error("Occurred an exception trying to added an item \n"+ex.Message +"\n" +ex.StackTrace);			
				Messenger.Default.Send(new Messaging.ShowMessage(ex.Message));
			}

		}
		
		
		/* FIND ITEMS Browse ON TEXT CHANGE */
		private RelayCommand _TextChangeFilterItemsCommand;
		public ICommand TextChangeFilterItemsCommand
		{
			get
			{
				if (_TextChangeFilterItemsCommand == null)
				{
					_TextChangeFilterItemsCommand = new RelayCommand(FilterItems);
				}
				return _TextChangeFilterItemsCommand;
			}
		}
		private void FilterItems(object parameter)
		{

			//TODO
			TreeListItems =  OrderItems(ListItemsInServer
													.Where(
		  												x => x.Name.Contains(parameter.ToString())
														
															)
													.ToList());
		}
		#endregion



		#region Common

		private String _IpAddress;
		public String IpAddress
		{
			get
			{
				return _IpAddress;
			}
			set
			{
				_IpAddress = value;
				OnPropertyChange();
			}

		}
		/* Status Of Connection for change the name of the button */
		public String _TextButtonConect=StatusConnection.Connect;
		public String TextButtonConect {
			get {
				
					
					return _TextButtonConect;
				 }
				set
				{
					_TextButtonConect = value;
					OnPropertyChange();
		
				}
			}


		private int port;

		public int Port
		{
			get { return port; }
			set { port = value;OnPropertyChange(); }
		}


		/* variable isConnected for Enable Or Disabled elements in the form  */
		private bool _isConnected;
		public bool isConnected
		{
			get
			{
				return _isConnected;
			}
			set
			{
				_isConnected = value;
				OnPropertyChange();
			}
		}

		/* Button Connect */
		private RelayCommand _ConnectClick;
		public ICommand ConnectClick
		{
			get
			{
				if (_ConnectClick == null)
				{
					_ConnectClick = new RelayCommand(async (parameter)=> { await  ClickButtonConnect(parameter); },CanExecuteConnect );

				}
				return _ConnectClick;
			}
		}

		private bool CanExecuteConnect(object obj)
		{
			return !IsBusy;
		}

 		public async Task ClickButtonConnect(object parameter)
		{
			if(SelectedClient == ClientType.OPCDA && ServerSelected == null) { Messenger.Default.Send(new Messaging.ShowMessage("You have to select a ServerName"));/* MessageBox.Show("You have to select a ServerName");*/ return; }
			await Task.Run( async () =>
					{
						try
						{
						IsBusy = true;
						ConnectClick.CanExecute(null);
							if (Client == null)
							{

								Client = ClientContainer.Instance.Resolve(SelectedClient);
								Client.Ip = IpAddress;
								Client.Port = Port;//502;
								if(SelectedClient == ClientType.Modbus)
									Client.Name = "Modbus";
								if (SelectedClient == ClientType.OPCDA)
									Client.Name = ServerSelected;
								if (SelectedClient == ClientType.UA)
									Client.Name = "UA";
								await Client.ConnectAsync();
								TextButtonConect = StatusConnection.Disconnect;
								isConnected = true;
								BrowseList = new ObservableCollection<BrowserItem>(Client.BrowseItems());
								ListItemsInServer = Client.BrowseItems();
								//TreeListItems = SorterItems(ListItemsInServer);
								TreeListItems = OrderItems(ListItemsInServer);
								_Logger.Info("Connected to " +IpAddress + " Server "+ SelectedClient + " successful");
							}
							else
							{
								Client.Disconnect();								
								//TreeListItems = null;
								//This code need to run in the UI Thread so invoke a delegate to handle this job.
								Application.Current.Dispatcher.Invoke((System.Action)delegate
								{
									TreeListItems = new Dictionary<string, List<BrowserItem>>();
									ListOfItemsActive.Clear();
									BrowseList.Clear();
								});
								IpAddress = string.Empty;
								ModbusAddress = string.Empty;
								ModBusType = ItemType.None;
								TextButtonConect = StatusConnection.Connect;
								isConnected = false;
								Client = null;
							}
						}
						catch (Exception ex)
						{
							Client = null;
							TextButtonConect = StatusConnection.Connect;
							_Logger.Error("Occurred an error trying to Connect/Disconnect to the server:\n "+ ex.Message + "\n"+ex.StackTrace);
							Messenger.Default.Send(new Messaging.ShowMessage(ex.Message));
						}
						
							IsBusy = false;
					});
		}


		/*Button Remove Click*/
		private RelayCommand _buttonRemoveItem;
		public ICommand ButtonRemoveItem
		{
			get
			{
				if (_buttonRemoveItem == null)
				{
					_buttonRemoveItem = new RelayCommand(RemoveItem);
				}
				return _buttonRemoveItem;
			}
		}

		private void RemoveItem(object parameter){
			try
			{
					IItemView item = (IItemView)parameter;
					if (item != null)
					{				
								Client.RemoveItem(item.ItemID,item.ItemType);
								ListOfItemsActive.Remove(item);									
					}
					else
					{				
						Messenger.Default.Send(new Messaging.ShowMessage("You have to select an item",Enums.AlertType.Info));
					}
			}
			catch (Exception)
			{

				Messenger.Default.Send(new Messaging.ShowMessage("Error trying to remove an item", Enums.AlertType.Error));
			}
		}

		/* FIND ITEM ON TEXT CHANGE*/
		private RelayCommand _TxtChangeCommand;
		public ICommand TxtChangeCommand
		{
			get
			{
				if (_TxtChangeCommand == null)
				{
					_TxtChangeCommand = new RelayCommand(FindItem);
				}
				return _TxtChangeCommand;
			}
		}
		private void FindItem(object parameter){

			//TreeListItems = Server.GetListItems(parameter.ToString());
		}


		/* Button Remove All*/
		private RelayCommand _buttonRemoveAll;
		public ICommand ButtonRemoveAll
		{
			get
			{
				if (_buttonRemoveAll == null)
				{
					_buttonRemoveAll = new RelayCommand(RemoveAll);
				}
				return _buttonRemoveAll;
			}
		}
		private void RemoveAll(object obj)
		{			
					Client.RemoveAll();
					ListOfItemsActive.Clear();
					
		}

		/* FIND ITEM ACTIVE ON TEXT CHANGE*/
		private RelayCommand _TextChangeItemsActive;
		public ICommand TextChangeItemsActive
		{
			get
			{
				if (_TextChangeItemsActive == null)
				{
					_TextChangeItemsActive = new RelayCommand(FindItemAct);
				}
				return _TextChangeItemsActive;
			}
		}
		private void FindItemAct(object parameter){
			TextTable = parameter.ToString();
			ListOfItemsActive = new ObservableCollection<IItemView>(Client.Items.Where(x=> x.ItemID.Contains(parameter.ToString())).Select(y => new ItemViewModel(y)).ToList());
		}


		/* UPDATE ITEM*/
		private RelayCommand _UpdateElement;
		public ICommand UpdateElement
		{
			get
			{
				if (_UpdateElement == null)
				{
					_UpdateElement = new RelayCommand(UpdateOPC);
				}
				return _UpdateElement;
			}
		}
		private void UpdateOPC(object parameter)
		{

			try
			{
				IItemView item = (IItemView)parameter;
				WindowUpdate pad = new WindowUpdate(item);
				if (pad.ShowDialog() == true)
				{

					switch (SelectedClient)
					{
						case ClientType.Modbus:							
							if (item.ItemType == ItemType.HoldingRegister || item.ItemType == ItemType.Coils)
								Client.WriteItem(item.ItemID, pad.textBox.Text);
							break;
						case ClientType.OPCDA:
							Client.WriteItem(item.ItemID, pad.textBox.Text);
							break;

						case ClientType.UA:
							Client.WriteItem(item.ItemID, pad.textBox.Text);
							break;
						default:
							break;
					}					
				}
			}
			catch (Exception ex)
			{
				_Logger.Error("Error trying to updated item \n" + ex.Message + "\n" + ex.StackTrace);
				Messenger.Default.Send(new ShowMessage("Error trying to updated item",ex));
			}			
					
		}



		/* button SAVE in file */
		public RelayCommand _ButtonSave;
		public ICommand ButtonSave
		{
			get
			{
				if (_ButtonSave == null)
				{
					_ButtonSave = new RelayCommand(SaveItems);
				}
				return _ButtonSave;
			}
		}
		public void SaveItems(object parameter)
		{
			if (FileType == FileType.None) { Messenger.Default.Send(new Messaging.ShowMessage("Select a file type",Enums.AlertType.Info)); return; }
			SaveFileDialog SaveFileDialog = new SaveFileDialog();
			switch (FileType)	
			{
				case FileType.XML:
					SaveFileDialog.Filter = "XML file (*.xml; *.xml) | *.xml;";
					break;
				case FileType.JSON:
					SaveFileDialog.Filter = "JSON file (*.json; *.json) | *.json;";
					break;
			}
			if (SaveFileDialog.ShowDialog() == true)
			{
				if (_fileManager != null)
				{
					if (_fileManager.SaveItems(Client.Items, SaveFileDialog.FileName))
						Messenger.Default.Send(new Messaging.ShowMessage("Items Saved",Enums.AlertType.Success));

				}
			}
		}

		/* button LOAD from file  */
		public RelayCommand _ButtonLoad;
		public ICommand ButtonLoad
		{
			get
			{
				if (_ButtonLoad == null)
				{
					_ButtonLoad = new RelayCommand(LoadItems);
				}
				return _ButtonLoad;
			}
		}
		public void LoadItems(object parameter)
		{
			if (FileType == FileType.None) { Messenger.Default.Send(new Messaging.ShowMessage("Select a file type",Enums.AlertType.Info)); return; }
			OpenFileDialog OpenFileDialog = new OpenFileDialog();
			switch (FileType)
			{
				case FileType.XML:
					OpenFileDialog.Filter = "XML file (*.xml; *.xml) | *.xml;";
					break;
				case FileType.JSON:
					OpenFileDialog.Filter = "JSON file (*.json; *.json) | *.json;";
					break;
			}
			if (OpenFileDialog.ShowDialog() == true)
			{
				try
				{
					if (_fileManager != null)
					{
						var items = _fileManager.ReadItems(OpenFileDialog.FileName);

					foreach (var item in items)
					{
						IItem itemCreated = Client.CreateItem(item.ItemID,item.ItemType);
						if (itemCreated != null)
							ListOfItemsActive.Add(new ItemViewModel(itemCreated));					
					}
					}
				}
				catch (Exception ex)
				{
					_Logger.Error("Occurred an exception trying to load the items \n" + ex.Message + "\n" + ex.StackTrace);
					Messenger.Default.Send(new Messaging.ShowMessage(ex.Message));
					
				}
			}
		}
		
		/* File Type change*/

		private FileType fileType;

		public FileType FileType
		{
			get { return fileType; }
			set { fileType = value; OnPropertyChange(); }
		}

		private RelayCommand _fileTypeCommand;

		public ICommand FileTypeCommand
		{
			get
			{
				if (_fileTypeCommand == null)
				{
					_fileTypeCommand = new RelayCommand(FileTypeChange);
				}
				return _fileTypeCommand;
			}

		}

		public void FileTypeChange(object parameter)
		{
			if (parameter == null) return;
			switch (parameter.ToString())
			{
				case nameof(FileType.XML):
					FileType = FileType.XML;
					break;
				case nameof(FileType.JSON):
					FileType = FileType.JSON;
					break;
				default:
					FileType = FileType.None;
					break;
			}
		}

		private Dictionary<string,List<string>> SorterItems(List<string> items)
		{
				var dic = new Dictionary<string, List<string>>();
			if(items!= null && items.Count() > 0)
			{
				foreach (var item in items)
				{
					string path = item;
					string parent="";
					do
					{
						var pos=path.IndexOf(".");
						parent = parent + path.Substring(0, pos+1);
						path =path.Substring(pos+1);
					
					} while (path.Contains("."));

					if (dic.ContainsKey(parent))
					{
						dic[parent].Add(item);
					}
					else
					{
						dic.Add(parent, new List<string>());
							dic[parent].Add(item);
					}
				}
			}
			return dic;
		}
		#endregion

		private Dictionary<string,List<BrowserItem>> OrderItems(List<BrowserItem> items)
		{			

			var dic = new Dictionary<string, List<BrowserItem>>();
			if (items != null && items.Count() > 0)
			{
				foreach (var item in items)
				{

					if (item.Path == null) { continue; }
					if (dic.ContainsKey(item.Path))
					{
						dic[item.Path].Add(item);
					}
					else
					{
						dic.Add(item.Path, new List<BrowserItem>());
						dic[item.Path].Add(item);
					}
				}
			}
			return dic;
		}
	

	}
	/* Status of connection for change the name of the button */
	public static class StatusConnection
	{
		public const String Connect= "Connect";
		public const String Disconnect = "Disconnect";	
	}

	public class TreeViewItem : BrowserItem
	{
		public TreeViewItem()
		{
			this.Items = new ObservableCollection<TreeViewItem>();

		}
		public string Title { get; set; }

		public ObservableCollection<TreeViewItem> Items { get; set; }

	}

}
