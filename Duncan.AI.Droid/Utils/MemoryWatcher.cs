using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Threading;

using Duncan.AI.Droid.Utils.HelperManagers;
using Reino.ClientConfig;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Duncan.AI.Droid.Utils
{
    class MemoryWatcher
    {

        public static void DeleteApplicationCache(Context context)
        {
            try
            {
                Java.IO.File dir = context.CacheDir;
                if (dir != null && dir.IsDirectory)
                {
                    deleteDir(dir);
                }
            }
            catch (Exception ex) 
            {
                LoggingManager.LogApplicationError(ex, "MemoryWatcher.DeleteApplicationCache", ex.TargetSite.Name);
                System.Console.WriteLine("MemoryWatcher.DeleteApplicationCache source {0}: {1}", ex.Source, ex.ToString());
            }
        }

        public static bool deleteDir(Java.IO.File dir)
        {
            try
            {
                if (dir != null && dir.IsDirectory)
                {
                    String[] children = dir.List();
                    for (int i = 0; i < children.Length; i++)
                    {
                        bool success = deleteDir(new Java.IO.File(dir, children[i]));
                        if (!success)
                        {
                            return false;
                        }
                    }
                }
                return dir.Delete();
            }
            catch (Exception ex)
            {
                LoggingManager.LogApplicationError(ex, "MemoryWatcher.deleteDir", ex.TargetSite.Name);
                System.Console.WriteLine("MemoryWatcher.deleteDir source {0}: {1}", ex.Source, ex.ToString());
            }

            return false;
        }




        public static void RestartApplication( Context context )
        {

            // try to clean up
            if ( DroidContext.mainActivity != null )
            {
                try
                {

                    Droid.DroidContext.mainActivity.CleanUpResourcesForShutdown();
                }
                catch (Exception ex)
                {
                    LoggingManager.LogApplicationError(ex, "MemoryWatcher.RestartApplication", ex.TargetSite.Name);
                    System.Console.WriteLine("MemoryWatcher.RestartApplication Exception source {0}: {1}", ex.Source, ex.ToString());
                }

            }





        #if _reset_1_
            ////////////

            Intent restartIntent = context.PackageManager.GetLaunchIntentForPackage(context.PackageName);
            PendingIntent intent = PendingIntent.GetActivity(
                    context, 0,
                    restartIntent, PendingIntentFlags.CancelCurrent);

            AlarmManager manager = (AlarmManager)context.GetSystemService(Context.AlarmService);
            manager.Set(AlarmType.Rtc, Java.Lang.JavaSystem.CurrentTimeMillis() + iMSdelay, intent);


            Java.Lang.JavaSystem.Exit(2);

            ////////////////////////
#else

            Intent restartIntent = context.PackageManager.GetLaunchIntentForPackage(context.PackageName);
            if (restartIntent != null)
            {
                restartIntent = null;
            }



            Intent mStartActivity = new Intent(Application.Context, typeof(SplashActivity));
            int mPendingIntentId = 123456;

            PendingIntent intent = PendingIntent.GetActivity(context, mPendingIntentId, mStartActivity, PendingIntentFlags.CancelCurrent);


            AlarmManager manager = (AlarmManager)context.GetSystemService(Context.AlarmService);
            //manager.Set(AlarmType.Rtc, Java.Lang.JavaSystem.CurrentTimeMillis() + 100, intent);
            manager.Set(AlarmType.Rtc, Java.Lang.JavaSystem.CurrentTimeMillis() + 500, intent);
            
            //System.exit(0);
            Java.Lang.JavaSystem.Exit(0);

            /////////////////////
#endif
}



        /// <summary>
        /// Restart the app from boot
        /// 
        /// Using this strategy after clean the complex resources mess left over from sync and reload of XML 
        /// 
        /// Also using when mem/resources get low
        /// 
        /// Adapted from 
        /// http://stackoverflow.com/questions/2470870/force-application-to-restart-on-first-activity
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="iMSdelay"></param>
        public static void RestartApp(string iBootModeTag, Activity iActivity, Context context, int iMSdelay)
        {
            if (iMSdelay == 0)
            {
                iMSdelay = 1;
            }

            if (iMSdelay < 10)
            {
                //iMSdelay = 10;
            }

            if (string.IsNullOrEmpty(iBootModeTag) == true)
            {
                iBootModeTag = string.Empty;
            }

            // set the mode
            //ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            //ISharedPreferencesEditor editor = prefs.Edit();
            //editor.PutString(Constants.appsettings_property_name_bootmode, iBootModeTag);
            //editor.Apply();




            //Log.e("", "Reestarting app");
            LoggingManager.LogApplicationError(null, "MemoryWatcher.RestartApp", iBootModeTag);
            System.Console.WriteLine("MemoryWatcher.RestartApp {0}: {1}", iBootModeTag, "");

            // clean up cache
            DeleteApplicationCache(context);

            if (iBootModeTag.Equals(Constants.appsettings_bootmode_value_resync) == true)
            {
                // let em know its gonna happen
                UserNotificationPopUp_SystemRefreshDialog(iActivity, context);
            }
            else
            {
                // just do it!
                RestartApplication(context);
            }


        }


        private static async void AsyncWait(int iMSWait)
        {
            await Task.Delay(iMSWait);
        }

        public static void UserNotificationPopUp_SystemRefreshDialog(Activity iActivity, Context context)
        {

            string[] response = null;


            var ft = iActivity.FragmentManager.BeginTransaction();

            // always new dialog created
            SystemRefreshPopUpDialogFragment oneUserCommentPopUpDialogFragment = new SystemRefreshPopUpDialogFragment { Arguments = new Bundle() };


            // get the user calming text
            string refreshTitle = TTRegistry.glRegistry.GetRegistryValue(
                                                        TTRegistry.regSECTION_ISSUE_AP,
                                                        TTRegistry.regAPP_REFRESH_REASON_TITLE,
                                                        TTRegistry.regAPP_REFRESH_REASON_TITLE_DEFAULT);
            string refreshReason = TTRegistry.glRegistry.GetRegistryValue(
                                                        TTRegistry.regSECTION_ISSUE_AP,
                                                        TTRegistry.regAPP_REFRESH_REASON_TEXT,
                                                        TTRegistry.regAPP_REFRESH_REASON_TEXT_DEFAULT);


            oneUserCommentPopUpDialogFragment.SetUserCommentDialogItems(refreshTitle, refreshReason, null);

            oneUserCommentPopUpDialogFragment.SetRebootParams(iActivity, context);
            //oneUserCommentPopUpDialogFragment.SetCallbackActivity(this, Constants.ISSUE_NOTES_FRAGMENT_TAG_PREFIX);
            oneUserCommentPopUpDialogFragment.Show(iActivity.FragmentManager, Constants.ISSUE_NOTES_FRAGMENT_TAG_PREFIX);

            ft.Commit();

            // we need to see this dialog immediately
            iActivity.FragmentManager.ExecutePendingTransactions();
        }





        /// <summary>
        /// Returns TRUE if circumstances have warranted a restart request.... low resources, too long since last, etc
        /// </summary>
        /// <returns></returns>
        /// 
        private static bool fRestartRequestFlag = false;
        public static bool RestartRequestFlagIsRaised()
        {
            // TO START
            // a simple count
            if (DroidContext.CitationsSinceRestart >= fCitationsBetweenRestart)
            {
                // zero val disables the restart
                if (fCitationsBetweenRestart > 0)
                {
                    fRestartRequestFlag = true;
                }
            }


            return fRestartRequestFlag;
        }


        /// <summary>
        /// Called when OnLowMemory event recieved
        /// </summary>
        public static void RaiseRestartRequestFlag()
        {
            fRestartRequestFlag = true;
        }


        private static int fCitationsBetweenRestart = TTRegistry.regAPP_REFRESH_TICKET_INTERVAL_DEFAULT; 
        public static void InitMemoryWatcher()
        {
            fCitationsBetweenRestart = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP,
                                                            TTRegistry.regAPP_REFRESH_TICKET_INTERVAL,
                                                            TTRegistry.regAPP_REFRESH_TICKET_INTERVAL_DEFAULT);

        }
    }

    //Timer that will clean up memory when AutoIssue app goes in sleep
    public class MemoryCleaningUpTimer : Java.Lang.Object, Java.Lang.IRunnable
    {
        private readonly long fTimerInterval = 0;
        private readonly Handler fHandler = new Handler();
        private static bool fAppOkToRestart = false;

        public MemoryCleaningUpTimer(int iTimerIntervalMin)
        {            
            if(iTimerIntervalMin > 0)
            {
                fTimerInterval = iTimerIntervalMin * 60 * 1000;  // //convert to msec                
            }
        }

        public void Run()
        {
            fHandler.PostDelayed(UpdateTimer, fTimerInterval);
        }

        public void Start()
        {            
            Run();
        }

        public void Stop()
        {            
            fHandler.RemoveCallbacks(UpdateTimer);            
        }
                                
        private void UpdateTimer()
        {
            //Do the cleaning up now
            
            // clean up cache
            MemoryWatcher.DeleteApplicationCache(DroidContext.ApplicationContext);
            GC.Collect(0);
            
            //Re-schedule the timer
            fHandler.PostDelayed(UpdateTimer, fTimerInterval);
            
        }
    }
}