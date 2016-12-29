using System.Collections.Generic;

namespace Duncan.AI.Droid.Common
{
	#region FileSet
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
			var setCopy = new FileSet();
			foreach (FileInformation oneFile in FileList)
				setCopy.FileList.Add(oneFile.CopyInformation());
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
			var setCopy = new FileSet();
			foreach (FileInformation oneFile in FileList)
				setCopy.FileList.Add(oneFile.Copy());
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
			var predicate = new FileInformationPredicate(fileData.FileName);
			FileInformation foundFile = FileList.Find(predicate.CompareByFileName_CaseInsensitive);
			if (foundFile != null)
				FileList.Remove(foundFile);

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
				Merge(oneFile);
		}
	}
	#endregion
}

