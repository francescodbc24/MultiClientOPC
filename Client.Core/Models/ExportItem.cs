using Client.Core.Services.Enums;


namespace Client.Core.Models
{
	/// <summary>
	/// Model of the Item to export/import in the different formats
	/// </summary>
	public class ExportItem
	{
		public string ItemID { get; set; }
		public ItemType ItemType { get; set; }
	}
}
