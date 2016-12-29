
namespace Duncan.AI.Webservices
{
    public class UserWebservices
    {
        public string GetUsers()
        {
            var sp = new AIWebServices.AutoISSUEHostService();
            return  sp.ApiGetAllUsers("Sz8tTtrh4CYkD88jXOjp6LpFAXU11BlGYFYdGkvK0EU=");
        }

    }
}
