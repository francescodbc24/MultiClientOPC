using Client.Core.Exceptions;
using Client.Core.Interfaces;
using Client.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Core.Services
{
	public class JsonServices : IFileService
	{
		public List<IItem> ReadItems(string file)
		{
			if (string.IsNullOrEmpty(file))
				throw new ArgumentNullException(nameof(file));

			List<IItem> Items = new List<IItem>();
			Items.AddRange(JsonConvert.DeserializeObject<List<ModBusItem>>(File.ReadAllText(file)));
			return Items;
		}

		public bool SaveItems(IList<IItem> ListItems, string file = "")
		{
			if (ListItems == null)
				throw new ArgumentNullException(nameof(ListItems));
			if (ListItems.Count() == 0)
				throw new FileException("The list of items is empty.");
			var list = new List<ExportItem>();
			foreach (var item in ListItems)
				list.Add(new ExportItem { ItemID = item.ItemID, ItemType = item.ItemType });			
			// serialize JSON directly to a file
			using (StreamWriter file2 = File.CreateText(file))
			{
				JsonSerializer serializer = new JsonSerializer();
				serializer.Serialize(file2, list);
			}

			return true;
		}
	}
}
