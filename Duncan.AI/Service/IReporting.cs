using System;
using System.Threading;
using System.Threading.Tasks;

namespace Duncan.AI
{
	public interface IReporting
	{
		Task<bool> ReportActivity (string username, string desc, CancellationToken cancellationToken);
		Task<string[]> GetAllActivityLogsByUser (string username, CancellationToken cancellationToken);
	}
}

