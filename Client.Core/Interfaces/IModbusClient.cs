using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Core.Interfaces
{
	public interface IModbusClient : IClient
	{
		/// <summary>
		 /// Start Suscription to the server
		 /// </summary>
		 /// <param name="Interval">Interval time of refresh data from the PLC</param>
		void Suscribe(int Interval = 500);
		/// <summary>
		/// Stop suscription to the server
		/// </summary>
		void UnSuscribe();

		bool ReadCoil(int ItemID);
		bool ReadDiscreteInputs(int ItemID);
		int ReadInputRegister(int ItemID);
		int ReadHoldingRegister(int ItemID);
	}
}
