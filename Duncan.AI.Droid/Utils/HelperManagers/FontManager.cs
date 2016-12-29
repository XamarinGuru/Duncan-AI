using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Graphics;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Duncan.AI.Droid.Utils.EditControlManagement.Controls;

namespace Duncan.AI.Droid.Utils.HelperManagers
{
    class FontManager
    {

//        Fonts - Roboto
//
//          Titles: Roboto Title style, Medium 20sp
//          SubTitles: Roboto Medium 14sp
//          Form Titles: Title style, Medium 20sp
//          Text Fields: Roboto Regular 16sp



        // edit field title label
        public const string cnEditTextLabelTypeface = "Roboto-Medium.ttf";
        public const float cnEditTextLabelTypefaceSizeSp = 14;

        // edit fields
        public const string cnAutoCompleteTextViewTypeface = "Roboto-Regular.ttf";
        public const float cnAutoCompleteTextViewTypefaceSizeSp = 16;

        public const string cnEditTextViewTypeface = "Roboto-Regular.ttf";
        public const float cnEditTextViewTypefaceSizeSp = 16;

        public const string cnTextViewTypeface = "Roboto-Regular.ttf";
        public const float cnTextViewTypefaceSizeSp = 16;


        // section headers
        //public const string cnDividerLabelTypeface = "Roboto-Regular.ttf";
        //public const float cnDividerLabelTypefaceSizeSp = 12;
        public const string cnDividerLabelTypeface = "Roboto-Medium.ttf";
        public const float cnDividerLabelTypefaceSizeSp = 20;





        public const string cnButtonTypeface = "Roboto-Bold.ttf";
        public const float cnButtonTypefaceSizeSp = 20;


        // LPR
        public const string cnLPRDialogHeaderTextTypeface = "Roboto-Bold.ttf";
        public const float cnLPRDialogHeaderTextTypefaceSizeSp = 22;

        public const string cnLPRDialogButtonTypeface = "Roboto-Bold.ttf";
        public const float cnMLPRDialogButtonTypefaceSizeSp = 16;



        // search match cards 
        public const string cnCardViewDialogHeaderTextTypeface = "Roboto-Bold.ttf";
        public const float cnCardViewDialogHeaderTextTypefaceSizeSp = 16;

        public const string cnCardViewHeaderTextClockTypeface = "Roboto-Bold.ttf";
        public const float cnCardViewHeaderTextClockTypefaceSizeSp = 16;


        public const string cnCardViewHeaderTextLargeTypeface = "Roboto-Medium.ttf";
        public const float cnCardViewHeaderTextLargeTypefaceSizeSp = 20;

        public const string cnCardViewHeaderTextMediumTypeface = "Roboto-Regular.ttf";
        public const float cnCardViewHeaderTextMediumTypefaceSizeSp = 18;

        public const string cnCardViewHeaderTextSmallTypeface = "Roboto-Regular.ttf";
        public const float cnCardViewHeaderTextSmallTypefaceSizeSp = 14;

        public const string cnCardViewHeaderTextSmallestTypeface = "Roboto-Regular.ttf";
        public const float cnCardViewHeaderTextSmallestTypefaceSizeSp = 12;


        public const string cnCardViewSummaryTextLargeTypeface = "Roboto-Regular.ttf";
        public const float cnCardViewSummaryTextLargeTypefaceSizeSp = 20;

        public const string cnCardViewSummaryTextSmallTypeface = "Roboto-Regular.ttf";
        public const float cnCardViewSummaryTextSmallTypefaceSizeSp = 16;


        public const string cnCardViewCounterTextSmallTypeface = "Roboto-Regular.ttf";
        public const float cnCardViewCounterTextSmallTypefaceSizeSp = 12;






        // pop-up menu for switching between activity fragments
        public const string cnMenuPopupTitleTypeface = "Roboto-Medium.ttf";
        public const float cnMenuPopupTitleTypefaceSizeSp = 28;

        public const string cnMenuPopupButtonParentItemTypeface = "Roboto-Medium.ttf";
        public const float cnMenuPopupButtonParentItemTypefaceSizeSp = 20;

        public const string cnMenuPopupButtonChildActionTypeface = "Roboto-Medium.ttf";
        public const float cnMenuPopupButtonChildActionTypefaceSizeSp = 20;

        public const string cnMenuPopupButtonExitTypeface = "Roboto-Medium.ttf";
        public const float cnMenuPopupButtonExitTypefaceSizeSp = 20;



