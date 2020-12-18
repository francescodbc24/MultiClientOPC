using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Core.Exceptions
{
	public class FileException : Exception
	{
		public FileException() : base("Error trying to save or load elements from the file")
		{

		}

		public FileException(string message) : base(message)
		{

		}


		public FileException(string name, Exception exception) : base($"{name}", exception)
		{
		}
		public FileException(Exception exception) : base($"Error trying to save or load elements from the file",exception)
		{
		}

	}
}
