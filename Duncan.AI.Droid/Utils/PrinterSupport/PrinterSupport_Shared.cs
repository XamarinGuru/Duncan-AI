
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


using Android.Graphics;
using Android.Views.Animations;
using Android.Animation;
using System.IO;
using Duncan.AI.Droid.Common;
using Android.Util;
using Android.Preferences;
using System.Threading.Tasks;
using Duncan.AI.Droid.Utils.HelperManagers;
using System.Drawing;
using System.Duncan.Drawing;
using Reino.ClientConfig;




namespace Duncan.AI.Droid.Utils.PrinterSupport
{    
    public enum PrintersSupported : int
    {
        Printer_None = 0,
        Printer_BixolonSPP200 = 1,
        Printer_ZebraRW220 = 2,
        Printer_ZebraRW420 = 3,
        Printer_ZebraMZ320 = 4,
        Printer_ZebraiMZ30 = 5,
        Printer_ZebraZQ510 = 6,
        Printer_ZebraZQ520 = 7,
        Printer_PrinTekFieldPro530 = 8,
        Printer_TwoTechnologiesN5Class_Text = 9,
        Printer_TwoTechnologiesN5Class_Graphic = 10,

        // user selection stored by index, do NOT change numbers, only add to the END of the list before this entry
        Printer_ZZZSupportedCount
    }


    // our version of N5 fonts, re-defined internally so we don't have to include the N5 SDK library references in our builds
    public enum N5Fonts
    {
        SAN_SERIF_5_5_CPI = 0,
        SAN_SERIF_10_2_CPI,
        SAN_SERIF_10_7_CPI,
        COURIER_12_7_CPI,
        COURIER_13_5_CPI,
        COURIER_14_5_CPI,
        COURIER_15_6_CPI,
        COURIER_16_9_CPI,
        COURIER_18_5_CPI,
        COURIER_20_3_CPI,
        COURIER_22_6_CPI,
        COURIER_25_4_CPI,
        SAN_SERIF_16_9_CPI,
        SAN_SERIF_18_5_CPI,
        SAN_SERIF_20_3_CPI,
        SAN_SERIF_4_2_CPI,
        //End of text fonts. New text fonts to be added above this comment
        code128,
        code39
        //Add new barcode (non text) fonts here
    }


    public enum N5FontStyleFlags
    {
        STANDARD = 0,
        EMPHASIZED = 1,
        DOUBLEHEIGHT = 2,
        DOUBLESIZE = 4
    }





    //class to collect all printed texts in the ticket, sort and align within paper border
    public class PCLPrintingClass
    {        
        public struct PCLStringObject
        {
            public string strText;
            public int x;
            public int y;
            public N5Fonts font;
            public N5FontStyleFlags fontStyleFlags;
            public int spaces; //preecding spaces based on string font 
            public int fontHeight;
            public Justification_Android Justification;
            public Rotation_Android Rotation;
        }

        public struct PCLStringRow
        {
            public string strText;
            public N5Fonts font;
            public N5FontStyleFlags fontStyleFlags;
            public int forwardFeedVal;
        }

        public PCLPrintingClass()
        {
            //Add init's here
        }

        /// <summary>
        /// Adjust the spaces in the text so that it will fit in one line
        /// </summary>
        /// <param name="dFont"></param>
        /// <returns></returns>
        /// 
        public string AdjustStringLength(string iOrgStr, N5Fonts iFont, N5FontStyleFlags iN5FontStyleFlags)
        {
            if (string.IsNullOrEmpty(iOrgStr) || iFont == null) return iOrgStr;

            int loMaxLineSize = GetCharactersPerLineForN5Font(iFont, iN5FontStyleFlags);

            if(loMaxLineSize <= 0) return iOrgStr;

            return AdjustStringLength(iOrgStr, loMaxLineSize);
        }

