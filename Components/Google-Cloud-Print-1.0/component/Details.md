Google Cloud Print for Xamarin.IOS and Xamarin.Android.

Includes Printer Sharing.

The github repository is public.  

https://github.com/slackshot/MonoGCP

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
```
