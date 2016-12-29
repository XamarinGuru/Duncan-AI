using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Duncan.AI.Droid.Utils.HelperManagers;
using Duncan.AI.Droid.Utils.EditControlManagement.Entities;
using XMLConfig;
using System.Collections.Generic;

namespace Duncan.AI.Droid.Utils.EditControlManagement.Controls
{
    public class CustomSpinner : Spinner
    {
        public CustomSpinner(Context context)
            : base(context)
        {
            Ctx = context;
            // Init
            //  SetText("", TextView.BufferType.Normal);
            Visible = true;

            // initialize our typeface 
            Typeface loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnSpinnerTypeface);
            if (loCustomTypeface != null)
            {
               // this.Typeface = loCustomTypeface;
            }

            //SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnSpinnerTypefaceSizeSp);


        }

        #region Members

        protected EditControlBehavior _behavior = null;
        protected DBList _optionsList = null;

        #endregion

        #region Properties

        public Context Ctx { get; set; }

        public EditControlBehavior BehaviorAndroid
        {
            get { return _behavior; }
            set { _behavior = value; }
        }

        public DBList OptionsList
        {
            get { return BehaviorAndroid.PanelField.OptionsList; }
        }

        public EditEnumerations.EditFieldType FieldType
        {
            get { return _behavior.GetFieldType(); }
            set { _behavior.SetFieldType(value); }
        }

        public LinearLayout ParentLayout;
        public Panel ParentPanel;


        public bool Visible { get; set; }
               

        public string Text
        {
            get { return GetText(); }
            set { SetText(value); }
        }

        public int MaxLength
        {
            get { return _behavior.MaxLength; }
            set { _behavior.MaxLength = value; }
        }

        public bool EventsAttached { get; set; }
        public string CustomId { get; set; }
        public string FormStatus { get; set; }
        public bool HasBeenFocused { get; set; }
        private bool DropDownOpen { get; set; }
        #endregion

        #region Methods


        public void HookupEvents(LinearLayout layout, Panel panel)
        {
            ParentLayout = layout;
            ParentPanel = panel;
            if (!EventsAttached)
            {
                this.ItemSelected += CustomSpinner_ItemSelected;
                this.Focusable = true;
                this.FocusableInTouchMode = true;
                this.FocusChange += FocusChangeEventHandler;
                EventsAttached = true;
            }
        }
        public bool IgnoreEvents { get; set; }
        private void CustomSpinner_ItemSelected(object sender, ItemSelectedEventArgs e)
        {
            //if (!IgnoreEvents)
            //{
            //    if (sender is CustomSpinner == false)
            //        return;
            //    var loEditCtrl = ((CustomSpinner)(sender));
            //    EditControlBehavior loBehavior = loEditCtrl.Behavior;
            //    loBehavior.SetEditBuffer(loEditCtrl.Text);
            //    loBehavior.SetText(loEditCtrl.SelectedItem.ToString());

            //    //now validate the field (if we need to)
            //    // Exit if validation is currently disabled
            //    if (loBehavior.ValidationDisabled)
            //        return;

            //    // Are we flagged to skip this validation attempt?
            //    if (loBehavior.SkipNextValidation)
            //    {
            //        // Reset the flag so next validation does occur
            //        loBehavior.SkipNextValidation = false;
            //        return;
            //    }
            //    else
            //    {
            //        // Reset the flag so next validation does occur
            //        loBehavior.SkipNextValidation = false;
            //    }
            //    loBehavior.HandleTextChange();
            //}


            // just for readability
            CustomSpinner customControl = this;


            //position = e.Position;

            string iNewValue = GetItemAtPosition(e.Position).ToString();


            if (string.IsNullOrEmpty(iNewValue))
            {
                customControl.SetListIndex(-1);
                //customControl.SetListIndex(0);

                BehaviorAndroid.SetEditBuffer("");
            }
            else
            {
                customControl.SetListItemDataByValue(iNewValue);
                BehaviorAndroid.SetEditBuffer(iNewValue);
            }


            if (IgnoreEvents == false)
            {

                //if (iSetHasBeenFocused == EditEnumerations.SetHasBeenFocusedType.btSetHasBeenFocusedTrue)
                //{
                //    // TODO - there should be a InitForEntry to take care of this kind of stuff
                //    customControl.HasBeenFocused = true;
                //    //customControl.FormStatus = "Processed";
                //}


                //if (iIgnoreEvents == EditEnumerations.IgnoreEventsType.ieIgnoreEventsTrue)
                //{
                //    customControl.IgnoreEvents = false;
                //}


                //now validate the field (if we need to)
                // Exit if validation is currently disabled
                if (BehaviorAndroid.ValidationDisabled)
                {
                    return;
                }

                // Are we flagged to skip this validation attempt?
                if (BehaviorAndroid.SkipNextValidation)
                {
                    // Reset the flag so next validation does occur
                    BehaviorAndroid.SkipNextValidation = false;
                    return;
                }
                else
                {
                    // Reset the flag so next validation does occur
                    BehaviorAndroid.SkipNextValidation = false;
                }

            }



            // TODO - this isn't needed to called explicitly??
            BehaviorAndroid.HandleTextChange();
        }


