using Client.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Core.Interfaces
{
	public interface IFileService
	{
		/// <summary>
		/// Save items in a file, throw an exception if ListItems is null
		/// </summary>
		/// <param name="ListItems"></param>
		/// <param name="file"></param>
		/// <returns>return true if the save go correctly otherwise false </returns>
		bool SaveItems(IList<IItem> ListItems, String file = "");

		/// <summary>
		/// Read items from a file
		/// </summary>
		/// <param name="file"> Path of the file to read</param>
		/// <returns>returns list of items read</returns>
		List<IItem> ReadItems(string file);
	}
}
