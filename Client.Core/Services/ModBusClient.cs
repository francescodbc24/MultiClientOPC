using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Client.Core.Exceptions;
using Client.Core.Interfaces;
using Client.Core.Models;
using Client.Core.Services.Enums;
using EasyModbus;
namespace Client.Core.Services
{
	/// <summary>
	/// Client OPC ModbusServer class
	/// </summary>
	public class ModBusClient : IModbusClient, IDisposable
	{

		public ModBusClient()
		{			
		}		
		#region Properties

		private Timer timer = new Timer();

		public string Name { get; set; }
		public ModbusClient modbusClient { get; set; }
		public IList<IItem> Items
		{
			get => _items;
			private set => _items = value;
		}
		IList<IItem> _items = new List<IItem>();

		public int Port
		{
			get { return _port; }
			set { _port = value; }
		}
		private int _port;// = 502;
	
		public string Ip
		{
			get { return _ip; }
			set { _ip = value; }
		}
		private string _ip; //= "190.201.100.63";


		#endregion

		#region Connection

		/// <summary>
		/// Connect to the ModbusServer, throw an exception if the ip/port is incorrect
		/// </summary>
			public void Connect()
			{
				if (Ip == null)
					throw new ArgumentNullException(nameof(Ip));
				if (Port == 0)
					throw new ArgumentNullException(nameof(Port));

			try
			{
				modbusClient = new ModbusClient(_ip, _port);
				modbusClient.ConnectionTimeout = 5000;
				modbusClient.Connect();
				modbusClient.ReceiveDataChanged += ModbusClient_ReceiveDataChanged;
				modbusClient.SendDataChanged += ModbusClient_SendDataChanged;
				//modbusClient.LogFileFilename = "C:/HMI-WPF/ClientOPC/ClientOPC/Logs/log-modbus.txt";
				Suscribe();
			}
			catch (EasyModbus.Exceptions.ConnectionException ex)
			{
				throw new ServerException("Error trying to connect to the server, check ip or port", ex);
			}				
			}
	

		public async Task ConnectAsync()
		{
			await Task.Run(() => Connect());
		}

	

		/// <summary>
		/// Disconnect to the ModbusServer
		/// </summary>
		public void Disconnect()
			{
			try
			{
				this.UnSuscribe();
				this.modbusClient.Disconnect();
				this.modbusClient = null;
				this.Items.Clear();

			}
			catch (Exception ex)
			{

				throw new Exception("Error trying to disconnect server...",ex);
			}
		}

		/// <summary>
		/// Check if the server is connected
		/// </summary>
		/// <returns> returns true if server is connected to the server, otherwise false</returns>
		public bool isConnect()
			{
				if(this.modbusClient != null)
					return this.modbusClient.Connected;

			return false;
			}
		
		#endregion

		#region Subscription
		/// <summary>
		/// Start Suscription to the server
		/// </summary>
		/// <param name="Interval">Interval time of refresh data from the PLC</param>
			public void Suscribe(int Interval = 500)
			{
				if (timer != null)
				{
					timer.Interval = Interval;
					timer.Elapsed -= Timer_Elapsed;
					timer.Elapsed += Timer_Elapsed;
					timer.Enabled = true;
				}
			}
		/// <summary>
		/// Stop suscription to the server
		/// </summary>
		public void UnSuscribe()
			{
				timer.Enabled = false;
			}

			private void Timer_Elapsed(object sender, ElapsedEventArgs e)
			{		
				timer.Enabled = false;
				UpdateItems();						
			}
		private async void UpdateItems()
		{
			try
			{
				if (Items != null)
				{

					foreach (var item in Items.ToList())
						await item.ReadAsync();

				}
				timer.Enabled = true;
			}
			catch(Exception ex)
			{
				Console.Write(ex.Message);
			}
		}

		#endregion

