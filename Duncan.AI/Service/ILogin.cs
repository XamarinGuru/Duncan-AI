using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Duncan.AI
{
	public interface ILogin
	{
		 UserDAO ValidateLogin (string username, string password, CancellationToken cancellationToken);
		bool PopulateUserNames (List<UserDAO> users);
		PropertiesDAO RetrieveAppProperties ();
		bool SaveAppProperties (PropertiesDAO propertiesDAO);
	}
}

