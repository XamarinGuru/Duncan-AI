using Android.Text;
using Java.Lang;

namespace Duncan.AI.Droid.Utils.InputFilters
{
    /// <summary>
    /// Custom input filter that validates letter - only characters
    /// </summary>
    public class AlphaInputFilter : Object, IInputFilter
    {
        public ICharSequence FilterFormatted(ICharSequence source, int start, int end, ISpanned dest, int dstart, int dend)
        {
            var keepOriginal = true;
            var value = string.Empty;
            for (int i = start; i < end; i++)
            {
                char c = source.CharAt(i);
                if (Character.IsLetter(c))
                    value += c.ToString();
                else
                    keepOriginal = false;
            }
            if (keepOriginal)
                return null;
            var s = source as SpannableString;
            if (s != null)
            {
                var sp = new SpannableString(value);
                TextUtils.CopySpansFrom(s, start, value.Length, null, sp, 0);
                return sp;
            }

            return new String(value);
        }
    }
}