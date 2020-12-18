using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Core.Interfaces
{
	public interface IUAClient :IClient
	{
		Task CreateSession(Uri uri);
		void CreateSubscription();
	}
}
