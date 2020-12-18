using Client.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Messaging
{
	public class ShowMessage
	{
		public ShowMessage()
		{
		}
		public ShowMessage(string message)
		{
			this.Text = message;
			this.Type = AlertType.Error; //Default Error
		}
		public ShowMessage(string message,AlertType alertType):this(message)
		{
			this.Type = alertType;
		}

		public ShowMessage(string message, Exception exception)
		{
			this.Text = message + exception.Message + exception.StackTrace;
		}
		public string Text { get; set; }
		public AlertType Type { get; set; }
	}
}
