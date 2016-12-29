using System.Data;
namespace Duncan.AI
{
    internal interface IImportRecord
    {
        int ImportRecord(int iStructCfgRev, string iMasterKeyValues, byte[] iStructBytes, byte[] iAttachmentBytes,
            string iOfficerName, string iOfficerId, ref string errorMsg, string structName, string parentStructName);       
    }
}