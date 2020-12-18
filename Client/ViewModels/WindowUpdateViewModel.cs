
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Models;
using Client.Models.Common;

namespace Client.ViewModels
{
	class WindowUpdateViewModel :BaseModel
	{
		public WindowUpdateViewModel(IItemView oPCItem)
		{
			Item = oPCItem;
			if(Item.DataType == typeof(bool))
			{
				isBool = true;
				isNumeric = false;
			}			
			else
			{
				isBool = false;
				isNumeric = true;
			}
			}

		/*Item to update*/
		private IItemView _Item;
		public IItemView Item
		{
			get
			{
				return _Item;
			}
			set
			{
				_Item = value;
				OnPropertyChange();

			}
		}


		/* IsBool */
		private bool _isBool;
		public bool isBool
		{
			get
			{
				return _isBool;
			}
			set
			{
				_isBool = value;
				OnPropertyChange();
			}
		}


		/*IsNumeric*/
		private bool _isNumeric;
			public bool isNumeric
		{
			get
			{
				return _isNumeric;
			}
			set
			{
				_isNumeric = value;
				OnPropertyChange();
			}
		}
	}
}
