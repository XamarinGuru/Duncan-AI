using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Duncan.AI.Droid.Utils.HelperManagers;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Duncan.AI.Droid.Utils.EditControlManagement.Controls
{
    public class CustomAutoCompleteTextViewAdapter2 : BaseAdapter, IFilterable
    {
        string[] items;
        ArrayFilterr filterr;
        public string[] OriginalItems
        {
            get { return this.items; }
            set 
            { 
                this.items = value; 
            }
        }
        Context context;

        public CustomAutoCompleteTextViewAdapter2(Context context) : base( )
        {
            this.context = context;
        }
        public override int Count
        {
            get 
            {
                if (items == null)
                {
                    return 0;
                }
                else
                {
                    return items.Length;
                }
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            if (items == null)
            {
                return null;
            }
            else
            {
                return items[position];
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {

            //convertView = View.Inflate(context, Resource.Layout.CustomAutoTextViewDialogItem, null);
            //var text = convertView.FindViewById<TextView>(Resource.Id.textView1);

            //text.Text = items[position];

            //return convertView;


            View view = convertView;
            if (view == null)
            {
                //view = View.Inflate(context, Android.Resource.Layout.SimpleDropDownItem1Line, null);
                view = View.Inflate(context, Resource.Layout.autocompletetextview_dropdown_item, null);
            }

            if (view == null)
            {
                return null;
            }
            else
            {
                view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = items[position];

                return view;
            }
        }

        public Filter Filter
        {
            get
            {
                if (filterr == null)
                {
                    filterr = new ArrayFilterr();

                    filterr.OriginalData = this.OriginalItems;

                    filterr.SAdapter = this;

                    NotifyDataSetInvalidated();
                    NotifyDataSetChanged();

                }
                return filterr;
            }
        }


    }



    public class ArrayFilterr : Filter
    {
        string[] originalData;
        public string[] OriginalData
        {
            get { return this.originalData; }
            set { this.originalData = value; }
        }

        CustomAutoCompleteTextViewAdapter2 adapter;
        public CustomAutoCompleteTextViewAdapter2 SAdapter
        {
            get { return adapter; }
            set { this.adapter = value; }
        }



        protected override Filter.FilterResults PerformFiltering(Java.Lang.ICharSequence constraint)
        {

            try
            {
                FilterResults oreturn = new FilterResults();
                if (constraint == null || constraint.Length() == 0)
                {
                    if (this.OriginalData != null)
                    {
                        oreturn.Values = this.OriginalData;
                        oreturn.Count = this.OriginalData.Length;
                    }
                    else
                    {
                        Console.WriteLine("defend against early voting");
                    }
                }
                else
                {


                    // ver - original

                    //if (this.OriginalData != null)
                    //{
                    //    oreturn.Values = this.OriginalData;
                    //    oreturn.Count = this.OriginalData.Length;
                    //}
                    //else
                    //{
                    //    Console.WriteLine("defend against early voting");
                    //}



                    /////////////////////////
                    // ver - rev 1

                    //string[] actualResults = new string[this.originalData.Length];
                    //int i = 0;
                    //foreach (string str in this.originalData)
                    //{
                    //    if (str.ToUpperInvariant().Contains(constraint.ToString().ToUpperInvariant()))
                    //        //if (str.ToUpperInvariant().StartsWith(constraint.ToString().ToUpperInvariant()))
                    //        {
                    //        actualResults[i] = str;
                    //        i++;
                    //    }
                    //}
                    //oreturn.Values = actualResults;
                    //oreturn.Count = actualResults.Length;



                    /////////////////////////
                    // ver - rev 2


                    string loConstraint = constraint.ToString().ToUpperInvariant();
                    if (string.IsNullOrEmpty(loConstraint) == false)
                    {
                        // remove abbrev descriptions when present
                        int loPosSep = loConstraint.IndexOf(Constants.LIST_ITEM_DESCRIPTION_SEPARATOR);
                        // has to be beyond the first char
                        if (loPosSep > 0)
                        {
                            // keep everything up to the space preceeding the seperator char
                            loConstraint = loConstraint.Substring(0, loPosSep - 1);
                        }
                    }



                    List<string> loFilteredResults = new List<string>();
                    foreach (string str in this.originalData)
                    {

                        if (str.ToUpper().Contains(loConstraint.ToUpper()))
                        //if (str.ToUpperInvariant().Contains(loConstraint))
                        //if (str.ToUpperInvariant().StartsWith(constraint.ToString().ToUpperInvariant()))
                        {
                            loFilteredResults.Add(str);
                        }
                    }

                    oreturn.Values = loFilteredResults.ToArray();
                    oreturn.Count = loFilteredResults.Count;

                }

                return oreturn;
            }
            catch (Exception exp)
            {
                LoggingManager.LogApplicationError(exp, "ArrayFilterR", "FilterResults");
                Console.WriteLine("Exception caught in process: {0} {1}", exp, "ArrayFilterR-FilterResults");

                FilterResults oreturn = new FilterResults();
                return oreturn;
            }

        }

        protected override void PublishResults(Java.Lang.ICharSequence constraint, Filter.FilterResults results)
        {
            try
            {
                if (this.SAdapter == null)
                {
                    return;
                }

                if (results == null)
                {
                    return;
                }



                if (results.Count == 0)
                {
                    this.SAdapter.NotifyDataSetInvalidated();
                }
                else
                {
                    SAdapter.OriginalItems = (string[])results.Values;
                    SAdapter.NotifyDataSetChanged();
                }
            }
            catch (Exception exp)
            {
                LoggingManager.LogApplicationError(exp, "ArrayFilterR", "PublishResults");
                Console.WriteLine("Exception caught in process: {0} {1}", exp, "ArrayFilterR-PublishResults");
            }
                
        }
    }

}