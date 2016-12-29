using System.Collections.Generic;
using System.Text.RegularExpressions;
using Android.Content;
using Android.Text;
using Android.Widget;
using Duncan.AI.Droid.Utils.ClickListeners;
using Duncan.AI.Droid.Utils.EditControlManagement.Controls;
using Duncan.AI.Droid.Utils.InputFilters;
using Duncan.AI.Droid.Utils.TextWatchers;

namespace Duncan.AI.Droid.Utils.HelperManagers
{
    public class EditMaskManager
    {
        private List<IInputFilter> _inputFilters;
        private InputTypes _inputType;
        private int _maxLength;
        private string _editMask;
        public InputTypes DetermineEditMask(string editMask, ref CustomEditText editText, int maxLength,int intParamForceCurrDtTime, string fieldType, Context ctx)
        {
            
            //setup private values to the defaults needed
            _maxLength = maxLength;
            _inputFilters = new List<IInputFilter> { new InputFilterLengthFilter(_maxLength) };
            _inputType = InputTypes.ClassText;
            _editMask = editMask;
            //have to check the type of field. string, date, or time are the options
            switch (fieldType)
            {
                case "efString":
                    HandleStringType(editText);
                    break;
                case "efDate":
                    editText.SetOnClickListener(new DatePickerClickListener(ctx, editText, _editMask, intParamForceCurrDtTime));
                    break;
                case "efTime":
                    editText.SetOnClickListener(new TimePickerClickListener(ctx, editText, _editMask, intParamForceCurrDtTime));
                    break;
            }

            editText.SetFilters(_inputFilters.ToArray());
            return _inputType;
        }

        public InputTypes DetermineEditMask(string editMask, ref CustomAutoTextView editText, int maxLength, int intParamForceCurrDtTime, string fieldType, Context ctx)
        {

            //setup private values to the defaults needed
            _maxLength = maxLength;

            // we're going to allow full display of ABBREV + DESC TEXT for input, then let the routines parse the combos
            int loMaxDisplayLen = 80;
            //_inputFilters = new List<IInputFilter> { new InputFilterLengthFilter(_maxLength) };
            _inputFilters = new List<IInputFilter> { new InputFilterLengthFilter(loMaxDisplayLen) };


            _inputType = InputTypes.ClassText;
            _editMask = editMask;
            //have to check the type of field. string, date, or time are the options
            switch (fieldType)
            {
                case "efString":
                    HandleStringType(editText);
                    break;
                case "efDate":
                    editText.SetOnClickListener(new DatePickerClickListener(ctx, editText, _editMask, intParamForceCurrDtTime));
                    break;
                case "efTime":
                    editText.SetOnClickListener(new TimePickerClickListener(ctx, editText, _editMask, intParamForceCurrDtTime));
                    break;

                default:
                    {
                        // default to all characters be upper case only
                        _inputFilters.Add(new UpperCaseInputFilter());
                        _inputType = InputTypes.TextFlagCapCharacters;
                        break;
                    }
            }

            editText.SetFilters(_inputFilters.ToArray());
            return _inputType;
        }

        private void HandleStringType(EditText editText)
        {
            _inputFilters.Add(new UpperCaseInputFilter());
            //_inputFilters.Add(new InputFilterAllCaps());

            // default to all characters be upper case only - can be overridden by specific mask characters belo
            _inputType = InputTypes.TextFlagCapCharacters;



            //lets handle all the single character masks first
            if (_editMask.Length == 1)
            {
                SetSingleCharacterMasks(editText);
                return;
            }

            //now we need to handle the non-single character formats.
           // manage all fo the non singular values (9999, 0900, X!2, etc)
            //todo
            //contains
            // .
            // -
          
            // XX:XX XX:XX
            
            


            // !! - issue no prefix 
            if (_editMask.Equals("!!"))
            {
         //       _inputFilters.Add(new UpperCaseInputFilter());
                _inputType = InputTypes.TextFlagCapCharacters;
                return;
            }

            // &amp;
            if (_editMask.Equals("&amp;"))
            {
                _inputFilters.Add(new AlphaNumericInputFilter());
         //       _inputFilters.Add(new UpperCaseInputFilter());
                _inputType = InputTypes.TextFlagCapCharacters;
                return;
            }

            //if it is currency, take that into consideration. This watcher also respects "-" values as well.
            if (_editMask.Contains("$") || _editMask.Contains("8"))
            {
                editText.AddTextChangedListener(new CurrencyTextWatcher(_maxLength, _editMask, editText));
                _inputType = InputTypes.ClassText;
                return;
            }

            // if the mask is all numbers, allow only numeric input
            var reNum = new Regex(@"^\d+$");
            if (reNum.Match(_editMask).Success)
            {
                _inputFilters.Add(new NumericInputFilter());
                _inputType = InputTypes.ClassNumber;
                return;
            }
        }

        private  void SetSingleCharacterMasks( EditText editText)
        {
            switch (_editMask)
            {
                //X: All characters are valid
                case "X":
                    _inputType = InputTypes.ClassText;
                    break;
                //!: All characters are valid, but forces uppercase
                case "!":
                  //  _inputFilters.Add(new UpperCaseInputFilter());
                    _inputType = InputTypes.TextFlagCapCharacters;
                    break;
                //A: All characters are valid except numerals
                case "A":
                    _inputFilters.Add(new AlphaInputFilter());
                    _inputType = InputTypes.ClassText;
                    break;
                //9: Only numeral characters are valid (0-9). Also means digit position is optional, and doesn’t need to contain a value.
                case "9":
                    _inputFilters.Add(new NumericInputFilter());
                    _inputType = InputTypes.ClassNumber;
                    break;
                //0: Only numeral characters are valid (0-9). Also means digit position is required, and will be filled with zero when value is not explicitly set.
                case "0":
                    _inputType = InputTypes.NumberFlagDecimal;
                    break;
                //8: Only numeral characters are valid (0-9). Also means the value should be interpreted as a currency value with an implied decimal position in 3rd character from right-hand side.
                case "8":
                    editText.AddTextChangedListener(new CurrencyTextWatcher(_maxLength, _editMask, editText));
                    _inputType = InputTypes.ClassText;
                    break;
                //-:  Specialized token for numeric fields to indicate a negative value is acceptable (but not required).
                case "-":
                    _inputType = InputTypes.NumberFlagSigned;
                    break;
                //&: Only alpha or numeral characters are valid, and forces uppercase.  (A-Z, and 0-9).
                case "&amp;": // this will never be hit, just here for editication. Will be handled in the multi-character mask section.
                case "&":
                    _inputFilters.Add(new AlphaNumericInputFilter());
                 //   _inputFilters.Add(new UpperCaseInputFilter());
                    _inputType = InputTypes.TextFlagCapCharacters;
                    break;
                //@: Only alpha characters are valid, and forces uppercase. (A-Z)
                //N: Only alpha characters are valid, and forces uppercase. (A-Z)
                case "@":
                case "N":
                    _inputFilters.Add(new AlphaInputFilter());
                   // _inputFilters.Add(new UpperCaseInputFilter());
                    _inputType = InputTypes.TextFlagCapCharacters;
                    break;
            }
        }
    }
}