        //Another ver of the funtion with font size
        public string AdjustStringLength(string iOrgStr, int iMaxLineLen)
        {
            if (string.IsNullOrEmpty(iOrgStr) || iMaxLineLen <= 0) return iOrgStr;
            string loAdjustedStr = iOrgStr;

            if (iOrgStr.Length <= iMaxLineLen)
            {
                //Nothing to do, the str fit in a line
                return loAdjustedStr;
            }

            //Now we need to do some trials to shrink the string to fit in a single line
            //First we will try to remove spaces
            loAdjustedStr = iOrgStr.TrimEnd();
            if (loAdjustedStr.Length <= iMaxLineLen)
            {
                return loAdjustedStr;
            }

            // remove leading spaces one character at a time
            while (loAdjustedStr.StartsWith(" ") && (loAdjustedStr.Length > iMaxLineLen))
            {
                loAdjustedStr = loAdjustedStr.Substring(1);
            }


            //loAdjustedStr = loAdjustedStr.TrimStart();
            //if (loAdjustedStr.Length <= iMaxLineLen)
            //{
            //    return loAdjustedStr;
            //}



            //Now we can try removing some spaces from the string
            //loAdjustedStr = loAdjustedStr.Trim();
            //if (loAdjustedStr.Length <= iMaxLineLen)
            //{
            //    return loAdjustedStr;
            //}

            //FixME: The last thing we can do now to use smaller font

            return loAdjustedStr;

        }


        public double GetCharactersPerInchForN5Font(N5Fonts iN5Font, N5FontStyleFlags iN5FontStyleFlags)
        {

            double loCharactersPerInch = 0;
            switch (iN5Font)
            {
                case N5Fonts.SAN_SERIF_5_5_CPI:
                    {
                        loCharactersPerInch = 5.5;
                        break;
                    }

                case N5Fonts.SAN_SERIF_10_2_CPI:
                    {
                        //        loMaxLineSize = 22;
                        loCharactersPerInch = 10.2;
                        break;
                    }

                case N5Fonts.SAN_SERIF_10_7_CPI:
                    {
                        //        loMaxLineSize = 30;
                        loCharactersPerInch = 10.7;
                        break;
                    }

                case N5Fonts.COURIER_12_7_CPI:
                    {
                        loCharactersPerInch = 12.7;
                        break;
                    }
                case N5Fonts.COURIER_13_5_CPI:
                    {
                        loCharactersPerInch = 13.5;
                        break;
                    }
                case N5Fonts.COURIER_14_5_CPI:
                    {
                        loCharactersPerInch = 14.5;
                        break;
                    }
                case N5Fonts.COURIER_15_6_CPI:
                    {
                        loCharactersPerInch = 15.6;
                        break;
                    }
                case N5Fonts.COURIER_16_9_CPI:
                    {
                        //        loMaxLineSize = 48;
                        loCharactersPerInch = 16.9;

                        // is it bold?
                        if (((uint)iN5FontStyleFlags | (uint)N5FontStyleFlags.EMPHASIZED) > 0)
                        {
                            loCharactersPerInch = 16.7;
                        }

                        break;
                    }
                case N5Fonts.COURIER_18_5_CPI:
                    {
                        loCharactersPerInch = 18.5;
                        break;
                    }
                case N5Fonts.COURIER_20_3_CPI:
                    {
                        loCharactersPerInch = 20.3;
                        break;
                    }
                case N5Fonts.COURIER_22_6_CPI:
                    {
                        loCharactersPerInch = 22.6;

                        // is it bold?
                        if (((uint)iN5FontStyleFlags | (uint)N5FontStyleFlags.EMPHASIZED) > 0)
                        {
                            loCharactersPerInch = 22.4;
                        }

                        break;
                    }
                case N5Fonts.COURIER_25_4_CPI:
                    {
                        loCharactersPerInch = 25.4;
                        break;
                    }
                case N5Fonts.SAN_SERIF_16_9_CPI:
                    {
                        loCharactersPerInch = 16.9;
                        break;
                    }
                case N5Fonts.SAN_SERIF_18_5_CPI:
                    {
                        loCharactersPerInch = 18.5;
                        break;
                    }
                case N5Fonts.SAN_SERIF_20_3_CPI:
                    {
                        loCharactersPerInch = 20.3;
                        break;
                    }
                case N5Fonts.SAN_SERIF_4_2_CPI:
                    {
                        loCharactersPerInch = 4.2;
                        break;
                    }
            }



            return loCharactersPerInch;
        }

