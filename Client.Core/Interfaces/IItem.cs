using Client.Core.Models;
using Client.Core.Services.Enums;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Client.Core.Interfaces
{
	public delegate void UpdatedEventHandler(object sender, ItemEventArgs args);
	public interface IItem
	{
		event UpdatedEventHandler Updated;

		string ItemID { get;  }
		object Value { get; }
		DateTime LastUpdate { get; }
		AccessRights AccessRights { get; }
		Type DataType { get;}
		ItemType ItemType { get;}


		void OnUpdated(object newValue);
		void Read();
		Task ReadAsync();
		void Write(object value);
		Task WriteAsync(object value);
	}
}