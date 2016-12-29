using Android.App;
using Android.Content;
using Android.Widget;
using Java.Lang;

namespace Duncan.AI.Droid.Utils.PickerDialogs
{
    public class CustomTimePickerDialog : TimePickerDialog
    {
        public string CustomTitle { private get; set; }
        private string _previousTitle;
        private IOnTimeSetListener onSetcallBack = null;

        public override void OnTimeChanged(TimePicker view, int hourOfDay, int minute)
        {
            onSetcallBack.OnTimeSet(view, hourOfDay, minute);
        }

        public CustomTimePickerDialog(Context context, IOnTimeSetListener callBack, int hourOfDay, int minute,
                                      bool is24HourView)
            : base(context, callBack, hourOfDay, minute, is24HourView)
        {
            onSetcallBack = callBack;
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