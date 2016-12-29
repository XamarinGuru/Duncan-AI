using Android.Text;
using Java.Lang;
namespace Duncan.AI.Droid.Utils.InputFilters
{
    public class UpperCaseInputFilter : Object, IInputFilter
    {
        public ICharSequence FilterFormatted(ICharSequence source, int start, int end, ISpanned dest, int dstart, int dend)
        {
            return new String(source.ToString().ToUpper());
        }
    }
}