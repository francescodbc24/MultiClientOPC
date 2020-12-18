using Client.Core.Interfaces;
using Client.Core.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Core.Services
{
	public class ClientContainer
	{

		private static Lazy<ClientContainer> _Instance = new Lazy<ClientContainer>();

		public static ClientContainer Instance
		{
			get { return _Instance.Value; }

		}

		private OPCDAClient OPCDAClient { get; set; }
		private ModBusClient ModBusClient { get; set; }
		private OPCUaClient OPCUaClient { get; set; }

		public IClient Resolve(ClientType clientType)
		{
			switch (clientType)
			{				
				case ClientType.Modbus:
				return ModBusClient == null ?  ModBusClient = new ModBusClient() : ModBusClient;					
				case ClientType.OPCDA:
					return OPCDAClient == null ? OPCDAClient = new OPCDAClient() : OPCDAClient;
				case ClientType.UA:
					return OPCUaClient == null ? OPCUaClient = new OPCUaClient() : OPCUaClient;					
				default:
					return null;
			}
			
		}
	}
}
