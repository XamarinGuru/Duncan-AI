using System;
using System.IO;
using System.Data;
using Mono.Data.Sqlite;
using System.Text;
using Android.Content;
using System.Collections;
using Android.Util;

namespace Duncan.AI.Droid
{
	public class DatFileManager
	{
		public SqliteConnection connection;
		public DatFileManager ()
		{
		}

		//Generate DAT file date and return byte[]
		public byte[] GenerateDatFile(string tableName)
		{
			try
			{
				//Get Connection
				connection = Helper.GetDBConnection (); 
				StringBuilder stringBuilder = new StringBuilder ();
				using (var cs = connection.CreateCommand ()) {
					cs.CommandText = "SELECT * FROM " + tableName;
					var reader = cs.ExecuteReader ();
					while (reader.Read ()) { //Each row
						for (int i = 0; i < reader.FieldCount; ++i) { //No of fields
							if (stringBuilder.Length > 0 && i == 0) {
								stringBuilder.Append("\n");
							}
							if (i != 0) {
								stringBuilder.Append("\t");
							}
							stringBuilder.Append(reader [i]);
						}
					}
				}
				string dataStr = stringBuilder.ToString ();
				return Encoding.ASCII.GetBytes(dataStr);
			}catch(Exception e)
			{
				Log.Error ("Error:::", e.ToString());
			}

			return null;

//			string fileData = "Test";
//
//			using (var o = new StreamWriter (
//				ctx.OpenFileOutput (Constants.TempActivityLogFilePath, FileCreationMode.Private)))
//				o.Write (fileData);
//
//			using (var i = new StreamReader (
//				ctx.OpenFileInput (Constants.TempActivityLogFilePath))) {
//					string line = i.ReadToEnd();
//				}

//			ArrayList queryStringList = new ArrayList ();
//			queryStringList.Add ("CREATE TABLE PARKING (_id INTEGER PRIMARY KEY AUTOINCREMENT, name ntext, color ntext);");
//			queryStringList.Add ("INSERT INTO PARKING (name, color) VALUES ('Smith', 'Red' ) ;");
//			queryStringList.Add ("INSERT INTO PARKING (name, color) VALUES ('Mike', 'Blue' ) ;");
//
//			foreach (string command in queryStringList) {
//				using (var c = connection.CreateCommand ()) {
//					c.CommandText = command;
//					var rowcount = 0;
//					try{
//						rowcount = c.ExecuteNonQuery ();
//					}catch(Exception e)
//					{
//						Log.Error ("Error:::", e.ToString());
//					}
//				}
//			}
		}
	}
}

