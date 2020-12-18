using Client.Core.Services.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Client.Converters
{
	public class ClientTypeToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || parameter == null)
				return Visibility.Hidden;
			var type = (ClientType)value;
			if(parameter.ToString().Contains(type.ToString()))
			{
				return Visibility.Visible;
			}
			else
			{
				return Visibility.Hidden;
			}

		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
