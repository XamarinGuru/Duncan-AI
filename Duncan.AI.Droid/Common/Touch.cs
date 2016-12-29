using System;
using Android.App;
using Android.Views;

namespace Duncan.AI.Droid.Common
{
    public class Touch : Activity
    {
        public float deltaX, deltaY;
        public float downX, downY, upX, upY;

        public bool ProcessTouch(MotionEvent e)
        {
            deltaX = 0;
            deltaY = 0;

            if (e.Action == MotionEventActions.Down)
            {
                downX = e.GetX();
                downY = e.GetY();

                return true; // allow other events like Click to be processed
            }
            else if (e.Action == MotionEventActions.Up || e.Action == MotionEventActions.Cancel)
            {
                upX = e.GetX();
                upY = e.GetY();

                deltaX = downX - upX;
                deltaY = downY - upY;

                return false;
            }
            else if (e.Action == MotionEventActions.Move)
            {
                return true;
            }
            return false;

        }

    }
}