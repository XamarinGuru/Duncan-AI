using Android.App;
using Android.Content;
using Java.Lang;

namespace Duncan.AI.Droid.Utils.PickerDialogs
{
    public class CustomDatePickerDialog : DatePickerDialog
    {
        public string CustomTitle { private get; set; }
        private string _previousTitle;

        public CustomDatePickerDialog(Context context, IOnDateSetListener callBack, int year, int monthOfYear,
                                      int dayOfMonth)
            : base(context, callBack, year, monthOfYear, dayOfMonth)
        {
        }

        public override void SetTitle(ICharSequence title)
        {
            if (CustomTitle != null && CustomTitle != _previousTitle)
            {
                _previousTitle = CustomTitle;
                base.SetTitle(new String(CustomTitle));
            }
        }
    }
}