using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Duncan.AI.Droid.Utils.HelperManagers;
using Duncan.AI.Droid.Utils.PickerDialogs;
using Duncan.AI.Droid.Utils.EditControlManagement.Controls;

using Object = Java.Lang.Object;
using String = Java.Lang.String;

namespace Duncan.AI.Droid.Utils.ClickListeners
{
    public class DatePickerClickListener: Object, View.IOnClickListener, DatePickerDialog.IOnDateSetListener, IDialogInterfaceOnCancelListener, IDialogInterfaceOnDismissListener
    {
        readonly EditText _editText;
        private readonly string _editMask;
        private readonly Context _context;
        private bool _allowEditing ;
        private DateTime _date;
        private bool _forceCanel;
        private CustomDatePickerDialog dateDialog;

        private string _DialogTitle = string.Empty;

        public DatePickerClickListener(Context context, EditText editText, string editMask, int intParamForceCurrDtTime)
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
                _date = DateTime.Now.AddDays(intParamForceCurrDtTime);
            }
            else
            {
                // just use right now as default
                _date = DateTime.Now;
            }
            

            // convert to display string
            if (string.IsNullOrEmpty(_editMask) == false)
            {
                _editText.Text = _date.ToString(_editMask);
            }



            _allowEditing = false;
            _forceCanel = false;
        }

     

        public void OnClick(View v)
        {
            if (!_allowEditing)
            {
                _forceCanel = false;
                _allowEditing = true;

                // is there something already in the field?
                if (_editText.Text.Length > 0)
                {
                    // and there us an edit mask?
                    if (_editMask.Length > 0)
                    {
                        DateTime loOSDate = DateTime.Now;

                        // first convert it to a DateTime object
                        if (DateTimeManager.DateStringToOSDate(_editMask, _editText.Text, ref loOSDate) != 0)
                        {
                            string loMethodName = "DatePickerClickListener.OnClick";
                            string loErrMsg = "Error converting DateStringToOSDate( " +
                                              "*" + _editMask + "*" + " " + 
                                              "*" + _editText.Text + "*" + 
                                              ", ref DateTime )";
                            LoggingManager.LogApplicationError(null, loErrMsg, loMethodName);
                            Console.WriteLine(" Error in " + loMethodName + ": " + loErrMsg);
                        }
                        else
                        {
                            // coversion OK, use it
                            _date = loOSDate;
                        }

                    }


                }



                dateDialog = new CustomDatePickerDialog(_context, this, _date.Year, _date.Month - 1, _date.Day);



                dateDialog.SetCancelable(true);
                dateDialog.SetButton((int)DialogButtonType.Negative, new String("Cancel"), CancelCLick);
                dateDialog.SetButton((int)DialogButtonType.Positive, new String("Save"), SaveCLick);
                dateDialog.SetButton((int)DialogButtonType.Neutral, new String("Clear"), ClearCLick);
                dateDialog.SetOnCancelListener(this);
                dateDialog.SetOnDismissListener(this);


                // must have correct label text - did we get it already?
                if (string.IsNullOrEmpty(_DialogTitle) == true)
                {
                    // get something presentable
                    _DialogTitle = Helper.SafeGetAssociatePromptWinLabel(_editText);
                }
                dateDialog.CustomTitle = _DialogTitle;


                dateDialog.DatePicker.CalendarViewShown = false;
                dateDialog.DatePicker.SpinnersShown = true;

                dateDialog.SetTitle(_DialogTitle);

                dateDialog.Show();
            }
        }

        private void ClearCLick(object sender, DialogClickEventArgs e)
        {
            _forceCanel = true;
            _editText.SetText(new String(""), TextView.BufferType.Normal);
            _date = DateTime.Now;
            _allowEditing = false;
        }

        private void CancelCLick(object sender, DialogClickEventArgs e)
        {
            _forceCanel = true;
            _allowEditing = false;
        }

        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            if (_allowEditing && !_forceCanel)
            {
                _date = new DateTime(year, monthOfYear + 1, dayOfMonth);
            }
        }

        private void SaveCLick(object sender, DialogClickEventArgs e)
        {
            int year = dateDialog.DatePicker.Year;
            int monthOfYear = dateDialog.DatePicker.Month;
            int dayOfMonth = dateDialog.DatePicker.DayOfMonth;

            // save the date for next time
            _date = new DateTime(year, monthOfYear + 1, dayOfMonth);
            _editText.SetText(_date.ToString(_editMask), TextView.BufferType.Normal);


            _allowEditing = false;
            _forceCanel = false;
        }

        public void OnCancel(IDialogInterface dialog)
        {
            _allowEditing = false;
        }

        private string ConvertEditMask(string originalMask)
        {
            if (string.IsNullOrEmpty(originalMask))
                originalMask = "mm/dd/yy";
            //we have to fix up the date formats. DD isnt valid where as dd is, etc.
            //values found in configs so far are:
            //      MM/DD/YYYY 
            //      mm/dd/yyyy
            //      mm/dd/yy
            //      MM/DD/YY

            //    todo - flesh these out and convert them to .net datetimes
            //     WWW - 3 character day of week abreviation.
            //     WWWW - day of week spelled out.
            //     MM           - month number left padded with 0 to 2 digits
            //     mm           - month number trimmed to length
            //     MON          - 3 character month abbreviation
            //     MONTH        - month spelled out.
            //     D or d       - day number trimmed to length.
            //     DD           - day number left padded with 0 to 2 digits
            //     dd           - day number left padded with space to 2 digits
            //     DDD or ddd   - day number within a given year (Used for Julian date formats, ie. "yyyyddd")
            //     YY or yy     - 2 digit year
            //     YYYY or yyyy - 4 digit year


            originalMask = NumericManager.FixDateEditMask(originalMask);
            return originalMask;
        }

        public void OnDismiss(IDialogInterface dialog)
        {
            _allowEditing = false;
        }
    }
}