        // issue form navigation button
        public const string cnIssueFormNavigationTypeface = "Roboto-Medium.ttf";
        public const float cnIssueFormNavigationTypefaceSizeSp = 21;


        // general messages
        public const string cnTextMessageLargeTypeface = "Roboto-Medium.ttf";
        public const float cnTextMessageLargeSizeSp = 22;


        public const string cnTextMessageMediumTypeface = "Roboto-Medium.ttf";
        public const float cnTextMessageMedimSizeSp = 18;

        public const string cnTextMessageSmallTypeface = "Roboto-Medium.ttf";
        public const float cnTextMessageSmallSizeSp = 16;



        // web view pages
        public const string cnWebViewErrorMessageTypeface = "Roboto-Medium.ttf";
        public const float cnWebViewErrorMessageSizeSp = 22;


        public const string cnWebViewErrorMessageDetailTypeface = "Roboto-Medium.ttf";
        public const float cnWebViewErrorMessageDetailSizeSp = 18;

        public const string cnWebViewErrorMessageFinePrintTypeface = "Roboto-Medium.ttf";
        public const float cnWebViewErrorMessageFinePringtSizeSp = 16;






        public const string cnSignatureImageViewTypeface = "Roboto-BlackItalic.ttf";
        public const float cnSignatureImageViewTypefaceSizeSp = 20;
        
        public const string cnSpinnerTypeface = "Roboto-BlackItalic.ttf";
        public const float cnSpinnerTypefaceSizeSp = 20;



        public static class AndroidTypefaceUtility
        {
            static AndroidTypefaceUtility()
            {
            }
            //Refer to the code block beneath this one, to see how to create a typeface.
            public static void SetTypefaceOfView(View view, Typeface customTypeface)
            {
                if (customTypeface != null && view != null)
                {
                    try
                    {
                        if (view is TextView)
                            (view as TextView).Typeface = customTypeface;
                        else if (view is Button)
                            (view as Button).Typeface = customTypeface;

                        else if (view is CustomAutoTextView)
                            (view as CustomAutoTextView).Typeface = customTypeface;

                        else if (view is CustomEditText)
                            (view as CustomEditText).Typeface = customTypeface;

                        else if (view is EditText)
                            (view as EditText).Typeface = customTypeface;
                        else if (view is ViewGroup)
                            SetTypefaceOfViewGroup((view as ViewGroup), customTypeface);
                        else
                            Console.Error.WriteLine("AndroidTypefaceUtility: {0} is type of {1} and does not have a typeface property", view.Id, typeof(View));
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine("AndroidTypefaceUtility threw:\n{0}\n{1}", ex.GetType(), ex.StackTrace);
                        throw ex;
                    }
                }
                else
                {
                    Console.Error.WriteLine("AndroidTypefaceUtility: customTypeface / view parameter should not be null");
                }
            }

            public static void SetTypefaceOfViewGroup(ViewGroup layout, Typeface customTypeface)
            {
                if (customTypeface != null && layout != null)
                {
                    for (int i = 0; i < layout.ChildCount; i++)
                    {
                        SetTypefaceOfView(layout.GetChildAt(i), customTypeface);
                    }
                }
                else
                {
                    Console.Error.WriteLine("AndroidTypefaceUtility: customTypeface / layout parameter should not be null");
                }
            }

        }




        class TypefaceContainer
        {
            public Typeface typeface;
            public string fontname;
        }

        private static List<TypefaceContainer> typefaces = new List<TypefaceContainer>();
        private static Typeface GetTypeface(string ifontname)
        {
            foreach (TypefaceContainer oneTypeface in typefaces)
            {
                if (oneTypeface.fontname.Equals(ifontname) == true)
                {
                    return oneTypeface.typeface;
                }
            }

            return null;
        }


        public static Typeface GetTypeface(Context ctx, string fontname)
        {
            Typeface returnTypeface = GetTypeface(fontname);
            if (returnTypeface == null)
            {
                try
                {
                    returnTypeface = Typeface.CreateFromAsset(ctx.Assets, fontname);

                    TypefaceContainer newTypeface = new TypefaceContainer();
                    newTypeface.typeface = returnTypeface;
                    newTypeface.fontname = fontname;

                    typefaces.Add(newTypeface);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("AndroidTypefaceUtility threw:\n{0}\n{1}", ex.GetType(), ex.StackTrace);
                }

            }

            return returnTypeface;
        }




    }
}