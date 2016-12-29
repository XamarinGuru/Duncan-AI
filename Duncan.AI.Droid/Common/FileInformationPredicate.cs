using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.IO;

namespace Duncan.AI.Droid
{
	#region FileInformationPredicate
	/// <summary>
	/// Class that contains functions that can be used as predicates for searches 
	/// in collections based on FileInformation.
	/// </summary>
	public class FileInformationPredicate
	{
		private string compareName;

		// Constructor used when comparing strings 
		public FileInformationPredicate(string iCompareName)
		{
			compareName = iCompareName;
		}

		// Compare by File Name (Case-Sensitive)
		public bool CompareByFileName(FileInformation iFileInformation)
		{
			return (iFileInformation.FileName == compareName);
		}

		// Compare by File Name (Case-Insensitive)
		public bool CompareByFileName_CaseInsensitive(FileInformation iFileInformation)
		{
			return (String.Compare(iFileInformation.FileName, compareName, true) == 0);
		}

		// Compare by File Name Without Path (Case-Insensitive)
		public bool CompareByFileName_NoPath_CaseInsensitive(FileInformation iFileInformation)
		{
			return (String.Compare(Path.GetFileName(iFileInformation.FileName), Path.GetFileName(compareName), true) == 0);
		}

		// Compare by Table Name (Case-Sensitive)
		public bool CompareByTableName(FileInformation iFileInformation)
		{
			return (iFileInformation.TableName == compareName);
		}

		// Compare by Table Name (Case-Insensitive)
		public bool CompareByTableName_CaseInsensitive(FileInformation iFileInformation)
		{
			return (String.Compare(iFileInformation.TableName, compareName, true) == 0);
		}
	}
	#endregion

	#region FileInformation
	/// <summary>
	/// This contains the information (like date, its filename, etc.) for a file.
	/// Gotta use XmlInclude so serializer knows we can also be one of our child types.
	/// </summary>
	[XmlInclude(typeof(TextFileData)), XmlInclude(typeof(BinaryFileData))]
	public class FileInformation
	{
		// This is the basic file info.
		public string TableName = "";
		public string FileName = "";
		public DateTime FileDate = DateTime.MinValue.ToUniversalTime();
		public long Length = 0;
		public bool Compressed = false;
		public bool Exists = true;
		public bool Optional = false; // mcb - new property to designate hh files as optional.
		public bool Fragmentable = false;    // new properties for 
		public bool PreCompressed = false;   // file attribute
		public bool BinaryImage = false;     // and file types
		public bool ShortDelimitedRecord = false;  /////////
		public bool ToHHMemory = false;         //////////////    
		private string _HHFileName = "";
		public string HHFileName
		{
			get { if (_HHFileName != "") return _HHFileName; return FileName; }
			set
			{
				if (value != FileName) _HHFileName = value;
				else _HHFileName = "";
			}
		}
		public bool AlreadyFlushedToFile = false;
		public bool ForceAsHighestFileRevision = false;

		// This will return the data we hold in the form of a byte array. This
		// must be overridden by child classes, because we dont hold any actual
		// data, only our child classes do.
		[XmlIgnoreAttribute] // For .NET Compact Framework support, we can't include this property as serializable (even though its a read-only property) 
		public virtual byte[] Data
		{
			get { throw new Exception("FileInformation.Data needs to be overridden in descendant class."); }
		}

		/// <summary>
		/// This will set all properties of the passed object equal to our properties.
		/// </summary>
		/// <param name="oFileToCopy"></param>
		public virtual void Copy(FileInformation oFileToCopy)
		{
			oFileToCopy.TableName = TableName;
			oFileToCopy.FileDate = FileDate;
			oFileToCopy.FileName = FileName;
			oFileToCopy.Length = Length;
			oFileToCopy.Compressed = Compressed;
			oFileToCopy.Exists = Exists;

			oFileToCopy.Optional = Optional;
			oFileToCopy.Fragmentable = Fragmentable;
			oFileToCopy.PreCompressed = PreCompressed;
			oFileToCopy.BinaryImage = BinaryImage;
			oFileToCopy.ShortDelimitedRecord = ShortDelimitedRecord;
			oFileToCopy.ToHHMemory = ToHHMemory;
			oFileToCopy.HHFileName = HHFileName;
			oFileToCopy.AlreadyFlushedToFile = AlreadyFlushedToFile;
			oFileToCopy.ForceAsHighestFileRevision = ForceAsHighestFileRevision;
		}