        public double GetCharacterWidthForN5Font(N5Fonts iN5Font, N5FontStyleFlags iN5FontStyleFlags )
        {
            double loFontCharactersPerInch = GetCharactersPerInchForN5Font(iN5Font, iN5FontStyleFlags);

             // 1 inch = 203 dots 
            //  char per inch = 16.9 (example)
            //
            // so one char width is 203 / CPI

            double loFontCharacterWidth  = ((float)203.0 / loFontCharactersPerInch);
            
            return loFontCharacterWidth;
        }


        public int GetCharactersPerLineForN5Font(N5Fonts iN5Font, N5FontStyleFlags iN5FontStyleFlags)
        {
            // the N5Print has only 2.8" printer ( 72mm )
            double loPrinterWidthInInches = 2.8;

            double loFontCharactersPerInch = GetCharactersPerInchForN5Font(iN5Font, iN5FontStyleFlags);
            int loCharactersPerLine = (int)(loPrinterWidthInInches * loFontCharactersPerInch);

            return loCharactersPerLine;
        }

        /// <summary>
        /// Convert original Duncan font types to closest N5 equivalent
        /// </summary>
        /// <param name="dFont"></param>
        /// <returns></returns>
        public void ConvertDuncanFontToN5Font( TWinBase.TFont dFont, ref N5Fonts ioN5Font, ref N5FontStyleFlags ioN5FontStyleFlags )
        {

            // default to standard
            ioN5FontStyleFlags = N5FontStyleFlags.STANDARD; 

            switch (dFont)
            {

                case TWinBase.TFont.Font8x8:
                    {
                        ioN5Font = N5Fonts.COURIER_25_4_CPI;
                        break;
                    }


                case TWinBase.TFont.Font12x12:
                    {
                        ioN5Font = N5Fonts.COURIER_22_6_CPI;
                        break;
                    }

                case TWinBase.TFont.Font12x12Bold:
                    {
                        ioN5Font = N5Fonts.COURIER_22_6_CPI;
                        // msut also specify emphasized
                        ioN5FontStyleFlags = N5FontStyleFlags.EMPHASIZED;
                        break;
                    }

                case TWinBase.TFont.Font16x16:
                    {
                        ioN5Font = N5Fonts.COURIER_16_9_CPI;
                        break;
                    }


                case TWinBase.TFont.Font16x16Bold:
                    {
                        ioN5Font = N5Fonts.COURIER_16_9_CPI;
                        // msut also specify emphasized
                        ioN5FontStyleFlags = N5FontStyleFlags.EMPHASIZED;
                        break;
                    }

                case TWinBase.TFont.Font20x20:
                    {
                        ioN5Font = N5Fonts.SAN_SERIF_10_7_CPI;
                        break;
                    }

                case TWinBase.TFont.Font24x24:
                    {
                        ioN5Font = N5Fonts.SAN_SERIF_10_2_CPI;
                        break;
                    }

                case TWinBase.TFont.Font36x36:
                    {
                        ioN5Font = N5Fonts.SAN_SERIF_5_5_CPI;
                        break;
                    }

                case TWinBase.TFont.FontBC128:
                    {
                        ioN5Font = N5Fonts.code128;
                        break;
                    }
              
                case TWinBase.TFont.FontBC39:
                case TWinBase.TFont.FontBC3of9:
                    {
                        ioN5Font = N5Fonts.code39;
                        break;
                    }

                case TWinBase.TFont.FontOCR:
                default:
                    {
                        // this will print obnoxiously big to be obvious that it needs to be addressed
                        ioN5Font = N5Fonts.SAN_SERIF_5_5_CPI;
                        break;
                    }
            }

        }



        ////Converting Duncan font to N5 font
        ////For converting the font we will consider the size only for now, font type will be ignored as we have limited fonts in N5
        //public Fonts ConvertDuncanFontToN5Font(Font dFont)
        //{
        //    // the origin fonts as defined in the layout, pasted here for reference
        //    //Font8x8 = 0,

        //    //Font12x12,
        //    //Font12x12Bold,

        //    //Font16x16,
        //    //Font16x16Bold

        //    //Font20x20,
        //    //Font24x24,

        //    //Font36x36,

        //    //FontBC128,

        //    //FontOCR,
        //    //FontBC39,

        //    //FontBC3of9,




        //    if (dFont.Size >= 36)
        //    {
        //        return Fonts.SanSerif55Cpi;
        //    }

