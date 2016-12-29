using System;
using SQLite;

//Reporting table to report user activity
namespace Duncan.AI
{
	[Table("Reporting")]
    public class ReportingDAO
	{
		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }
		public string desc { get; set; }
		public string username { get; set; }
		public DateTime date { get; set; }
	}
}

