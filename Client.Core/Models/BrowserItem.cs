using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Core.Models
{
	/// <summary>
	/// Model for retrive the items in a server.
	/// </summary>
	public class BrowserItem
	{
		public string ItemID { get; set; }
		public string Name { get; set; }
		public string Path { get; set; }
	}
}
