using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Duncan.AI.Droid.Utils.HelperManagers;

namespace Duncan.AI.Droid.Utils.EditControlManagement.Controls
{


    /// <summary>
    /// Custom Adapter that overrides the drop down filters so we will always get the whole list.. adapted from stackoverflow post
    /// 
    /// http://stackoverflow.com/questions/8512762/autocompletetextview-disable-filtering/9286980#9286980
    /// 
    /// </summary>
    public class CustomAutoCompleteTextViewAdapter : ArrayAdapter
    {
        private KNoFilter filter;
        public JavaList items;

        //public JavaList _originalData;
        //public List<string> _originalData;
        //public List<string> _itemsAtCreation;

        public override Filter Filter
        {
            get
            {
                //return base.Filter;
                return filter;
            }
        }

        public override void NotifyDataSetChanged()
        {
            base.NotifyDataSetChanged();
        }


        public CustomAutoCompleteTextViewAdapter(Context context, int textViewResourceId, JavaList objects)
            : base(context, textViewResourceId, objects)
        {
            //Log.v("Krzys", "Adapter created " + filter);
            items = objects;


            //_originalData = new List<string>(objects.ToArray());

            filter = new KNoFilter(this);
        }
    }

    class KNoFilter : Filter
    {

        private readonly CustomAutoCompleteTextViewAdapter _adapter;
        public KNoFilter(CustomAutoCompleteTextViewAdapter iAdapter)
        {
            _adapter = iAdapter;
        }



        protected override FilterResults PerformFiltering(Java.Lang.ICharSequence constraint)
        {
            FilterResults result = new FilterResults();

            if (_adapter != null)
            {
                if (_adapter.items != null)
                {
                    result.Values = _adapter.items;
                    result.Count = _adapter.items.Count;
                }
            }

            return result;
        }


        //protected override FilterResults PerformFiltering(Java.Lang.ICharSequence constraint)
        //{
        //    var returnObj = new FilterResults();

        //    var results = new List<string>();

        //    if (_adapter._originalData == null)
        //    {
        //        _adapter._originalData = _adapter._items;
        //    }

        //    if (constraint == null) return returnObj;

        //    if (_adapter._originalData != null && _adapter._originalData.Any())
        //    {
        //        // Compare constraint to all names lowercased. 
        //        // It they are contained they are added to results.
        //        results.AddRange(
        //            _adapter._originalData.Where(
        //                chemical => chemical.Name.ToLower().Contains(constraint.ToString())));
        //    }

        //    // Nasty piece of .NET to Java wrapping, be careful with this!
        //    returnObj.Values = FromArray(results.Select(r => r.ToJavaObject()).ToArray());
        //    returnObj.Count = results.Count;

        //    constraint.Dispose();

        //    return returnObj;
        //}





        protected override void PublishResults(Java.Lang.ICharSequence constraint, FilterResults results)
        {
            try
            {

                //NotifyDataSetChanged();   getting this too work will fix the anticipation?
                _adapter.NotifyDataSetChanged();

                // for the filtered results using the JNI as in the chemical example code
                // Don't do this and see GREF counts rising
                if (constraint != null)
                {
                    constraint.Dispose();
                }

                if (results != null)
                {
                    results.Dispose();
                }
            }
            catch (System.Exception exp)
            {
                LoggingManager.LogApplicationError(exp, "KNoFilter", "PublishResults");
                Console.WriteLine("Exception caught in process: {0} {1}", exp, "KNoFilter-PublishResults");
            }
                

        }
    }


}