using Client.Core.Interfaces;
using Client.Core.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Core.Services
{
	public sealed class FileContainer
	{

		
		private static Lazy<FileContainer> _Instance = new Lazy<FileContainer>();
	
		public static FileContainer Instance
		{
			get { return _Instance.Value; }

		}

		private XmlServices XmlServices { get; set; }
		private JsonServices JsonServices { get; set; }
		
		public IFileService Resolve(FileType fileType)
		{
			switch (fileType)
			{
				case FileType.XML:
					return XmlServices == null ? XmlServices = new XmlServices() : XmlServices;
					
				case FileType.JSON:
					return JsonServices == null ? JsonServices = new JsonServices() : JsonServices;					
				default:
					return null;

			}

		}
	}
}
