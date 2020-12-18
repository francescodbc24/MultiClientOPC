using Client.Core.Exceptions;
using Client.Core.Interfaces;
using Client.Core.Services;
using Client.Core.Services.Enums;
using System;
using System.Runtime.InteropServices;

using System.Threading.Tasks;
namespace Client.Core.Models
{
	public class ModBusItem :IItem
	{
		public ModBusItem() { }
		public ModBusItem(string ItemID,ItemType type, ModBusClient parent) {
			this.ItemID = ItemID;
			this.Parent = parent;
			this.ItemType = type;
			if (type == ItemType.Coils || type == ItemType.DiscreteInputs)
				this.DataType = typeof(bool);
			if (type == ItemType.InputRegister || type == ItemType.HoldingRegister)
				this.DataType = typeof(int);
			if (type == ItemType.HoldingRegister || type == ItemType.Coils)
				this.AccessRights = AccessRights.READWRITEABLE;
			if (type == ItemType.DiscreteInputs || type == ItemType.InputRegister)
				this.AccessRights = AccessRights.READABLE;
		}


		public delegate void UpdatedEventHandler(object sender, ItemEventArgs args);
		public string ItemID { get; set; }
		public object Value { get; private set; }
		public readonly ModBusClient Parent;

		public event Interfaces.UpdatedEventHandler Updated;

		public DateTime LastUpdate { get; private set; }
		public Type DataType { get; }
		public AccessRights AccessRights { get; }
		public ItemType ItemType { get; set; }
		public void OnUpdated(object newValue) { this.LastUpdate = DateTime.Now; this.Value = newValue; this.Updated?.Invoke(this, new ItemEventArgs { oldValue = this.Value, newValue = newValue }); }

		public void Read()
		{
			try
			{
				if (int.TryParse(this.ItemID, out int resultint))
				{
					if (resultint > 65535)
						throw new ServerException("Range invalid the number can't be to bigger that 65535");
				}
				else
				{
					throw new ServerException("The address is not a valid integer");
				}
				

			if(typeof(bool) == this.DataType)
				{
					bool result;
					if (this.AccessRights == AccessRights.READWRITEABLE)
					{
						// read the coil (bool) ReadWrite
						result = Parent.ReadCoil(int.Parse(this.ItemID));
					}
					else
					{
						// read the coil (bool) ReadOnly
						result = Parent.ReadDiscreteInputs(int.Parse(this.ItemID));
					}

					if (this.Value == null)
					{
						OnUpdated(result);

					}
					else
					{
						if (result != (bool)this.Value)
							OnUpdated(result);
					}

				}else if (typeof(int) == this.DataType)
				{

					int result2;
					if (this.AccessRights == AccessRights.READWRITEABLE)
					{
						
						result2 = Parent.ReadHoldingRegister(int.Parse(this.ItemID));

											
					}
					else
					{
						 result2 = Parent.ReadInputRegister(int.Parse(this.ItemID));
					}

						if(this.Value == null)
							OnUpdated(result2);

						int.TryParse(this.Value.ToString(), out int actual);
						if (result2 != actual)
							OnUpdated(result2);
				}						

			}
			catch (ServerException ex)
			{
				throw new ServerException(ex.Message, ex);
			}
			catch (EasyModbus.Exceptions.StartingAddressInvalidException ex)
			{
				//Return an exception when the variable not exist in the server
				throw new ServerException("Starting address invalid", ex);
			}
			catch (EasyModbus.Exceptions.ConnectionException ex)
			{
				//Return an exception when the variable not exist in the server
				throw new ServerException("Connection error", ex);
			}
			catch (EasyModbus.Exceptions.CRCCheckFailedException ex)
			{

				throw new ServerException("CRC Check Failed", ex);
			}
			catch (EasyModbus.Exceptions.SerialPortNotOpenedException ex)
			{
				throw new ServerException("Serial Port Not Opened", ex);

			}
			catch (EasyModbus.Exceptions.QuantityInvalidException ex)
			{
				throw new ServerException("Quantity invalid address invalid", ex);
			}
			catch (Exception ex)
			{
				// this conditional is for handle other exceptions that are not explicit in the EasyModBus Exceptions
				if (ex.Source == nameof(EasyModbus))
				{
					throw new ServerException(ex.Message);
				}
				//other types of exceptions don't handle (mostly they are that the reading time ends)
				return;
			}
		}

		public async Task ReadAsync()
		{

			await Task.Run(() =>
			{
				Read();
			}			

			);
		}

		public void Write(object value)
		{
			Parent.WriteItem(ItemID, value);
		}

		public async Task WriteAsync(object value)
		{
			await Task.Run(() =>
			{
				Write(value);
			});
		}
	}


}