        //    //if (dFont.Size >= 25)
        //    //{
        //    //    return Fonts.Courier254Cpi; //This is the max we can support
        //    //}
        //    if (dFont.Size >= 24)
        //    {
        //        return Fonts.Courier226Cpi; // 22 up to 25
        //    }
        //    //if (dFont.Size >= 22)
        //    //{
        //    //    return Fonts.Courier226Cpi; // 22 up to 25
        //    //}
        //    if (dFont.Size >= 20)
        //    {
        //        //return Fonts.SanSerif203Cpi; //20 up to 22
        //        return Fonts.SanSerif107Cpi;
        //    }
        //    //if (dFont.Size >= 18)
        //    //{
        //    //    return Fonts.SanSerif185Cpi; //18 up to 20
        //    //}
        //    if (dFont.Size >= 16)
        //    {
        //        //return Fonts.SanSerif169Cpi; //16 up to 18


        //        // TODO - we need to be able specify emphasized
        //        if (dFont.Style == FontStyle.Bold)
        //        {
        //            return Fonts.Courier169Cpi;
        //        }
        //        else
        //        {
        //            return Fonts.Courier169Cpi;
        //        }

        //    }
        //    //if (dFont.Size >= 15)
        //    //{
        //    //    return Fonts.Courier156Cpi; //15 up to 16
        //    //}
        //    //if (dFont.Size >= 14)
        //    //{
        //    //    return Fonts.Courier145Cpi; //14 up to 15
        //    //}
        //    //if (dFont.Size >= 13)
        //    //{
        //    //    return Fonts.Courier135Cpi; //13 up to 14
        //    //}
        //    if (dFont.Size >= 12)
        //    {
        //        //return Fonts.Courier127Cpi; //12 up to 13

        //        // TODO - we need to be able specify emphasized
        //        if (dFont.Style == FontStyle.Bold)
        //        {
        //            //return Fonts.Courier169Cpi;
        //            return Fonts.Courier226Cpi;
        //        }
        //        else
        //        {
        //            return Fonts.Courier226Cpi;
        //        }

        //    }
        //    //if (dFont.Size >= 10)
        //    //{
        //    //    return Fonts.SanSerif107Cpi; //10 up to 12
        //    //}

        //    if (dFont.Size >= 8)
        //    {
        //        return Fonts.Courier254Cpi;
        //    }


        //    return Fonts.SanSerif55Cpi; //default

        //}