        public void ProcessRestrictions(int iNotifyEvent)
        {
            _behavior.ProcessRestrictionsFormInit(iNotifyEvent);
        }

        //public override void OnClick(IDialogInterface dialog, int which)
        //{
        //    base.OnClick(dialog, which);
        //}


        private void FocusChangeEventHandler(object sender, View.FocusChangeEventArgs e)
        {

            // cast for simplified access
            var oneCustomSpinnerCtrl = ((CustomSpinner)(sender));


            //if it is recieving focus, that means that the drop down list either needs to be displayed or will be hidden. 
            if (e.HasFocus == true)
            {
                // is there any selection yet?
                if (oneCustomSpinnerCtrl.BehaviorAndroid != null)
                {
                    //if (oneCustomSpinnerCtrl.Behavior.GetValue().ToUpper().Equals(Constants.SPINNER_DEFAULT_AS_UPPER) == true)
                    //{
                    //    // show the spinner list pop-up, only when a choice has yet to made
                    //    PerformClick();
                    //}

                    if (string.IsNullOrEmpty(oneCustomSpinnerCtrl.BehaviorAndroid.GetValue().Trim()) == true)
                    {
                        try
                        {
                            // show the spinner list pop-up, only when a choice has yet to made
                            PerformClick();
                        }
                        catch (Exception exp)
                        {

                            // AJW - TODO - how to prevent premature access?
                            //if ( Activity.IsFinishing
                            Console.WriteLine(exp.Message);

                            // don't do anything more here
                            return;
                        }

                    }



/*
                    var customAdapter = (ArrayAdapter<String>)Adapter;

                    //if there are no items in the adapter, just leave
                    if (customAdapter != null)
                    {
                        int loIdx = oneCustomSpinnerCtrl.Behavior.ListItemIndex;
                        String itemAsText = (String)customAdapter.GetItem(oneCustomSpinnerCtrl.Behavior.ListItemIndex);


                        if (itemAsText.ToUpper().Equals(Constants.SPINNER_DEFAULT_AS_UPPER) == true)
                        {
                            // show the spinner list pop-up, only when a choice has yet to made
                            PerformClick();
                        }
                    }
 */ 
                }

                

                // change the background to highlight the focused entry
                oneCustomSpinnerCtrl.SetBackgroundResource(Resource.Drawable.EditTextFocused);

                // update global notification
                oneCustomSpinnerCtrl.ParentPanel.FocusedViewCurrent = sender;


                // we done need a keyboard
                Helper.HideKeyboard(oneCustomSpinnerCtrl); // hide the keyboard 

            }
            else
            {
             
               
                //oneCustomSpinnerCtrl.SetBackgroundResource(Resource.Drawable.EditTextFocusLost);

                if (oneCustomSpinnerCtrl.Enabled == true)
                {
                    oneCustomSpinnerCtrl.SetBackgroundResource(Resource.Drawable.EditTextFocusLost);
                }
                else
                {
                    oneCustomSpinnerCtrl.SetBackgroundResource(Resource.Drawable.EditTextDisabled);
                }


                //Helper.HideKeyboard(oneCustomSpinnerCtrl); // hide the keyboard 
                // lets not hide every time, let the next field decide if it should be available or not Helper.HideKeyboard(oneCustomSpinnerCtrl); // hide the keyboard 


                oneCustomSpinnerCtrl.ParentPanel.FocusedViewPrevious = sender;
                if (oneCustomSpinnerCtrl.ParentPanel.FocusedViewCurrent == sender)
                {
                    oneCustomSpinnerCtrl.ParentPanel.FocusedViewCurrent = null;
                }

            }


            // AJW - review how this flow is structured  

            if (!IgnoreEvents)
            {
                if (sender is CustomSpinner == false)
                    return;
                var loEditCtrl = ((CustomSpinner) (sender));
                EditControlBehavior loBehavior = loEditCtrl.BehaviorAndroid;
                //if this control has focus and has not been 
                loBehavior.HandleFocusChange(e.HasFocus);
            }
        }

