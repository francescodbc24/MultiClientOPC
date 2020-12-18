using System;
using System.Globalization;
using System.Windows.Data;

namespace Client.Converters
{
	public class SubStringKeyConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return "";
			if (parameter == null)
				return "";
			var element=value.ToString();
			//var result = element.Replace(parameter.ToString(), "");
			var result = element.Substring(element.LastIndexOf('.') + 1);
			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
