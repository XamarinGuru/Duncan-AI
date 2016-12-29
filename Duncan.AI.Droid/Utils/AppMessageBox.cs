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

namespace Duncan.AI.Droid.Utils
{
    class AppMessageBox
    {

        public static bool QueryUserLastResult = false;


        public static void ShowMessage(string iLine1, string iLine2, string iCaption)
        {
            var builder = new AlertDialog.Builder(DroidContext.ApplicationContext);
            builder.SetTitle(iCaption);
            builder.SetMessage(  iLine1 + "\n" + iLine2);
            builder.SetPositiveButton("OK", delegate
            {
            });

            builder.Show();
        }


        public static void ShowMessageWithBell(string iLine1, string iLine2, string iCaption)
        {
            var builder = new AlertDialog.Builder(DroidContext.ApplicationContext);
            builder.SetTitle(iCaption);
            builder.SetMessage(iLine1 + "\n" + iLine2);
            builder.SetPositiveButton("OK", delegate
            {
            });

            builder.Show();
        }

        public static void ShowMultiLineMessageWithBell(string iText, string iCaption)
        {
            var builder = new AlertDialog.Builder(DroidContext.ApplicationContext);
            builder.SetTitle(iCaption);
            builder.SetMessage(iText);
            builder.SetPositiveButton("OK", delegate
            {
            });

            builder.Show();
        }



// TODO - needs work to be modal and useful


        ///// <summary>
        ///// displays a "Yes/No" form, returns True if user exited by pressing Yes.
        ///// </summary>
        //public static bool QueryUser(string iLine1, string iLine2)
        //{
        //    var builder = new AlertDialog.Builder(DroidContext.ApplicationContext);
        //    builder.SetTitle("");
        //    builder.SetMessage(iLine1 + "\n" + iLine2);
        //    builder.SetPositiveButton("Yes", delegate
        //    {
        //        QueryUserLastResult = true;
        //    });

        //    builder.SetNegativeButton("No", delegate
        //    {
        //        QueryUserLastResult = false;
        //    });

        //    builder.Show();

        //    return QueryUserLastResult;  
        //}


    }
}