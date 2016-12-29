using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

//Login service to CRUD user information to the SQLite DB
namespace Duncan.AI
{
    public class LoginImpl 
    {
        //todo - remove this one.
        //No arg constructor
        public LoginImpl()
        {

        }

        ////Asyn task to authenticate and authorize user by username and password 
        //public async Task<UserDAO> ValidateLogin(string username, string password, CancellationToken cancellationToken)
        //{
        //    var result = await (new DatabaseManager()).ValidateLogin(username, password, cancellationToken);
        //    return result;
        //}

        ////This method is invoked by Android service, this service will be started by a Broadcast reciever on 
        ////action boot completed and internet connected.
        //public async Task<bool> PopulateUserNames(List<UserDAO> users)
        //{
        //    var result = await (new DatabaseManager()).PopulateUserNames(users);
        //    return result;
        //}

        ////Retrieve App Praperties  
        //public async Task<PropertiesDAO> RetrieveAppProperties()
        //{
        //    var props = await (new DatabaseManager()).RetrieveAppProperties();
        //    return props;
        //}

        ////Save App Properties
        //public async Task<bool> SaveAppProperties(PropertiesDAO propertiesDAO)
        //{
        //    var props = await (new DatabaseManager()).SaveAppProperties(propertiesDAO);
        //    return props;
        //}
    }
}

