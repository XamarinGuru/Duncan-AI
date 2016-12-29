using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Lang;
using Com.Zebra.Android.Printer;
using Com.Zebra.Android.Comm;
using System.Drawing;

namespace Duncan.AI.Droid.Common
{
    public class Printer : Activity
    {

        /// <summary>
        /// AJW - this is the version that gets the reproduction view from the webservice. not recommeded!
        /// </summary>
        /// <param name="IssueNum"></param>
        /// <param name="IssueDate"></param>
        /// <returns></returns>
        public System.String PrintTicket(System.String IssueNum, DateTime IssueDate)
        {
            try
            {
                BluetoothAdapter mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;

                if (mBluetoothAdapter == null)
                {
                    return "No Bluetooth Adapter Available";
                }

                if (!mBluetoothAdapter.IsEnabled)
                {
                    return "Bluetooth is not enabled.";
                    /*
                    Intent enableBluetooth = new Intent(BluetoothAdapter.ActionRequestEnable);
                    StartActivityForResult(enableBluetooth, 0);
                    return "Bluetooth was not enabled.. now it is.";
                     */ 
                }
                
                /* 
                try {
                    BluetoothDiscoverer.findPrinters(this, new DiscoveryHandler() {

                        public void foundPrinter(DiscoveredPrinter printer) {
                            String macAddress = printer.address;
                            //I found a printer! I can use the properties of a Discovered printer (address) to make a Bluetooth Link
                        }

                        public void discoveryFinished() {
                            //Discovery is done
                        }

                        public void discoveryError(String message) {
                            //Error during discovery
                        }
                    });
                } catch (ConnectionException e) {
                    e.printStackTrace();
                */

                ICollection<BluetoothDevice> pairedDevices = mBluetoothAdapter.BondedDevices;

                if (pairedDevices.Count > 0)
                {

                    foreach (BluetoothDevice device in pairedDevices)
                    {
                        if (device.BluetoothClass.MajorDeviceClass != Android.Bluetooth.MajorDeviceClass.Imaging )  // || !device Name.Equals("zebra")) {
                        {
                            break;
                        }
                        
                        try
                        {
                            // get the byte array from the web service
                            DuncanWebServicesClient.UserDetailsService userService = new DuncanWebServicesClient.UserDetailsService();
                            byte[] imageBytes = userService.GetTicketBytes(IssueNum, IssueDate);

                            Bitmap imgBitMap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                            // this doesn't work  imgBitMap = CropBitmap(imgBitMap, 0, 0, imgBitMap.Width, imgBitMap.Height-500);

                            BluetoothPrinterConnection connection = new BluetoothPrinterConnection(device.Address);
                            connection.Open();

                            IZebraPrinter printer = ZebraPrinterFactory.GetInstance(connection);
                            printer.GraphicsUtil.PrintImage(imgBitMap, 0, 0, imgBitMap.Width, imgBitMap.Height, false);

                            connection.Close();
                        }

                        catch (SystemException e)
                        {
                            return e.ToString();
                        }
                                
                        return "Ticket printed.";
                        
                    }  // end foreach
                }  // end if

            }
            catch (Java.Lang.Exception e)
            {

                e.PrintStackTrace();
                return e.StackTrace.ToString();
            }

            return "No Paired Bluetooth Printer Found";

        }  // end PrintTicket()

        public Bitmap CropBitmap(Bitmap bitmap, int cropX, int cropY, int cropWidth, int cropHeight)
        {
            Bitmap croppedImage = Bitmap.CreateBitmap(cropWidth, cropHeight, Bitmap.Config.Rgb565);
            Canvas canvas = new Canvas(croppedImage);

            /*
            Rect srcRect = new Rect(0, 0, bitmap.Width, bitmap.Height); 
            Rect dstRect = new Rect(cropX, cropY, cropWidth, cropHeight);

            canvas.DrawBitmap(bitmap, srcRect, dstRect, null);
            */

            canvas.ClipRect(cropX, cropY, cropWidth, cropHeight);
            canvas.DrawBitmap(bitmap, 0, 0, null);

            return croppedImage;
        }

    }  // end class

}  // end namespace
