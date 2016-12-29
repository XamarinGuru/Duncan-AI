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

    /// <summary>
    /// 
    /// Helper class to effectivley smooth scroll ScrollView component
    /// 
    /// by calling this method via PostDelayed the scrolling waits until the scroll view is finished measuring and rendering
    /// </summary>
    public class RunnableAnonymousScrollViewClassHelper : Java.Lang.Object, Java.Lang.IRunnable
    {
        private ScrollView mScrollView;
        private int xTarget;
        private int yTarget;

        public RunnableAnonymousScrollViewClassHelper(ScrollView iScrollView, int x, int y)
        {
            this.mScrollView = iScrollView;
            this.xTarget = x;
            this.yTarget = y;
        }

        public void Run()
        {
            mScrollView.SmoothScrollTo(xTarget, yTarget);
        }
    }
}