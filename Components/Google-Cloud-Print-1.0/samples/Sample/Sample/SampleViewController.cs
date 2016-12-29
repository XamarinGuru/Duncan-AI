using System;
using System.Drawing;
using Google.CloudPrint;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Linq;
using System.IO;
using System.Collections.Generic;
namespace Sample
{
	public partial class SampleViewController : UIViewController
	{
		// Please use the attached keychain handler static class to save credentials.

		string UserName = "";
		string Password = "";

		public SampleViewController () : base ("SampleViewController", null)
		{
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Print.TouchUpInside += delegate {


				var printer = new GoogleCloudPrint {
					UserName = UserName,
					Password = Password
				};
				
				if (printer.GetPrinters ().success) {
					var data = File.ReadAllBytes ("dropbox.pdf");
					var job = printer.PrintDocument (printer.Printers[0].id, "Title", data, "application/pdf");
					new UIAlertView ("Print Job", job.success ? "Succeeded" : "Failed", null, "Ok", null).Show ();
				}

			};
			// Perform any additional setup after loading the view, typically from a nib.
		}

		void PrintAsync()
		{
			
			var printer = new GoogleCloudPrint();
			printer.UserName = UserName;
			printer.Password = Password;
			printer.GetPrintersAsync ().ContinueWith (t => { 
				if(t.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
				{
					CloudPrinters printersRequest = t.Result;
					if(printersRequest.success)
					{
						var data = File.ReadAllBytes ("dropbox.pdf");
						var job = printer.PrintDocument (printer.Printers[0].id, "Title", data, "application/pdf");
						new UIAlertView ("Print Job", job.success ? "Succeeded" : "Failed", null, "Ok", null).Show ();
					}	
				}
			});
		}
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
	}
}