        public int GetPosition()
        {
            //return SelectedItemPosition > 0 ? SelectedItemPosition : 0;
            return SelectedItemPosition;  // no more (SELECT)
        }

        public string GetText()
        {
            if (SelectedItem != null)
                return SelectedItem.ToString();

            //return Constants.SPINNER_DEFAULT;
            return "";
        }

        public  string GetValue(DBList optionsList)
        {
            if (optionsList != null)
            {
//                string[] response =  (new ListSupport()).GetFilteredListData(optionsList.ListName, optionsList.saveColumn, new List<ListFilter>(_behavior.Filters));

//                //if ((response != null) && (response.Length >= 1) && (SelectedItem != null))
//                //    return this.SelectedItemPosition > 0? response[SelectedItemPosition - 1] : SelectedItem.ToString();

//                if ((response != null) && (response.Length >= 1) && (SelectedItem != null))
//                {
////                    return this.SelectedItemPosition > 0 ? response[SelectedItemPosition - 1] : SelectedItem.ToString();
//                    return response[SelectedItemPosition];  // no more (SELECT)
//                }



                List<string> itemsBySaveCol = _behavior.GetFilteredListItemsBySaveColumn();
                if (itemsBySaveCol != null)
                {
                    int loItemIdx = -1;

                    if ((SelectedItemPosition != null) && (SelectedItemPosition < itemsBySaveCol.Count))
                    {
                        loItemIdx = SelectedItemPosition;
                    }

                    if (loItemIdx != -1 )
                    {
                        return itemsBySaveCol[loItemIdx];
                    }
                }



            }
            return GetText();
        }

        //assuption is that the items are an array of strings.
        private void SetText(string newText)
        {
            ////if hte text is null or empty, we wil be defaulting it to the constants default value here.
            //if (string.IsNullOrEmpty(newText))
            //    newText = Constants.SPINNER_DEFAULT;

            // Don't bother if the text isn't changing
            if (Text == newText)
                return;
            var customAdapter = (ArrayAdapter<String>) Adapter;
            //if there are no items in the adapter, just leave
            if (customAdapter == null)// || !customAdapter.Objects.Any())
                return;

            int spinnerPosition;
            //if behavior is null, just set the spinner position and return
            if (_behavior == null)
            {
                spinnerPosition = customAdapter.GetPosition(newText);
                // code to fix dropdowns
                if (spinnerPosition < 0)
                {
                    spinnerPosition = 0;
                }
                SetSelection(spinnerPosition);
                return;
            }

            // Set the edit buffer with the passed text. 
            // so the final edit buffer may differ.
            _behavior.SetEditBuffer(newText, true);
            // Now set the real text to match the edit buffer
            IgnoreEvents = !IgnoreEvents;
            _behavior.SetText(_behavior.EditBuffer, ReinoControls.WinControl_SetText_Options.sto_ReplaceText, 0, true);
            //we also need ot keep the panel field up to date
            _behavior.PanelField.Value = _behavior.EditBuffer;
            //now try to set the selected item accordingly base on the edit buffer and not the newtext
            SetListItemDataByText(_behavior.EditBuffer);
            IgnoreEvents = !IgnoreEvents; 

        }

