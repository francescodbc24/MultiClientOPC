using System;
namespace Client.Core.Models
{
	public class ItemEventArgs : EventArgs
	{
		public object oldValue { get; set; }
		public object newValue { get; set; }
	}


}
