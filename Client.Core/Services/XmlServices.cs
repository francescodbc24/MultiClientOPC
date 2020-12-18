using Client.Core.Exceptions;
using Client.Core.Interfaces;
using Client.Core.Models;
using Client.Core.Services.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Client.Core.Services
{
	public class XmlServices : IFileService
	{

		public T DeserializeXMLFileToObject<T>(string XmlFilename)
		{
			T returnObject = default(T);
			if (string.IsNullOrEmpty(XmlFilename)) return default(T);

			try
			{
				StreamReader xmlStream = new StreamReader(XmlFilename);
				XmlSerializer serializer = new XmlSerializer(typeof(T));
				returnObject = (T)serializer.Deserialize(xmlStream);
				xmlStream.Close();
			}
			catch (Exception ex)
			{
				//ExceptionLogger.WriteExceptionToConsole(ex, DateTime.Now);
			}
			return returnObject;
		}

		/// <summary>
		/// Read Items in xml file, throw an exception if occurred an error reading the file,ListItems null or empty
		/// </summary>
		/// <param name="file"> Path of the file xml to read</param>
		/// <returns> return list of items in xml </returns>
		public List<IItem> ReadItems(string file)
		{
			if (string.IsNullOrEmpty(file))
				throw new ArgumentNullException(nameof(file));
			try
			{
				XmlDocument xdoc = new XmlDocument();
				xdoc.Load(file);
				XmlNodeList serverNodes = xdoc.DocumentElement.SelectNodes("//Items");
				List<IItem> Items = new List<IItem>();

				if (serverNodes.Count > 0) //Deve esserci almeno un elemento...
				{
					XmlElement serverElement = (XmlElement)serverNodes[0];

					//Load items
					foreach (XmlElement element in serverElement.GetElementsByTagName("Item"))
					{						
						Items.Add(
							new ModBusItem()
							{
								ItemID = element.GetAttribute("itemID"),
								ItemType = (ItemType)Enum.Parse(typeof(ItemType), element.GetAttribute("ItemType"), true)
							});
							
					}
				}
				return Items;			
			}
			catch (Exception ex)
			{
				throw new FileException(ex);
				//return null;
				//MessageBox.Show("Error Loading data");


			}
		}
		/// <summary>
		/// Save items in a file XML 
		/// </summary>
		/// <param name="ListItems"> List of items to save</param>
		/// <param name="file"> Path where save the file XML</param>
		/// <returns> return true if the items saved correctly. </returns>
		public bool SaveItems(IList<IItem> ListItems, string file = "")
		{
			if (ListItems == null)
				throw new ArgumentNullException(nameof(ListItems));

			if (ListItems.Count() == 0)
				throw new FileException("The list of items is empty.");

			if (ListItems == null || ListItems.Count() == 0) { return false; }
			string FileName = AppDomain.CurrentDomain.BaseDirectory+ "/Save.xml";
			if (file != "") { FileName = file; }
			try
			{
				XmlTextWriter writter = new XmlTextWriter(FileName, System.Text.Encoding.UTF8);

				writter.Formatting = Formatting.Indented;
				writter.WriteStartDocument(false);
				writter.WriteComment("Items File generated at: " + DateTime.Now);
				writter.WriteStartElement("Items");

				if (ListItems.Count > 0)
				{
					foreach (var item in ListItems)
					{
						writter.WriteStartElement("Item");
						writter.WriteAttributeString("ItemType", item.ItemType.ToString());
						writter.WriteAttributeString("itemID", item.ItemID);
						writter.WriteEndElement();
					}
				}
				writter.WriteEndElement();
				writter.Close();
				return true;
			}
			catch (Exception ex)
			{
				throw new FileException(ex);
			}
		}
	}
}