        public List<PCLStringObject> ConvertDuncanTextFieldToN5PCLText(List<PrintTextInfo> iTicketStrings)
        {
            //Do we have a valid string 
            if (iTicketStrings == null || iTicketStrings.Count == 0) return null;

            List<PCLStringObject> loPCLStrObjects = new List<PCLStringObject>();
            foreach (PrintTextInfo loDuncanStrObj in iTicketStrings)
            {
                //Map the Duncan font to N5 font
                N5Fonts loN5Font = N5Fonts.SAN_SERIF_5_5_CPI;
                N5FontStyleFlags loN5FontStyleFlags = N5FontStyleFlags.STANDARD;

                ConvertDuncanFontToN5Font(loDuncanStrObj.duncanFont, ref loN5Font, ref loN5FontStyleFlags );

                //Add new string to our array
                PCLStringObject loStrObj = new PCLStringObject();
                loStrObj.strText = loDuncanStrObj.text;
                loStrObj.x = loDuncanStrObj.x;
                loStrObj.font = loN5Font;
                //Some fields depends on printing data type (text vs. barcode).
                if (loStrObj.font < N5Fonts.code128)
                {
                    loStrObj.y = loDuncanStrObj.y;
                    loStrObj.fontStyleFlags = loN5FontStyleFlags;

                    //Keep orig Duncan font info for use in sorting/organizing the fields in the ticket 
                    loStrObj.fontHeight = loDuncanStrObj.font.Size;
                    loStrObj.Justification = loDuncanStrObj.font.Justification;
                    loStrObj.Rotation = loDuncanStrObj.font.Rotation;


                    double loFontCharactersPerInch = GetCharactersPerInchForN5Font(loStrObj.font, loStrObj.fontStyleFlags);

                    double loOneCharWidthForMonoSpacedFont = GetCharacterWidthForN5Font(loStrObj.font, loStrObj.fontStyleFlags);

                    // this calculates the number of spaces to insert before the text to place it as specified by the layout
                    //loStrObj.spaces = (int)(loDuncanStrObj.x / loDuncanStrObj.font.Size);

                    if (loFontCharactersPerInch > 0)
                    {
                        // re-calc from base 
                        loStrObj.spaces = (int)((float)loDuncanStrObj.sourceRect.Left / loOneCharWidthForMonoSpacedFont);
                        //loStrObj.spaces = (int)((float)loDuncanStrObj.x / loFontCharactersPerInch);

                        // copy the original position before justification applied on image canvas
                        if (loStrObj.x != loDuncanStrObj.sourceRect.Left)
                        {
                           loStrObj.x = loDuncanStrObj.sourceRect.Left;
                        }



                        // the N5 printer is stil 203 DPI even when it is in text mode, so the original placement doesn't need to be scaled
                        // it just has to be translated into mono spaced fonts
                        //



                        //Calc the x based on the justification
                        int loExtraSpacesForJustification = 0;


                        int loTextSize = (int)(loDuncanStrObj.text.Length * loOneCharWidthForMonoSpacedFont);
                        //int loTextSize = loDuncanStrObj.text.Length * loDuncanStrObj.font.Size;

                        switch (loStrObj.Justification)
                        {
                            case Justification_Android.Center:
                                {

                                    int loPixelsForJustification = (int)((loDuncanStrObj.sourceRect.Width - loTextSize) / 2);
                                    //int loPixelsForJustification = (int)(loDuncanStrObj.sourceRect.Left + loDuncanStrObj.sourceRect.Width / 2);
                                    // adjust for font
                                    loExtraSpacesForJustification = (int)(loPixelsForJustification / loOneCharWidthForMonoSpacedFont);
                                    break;
                                }
                            case Justification_Android.Right:
                                {
                                    if (loTextSize < loDuncanStrObj.sourceRect.Width)
                                    {
                                        // calc
                                        int loPixelsForJustification = (int)(loDuncanStrObj.sourceRect.Width - loTextSize);
                                        //int loPixelsForJustification = (int)(loDuncanStrObj.sourceRect.Left + loDuncanStrObj.sourceRect.Width - loTextSize);
                                        // adjust for font
                                        loExtraSpacesForJustification = (int)(loPixelsForJustification / loOneCharWidthForMonoSpacedFont);
                                    }
                                    else
                                    {
                                        loExtraSpacesForJustification = 0; // (int)(loDuncanStrObj.sourceRect.Left);
                                    }
                                    break;
                                }
                            case Justification_Android.Left:
                            default:
                                {
                                    loExtraSpacesForJustification = 0; // (int)(loDuncanStrObj.sourceRect.Left);
                                    break;
                                }
                        }


                        // add any extra needed spaces 
                        loStrObj.spaces += loExtraSpacesForJustification;
                    }
                    else
                    {
                        loStrObj.spaces = 0;
                    }

                }
                else
                {
                    loStrObj.y = 0;  //For N5 we can control where to print the barcode, so we will print it at the top side for now.
                    loStrObj.fontStyleFlags = N5FontStyleFlags.STANDARD;
                    loStrObj.fontHeight = loDuncanStrObj.sourceRect.Height; // get the barcode height

                    if (loDuncanStrObj.font != null)
                    {
                        loStrObj.Justification = loDuncanStrObj.font.Justification;
                        loStrObj.Rotation = loDuncanStrObj.font.Rotation;
                    }
                    else
                    {
                        loStrObj.Justification = Justification_Android.Left;
                        loStrObj.Rotation = Rotation_Android.Rotate0;
                    }

                    loStrObj.spaces = 0;
                }
                loPCLStrObjects.Add(loStrObj);
            }
            return loPCLStrObjects;
        }

        private List<PCLStringObject> SortStringPCLObjects(List<PCLStringObject> iPCLStringObjects)
        {
            if (iPCLStringObjects == null || iPCLStringObjects.Count() <= 0) return null;
            List<PCLStringObject> loSrcList = new List<PCLStringObject>();
            List<PCLStringObject> loSortedList = new List<PCLStringObject>();
            //Make local copy of the object list
            foreach (PCLStringObject loStrObj in iPCLStringObjects)
            {
                loSrcList.Add(loStrObj);
            }

            while (loSrcList.Count() > 0)
            {
                int loRef = 0xFFFFFF;
                int loIdx = 0;
                for (int i = 0; i < loSrcList.Count(); i++)
                {
                    if (loSrcList[i].y < loRef)
                    {
                        loRef = loSrcList[i].y;
                        loIdx = i;
                    }
                }
                //copy the found stiring
                loSortedList.Add(loSrcList[loIdx]);
                //delete the copied one
                loSrcList.RemoveAt(loIdx);
            }
            return loSortedList;
        }

