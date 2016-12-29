


using System;
using System.Collections.Generic;

using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Views;
using Android.Widget;

using Duncan.AI.Droid.Utils.EditControlManagement.Entities;
using Duncan.AI.Droid.Utils.HelperManagers;
using XMLConfig;

namespace Duncan.AI.Droid.Utils.EditControlManagement.Controls
{
    public class CustomAutoTextView : AutoCompleteTextView, AdapterView.IOnItemClickListener
    {
        public CustomAutoTextView(Context context)
            : base(context)
        {
            Ctx = context;
            // Init
            SetText("", BufferType.Normal);
            Visible = true;


            // initialize our typeface 
            Typeface loCustomTypeface = FontManager.GetTypeface(DroidContext.ApplicationContext, FontManager.cnAutoCompleteTextViewTypeface);
            if (loCustomTypeface != null)
            {
                this.Typeface = loCustomTypeface;
            }

            SetTextSize(Android.Util.ComplexUnitType.Sp, FontManager.cnAutoCompleteTextViewTypefaceSizeSp);

            OnItemClickListener = this;
        }

        #region Members



        protected EditControlBehavior _behavior = null;

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

        public bool Visible { get; set; }

        public new string Text
        {
            get { return base.Text; }
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

        public LinearLayout ParentLayout;
        public Panel ParentPanel;

        #endregion

        public bool IgnoreEvents { get; set; }

        public void HookupEvents(LinearLayout layout, Panel panel)
        {
            ParentLayout = layout;
            ParentPanel = panel;
            if (!EventsAttached)
            {

                // this event doesn't fire, so we extend from AdapterView.IOnItemClickListener and implement onItemClick
                //this.ItemSelected += CustomAutoTextView_ItemSelected; 


                this.BeforeTextChanged += HandleBeforeTextChanged;
                this.TextChanged += HandleTextChanged;
                this.AfterTextChanged += HandleAfterTextChanged;
                this.FocusChange += HandleFocusChange;

                this.Click += CustomAutoTextView_Click;

                EventsAttached = true;
            }
        }



        public void OnItemClick(AdapterView parent, View view, int position, long id)
        //private void CustomAutoTextView_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            // ItemSelected event doesn't fire, so we extend from AdapterView.IOnItemClickListener and implement onItemClick
            //SomeObject whatIwant = ((ArrayAdapter<SomeObject>)this.Adapter).GetItem(position);


            string loText = (string)Adapter.GetItem(position);

            //if (sender is CustomAutoTextView == false)
            //    return;
            //var loEditCtrl = ((CustomAutoTextView)(sender));
            EditControlBehavior loBehavior = this.BehaviorAndroid; // loEditCtrl.Behavior;


            //IListAdapter loSavedAdapter =  this.Adapter;

            //// set it to null so we dont get another dropdown
            //this.Adapter = null;

            //loBehavior.SetEditBuffer(loText);
            //loBehavior.SetText(loText);
            //SetListItemDataByText(loText);

            // this will set the list item index
            SetListItemDataByText(loText);
            // this will set the text value
            SetText(loText); // this is our override



            //// restore it
            //this.Adapter = loSavedAdapter;



            if (!IgnoreEvents)
            {

                //now validate the field (if we need to)
                // Exit if validation is currently disabled
                if (loBehavior.ValidationDisabled)
                    return;

                // Are we flagged to skip this validation attempt?
                if (loBehavior.SkipNextValidation)
                {
                    // Reset the flag so next validation does occur
                    loBehavior.SkipNextValidation = false;
                    return;
                }
                else
                {
                    // Reset the flag so next validation does occur
                    loBehavior.SkipNextValidation = false;
                }

            }


            // this isn't needed to called explicitly??
            // NO don;t need to call explicitly BehaviorAndroid.HandleTextChange();
        }



