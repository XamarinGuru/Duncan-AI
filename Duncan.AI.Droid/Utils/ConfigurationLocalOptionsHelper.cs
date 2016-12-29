using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Duncan.AI.Droid.Managers;
using Duncan.AI.Droid.Utils.HelperManagers;
using DuncanWebServicesClient;


using System.IO;
using Java.IO;
using File = Java.IO.File;

namespace Duncan.AI.Droid.Utils
{
    class ConfigurationLocalOptionsHelper
    {
        private static string GetConfigFileName()
        {
            string loConfigFileName = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads) +
                                      "/" +
                                      Constants.CONFIG_FILE_NAME;

            return loConfigFileName;
        }




        public static bool WriteConfigData()
        {
            bool retValue = false;
            FileWriter loFileWriter = null;

            try
            {
                var id = 0;  // AJW TODO - this is fixed value in current code, do we really need it? YES, there is reference against list item - this needs to be cleaned up
                var fileName = GetConfigFileName();

                List<string> loConfigFileLines = new List<string>();

                /*
               // URL@http://aidev.duncan-usa.com/ReinoWebServices/Android%20DemoAutoISSUEPublic/AutoISSUEpublicService.asmx
               // PRIVATEURL@http://aidev.duncan-usa.com/ReinoWebServices/Android%20DemoAutoISSUEHost/AutoISSUEHostService.asmx
                // Serial@N4-22000
                 * */



                loConfigFileLines.Add(Constants.CONFIG_FILE_PARAMETER_PUBLIC_URL +
                                       Constants.CONFIG_FILE_PARAMETER_VALUE_DELIMETER +
                                       WebserviceConstants.CLIENT_URL);

                loConfigFileLines.Add(Constants.CONFIG_FILE_PARAMETER_PRIVATE_URL +
                                       Constants.CONFIG_FILE_PARAMETER_VALUE_DELIMETER +
                                       WebserviceConstants.PRIVATE_CLIENT_URL);

                loConfigFileLines.Add(Constants.CONFIG_FILE_PARAMETER_UNIT_SERIAL_NUMBER +
                                       Constants.CONFIG_FILE_PARAMETER_VALUE_DELIMETER +
                                       Helper.GetDeviceUniqueSerialNumber());
                // TODO - this is not updated yet.... is this a lurking issue with "public const" decl?   Constants.SERIAL_NUMBER);

                // AJW this value doesn't exist - does it need to be?
                //loConfigFileLines.Add( Constants.CONFIG_FILE_PARAMETER_CLIENT_ID + 
                //                       Constants.CONFIG_FILE_PARAMETER_VALUE_DELIMETER + 
                //                       Constants.ID  );


                loConfigFileLines.Add(Constants.CONFIG_FILE_PARAMETER_PRINTER_TYPE +
                                       Constants.CONFIG_FILE_PARAMETER_VALUE_DELIMETER +
                                       Helper.GetSelectedPrinterType());



                loFileWriter = new FileWriter(fileName);
                var loWriter = new BufferedWriter(loFileWriter);

                foreach (string oneLine in loConfigFileLines)
                {
                    loWriter.Write(oneLine);
                    loWriter.NewLine();
                }
                loWriter.Flush();
                loFileWriter.Flush();
                loFileWriter.Close();
                loFileWriter.Dispose();
                loFileWriter = null;

                retValue = true;

            }
            catch (Exception e)
            {
                LoggingManager.LogApplicationError(e, null, "WriteConfigData");
            }
            finally
            {
                if (loFileWriter != null)
                {
                    loFileWriter.Dispose();
                    loFileWriter = null;
                }
            }

            return (retValue);
        }



        public static bool ReadConfigData()
        {
            bool retValue = false;
            try
            {
                var id = 0;
                var fileName = GetConfigFileName();

                //var fileName =
                //           Android.OS.Environment.GetExternalStoragePublicDirectory
                //           (Android.OS.Environment.DirectoryDownloads) + "/" + Constants.CONFIG_FILE_NAME;


                // this is a legacy file - it won't usually be there
                if (System.IO.File.Exists(fileName) == false)
                {
                    return false;
                }



                //var file = new File(fileName);
                var fileReader = new FileReader(fileName);
                //if (file.Exists())
                if (fileReader != null)
                {

                    var reader = new BufferedReader(fileReader);
                    string line = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        var linedata = line.Split('@');
                        if (linedata.Length >= 2)
                        {
                            retValue = true;


                            string loOneParameterName = linedata[0].ToUpper();

                            switch (loOneParameterName)
                            {
                                case Constants.CONFIG_FILE_PARAMETER_PUBLIC_URL:
                                    {
                                        WebserviceConstants.CLIENT_URL = linedata[1];
                                        break;
                                    }

                                case Constants.CONFIG_FILE_PARAMETER_PRIVATE_URL:
                                    {
                                        WebserviceConstants.PRIVATE_CLIENT_URL = linedata[1];
                                        break;
                                    }

                                case Constants.CONFIG_FILE_PARAMETER_UNIT_SERIAL_NUMBER:
                                    {
                                        Constants.SERIAL_NUMBER = linedata[1];
                                        break;
                                    }

                                case Constants.CONFIG_FILE_PARAMETER_CLIENT_ID:
                                    {
                                        try
                                        {
                                            id = Convert.ToInt32(linedata[1]);
                                        }
                                        catch (Exception e)
                                        {
                                            id = 0;
                                        }
                                        break;
                                    }


                                case Constants.CONFIG_FILE_PARAMETER_PRINTER_TYPE:
                                    {
                                        try
                                        {
                                            Constants.PRINTER_TYPE = linedata[1];
                                        }
                                        catch (Exception e)
                                        {
                                            Constants.PRINTER_TYPE = Constants.PRINTER_TYPE_NAME_ZEBRA_MZ320;
                                        }
                                        break;
                                    }

                                default:
                                    {
                                        // unknown parameter - do nothing
                                        break;
                                    }
                            }


                            /*
                            if (linedata[0].Equals("URL"))
                            {
                                WebserviceConstants.CLIENT_URL = linedata[1];
                            }
                            if (linedata[0].Equals("PRIVATEURL"))
                            {
                                WebserviceConstants.PRIVATE_CLIENT_URL = linedata[1];
                            }
                            if (linedata[0].Equals("Serial"))
                            {
                                Constants.SERIAL_NUMBER = linedata[1];
                            }
                            if (linedata[0].Equals("Client"))
                            {
                                id = Convert.ToInt32(linedata[1]);
                            }
                            */

                        }
                    }
                    fileReader.Close();
                    fileReader.Dispose();
                }

                //var propertiesDAO = new PropertiesDAO
                //{
                //    name = Constants.SERIAL_NUMBER,
                //    url = WebserviceConstants.CLIENT_URL,
                //    private_url = WebserviceConstants.PRIVATE_CLIENT_URL,
                //    clientId = id,
                //    PrinterType = Constants.PRINTER_TYPE
                //};
                //SaveAppProperties(propertiesDAO);
            }
            catch (Exception e)
            {
                LoggingManager.LogApplicationError(e, null, "ReadConfigData");
            }

            return (retValue);
        }

    }

}