        #endregion

        public void AssociateControl(EditControlBehavior behavior )
        {
              _behavior = behavior;
              _behavior.AssociateWithCustomSpinner(this);
        }

        public void SetDataSource(Context context)
        {
            var items =   BehaviorAndroid.GetFilteredListItems();

            //items.Insert(0, Constants.SPINNER_DEFAULT);

            var adr = new ArrayAdapter<String>(context, Android.Resource.Layout.SimpleSpinnerItem, items.ToArray());
            adr.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            Adapter = adr;


            // setting the adapter also results in the Text getting set to the first item - we don't want that
            SetText("");

            // assume no matching spinner item
            //int spinnerPosition = 0;
            int spinnerPosition = -1;
           

            //if (!string.IsNullOrEmpty(Behavior.PanelField.Value) 
            //    && Behavior.PanelField.Value != Constants.SPINNER_DEFAULT)
            if (!string.IsNullOrEmpty(BehaviorAndroid.PanelField.Value))
            {
                //Below 3 lines of code to handle spinners value with colon
                var itemsSaveCol = BehaviorAndroid.GetFilteredListItemsBySaveColumn();

                //itemsSaveCol.Insert(0, Constants.SPINNER_DEFAULT);

                var itemsSaveColAdr = new ArrayAdapter<String>(context, Android.Resource.Layout.SimpleSpinnerItem, itemsSaveCol.ToArray());
                spinnerPosition = itemsSaveColAdr.GetPosition(BehaviorAndroid.PanelField.Value);
            }

                //if this is te second go around and the field isnt blank, means force cleared hasnt fired off and the edit buffer still has data in it.
            //set the list item data by the text of the edit buffer (so the items wth value:value will work)
            else if (!BehaviorAndroid.FieldIsBlank())
            {
                var customAdapter = (ArrayAdapter)Adapter;
                spinnerPosition = customAdapter.GetPosition(BehaviorAndroid.EditBuffer);
            }

            SetListIndex(spinnerPosition);
        }

        //internal void SetListItemDataByText(string text)
        //{
        //    //this method takes some text and tries to set the value based on that text. 
        //    var customAdapter = (ArrayAdapter)Adapter;
        //    //if there are no items in the adapter, just leave
        //    if (customAdapter == null)//|| customAdapter.Objects.Any())
        //        return;
        //    //SetListIndex(customAdapter.GetPosition(text));

        //    // AJW - kludge for demo - let partial matches work
        //    int loIdx = customAdapter.GetPosition(text);
        //    if (loIdx == -1) 
        //    //if ((loIdx == -1) && (text.Equals(Constants.SPINNER_DEFAULT) == false))
        //    {
        //        for (int loItemIdx = 0; loItemIdx < customAdapter.Count; loItemIdx++)
        //        {
        //            string oneItem = (string)customAdapter.GetItem(loItemIdx);
        //            if (oneItem.Length >= text.Length)
        //            {
        //                if (oneItem.Substring(0, text.Length).Equals(text) == true)
        //                {
        //                    //text = oneItem;
        //                    loIdx = loItemIdx;
        //                    break;
        //                }
        //            }
        //        }
        //    }

        //    SetListIndex(loIdx);
        //}

