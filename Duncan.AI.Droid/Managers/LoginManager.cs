using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;
using System.Collections.Generic;
using Duncan.AI.Droid.Utils.DataAccess;
using Duncan.AI.Droid.Utils.HelperManagers;

namespace Duncan.AI.Droid.Managers
{
    public class LoginManager //: - todo - when we do iphone, we can implement the interfaces.
    {
        //Asyn task to authenticate and authorize user by username and password 
        public    UserDAO ValidateLogin(string username, string password, CancellationToken cancellationToken)
        {
            var result = (new DatabaseManager()).ValidateLogin(username, password, cancellationToken);
            return result;
        }

        //This method is invoked by Android service, this service will be started by a Broadcast reciever on 
        //action boot completed and internet connected.
        public    bool  PopulateUserNames(List<UserDAO> users)
        {
            var result =  (new DatabaseManager()).PopulateUserNames(users);
            return result;
        }



        //Retrieve App Properties  
        public bool ValidateAppPropertiesTable()
        {
            try
            {
                var props = (new DatabaseManager()).CreateAppPropertiesTable();
                return props;

            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, null, "ValidateAppPropertiesTable");
            }
            return false;
        }


        //Retrieve App Praperties  
        public  PropertiesDAO RetrieveAppProperties()
        {
            try
            {
                var props = (new DatabaseManager()).RetrieveAppProperties();


                // TEMP - force default if not already defined
                if (string.IsNullOrEmpty(props.name) == true)
                {
                    props.name = "Miami Dade Parking";
                    props.url = "aidev02.duncan-usa.com";
                }



                return props;

            }
            catch (Exception ex)
            {

                LoggingManager.LogApplicationError(ex, null, "RetrieveAppProperties");
            }
            return null;
        }

        //Save App Properties
        public long SaveAppProperties(PropertiesDAO propertiesDAO)
        {
            try
            {
                var props = (new DatabaseManager()).SaveAppProperties(propertiesDAO);
                return props;
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, null, "SaveAppProperties");
            }
            return -1;
        }


        private void PreLoadClientInfoFromXML(byte[] iXMLFileData)
        {
            // not in there? 
            if (iXMLFileData == null)
            {
                // nothing to do
                return;
            }

            XmlReader loXMLReader = null;

            try
            {

                // Create an XMLSerialier that can deserialize ISSUE_AP.XML into a TClientDef object
                XmlSerializer serializer = new XmlSerializer(typeof(Reino.ClientConfig.TClientDef));

                // todo - may need to specify encoding....
                //MemoryStream loXMConfigMemoryStream = new MemoryStream(iXMLFileData, 0, iXMLFileData.Length);
                //XmlReader reader = new XmlTextReader(loXMConfigMemoryStream);

                string loXMLConfigString = System.Text.UTF8Encoding.UTF8.GetString(iXMLFileData);
                loXMLReader = new XmlTextReader(new StringReader(loXMLConfigString));


                // Use the Deserialize method to restore the object's state from XML.
                Reino.ClientConfig.TClientDef loBootupClientDef = (Reino.ClientConfig.TClientDef)serializer.Deserialize(loXMLReader);

                if (loBootupClientDef != null)
                {
                    // update these for global reference
                    DroidContext.XmlClientName = loBootupClientDef.Client;
                    DroidContext.XmlAgencyDesignator = loBootupClientDef.AgencyDesignator;
                    DroidContext.XmlLayoutRevision = loBootupClientDef.Revision;
                }
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, null, "PreLoadClientInfoFromXML");
            }
            finally
            {
                // Close the reader
                if (loXMLReader != null)
                {
                    loXMLReader.Close();
                }
            }
        }


        public void GetIssueAPXMLRevisionInfo()
        {
            try
            {
                // initialize
                DroidContext.XmlClientName = string.Empty;
                DroidContext.XmlAgencyDesignator = string.Empty;
                DroidContext.XmlLayoutRevision = -1;


                // see if we can load the issue ap info
                AIFileSystemDAO loFileInfo = (new DatabaseManager()).GetAIFileSystemData(Constants.ISSUE_AP_XML_FILENAME);
                if (loFileInfo != null)
                {
                    PreLoadClientInfoFromXML( loFileInfo.FILEDATA );
                }
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, null, "GetIssueAPXMLRevisionInfo");
            }

        }
    }
}
