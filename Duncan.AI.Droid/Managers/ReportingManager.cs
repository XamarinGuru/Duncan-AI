using System.Threading;
using Duncan.AI.Droid.Utils.DataAccess;
namespace Duncan.AI.Droid.Managers
{
   public  class ReportingManager  
    {
        //Report user activity to DB
        public  bool ReportActivity(string username, string desc, CancellationToken cancellationToken)
        {
            var result =  (new DatabaseManager()).ReportActivity(username, desc, cancellationToken);
            return result;
        }

        //Get all pending activities submitted by user from DB
        public   string[]  GetAllActivityLogsByUser(string username, CancellationToken cancellationToken)
        {
            var result =   (new DatabaseManager()).GetAllActivityLogsByUser(username,cancellationToken);
            return result;
        }
    }
}