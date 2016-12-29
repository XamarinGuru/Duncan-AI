using System;
using Android.OS;
using Android.Widget;
using Duncan.AI.Droid.Utils.HelperManagers;
using Java.Lang;
using Object = Java.Lang.Object;

namespace Duncan.AI.Droid.Utils.EditControlManagement.Controls
{
    public class CustomTimer : Object, IRunnable 
    {
        private readonly LinearLayout _layout;
        private readonly string _editMask;
        private readonly CustomEditText _currentTimeView;
        private readonly Handler mHandler = new Handler();
        public CustomTimer (LinearLayout layout, string editMask, CustomEditText currentTimeView)
        {
            _layout = layout;
            _editMask = editMask;
            _currentTimeView = currentTimeView;
        }
        public void Run()
        {
            mHandler.PostDelayed(UpdateTimer, 1000);
        }
        private void UpdateTimer()
        {
            if (_layout != null && !string.IsNullOrEmpty(_editMask))
            {
                if (_currentTimeView != null)
                {
                    string loTmpBuf = "";
                    DateTimeManager.OsTimeToTimeString(DateTime.Now, _editMask, ref loTmpBuf);
                    _currentTimeView.IgnoreEvents = true;
                    _currentTimeView.SetText(loTmpBuf, TextView.BufferType.Normal);
                    _currentTimeView.IgnoreEvents = false;
                    mHandler.PostDelayed(UpdateTimer, 1000);
                }
            }
        }
    }
}