        public void ShowDropDownAutoTextViewCustom()
        {

            // is an Adapter assigned?
            if (this.Adapter == null)
            {
                return;
            }

            if (Adapter is CustomAutoCompleteTextViewAdapter)
            {
                // just make the list drop down 
                ShowDropDown();
                return;
            }

            if (Adapter is CustomAutoCompleteTextViewAdapter2)
            {
                // clear the filter so all items will show
                //String emptyFilterStr = "*";
                //Java.Lang.ICharSequence filter = emptyFilterStr;//new String(byte[]);   // new String(['*']);
                //(this.Adapter as CustomAutoCompleteTextViewAdapter2).Filter.InvokeFilter(filter);
               // (this.Adapter as CustomAutoCompleteTextViewAdapter2).Filter.InvokeFilter((Java.Lang.ICharSequence)(null));


                // just make the list drop down 
                ShowDropDown();
                return;
            }









            //int loIdx = Helper.GetListItemIndexFromArrayAdapter(myArrayAdapter, loText, Helper.ListItemMatchType.searchAllowPartialMatch);



            //string loText2 = (string)Adapter.GetItem(position);

            //string loText2 = myArrayAdapter.GetItem(myArrayAdapter.GetPosition());

            //string loText = (string)Adapter.GetItem(position);





            // save what's there 
            string loText = Text;


            if (string.IsNullOrEmpty(loText) == true)
            {
                if (Adapter.Count > 0)
                {
                    string loText2 = (string)Adapter.GetItem(0);

                    // if the field is blank, feed it the first value in the list so it will dropdown all items
                    SetText(loText2, false);

                    // we need to keep this because if we leave it blank it doesn't dropdown
                    loText = loText2;

                }
            }
            else
            {
                // clear the existing text so you will see the whole list
                SetText("", false);
            }

            //Adapter.notifydatasetchanged();
            if (Adapter is ArrayAdapter)
            {
                ((ArrayAdapter)Adapter).NotifyDataSetChanged();
            }


            // make the list drop down
            ShowDropDown();

            // restore the text
            SetText(loText);

        }


        void CustomAutoTextView_Click(object sender, EventArgs e)
        {
            if (this.IsPopupShowing == false)
            {
                ShowDropDownAutoTextViewCustom();
            }
        }


        public void ProcessRestrictions(int iNotifyEvent)
        {
            _behavior.ProcessRestrictionsFormInit(iNotifyEvent);
        }


        private void HandleFocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (sender is CustomAutoTextView == false)
                return;
            var loEditCtrl = ((CustomAutoTextView)(sender));
            EditControlBehavior loBehavior = loEditCtrl.BehaviorAndroid;


            if (e.HasFocus == true)
            {
                // edit text from the end when focused
                loEditCtrl.SetSelection(loEditCtrl.Length());

                loEditCtrl.SetBackgroundResource(Resource.Drawable.EditTextDropDownListFocused);

                if (loEditCtrl.ParentPanel != null)
                {
                    loEditCtrl.ParentPanel.FocusedViewCurrent = sender;
                }

                Helper.ShowKeyboard(loEditCtrl); // show the keyboard 
            }
            else
            {     // e.HasFocus == false

                // display text from start when not focused
                loEditCtrl.SetSelection(0, 0);


                if (loEditCtrl.Enabled == true)
                {
                    loEditCtrl.SetBackgroundResource(Resource.Drawable.EditTextFocusLost);
                }
                else
                {
                    loEditCtrl.SetBackgroundResource(Resource.Drawable.EditTextDisabled);
                }


                // lets not hide every time, let the next field decide if it should be available or not Helper.HideKeyboard(loEditCtrl); // hide the keyboard 


                if (loEditCtrl.ParentPanel != null)
                {
                    loEditCtrl.ParentPanel.FocusedViewPrevious = sender;
                    if (loEditCtrl.ParentPanel.FocusedViewCurrent == sender)
                    {
                        loEditCtrl.ParentPanel.FocusedViewCurrent = null;
                    }
                }

            }

            loBehavior.HandleFocusChange(e.HasFocus);
        }

        private void HandleAfterTextChanged(object sender, AfterTextChangedEventArgs e)
        {
            if (!IgnoreEvents)
            {
                // IMPORTANT! Make sure we use the "sender" object instead of "this"
                // because we might be called via the message loop inside a different behavior object!!!
                // We can't do anything if the sender is not a textbox!
                if (sender is CustomAutoTextView == false)
                    return;
                var loEditCtrl = ((CustomAutoTextView)(sender));
                EditControlBehavior loBehavior = loEditCtrl.BehaviorAndroid;

                // Exit if validation is currently disabled
                if (loBehavior.ValidationDisabled)
                    return;

                // Are we flagged to skip this validation attempt?
                if (loBehavior.SkipNextValidation)
                {
                    // Reset the flag so next validation does occur
                    loBehavior.SkipNextValidation = false;
                    return;
                }
                else
                {
                    // Reset the flag so next validation does occur
                    loBehavior.SkipNextValidation = false;
                }
                loBehavior.SetText(loEditCtrl.Text);
                loBehavior.HandleTextChange();
            }
        }

