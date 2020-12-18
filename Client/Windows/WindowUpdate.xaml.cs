using Client.Models;
using Client.ViewModels;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client.Windows
{
	/// <summary>
	/// Logica di interazione per Window1.xaml
	/// </summary>
	public partial class WindowUpdate : MetroWindow
	{
		public WindowUpdate(IItemView oPCItem)
		{
			this.DataContext = new WindowUpdateViewModel(oPCItem);
			InitializeComponent();
		}

		private void Button1_Click(object sender, RoutedEventArgs e)
		{
			var btn = sender as Button;
			if (textBox.Text != "0")
			{
				textBox.Text = $"{textBox.Text}{btn.Content}";
			}
			else
			{
				textBox.Text = btn.Content.ToString();
			}
		}

		private void ButtonOK_Click(object sender, RoutedEventArgs e)
		{

			this.DialogResult = true;
		
		}

		private void ButtonDelete_Click(object sender, RoutedEventArgs e)
		{
			textBox.Text = "";
		}
	}
}
