using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Duncan.AI
{
	public interface ISyncStartup
    {

#if _used_

        AJW TODO - these are defined but not used, commented out to reduce confusion - to be removed when we are certain they should not be fleshed out


		string SyncAuthenticate(string username, string password, string serialNumber, ref string errorMgs,
			out string sessionKey, out long assignedSubConfigKey,
			out string customerConfigKey, out string serialNumberStatus,
			out string availableSerialNumbers);

		void SyncBegin(string encryptedSessionKey, string serialNumber, string serializedClientList,
			ref string  errorMgs, out  string oSessionName, out string  platformFiles, out string  compFiles,
			out string  confFiles, out string  uploadFileList, out string deleteFiles, out long assignedSubConfigKey,
			out string customerConfigKey, out string serialNumberStatus, out string sequenceNames);

		string DownloadListFiles(string serialNumber, string compFiles, int assignedSubConfigKey,
			string encryptedSessionKey, ref string errorMgs);

		byte[] DownloadCongfigFiles (string serialNumber, string customerConfigKey, string fileName,
		                     string encryptedSessionKey, ref string errorMgs);

        byte[] GetSequenceFile(string serialNumber, string loSequenceName, string encryptedSessionKey,
			ref string errorMgs);
#endif

    }
}
