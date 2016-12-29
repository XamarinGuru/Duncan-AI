using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Duncan.AI.Droid.Utils.PickerDialogs;
using Duncan.AI.Droid.Utils.HelperManagers;
using Object = Java.Lang.Object;
using String = Java.Lang.String;

namespace Duncan.AI.Droid.Utils.ClickListeners
{
    public class TimePickerClickListener : Object, View.IOnClickListener, TimePickerDialog.IOnTimeSetListener, IDialogInterfaceOnCancelListener, IDialogInterfaceOnDismissListener
    {
        readonly EditText _editText;
        private readonly string _editMask;
        private readonly Context _context;
        private bool _allowEditing ;
        private int _hour;
        private int _minute;
        private bool _is24Hour;
        private bool _lowerCaseTt;
        private bool _forceCanel ;

        private string _DialogTitle = string.Empty;



        public TimePickerClickListener(Context context, EditText editText, string editMask, int intParamForceCurrDtTime)
        {
            _editText = editText;
            _editText.SetOnClickListener(this);

            // allow focusable so we can iterate through fields
            //_editText.Focusable = false;
            _editText.Focusable = true;
            _editText.FocusableInTouchMode = true;


            _context = context;
            //make sure we convert the mask with what is valid in .net
            _editMask = ConvertEditMask(editMask);





            // default to current date +- some delta days?
            if (intParamForceCurrDtTime > -1 && string.IsNullOrEmpty(_editText.Text))
            {
                // define the date +- delta
                DateTime loForcedDateTime = DateTime.Now.AddDays(intParamForceCurrDtTime);

                _hour = loForcedDateTime.Hour;
                _minute = loForcedDateTime.Minute;

            }
            else
            {
                // just use right now as default
                _hour = DateTime.Now.Hour;
                _minute = DateTime.Now.Minute;
            }


            // convert to display string
            if (string.IsNullOrEmpty(_editMask) == false)
            {

                string loReferenceTimeStr = string.Empty;
                DateTime loReferenceTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, _hour, _minute, 0, DateTimeKind.Local);

                if (DateTimeManager.OsTimeToTimeString(loReferenceTime, _editMask, ref loReferenceTimeStr) == 0)
                {
                    _editText.Text = loReferenceTimeStr;
                }
            }





            _allowEditing = false;
            _forceCanel = false;
        }

        public void OnClick(View v)
        {
            if (!_allowEditing)
            {
                _allowEditing = true;
                _forceCanel = false;


                // is there something already in the field?
                if (_editText.Text.Length > 0)
                {
                    // and there us an edit mask?
                    if (_editMask.Length > 0)
                    {
                        DateTime loOSTime = DateTime.Now;

                        // first convert it to a DateTime object
                        if (DateTimeManager.TimeStringToOSTime(_editMask, _editText.Text, ref loOSTime) != 0)
                        {
                            string loMethodName = "TimePickerClickListener.OnClick";
                            string loErrMsg = "Error converting TimeStringToOSTime( " +
                                              "*" + _editMask + "*" +
                                              " " + 
                                              "*" + _editText.Text + "*" +
                                              ", ref DateTime )";
                            LoggingManager.LogApplicationError(null, loErrMsg, loMethodName);
                            Console.WriteLine(" Error in " + loMethodName + ": " + loErrMsg);
                        }
                        else
                        {
                            // coversion OK, use it
                            _hour = loOSTime.Hour;
                            _minute = loOSTime.Minute;
                        }

                    }


                }



                var dialog = new CustomTimePickerDialog(_context, this, _hour, _minute, _is24Hour);
                dialog.SetButton((int)DialogButtonType.Negative, new String("Cancel"), CancelCLick);
                dialog.SetButton((int)DialogButtonType.Positive, new String("Save"), SaveCLick);
                dialog.SetButton((int)DialogButtonType.Neutral, new String("Clear"), ClearCLick);
                dialog.SetOnCancelListener(this);
                dialog.SetOnDismissListener(this);



                // must have correct label text - did we get it already?
                if (string.IsNullOrEmpty(_DialogTitle) == true)
                {
                    // get something presentable
                    _DialogTitle = Helper.SafeGetAssociatePromptWinLabel(_editText);
                }

                dialog.CustomTitle = _DialogTitle;
                dialog.SetTitle(_DialogTitle);


                dialog.Show();
            }
        }

        private void ClearCLick(object sender, DialogClickEventArgs args)
        {
            _forceCanel = true;
            _editText.SetText(new String(""), TextView.BufferType.Normal);
            _hour = DateTime.Now.Hour;
            _minute = DateTime.Now.Minute;
            _allowEditing = false;
        }

        private void CancelCLick(object sender, DialogClickEventArgs args)
        {
            _forceCanel = true;
            _allowEditing = false;
        }

        public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
        {
            if (_allowEditing && !_forceCanel)
            {
                _hour = hourOfDay;
                _minute = minute;
            }
        }

        private void SaveCLick(object sender, DialogClickEventArgs args)
        {
             var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, _hour, _minute, 0);
             _editText.SetText(FormatTime(date), TextView.BufferType.Normal);
            _forceCanel = false;
            _allowEditing = false;

        }

        public void OnCancel(IDialogInterface dialog)
        {
            _allowEditing = false;
        }

        private string FormatTime(DateTime date)
        {
            //we have to do this to respect the original lowercase and uppercase TT / tt from the original edit mask.
            var time = date.ToString(_editMask);
            if (_lowerCaseTt)
            {
                time = time.Replace("AM", "am");
                time = time.Replace("PM", "pm");
            }
            return time;
        }

        private string ConvertEditMask(string originalMask)
        {
            if (string.IsNullOrEmpty(originalMask))
                originalMask = "hh:mm TT";

            _is24Hour = originalMask.Contains("HH");
            _lowerCaseTt = originalMask.Contains("tt");
            //we have to fix up the date formats. DD isnt valid where as dd is, etc.
            //values found in configs so far are:
            //      hh:mm TT      hh:mm:ss TT      hh:mmTT      hh:mmtt      hh:mm      HHmmtt      ss

            //    todo - flesh these out and convert them to .net datetimes
            //     MM,mm        - minute left padded with 0 to 2 digits
            //     HH,hh        - hour left padded with 0 to 2 digits. (12 hr in presence of TT/tt, 24 hr in absence)
            //     SS,ss        - seconds left padded with 0 to 2 digits
            //     TT           - Uppercase AM/PM
            //     tt           - Lowercase am/pm

            originalMask = NumericManager.FixTimeEditMask(originalMask);
            return originalMask;
        }

        public void OnDismiss(IDialogInterface dialog)
        {
            _allowEditing = false;
        }
    }

}