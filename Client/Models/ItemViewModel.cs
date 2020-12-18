

using Client.Core.Interfaces;
using Client.Core.Models;
using Client.Core.Services.Enums;
using Client.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{	
	public class ItemViewModel : BaseModel,IItemView
	{
		IItem Item;
		public ItemViewModel(IItem item)
		{
			this.Item = item;		
			this.Item.Updated += Item_Updated;
		}

		public string ItemID { get => Item.ItemID.ToString(); }

		public object Value { get => Item.Value; }

		public Type DataType { get => Item.DataType; }

		public ItemType ItemType { get => Item.ItemType; }

		private void Item_Updated(object sender, ItemEventArgs args)
		{
			OnPropertyChange(nameof(Value));
			OnPropertyChange(nameof(DataType));
		}
	}
}

