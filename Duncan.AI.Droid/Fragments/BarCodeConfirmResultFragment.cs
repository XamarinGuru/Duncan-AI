namespace Duncan.AI.Droid.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Android.App;
    using Android.Content;
    using Android.Content.PM;
    using Android.Graphics;
    using Android.Provider;
    using Android.OS;
    using Android.Runtime;
    using Android.Util;
    using Android.Views;
    using Android.Widget;
    using System.Threading;
    using System.Threading.Tasks;
    using Java.IO;
    using Java.Lang;
    using Duncan.AI.Droid.Common;
    using Duncan.AI.Droid.Utils;
    using Duncan.AI.Droid.Utils.EditControlManagement.Controls;
    using AutoISSUE;
    using XMLConfig;

    [Activity(Label = "BarCodeConfirmResultFragment")]
    public class BarCodeConfirmResultFragment : Activity
    {
        public AlertDialog.Builder fBuilder;
        public List<string> fDataElementList;
        public Context fContext;
        public List<string> fFieldsNamesList;
        public List<string> fFieldsValuesList;
        public static LinearLayout gPanelRootPageLinearLayout = null;
        //public static FormPanel gFormPanel = null;

        public void InitBarCodeConfirmResultFragment()
        {
			if((fDataElementList == null) || (fDataElementList.Count <= 0))
            {
				return;
            }
            fContext = this;		
            fBuilder = new AlertDialog.Builder(fContext);

            fBuilder.SetTitle("BarCode Scanned Data");
            fBuilder.SetMessage("Please confirm the scanned data");
            var layoutVert = new LinearLayout(fContext) { Orientation = Orientation.Vertical };
            foreach (string loStr in fDataElementList)
            {
                TextView loTextView = new TextView(fContext);
                if (loStr  == "") continue;
                loTextView.Text = loStr;
              
                loTextView.Visibility = ViewStates.Visible;
                layoutVert.AddView(loTextView);
            }

            for (int i = 0; i < 2; i++ )
            {
                TextView loTextView = new TextView(fContext);
                loTextView.Text = " ";
                loTextView.Visibility = ViewStates.Visible;
                layoutVert.AddView(loTextView);
            }
            fBuilder.SetView(layoutVert);
            fBuilder.SetPositiveButton("CONFIRM", delegate 
            {
                try
                {
                    //Update current parking ticket fields
                    OnFinishBarCodeScanDialog();
                    ((Activity)fContext).FinishFromChild(this);
                }
                catch (System.Exception exp)
                {
                    System.Console.WriteLine("Failed to confirm scanned barcode:" + exp.Message);
                }
            });

            fBuilder.SetNegativeButton("CANCEL", delegate 
            {
                try
                {
                    ((Activity)fContext).FinishFromChild(this);
                }
                catch (System.Exception exp)
                {
                    System.Console.WriteLine("Failed to confirm scanned barcode:" + exp.Message);
                }
            });

            fBuilder.Show();
        }

        
        // this is being called from OnActivityResult in CommonFragment when user confirms the parsed barcode 2D data
        public void OnFinishBarCodeScanDialog()
        {
            try
            {
                if (fFieldsNamesList == null || fFieldsValuesList == null)
                {
                    return;     //nothing to do
                }

                if (fFieldsNamesList.Count <= 0 ||
                    fFieldsValuesList.Count <= 0)
                {
                    return;     //nothing to do
                }

                // go find all affected fields
                for (int loIdx = 0; loIdx < fFieldsNamesList.Count; loIdx++)
                {
                    string loPanelFieldName = fFieldsNamesList[loIdx];
                    View uiComponent = gPanelRootPageLinearLayout.FindViewWithTag(loPanelFieldName);
                    if (uiComponent != null)
                    {
                        if (uiComponent is CustomEditText)
                        {
                            CustomEditText customView = (CustomEditText)gPanelRootPageLinearLayout.FindViewWithTag(loPanelFieldName);
                            if (customView != null)
                            {
                                customView.HasBeenFocused = true;
                                if (loPanelFieldName.Equals(DBConstants.sqlVehLicExpDateStr))
                                {
                                    customView.Text = fFieldsValuesList[loIdx].Substring(0, 4);  //Only year is needed
                                }
                                else
                                {
                                    customView.Text = fFieldsValuesList[loIdx];
                                }
                            }
                        }
                        else if (uiComponent is CustomAutoTextView)
                        {
                            CustomAutoTextView autoView = (CustomAutoTextView)gPanelRootPageLinearLayout.FindViewWithTag(loPanelFieldName);
                            if (autoView != null)
                            {
                                autoView.HasBeenFocused = true;
                                IListAdapter uiComponentAdapter = autoView.Adapter;
                                int loItemIdx;
                                for (loItemIdx = 0; loItemIdx < uiComponentAdapter.Count; loItemIdx++)
                                {
                                    string loItemStr = (string)uiComponentAdapter.GetItem(loItemIdx);
                                    if (loItemStr.StartsWith(fFieldsValuesList[loIdx]))
                                    {
                                        autoView.SetListItemDataByValue(loItemStr);
                                        break;
                                    }
                                }
                                if (loItemIdx == uiComponentAdapter.Count)
                                {
                                    autoView.SetListItemDataByValue(fFieldsValuesList[loIdx]);
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception exp)
            {
                System.Console.WriteLine("Failed to populate barcode fields:" + exp.Message);
            }
        }
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // get data element list and fields info
            var loElementList = Intent.Extras.GetStringArrayList(Constants.ACTIVITY_INTENT_CONFIRM_BARCODE_PREVIEWLIST_KEY);
            if (loElementList == null) return;
            fDataElementList = (List<string>)loElementList.ToList();
            fFieldsNamesList = Intent.Extras.GetStringArrayList(Constants.ACTIVITY_INTENT_CONFIRM_BARCODE_FIELDSNAMES_KEY).ToList();
            fFieldsValuesList = Intent.Extras.GetStringArrayList(Constants.ACTIVITY_INTENT_CONFIRM_BARCODE_FIELDSVALUES_KEY).ToList();
            InitBarCodeConfirmResultFragment();
        }

        
        protected override void OnRestart()
        {
            base.OnRestart();
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnStop()
        {
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            fBuilder.Dispose();
            base.OnDestroy();
        }
         
    }

}