		#region CrudOperations
		/// <summary>
		/// Write a new value in the specific address.
		/// </summary>
		/// <param name="ItemID">ID of the item.</param>
		/// <param name="value">New value of the item.</param>
		public void WriteItem(string ItemID,object value)
		{
			if (ItemID == null)
				throw new ArgumentNullException(ItemID);
			if (value == null)
				throw new ArgumentNullException(value.ToString());


			int.TryParse(ItemID, out int Address);

			if(Boolean.TryParse(value.ToString(),out bool result_bool))
			{
				bool.TryParse(value.ToString(), out bool valuebool);
				WriteCoil(Address, valuebool);
			}
			else if(int.TryParse(value.ToString(), out int result_int))
			{
				int.TryParse(value.ToString(), out int valueint);
				WriteRegister(Address, valueint);
			}
				
			

	
		}
		/// <summary>
		/// Create a new item in the server.
		/// </summary>
		/// <param name="ItemID">Id of the item to create</param>
		/// <param name="type">type of the item to create</param>
		/// <returns> return a new item or null if the element already exist.</returns>
		public IItem CreateItem(string ItemID,ItemType type)
		{
			//int.TryParse(ItemID, out int itemid);
			if(type == ItemType.None) { return null; }
			var ItemExist = Items.FirstOrDefault(x => x.ItemID == ItemID && x.ItemType==type);
			if (ItemExist == null)
			{
				var item = new ModBusItem(ItemID, type, this);
				//This operation "read" must to be update, i do  this read here first for know if the item is accessible.
				item.Read(); 
				item.Updated += Item_Updated;
				Items.Add(item);
				return item;
			}
			return null;
		}

		/// <summary>
		/// Remove an item from the server
		/// </summary>
		/// <param name="ItemID">Id of the item to delete</param>
		/// <param name="type">Type of the item to delete</param>
		public void RemoveItem(string ItemID,ItemType type)
		{
			int.TryParse(ItemID, out int itemid);
			var ItemExist = Items.FirstOrDefault(x => x.ItemID == ItemID && x.ItemType == type);
			if (ItemExist != null)
			{
				Items.Remove(ItemExist);
				ItemExist.Updated -= Item_Updated;
				ItemExist = null;

			}
		}
		/// <summary>
		/// Remove all items in the server
		/// </summary>
		public void RemoveAll()
		{
			foreach (var item in Items)
			{

				item.Updated -= Item_Updated;
			}
			Items.Clear();
		}
		#endregion


		#region Events
		private void ModbusClient_SendDataChanged(object sender)
		{
			//throw new NotImplementedException();
		}

		private void ModbusClient_ReceiveDataChanged(object sender)
		{
			//throw new NotImplementedException();
		}

		private void Item_Updated(object sender, ItemEventArgs args)
		{			
		}
		#endregion

		#region ModbusReadAndWrite
		//Read and Write Bool
			public bool ReadCoil(int ItemID)
			{
				try
				{
					bool[] coils = modbusClient.ReadCoils(ItemID, 1);
					return coils.First();
				}
			catch (EasyModbus.Exceptions.StartingAddressInvalidException ex)
			{
				throw;
			}
			catch (Exception ex)
			{
				
				throw;
			}
		}
			//Read 
			public bool ReadDiscreteInputs(int ItemID)
			{
				try
				{
					bool[] readHoldingRegisters = modbusClient.ReadDiscreteInputs(ItemID, 1);
					return readHoldingRegisters.First();

				}
			catch (EasyModbus.Exceptions.StartingAddressInvalidException ex)
			{
				throw;
			}
			catch (Exception ex)
			{
				
				throw;
			}
		}
		//Read 
			public int ReadInputRegister(int ItemID)
			{
			try
			{
				int[] items = modbusClient.ReadInputRegisters(ItemID, 1);
				return items.First();
			}
			catch (EasyModbus.Exceptions.StartingAddressInvalidException ex)
			{
				throw;
			}
			catch (Exception ex)
			{
				
				throw;
			}

		}
			//Read and Write INT
			public int ReadHoldingRegister(int ItemID)
			{
				try
				{

					int[] readHoldingRegisters = modbusClient.ReadHoldingRegisters(ItemID, 1);
					return readHoldingRegisters.First();
				}
				catch(EasyModbus.Exceptions.StartingAddressInvalidException ex)
				{
					throw;
				}
				catch (Exception ex)
				{
					
					throw;
				}
			}


			//Write INT 
			private void WriteRegister(int ItemID, int value)
			{
				try
				{

					modbusClient.WriteSingleRegister(ItemID, value);
				}
				catch (Exception ex)
				{

					throw ex;
				}
			}
			//Write Bool 
			private void WriteCoil(int ItemID, bool value)
			{
			try
			{
				modbusClient.WriteSingleCoil(ItemID, value);

			}
			catch (Exception ex)
			{

				throw ex;
			}
			}


		#endregion


		public void Dispose()
			{
			this.Disconnect();			
			}
		

		public List<BrowserItem> BrowseItems()
		{
			return new List<BrowserItem>();
		}

		public object ReadItem(string ItemID)
		{
			throw new NotImplementedException();
		}

	}

}
