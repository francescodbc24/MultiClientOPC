using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models.Common
{
	public class BaseModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private bool _isBusy;
		public bool IsBusy
		{
			get { return _isBusy; }
			set { _isBusy = value; OnPropertyChange(); }
		}

		protected void OnPropertyChange([CallerMemberName]String propertyName = "")
		{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