		/// <summary>
		/// This will return an exact copy of our object.
		/// </summary>
		/// <returns></returns>
		public virtual FileInformation Copy()
		{
			FileInformation fileCopy = new FileInformation();
			Copy(fileCopy);
			return fileCopy;
		}

		/// <summary>
		/// This is just like Copy except can be used by child classes to return just
		/// the information part of them (normal Copy returns everything, even the
		/// file data if its a child object). Really only difference is its not
		/// overridden so always just an instance of us.
		/// </summary>
		/// <returns></returns>
		public FileInformation CopyInformation()
		{
			FileInformation fileCopy = new FileInformation();
			Copy(fileCopy);
			return fileCopy;
		}
	}
	#endregion

	#region TextFileData
	/// <summary>
	/// This contains the information and data for text files. All file data
	/// stored as an array of strings.
	/// </summary>
	public class TextFileData : FileInformation
	{
		// This will hold the actual file data. Since this is a text
		// file can hold it line by line. Initialize array to blank 
		// string array so its never a null reference.
		[XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
		public string[] FileLines = new string[] { };

		public byte[] FileData
		{
			get
			{
				// We must first convert our string[] into a byte[].
				List<byte> byteData = new List<byte>();
				// The data we have is really lines in a file. So to
				// accurately convert the lines to a byte array we 
				// must also include the CRLF after each line.
				foreach (string oneLine in FileLines)
				{
					byteData.AddRange(Encoding.ASCII.GetBytes(oneLine + "\r\n"));
				}

				// Got the bytes, return them as an array.
				return byteData.ToArray();
			}
			set
			{
				// We've come across text files containing NULL characters which screws up deserialization,
				// so we will serialize data as byte array instead of string array.
				string allData = Encoding.ASCII.GetString(value, 0, value.Length); // Need to use method that exists on .NET Compact Framework also
				string[] loFileLines = Regex.Split(allData, "\r\n");
				List<string> loGenericFileLines = new List<string>(loFileLines.Length);
				foreach (string oneLine in loFileLines)
				{
					// Lets ignore any blank lines since they are useless
					if (oneLine == "")
						continue;
					loGenericFileLines.Add(oneLine);
				}
				loGenericFileLines.Capacity = loGenericFileLines.Count;
				this.FileLines = loGenericFileLines.ToArray();
			}
		}

		// Override the Data property so child class can return our data.
		public override byte[] Data
		{
			get
			{
				// We must first convert our string[] into a byte[].
				List<byte> byteData = new List<byte>();
				// The data we have is really lines in a file. So to
				// accurately convert the lines to a byte array we 
				// must also include the CRLF after each line.
				foreach (string oneLine in FileLines)
				{
					byteData.AddRange(Encoding.ASCII.GetBytes(oneLine + "\r\n"));
				}

				// Got the bytes, return them as an array.
				return byteData.ToArray();
			}
		}

		/// <summary>
		/// This will set all properties of the passed object equal to our properties.
		/// </summary>
		/// <param name="oFileToCopy"></param>
		public override void Copy(FileInformation oFileToCopy)
		{
			// Let parent copy its fields.
			base.Copy(oFileToCopy);
			// If this is one of our objects, then copy our properties too.
			if (oFileToCopy is TextFileData)
			{
				(oFileToCopy as TextFileData).FileLines = (string[])FileLines.Clone();
			}
		}

		/// <summary>
		/// This will return an exact copy of our object.
		/// </summary>
		/// <returns></returns>
		public override FileInformation Copy()
		{
			// Return an exact copy of us.
			TextFileData fileCopy = new TextFileData();
			Copy(fileCopy);
			return fileCopy;
		}
	}
	#endregion

