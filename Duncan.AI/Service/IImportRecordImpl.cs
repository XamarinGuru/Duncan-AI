using System.Data;
using DuncanWebServicesClient;

namespace Duncan.AI
{
    class IImportRecordImpl : IImportRecord
    {
		public int ImportRecord(int iStructCfgRev, string iMasterKeyValues, byte[] iStructBytes, byte[] iAttachmentBytes,
            string iOfficerName, string iOfficerId, ref string errorMsg, string structName, string parentStructName)
        {
            var importRecordService = new ImportRecordService();
            return importRecordService.ImportRecord(Constants.SERIAL_NUMBER, iOfficerName, iOfficerId, parentStructName, 
                iMasterKeyValues, structName, iStructCfgRev, iStructBytes, iAttachmentBytes, ref errorMsg);
        }      
    }
}