        //Combine all fields in same Y pos then combine the fields to be ready for N5 printer app
        private List<PCLStringRow> GroupAndCombineStringPCLObjects(List<PCLStringObject> iPCLStringObjects)
        {
            if (iPCLStringObjects == null || iPCLStringObjects.Count() <= 0) return null;
            List<PCLStringRow> loN5PCLCommands = new List<PCLStringRow>();
            List<PCLStringObject> loSrcList = SortStringPCLObjects(iPCLStringObjects);
            List<PCLStringObject> loRow = new List<PCLStringObject>();
            bool loStartAddVertSpaces = false;
            int loPrevLineBottom = 0;
            //Get the feed scale value from registry
            int loVerticalSpaceScale = TTRegistry.glRegistry.GetRegistryValueAsInt(TTRegistry.regSECTION_ISSUE_AP,
                                                                                      TTRegistry.regPRINTER_VERTICAL_SPACE_SCALE_T5,
                                                                                      TTRegistry.regPRINTER_VERTICAL_SPACE_SCALE_T5_DEFAULT);


            while (loSrcList.Count() > 0)
            {
                //loRow.Add(loSrcList[0]); //add top field, then search for any one can be printed on same line
                int loYDelta = loSrcList[0].fontHeight / 4;
                int[] loIdxList = new int[16];
                int loSeq = 0;
                loRow.RemoveRange(0, loRow.Count); //make sure our collecting list is clear
                for (int i = 0; i < loSrcList.Count(); i++)
                {
                    if ((loSrcList[i].y <= (loSrcList[0].y + loYDelta)) &&
                        (loSrcList[i].y >= (loSrcList[0].y - loYDelta)))
                    {
                        loRow.Add(loSrcList[i]);
                        loIdxList[loSeq] = i;
                        loSeq++;
                    }
                }
                //Now construct the N5 PCL command for the current row
                PCLStringRow loTempCmd;
                int loVertSpace = 0;

                loTempCmd.forwardFeedVal = 0;  //Assume default value first

                // is it a barcode?
                if (
                     (loSrcList[0].font == N5Fonts.code128) ||
                    (loSrcList[0].font == N5Fonts.code39)
                )
                {
                    // the barcode height is sent instead
                    loTempCmd.forwardFeedVal = loSrcList[0].fontHeight;
                }




                //First item in the list is the ref one, check the vertical position
                if (loStartAddVertSpaces)
                {
                    loVertSpace = loSrcList[0].y - loPrevLineBottom;
                }
                //Calc the new bottom value for next line
                loPrevLineBottom = loSrcList[0].y + loSrcList[0].fontHeight;

                
                if (loRow.Count > 1)
                {
                    //First delete the copied objects from the src list
                    for (int i = loSeq - 1; i >= 0; i--)
                    {
                        loSrcList.RemoveAt(loIdxList[i]);
                    }
                    loTempCmd = CombineListOfPCLStrings(loRow);                    
                    loRow.RemoveRange(0, loRow.Count()); //Clear the list for next row
                }
                else
                {
                    loSrcList.RemoveAt(0); //remove the obj
                    loTempCmd.font = loRow[0].font;
                    loTempCmd.fontStyleFlags = loRow[0].fontStyleFlags;
                    loTempCmd.strText = "";
                    for (int i = 0; i < loRow[0].spaces; i++)
                    {
                        loTempCmd.strText = loTempCmd.strText + " ";
                    }
                    loTempCmd.strText = loTempCmd.strText + loRow[0].strText;                     
                }
                if (loVertSpace > 0)
                {
                    //Should we adjust the scale?
                    if (loVerticalSpaceScale > 0)
                    {
                        loVertSpace = (loVertSpace * loVerticalSpaceScale) / 100;
                    }

                    loTempCmd.forwardFeedVal = loVertSpace;
                }
                //Now we are ready to adjust line length
                loTempCmd.strText = AdjustStringLength(loTempCmd.strText, loTempCmd.font, loTempCmd.fontStyleFlags);
                loN5PCLCommands.Add(loTempCmd);
                loStartAddVertSpaces = true;
            }
            return loN5PCLCommands;
        }

