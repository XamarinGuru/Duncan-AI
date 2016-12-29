Google Cloud Printing for Xamarin.Android, Xamarin.IOS and Xamarin.Mac


```csharp
using Google.CloudPrint;
...
 
void Print ()
{
    var printer = new GoogleCloudPrint {
        UserName = "username",
        Password = "password"
    };
    
    if (printer.GetPrinters ().success) {
        var data = File.ReadAllBytes ("dropbox.pdf");
        var job = printer.PrintDocument (printer.Printers[0].id, "Title", data, "application/pdf");
        new UIAlertView ("Print Job", job.success ? "Succeeded" : "Failed", null, "Ok", null).Show ();
    }
}

void Print()
{
	var printer = new GoogleCloudPrint();
	printer.UserName = "username";
	printer.Password = "password";
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

```

## Other Resources

Github Repo:
https://github.com/slackshot/MonoGCP

Developer Reference:
https://developers.google.com/cloud-print/docs/devguide

Getting Started Guide with Google Cloud Print:
http://www.google.com/landing/cloudprint/