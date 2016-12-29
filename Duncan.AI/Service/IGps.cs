using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Duncan.AI
{
    interface IGps
    {
		Task<bool> SaveUserCurrentLocation (string username, string latitude, string longitude);
		Task<string[]> GetAllUserLocation (string username, CancellationToken cancellationToken);
    }
}
