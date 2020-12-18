using Client.Core.Models;
using Client.Core.Services;
using Client.Core.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Core.Interfaces
{
	public interface IClient
	{
		/// <summary>
		/// Address ip of the server to be connected
		/// </summary>
		string Ip { get; set; }
		/// <summary>
		/// Port of the server to be connected
		/// </summary>
		int Port { get; set; }
		/// <summary>
		/// Name of the server to be connected
		/// </summary>
		string Name { get; set; }
		/// <summary>
		/// List of currents items connected
		/// </summary>
		IList<IItem> Items { get; }
		/// <summary>
		/// Connect to the Server, throw an exception if the ip/port is incorrect
		/// </summary>
		void Connect();
		Task ConnectAsync();
		/// <summary>
		/// Disconnect to the ModbusServer
		/// </summary>
		void Disconnect();
		/// <summary>
		/// Check if the server is connected
		/// </summary>
		/// <returns> returns true if server is connected to the server, otherwise false</returns>
		bool isConnect();
		/// <summary>
		/// Write a new value in the specific address.
		/// </summary>
		/// <param name="ItemID">ID of the item.</param>
		/// <param name="value">New value of the item.</param>
		void WriteItem(string ItemID, object value);
		/// <summary>
		/// Read new value in the specific address
		/// </summary>
		/// <param name="ItemID">ID of the item</param>
		object ReadItem(string ItemID);
		/// <summary>
		/// Create a new item in the server.
		/// </summary>
		/// <param name="ItemID">Id of the item to create</param>
		/// <param name="type">type of the item to create</param>
		/// <returns> return a new item or null if the element already exist.</returns>
		IItem CreateItem(string itemName, ItemType type = ItemType.None);
		/// <summary>
		/// Remove an item from the server
		/// </summary>
		/// <param name="ItemID">Id of the item to delete</param>
		/// <param name="type">Type of the item to delete</param>
		void RemoveItem(string ItemID, ItemType type = ItemType.None);
		/// <summary>
		/// Remove all items in the server
		/// </summary>
		void RemoveAll();

		List<BrowserItem> BrowseItems();
	}
}
