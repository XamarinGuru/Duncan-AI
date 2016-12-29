using System;
using SQLite;

namespace Duncan.AI
{
    [Table(AutoISSUE.DBConstants.sqlAutoISSUEFileSystemTableName)]
    public class AIFileSystemDAO
	{
		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }


	    public string FILENAME { get; set; }
	    public long FILESIZE { get; set; }
        public DateTime FILECREATIONDATE { get; set; }
        public DateTime FILEMODIFIEDDATE { get; set; }

        public byte[] FILEDATA { get; set; }
	}
    

}

