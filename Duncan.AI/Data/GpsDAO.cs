using System;
using SQLite;

namespace Duncan.AI
{
	[Table("Gps")]
    class GpsDAO
    {
		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }
		public string username { get; set; }
		public string latitude { get; set; }
		public string longitude { get; set; }
		public DateTime date { get; set; }
    }
}