	#region BinaryFileData
	/// <summary>
	/// This contains the information and data for binary files. All file data
	/// stored as an array of bytes.
	/// </summary>
	public class BinaryFileData : FileInformation
	{
		// This will hold the actual file data. Binary files could have
		// anything in the file, so will just store all its bytes. Initialize
		// array to blank string array so its never a null reference.        
		public byte[] FileData = new byte[] { };

		// Override the Data property so child class can return our data.
		public override byte[] Data
		{
			get
			{
				return FileData;
			}
		}

		/// <summary>
		/// This will set all properties of the passed object equal to our properties.
		/// </summary>
		/// <param name="oFileToCopy"></param>
		public override void Copy(FileInformation oFileToCopy)
		{
			// Let parent copy its fields.
			base.Copy(oFileToCopy);
			// If this is one of our objects, then copy our properties too.
			if (oFileToCopy is BinaryFileData)
			{
				(oFileToCopy as BinaryFileData).FileData = (byte[])FileData.Clone();
			}
		}

		/// <summary>
		/// This will return an exact copy of our object.
		/// </summary>
		/// <returns></returns>
		public override FileInformation Copy()
		{
			// Return an exact copy of us.
			BinaryFileData fileCopy = new BinaryFileData();
			Copy(fileCopy);
			return fileCopy;
		}
	}
	#endregion

	#region FileSet
	/// <summary>
	/// This contains a list of file data objects and has methods to 
	/// manipulate these objects.
	/// </summary>
	public class FileSet
	{
		public List<FileInformation> FileList = new List<FileInformation>();

		public string ExceptionText = "";
		public string ExportFolder = "";
		public string ExportHistoryFolder = "";
		public string UnitSerial = "";

		/// <summary>
		/// "FlushExtraFilesAutomatically" is used as a flag during exports to indicate 
		/// if exported multimedia files should be written to export folder and export
		/// history immediately after being added to fileset.  In doing so, we can just 
		/// keep file info and remove the filedata from memory.
		/// </summary>
		public bool FlushExtraFilesAutomatically = false;

		public FileSet CopyInformation()
		{
			// We will make a copy of ourselves, with file information
			// only (no actual file data), and return this info in
			// this fileset object.
			FileSet setCopy = new FileSet();
			foreach (FileInformation oneFile in FileList)
			{
				setCopy.FileList.Add(oneFile.CopyInformation());
			}
			setCopy.ExceptionText = this.ExceptionText;
			setCopy.ExportFolder = this.ExportFolder;
			setCopy.ExportHistoryFolder = this.ExportHistoryFolder;
			setCopy.FlushExtraFilesAutomatically = this.FlushExtraFilesAutomatically;
			setCopy.UnitSerial = this.UnitSerial;

			// Return the new fileset.
			return setCopy;
		}

		public FileSet Copy()
		{
			// Will make exact copy of ourselves and return in
			// this fileset object.
			FileSet setCopy = new FileSet();
			foreach (FileInformation oneFile in FileList)
			{
				setCopy.FileList.Add(oneFile.Copy());
			}
			setCopy.ExceptionText = this.ExceptionText;
			setCopy.ExportFolder = this.ExportFolder;
			setCopy.ExportHistoryFolder = this.ExportHistoryFolder;
			setCopy.FlushExtraFilesAutomatically = this.FlushExtraFilesAutomatically;
			setCopy.UnitSerial = this.UnitSerial;

			// Return the new fileset.
			return setCopy;
		}

		public void Merge(FileInformation fileData)
		{
			// See if this file already in list. If is then remove it.
			FileInformationPredicate predicate = new FileInformationPredicate(fileData.FileName);
			FileInformation foundFile = FileList.Find(predicate.CompareByFileName_CaseInsensitive);
			if (foundFile != null)
			{
				FileList.Remove(foundFile);
			}

			// Add a copy of this object to our list.
			FileList.Add(fileData.Copy());
		}

		public void Merge(FileSet fileSet)
		{
			// Go thru all file data objects in passed fileset and
			// add them to us. If we already have a file object
			// with same file name then we will replace it with
			// the file object from passed set.
			foreach (FileInformation oneFile in fileSet.FileList)
			{
				Merge(oneFile);
			}
		}
	}
	#endregion
}