        private void HandleBeforeTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IgnoreEvents)
            {
                //set the edit buffer here so the text change event can fall back if needed.
                if (sender is CustomAutoTextView == false)
                    return;
                var loEditCtrl = ((CustomAutoTextView)(sender));
                EditControlBehavior loBehavior = loEditCtrl.BehaviorAndroid;
                loBehavior.EditBuffer = loEditCtrl.Text;
                //fire off the Before Edit restriction events
                loBehavior.HandleBeforeTextChange();
            }
        }

        public void HandleTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IgnoreEvents)
            {
                // need to keep the visual and abstract components in sync
                if (sender is CustomAutoTextView == false)
                    return;
                var loEditCtrl = ((CustomAutoTextView)(sender));
                EditControlBehavior loBehavior = loEditCtrl.BehaviorAndroid;
                loBehavior.EditBuffer = loEditCtrl.Text;
            }
            else
            {

                // debug breakpoint - we are ignoring the changed value


                // need to keep the visual and abstract components in sync
                if (sender is CustomAutoTextView == false)
                    return;
                var loEditCtrl = ((CustomAutoTextView)(sender));
                EditControlBehavior loBehavior = loEditCtrl.BehaviorAndroid;

                // are they out of sync?
                if (loBehavior.EditBuffer.Equals(loEditCtrl.Text) == false)
                {

                    // debug breakpoint - we are ignoring the changed value!
                    if (sender is CustomAutoTextView == false)
                    {
                        return;
                    }
                    else
                    {
                        return;
                    }
                }

            }
        }


        private void SetText(string newText)
        {
            if (string.IsNullOrEmpty(newText))
                newText = string.Empty;


            // Don't bother if the text isn't changing
            //if (this.Text == newText)
            //    return;

            // don't bother for same data, but make sure text and editbuffer both agree
            if (this.Text.Equals(newText) == true)
            {
                bool loUpdateNeeded = false;
                if (_behavior != null)
                {
                    // if we have a behavior, make sure its editbuffer is considered 
                    loUpdateNeeded = (_behavior.EditBuffer.Equals(newText) == false);
                }

                // we've checked what we can, are we leaving or staying?
                if (loUpdateNeeded == false)
                {
                    return;
                }
            }



            // Can only set base text if there is no behavior object
            if (_behavior == null)
            {
                base.SetText(newText, BufferType.Normal);
                SetSelection(Text == null ? 0 : Text.Length);
                return;
            }

            // Set the edit buffer with the passed text. 
            // so the final edit buffer may differ.
            _behavior.SetEditBuffer(newText, true);



            // Now set the real text to match the edit buffer
            _behavior.SetText(_behavior.EditBuffer, ReinoControls.WinControl_SetText_Options.sto_ReplaceText, 0, true);
            _behavior.PanelField.Value = _behavior.EditBuffer;



            // save the adapter
            var loSavedAdapter = this.Adapter;
            bool loSavedIgnoreEventsState = IgnoreEvents;

            try
            {
                IgnoreEvents = true;
                
                try
                {
                    // set it to null so we dont get another dropdown
                    this.Adapter = null;

                    base.SetText(_behavior.EditBuffer, BufferType.Normal);
                }
                catch (System.Exception exp)
                {
                    LoggingManager.LogApplicationError(exp, "FieldName: " + this._behavior.PanelField.Name, "CustomAutoTextView-SetText(Adapter=null)");
                    Console.WriteLine("Exception caught in process: {0} {1}", exp, this._behavior.PanelField.Name);
                }
                finally
                {
                    // restore it
                    this.Adapter = loSavedAdapter;
                }


                SetSelection(Text == null ? 0 : Text.Length);

                //update the list items for the behavior as well.
                SetListItemDataByText(_behavior.EditBuffer);
            }
            catch (System.Exception exp)
            {
                LoggingManager.LogApplicationError(exp, "FieldName: " + this._behavior.PanelField.Name, "CustomAutoTextView-SetText");
                Console.WriteLine("Exception caught in process: {0} {1}", exp, this._behavior.PanelField.Name);
            }
            finally
            {
                IgnoreEvents = loSavedIgnoreEventsState;
            }


        }

        //private void SetText(string newText, bool iFilter)
        //{
        //    if (string.IsNullOrEmpty(newText))
        //        newText = string.Empty;

        //    // Don't bother if the text isn't changing
        //    if (this.Text == newText)
        //        return;

        //    // Can only set base text if there is no behavior object
        //    if (_behavior == null)
        //    {
        //        if (iFilter == true)
        //        {
        //            base.SetText(newText, BufferType.Normal);
        //        }
        //        else
        //        {
        //            base.SetText(newText, false);
        //        }


        //        SetSelection(Text == null ? 0 : Text.Length);
        //        return;
        //    }

        //    // Set the edit buffer with the passed text. 
        //    // so the final edit buffer may differ.
        //    _behavior.SetEditBuffer(newText, true);
        //    // Now set the real text to match the edit buffer
        //    _behavior.SetText(_behavior.EditBuffer, ReinoControls.WinControl_SetText_Options.sto_ReplaceText, 0, true);
        //    _behavior.PanelField.Value = _behavior.EditBuffer;
        //    IgnoreEvents = !IgnoreEvents;


        //    //update the list items for the behavior as well.
        //    if (iFilter == true)
        //    {
        //        base.SetText(_behavior.EditBuffer, BufferType.Normal);
        //    }
        //    else
        //    {
        //        base.SetText(_behavior.EditBuffer, false);
        //    }

        //    SetSelection(Text == null ? 0 : Text.Length);
        //    SetListItemDataByText(_behavior.EditBuffer);
        //    IgnoreEvents = !IgnoreEvents;

        //}

        //private void SetText(string newText)
        //{
        //    SetText(newText, true);
        //}

        public void AssociateControl(EditControlBehavior behavior)
        {
            _behavior = behavior;
            _behavior.AssociateWithCustomAutoTextView(this);
        }

        //public void SetDataSource()
        //{
        //    var items =   BehaviorAndroid.GetFilteredListItems();
        //    var adr = new ArrayAdapter<String>(Ctx, Android.Resource.Layout.SimpleSpinnerItem, items.ToArray());
        //    adr.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);


        //    int spinnerPosition = adr.GetPosition(BehaviorAndroid.PanelField.Value);
        //    // code to fix dropdowns
        //    if (spinnerPosition < 0)
        //    {
        //        spinnerPosition = 0;
        //    }

        //    Threshold = 1;  // lets see help after 1 letter


        //    SetListIndex(spinnerPosition);
        //    SetBackgroundColor(Color.White);
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

            // set the index, don't set the text
            SetListIndex(loIdx, false);
        }


        internal void SetListItemDataByValue(string value)
        {
            ////this method takes some text and tries to set the value based on that text. 
            //var customAdapter = (ArrayAdapter) Adapter;
            ////if there are no items in the adapter, just leave
            //if (customAdapter == null) //|| customAdapter.Objects.Any())
            //    return;


            // SetText will set the base text and the list item index by lookup
            // SetText(value);

            // save the adapter
            var loSavedAdapter = this.Adapter;


            try
            {
                // set it to null so we dont get another dropdown
                this.Adapter = null;

                // SetText will set the base text and the list item index by lookup
                SetText(value);
            }
            catch (System.Exception exp)
            {
                LoggingManager.LogApplicationError(exp, "FieldName: " + this._behavior.PanelField.Name, "CustomAutoTextView-SetListItemDataByValue(Adapter=null)");
                Console.WriteLine("Exception caught in process: {0} {1}", exp, this._behavior.PanelField.Name);
            }
            finally
            {
                // restore it
                this.Adapter = loSavedAdapter;
            }




            //// AJW - TODO - review - why are we searching a different list if we are passing the result to SetListIndex which looks up AGAIN??
            ////var response = Behavior.GetFilteredListItemsBySaveColumn();
            //var response = Behavior.GetFilteredListItems();


            //int itemIndex = Array.IndexOf(response.ToArray(), value);
            //SetListIndex(itemIndex);

            //// was this value an actual list item?
            //if (itemIndex == -1)
            //{
            //    // it is not a list item, so the setlistindex did not update the text
            //    SetText(value);


            //    //// it is not a list item, so the setlistindex did not update the text
            //    //Behavior.SetText(value);

            //    ////if (setText)
            //    //{
            //    //    // AJW - don't filter or display drop down here
            //    //    //SetText(item);
            //    //    //SetText(item, false);
            //    //    SetText(value);
            //    //}
            //}
        }

        internal void SetListIndex(int position, bool setText = true)
        {
            try
            {
                //var customAdapter = (ArrayAdapter) Adapter;

                ////if there are no items in the adapter, just leave
                //if (customAdapter == null) //|| customAdapter.Objects.Any())
                //    return;

                //// code to fix dropdowns
                //if (position < 0)
                //    position = 0;

                // is it valid?
                if (position < 0)
                {
                    // when the text isn't in the list, there is no index to set - that's ok - not all fields are listonly
                    BehaviorAndroid.ListItemValue = "";
                    BehaviorAndroid.ListItemIndex = -1;
                    return;
                }




                var result = BehaviorAndroid.GetFilteredListItems();
                var response = result.ToArray();


                // AJW - defend against invalid values
                //var item = String.Empty;
                //if (position < customAdapter.Count)
                //{
                //    item = (String)customAdapter.GetItem(position);
                //}

                // the adapter is filtered based on what's been previously keyed, 
                // but are here to set the value by index, so we need to select from the underlying source list
                var item = String.Empty;
                if (position < response.GetLength(0))
                {
                    item = response[position];
                }



                //only do this if we found the item. if not, its probably the (SELECT) so we dont want to set the value to that, we will set it to nothing.
                BehaviorAndroid.ListItemValue = item; //  response[position] == item ? response[position] : item;
                BehaviorAndroid.ListItemIndex = position;
                BehaviorAndroid.SetText(item);

                if (setText)
                {
                    // save the adapter
                    var loSavedAdapter = this.Adapter;

                    // set it to null so we dont get another dropdown
                    this.Adapter = null;

                    // update the field
                    SetText(item);

                    // restore it
                    this.Adapter = loSavedAdapter;
                }
            }
            catch (System.Exception exp)
            {
                LoggingManager.LogApplicationError(exp, BehaviorAndroid.PanelField.Name, "CustomAutoTextView-SetListIndex");
                Console.WriteLine("Exception caught in process: {0} {1}", exp, "CustomAutoTextView-SetListIndex");
            }
        }


        // AJW TODO - the list should be stored in this object?
        public void SpinnerAutoTextViewSpin(bool iSpinToNextPosition)
        {

            // return;  // AJW todo merge autotext and spinner into single edit control


            try
            {

                //var customAdapter = (ArrayAdapter<String>)Adapter;

                ////if there are no items in the adapter, just leave
                //if (customAdapter == null)//|| customAdapter.Objects.Any())
                //    return;

                // AJW TO DO this all seems more dangerous than it should be


                var response = BehaviorAndroid.GetFilteredListItems();
                int loListItemCount = response.Count;


                // if current index < 0, this means the current item is not in the list... that's OK
                int loIdx = BehaviorAndroid.ListItemIndex;


                if (iSpinToNextPosition == true)
                {
                    // room to spin forward?
                    if (loIdx < loListItemCount - 1)
                    {
                        loIdx++;
                    }
                    else
                    {
                        // index is at the end, spin around to 0
                        loIdx = 0;
                    }
                }
                else
                {
                    // spin backwards
                    if (loIdx > 0)
                    {
                        loIdx--;
                    }
                    else
                    {
                        // index at the beginning (or non-list item), spin to the end of the list
                        loIdx = loListItemCount - 1;
                    }
                }


                //get the position before we insert the default.
                //response.Insert(0, Constants.SPINNER_DEFAULT);

                //int position = loIdx;
                //string loValue = response[loIdx];
                //SetListItemDataByValue(loValue);

                int position = loIdx;



                string loNewValue = response[position]; //response[position] == item ? response[position] : item;


                //SetText(loNewValue); // this is our override
                //SetListItemDataByValue(loNewValue);

                // this will set the list item index
                SetListItemDataByText(loNewValue);
                // this will set the text value
                SetText(loNewValue); // this is our override




                //SetText(loNewValue, false);  // this is the view's version of the text


                // this is redundant, these are updayed by SetListItemDataByValue 
                //only do this if we found the item. if not, its probably the (SELECT) so we dont want to set the value to that, we will set it to nothing.
                //Behavior.ListItemValue = loNewValue;
                //Behavior.ListItemIndex = position;
                //Behavior.SetText(loNewValue);

                //SetListItemDataByValue(loNewValue);

                //SetText(loNewValue); // this is our override


                // only for spinner SetSelection(position);



            }
            catch (System.Exception exp)
            {
                LoggingManager.LogApplicationError(exp, BehaviorAndroid.PanelField.Name, "CustomAutoTextView-SpinnerAutoTextViewSpin");
                Console.WriteLine("Exception caught in process: {0} {1}", exp, "CustomAutoTextView-SpinnerAutoTextViewSpin");
            }
        }

    }

   
}