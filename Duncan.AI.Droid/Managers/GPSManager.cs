using System.Threading;
using Duncan.AI.Droid.Utils.DataAccess;
namespace Duncan.AI.Droid.Managers
{
    public class GPSManager  
    {
        //Save user current location to DB
        public   bool  SaveUserCurrentLocation(string username, string latitude, string longitude)
        {
            var result =   (new DatabaseManager()).SaveUserCurrentLocation(username, latitude, longitude);
            return result;
        }

        //Get all user location by user from DB
        public    string[]  GetAllUserLocation(string username, CancellationToken cancellationToken)
        {
            var result =  (new DatabaseManager()).GetAllUserLocation(username, cancellationToken);
            return result;
        }
    }
}