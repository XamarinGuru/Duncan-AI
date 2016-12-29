using System.Linq;
using System.Text.RegularExpressions;
using Android.Text;
using Android.Widget;
using Duncan.AI.Droid.Utils.EditControlManagement.Controls;
using Java.Lang;
using Object = Java.Lang.Object;

namespace Duncan.AI.Droid.Utils.TextWatchers
{
    public class CurrencyTextWatcher : Object, ITextWatcher
    {
        public CurrencyTextWatcher(int maxLength, string editMask,  EditText editText)
        {
            _editText = editText;
            _maxLength = maxLength;
            _editMask = editMask;
        }

        private bool _changesActive = true;
        private readonly string _currencySymbol = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencySymbol;
        private readonly int _maxLength;
        private readonly string _editMask;
        readonly  EditText _editText;

        public void AfterTextChanged(IEditable s)
        {
            //this is a recursive method, so lets test to makes ure something should change first. IF it does, then we can run this.

             var newValue = s.ToString();
                var formattedValue = FormatCurrency(newValue);
                if (_editText.Text == formattedValue)
                    return;
              //need to turn off event textchange here. this will recursivly call this method which calls the text change. 
            if (_editText is CustomEditText)
            {
                var customEt = ((CustomEditText) _editText);
                  if (_changesActive && !customEt.IgnoreEvents)
                  {
                      _changesActive = false;
                      customEt.IgnoreEvents = true;
                      s.Clear();
                      customEt.IgnoreEvents = false;
                      s.Insert(0, formattedValue);
                      _changesActive = true;
                  }
            }

            if (_editText is CustomAutoTextView)
            {
                var customEt = ((CustomAutoTextView)_editText);
                if (_changesActive && !customEt.IgnoreEvents)
                {
                    _changesActive = false;
                    customEt.IgnoreEvents = true;
                    s.Clear();
                    customEt.IgnoreEvents = false;
                    s.Insert(0, formattedValue);
                    _changesActive = true;
                }
            }
        }

        private string FormatCurrency(string value)
        {
            //see if we have to mess with signs for this value
            var addNegative = DetermineNegativeSign(value);
            //strip off negative signs before any other logic, since its an optional character
            value = value.Replace("-", "");
            value = value.TrimStart('0');
            //strip off any extra characters. if it is at max length and they just plugged in 
            if (value.Length >= _maxLength)
                value = value.Substring(0, _maxLength - 1);
            //get rid of the decimal and the curency symbol
            value = Regex.Replace(value, "[^0-9]", "");

            //add a decimal at the third position over
            //one character "0"
            if (value.Length == 0)
                return "";
            
            string returnValue;
            if (value.Length == 1)
                returnValue = _currencySymbol + "0.0" + value;
            else if (value.Length == 2)
                returnValue = _currencySymbol + "0." + value;
            else
            {
                //if its greater than 2, then insert a decimal at the second right most location (12.45 or 1.24)
                value = value.Insert(value.Length - 2, ".");
                //lets get a decimal value out of this first
                //now lets format it to currency
                var decValue = decimal.Parse(value);
                 returnValue = string.Format("{0:C}", decValue);
            }
            //re-add the negative if you need to
            return addNegative ? "-" + returnValue : returnValue;
        }

        private bool DetermineNegativeSign(string value)
        {
            //if the edit mask doesnt allow it, then return false
            //see if the mask allows negatives.
            bool allowsNegative = _editMask.Contains("-");
            if (!allowsNegative)
                return false;
            
            //then, lets handle the negative values:
            //The presence of a “-” symbol indicates that a negative value is allowed (but not required). 
            //During entry, the user can toggle between negative and positive values by entering the “-” symbol on the keyboard. 
            //If the field value is negative, then the negative sign will be present in the left-most character position.
           
            //to test, we just count the signs, and if there are 2, then they are trying to set it back to positive. 
            //If there is only one, it was either aleready neg or they are trying to make it negative
            return value.Count(x => x == '-') == 1;
        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
            
        }
        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
        }
    }
}