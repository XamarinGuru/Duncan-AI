// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace Sample
{
	[Register ("SampleViewController")]
	partial class SampleViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton Print { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Print != null) {
				Print.Dispose ();
				Print = null;
			}
		}
	}
}