        internal void SetListItemDataByText(string text)
        {
            // if there is an associated behaviour with list, use it, over the adapter list, which is (sometimes) filtered
            int loIdx = -1;
            if (BehaviorAndroid != null)
            {
                // get it from the list
                loIdx = Helper.GetListItemIndexFromStringList(BehaviorAndroid.GetFilteredListItems(), text, Helper.ListItemMatchType.searchAllowPartialMatch);
            }
            else
            {
                // try the adaper
                loIdx = Helper.GetListItemIndexFromArrayAdapter((ArrayAdapter)Adapter, text, Helper.ListItemMatchType.searchAllowPartialMatch);
            }

            // set the index
            SetListIndex(loIdx);
        }


        public   void SetListItemDataByValue(string value)
        {

            // AJW - just re-direct to central/internal method
            SetListItemDataByText(value);
            return;



            //this method takes some text and tries to set the value based on that text. 
            var customAdapter = (ArrayAdapter)Adapter;
            //if there are no items in the adapter, just leave
            if (customAdapter == null)//|| customAdapter.Objects.Any())
                return;



            // AJW - TODO - review - why are we searching a different list ??
            //var response = Behavior.GetFilteredListItemsBySaveColumn();
            var response = BehaviorAndroid.GetFilteredListItems();



            int itemIndex = Array.IndexOf(response.ToArray(), value);
            SetListIndex(itemIndex + 1);
        }

        internal void SetListIndex(int position)
        {
            try
            {
                var customAdapter = (ArrayAdapter<String>)Adapter;

                //if there are no items in the adapter, just leave
                if (customAdapter == null)//|| customAdapter.Objects.Any())
                    return;

                //// code to fix dropdowns
                //if (position < 0)
                //    position = 0;
                ////Thats because spinner have defualt value select at 0 index

                // is it valid?
                if (position < 0)
                {
                    // when the text isn't in the list, there is no index to set - that's ok - not all fields are listonly
                    BehaviorAndroid.ListItemValue = "";
                    BehaviorAndroid.ListItemIndex = -1;
                    return;
                }


                var response = BehaviorAndroid.GetFilteredListItems();
                var item = (String)customAdapter.GetItem(position);

                //get the position before we insert the default.
                //response.Insert(0, Constants.SPINNER_DEFAULT);

                //only do this if we found the item. if not, its probably the (SELECT) so we dont want to set the value to that, we will set it to nothing.
                BehaviorAndroid.ListItemValue = item; // response[position] == item ? response[position] : item;
                BehaviorAndroid.ListItemIndex = position;
                BehaviorAndroid.SetText(item);
                              


                SetSelection(position);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }


        // AJW TODO - the list should be stored in this object?
        public void SpinnerSpin( bool iSpinToNextPosition)
        {
            try
            {
                var customAdapter = (ArrayAdapter<String>)Adapter;

                //if there are no items in the adapter, just leave
                if (customAdapter == null)//|| customAdapter.Objects.Any())
                    return;

                
                var response = BehaviorAndroid.GetFilteredListItems();

                int loIdx = BehaviorAndroid.ListItemIndex;
                //var item = (String)customAdapter.GetItem(Behavior.ListItemIndex);


                //the default is always in the list
                //response.Insert(0, Constants.SPINNER_DEFAULT);
                
                int loListItemCount = response.Count;

                if ( iSpinToNextPosition == true )
                {
                    // spin forward
                    if ( loIdx < loListItemCount-1 )
                    {
                        loIdx++;
                    }
                    else
                    {
                        // no more (SELECT) in our lists
                        loIdx = 0;
                    }
                }
                else
                {
                   // spin backwards
                    // no more (SELECT) in our lists
                    if (loIdx > 0)
                    {
                        loIdx--;
                    }
                    else
                    {
                        loIdx = loListItemCount-1;
                    }
                }


                //get the position before we insert the default.
                //response.Insert(0, Constants.SPINNER_DEFAULT);

                int position = loIdx;


                string loNewValue = response[position]; //response[position] == item ? response[position] : item;

                BehaviorAndroid.ListItemValue = loNewValue;
                BehaviorAndroid.ListItemIndex = position;
                BehaviorAndroid.SetText(loNewValue);

                SetSelection(position);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

    }
}