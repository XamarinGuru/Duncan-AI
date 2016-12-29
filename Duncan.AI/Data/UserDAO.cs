using System;
using SQLite;

//User table to store login information username and password
namespace Duncan.AI
{
	[Table("User")]
    public class UserDAO
	{
		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }
	    public string username { get; set; }
	    public string password { get; set; }
		public string officerName { get; set; }
		public string officerId { get; set; }
        public string agency { get; set; }
	}
    

}