        //Combines more than 2 fields to single N5 PCL command
        //The smaller font will be used.
        private PCLStringRow CombineListOfPCLStrings(List<PCLStringObject> iStrObjs)
        {
            int loFontSize = 0xFFFF;
            int loIdx = 0;
            int loXRef = 0xFFFF;
            PCLStringRow loN5PCLSmd = new PCLStringRow();


            bool loFirstAdded = false;

            List<PCLStringObject> loSrcList = new List<PCLStringObject>();

            //Make local copy of the object list
            foreach (PCLStringObject loStrObj in iStrObjs)
            {
                loSrcList.Add(loStrObj);
            }

            for (int i = 0; i < loSrcList.Count(); i++)
            {
                if (loSrcList[i].fontHeight < loFontSize)
                {
                    loFontSize = loSrcList[i].fontHeight;
                }
            }

            loN5PCLSmd.font = loSrcList[loIdx].font; //Store our smaller font
            loN5PCLSmd.fontStyleFlags = loSrcList[loIdx].fontStyleFlags;



            double loFontCharactersPerInch = GetCharactersPerInchForN5Font(loN5PCLSmd.font, loN5PCLSmd.fontStyleFlags);

            double loOneCharWidthForMonoSpacedFont = GetCharacterWidthForN5Font(loN5PCLSmd.font, loN5PCLSmd.fontStyleFlags);




            //Now we can go and build the N5 PCL command
            while (loSrcList.Count() > 0)
            {
                loIdx = 0;
                loXRef = 0xFFFF;
                for (int i = 0; i < loSrcList.Count(); i++)
                {
                    if (loSrcList[i].x < loXRef)
                    {
                        loXRef = loSrcList[i].x;
                        loIdx = i;
                    }
                }

                int loSpaces = 0;
                if (loN5PCLSmd.strText == null)
                {
                    loN5PCLSmd.strText = string.Empty;
                }


                // calculate absolute column position
                loSpaces = (int)((float)loSrcList[loIdx].x / loOneCharWidthForMonoSpacedFont);


                // adjust for existing data
                loSpaces -= loN5PCLSmd.strText.Length;


                if (loSpaces <= 0)
                {
                    if (loFirstAdded == true)
                    {
                        //Allow at least one space between every 2 fields
                        loSpaces = 1; 
                    }
                    else
                    {
                        // no padding for the first item 
                        loSpaces = 0;
                    }
                }

                for (int j = 0; j < loSpaces; j++)
                {
                    loN5PCLSmd.strText = loN5PCLSmd.strText + " ";
                }




                loN5PCLSmd.strText = loN5PCLSmd.strText + loSrcList[loIdx].strText;

                loFirstAdded = true;

                //Delete it so we don't print it again
                if (loSrcList.Count() > 0) loSrcList.RemoveAt(loIdx);
            }

            return loN5PCLSmd;
        }

        //Sort all texts in the array into PCL commands
        public List<PCLStringRow> ConvertPrintTextIntoPCLObjects(List<PrintTextInfo> ticketStrings)
        {
            if (ticketStrings == null || ticketStrings.Count <= 0) return null;
            //now we are ready to generate N5 based string list. 
            List<PCLStringObject> loPCLStrObjects = ConvertDuncanTextFieldToN5PCLText(ticketStrings);
            {
                if (loPCLStrObjects == null || loPCLStrObjects.Count <= 0) return null;
            }

            //1. Sort all fields based on vertical position (y value).
            //2. Now we should have a list of all text fields in the ticket sorted from top to bottom.
            //   We then need to combin all fields in same line or too close (space < font hight/2) and generate the N5 PCL commands arguments.
            List<PCLStringRow> loN5StrRows = GroupAndCombineStringPCLObjects(loPCLStrObjects);
            {
                if (loN5StrRows == null || loN5StrRows.Count <= 0) return null;
            }
            return loN5StrRows;
        }

    }


}