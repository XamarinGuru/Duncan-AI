using System;
using SQLite;

namespace Duncan.AI
{
    [Table("Properties")]
    public class PropertiesDAO
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string private_url { get; set; }
        public int clientId { get; set; }
        public string PrinterType { get; set; }
    }
}
