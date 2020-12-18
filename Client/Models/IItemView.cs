
using Client.Core.Services.Enums;
using System;
using System.Runtime.InteropServices;


namespace Client.Models
{
	public interface IItemView
	{
		string ItemID { get;}
		object Value { get; }
		Type DataType { get; }
		ItemType ItemType { get; }
	}
}