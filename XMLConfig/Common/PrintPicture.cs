// VSS Keyword information
// File last modified by:   $Author: James $
//                    on:     $Date: 12/04/13 8:15a $
//               Archive:  $Archive: /AutoISSUE Solution.root/AutoISSUE Solution/Common/ClientConfig/PrintPicture.cs $
//              Revision: $Revision: 37 $

using System;

using System.Text;

using System.Duncan.Drawing;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.InteropServices;
using Android.Graphics;
using Java.IO;
using System.IO;


namespace Reino.ClientConfig
{


    #region Structures used for GDI functions
    [StructLayout(LayoutKind.Sequential)]
    public struct BITMAPINFO
    {
        // BITMAPINFOHEADER	
        public uint biSize;
        public int biWidth;
        public int biHeight;
        public ushort biPlanes;
        public ushort biBitCount;
        public uint biCompression;
        public uint biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public uint biClrUsed;
        public uint biClrImportant;
        // RGBQUAD for Color 0
        public byte rgbBlue0;
        public byte rgbGreen0;
        public byte rgbRed0;
        public byte rgbReserved0;
        // RGBQUAD for Color 1
        public byte rgbBlue1;
        public byte rgbGreen1;
        public byte rgbRed1;
        public byte rgbReserved1;
    }

#if __ANDROID__
    public struct PrintTextInfo
    {
        public string text;
        public int x;
        public int y;
        public Rectangle sourceRect;
        public Font font;
        public TWinBase.TFont duncanFont;
    }
#endif

#if !WindowsCE && !__ANDROID__

    // Compact Framework has a LogFont class defined that we can use.
    // This is the structure that is needed for the Full Framework to use log fonts.
    // Apparently MS wanted to make life hard, because this same structure doesn't work on the Compact Framework,
    // so we're forced to use compiler directives that use the LogFont class on WinCE and LOGFONT structure on Win32
    [StructLayout(LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Auto)] // CharSet = CharSet.Auto
    public struct LOGFONT
    {
        public int Height;
        public int Width;
        public int Escapement;
        public int Orientation;
        public int Weight;
        public byte Italic;
        public byte Underline;
        public byte StrikeOut;
        public byte CharSet;
        public byte OutPrecision;
        public byte ClipPrecision;
        public byte Quality;
        public byte PitchAndFamily;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string FaceName;
    }

    // Specifies the character set of a font. (This definition came from the Compact Framework)
    public enum LogFontCharSet
    {
        ANSI = 0,
        Default = 1,
        Symbol = 2,
        Mac = 77,
        ShiftJIS = 128,
        Hangeul = 129,
        Johab = 130,
        GB2312 = 134,
        ChineseBig5 = 136,
        Greek = 161,
        Turkish = 162,
        Hebrew = 177,
        Arabic = 178,
        Baltic = 186,
        Russian = 204,
        Thai = 222,
        EastEurope = 238,
        OEM = 255,
    }

    // Specifies the output precision. (This definition came from the Compact Framework)
    public enum LogFontPrecision
    {
        Default = 0,
        String = 1,
        Raster = 6,
    }

    // Specifies the quality of a font. (This definition came from the Compact Framework)
    public enum LogFontQuality
    {
        Default = 0,
        Draft = 1,
        NonAntiAliased = 3,
        AntiAliased = 4,
        ClearType = 5,
        ClearTypeCompat = 6,
    }

    // Specifies categories of fonts. (This definition came from the Compact Framework)
    [FlagsAttribute]
    public enum LogFontPitchAndFamily
    {
        Default = 0,
        DontCare = 0,
        Fixed = 1,
        Variable = 2,
        Roman = 16,
        Swiss = 32,
        Modern = 48,
        Script = 64,
        Decorative = 80,
    }

    // Specifies the weight of a font. (This definition came from the Compact Framework)
    public enum LogFontWeight
    {
        DontCare = 0,
        Thin = 100,
        ExtraLight = 200,
        UltraLight = 200,
        Light = 300,
        Normal = 400,
        Regular = 400,
        Medium = 500,
        DemiBold = 600,
        SemiBold = 600,
        Bold = 700,
        ExtraBold = 800,
        UltraBold = 800,
        Black = 900,
        Heavy = 900,
    }

#endif

    // We haven't needed this yet, but DibSection structure is ready to be activated if needed
    /*
    [StructLayout(LayoutKind.Sequential)]
    public struct DibSection
    {
        public int bmType;
        public int bmWidth;
        public int bmHeight;
        public int bmWidthBytes;
        public int bmPlanesAndbmBitsPixel;
        public IntPtr bmBits;
        public int biSize;
        public int biWidth;
        public int biHeight;
        public int biPlanesAndBitCount;
        public int biCompression;
        public int biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public int biClrUsed;
        public int biClrImportant;
        public int dsBitfields0;
        public int dsBitfields1;
        public int dsBitfields2;
        public IntPtr dshSection;
        public int dsOffset;
    }
    */
    #endregion

    /// <summary>
    /// Summary description for TWinPrnBase.
    /// </summary>

    /* The XmlInclude attribute is used on a base type to indicate that when serializing 
     * instances of that type, they might really be instances of one or more subtypes. 
     * This allows the serialization engine to emit a schema that reflects the possibility 
     * of really getting a Derived when the type signature is Base. For example, we keep
     * field definitions in a generic collection of TWinPrnBase. If an array element is 
     * TWinPrnData, the XML serializer gets mad because it was only expecting TWinPrnBase. 
     */
    [XmlInclude(typeof(TPrnControl)), XmlInclude(typeof(TWinPrnPrompt)),
     XmlInclude(typeof(TWinBasePrnData)), XmlInclude(typeof(TWinPrnMemo)),
     XmlInclude(typeof(TWinPrnDraw)), XmlInclude(typeof(TWinPrnPanel)),
     XmlInclude(typeof(TWinPrnForm)), XmlInclude(typeof(TWinPrnData)),
     XmlInclude(typeof(TIssPrnFormRev)), XmlInclude(typeof(TWinPrnImage))]
    public class TWinPrnBase : Reino.ClientConfig.TWinBase
    {
        /*******************************************************************************/
        // Pixel elements to plot for BarCode128
        // We use BarCode128-A for letters and single digits, and BarCode128-C for double digits.
        ushort[] PrinterFontBC128Table = new ushort[256] {
			0x0000 /* 00 */,     0x0000 /* 01 */,     0x0000 /* 02 */,     0x0000 /* 03 */, 
			0x0000 /* 04 */,     0x0000 /* 05 */,     0x0000 /* 06 */,     0x0000 /* 07 */, 
			0x2020 /* 08 */,     0x2020 /* 09 */,     0x2020 /* 0A */,     0x2020 /* 0B */, 
			0x2020 /* 0C */,     0x2020 /* 0D */,     0x2020 /* 0E */,     0x2020 /* 0F */, 
			0x2020 /* 10 */,     0x2020 /* 11 */,     0x2020 /* 12 */,     0x2020 /* 13 */, 
			0x2020 /* 14 */,     0x2020 /* 15 */,     0x2020 /* 16 */,     0x2020 /* 17 */, 
			0x2020 /* 18 */,     0x2020 /* 19 */,     0x2020 /* 1A */,     0x2020 /* 1B */, 
			0x2020 /* 1C */,     0x2020 /* 1D */,     0x2020 /* 1E */,     0x2020 /* 1F */, 
			0x019B /*   */,      0x01B3 /* ! */,      0x0333 /* " */,      0x00C9 /* # */, 
			0x0189 /* $ */,      0x0191 /* % */,      0x0099 /* & */,      0x0119 /* ' */, 
			0x0131 /* ( */,      0x0093 /* ) */,      0x0113 /* * */,      0x0123 /* + */, 
			0x01CD /* , */,      0x01D9 /* - */,      0x0399 /* . */,      0x019D /* / */, 
			0x01B9 /* 0 */,      0x0339 /* 1 */,      0x0273 /* 2 */,      0x01D3 /* 3 */, 
			0x0393 /* 4 */,      0x013B /* 5 */,      0x0173 /* 6 */,      0x03B7 /* 7 */, 
			0x0197 /* 8 */,      0x01A7 /* 9 */,      0x0327 /* : */,      0x0137 /* ; */, 
			0x0167 /* < */,      0x0267 /* = */,      0x00DB /* > */,      0x031B /* ? */, 
			0x0363 /* @ */,      0x00C5 /* A */,      0x00D1 /* B */,      0x0311 /* C */, 
			0x008D /* D */,      0x00B1 /* E */,      0x0231 /* F */,      0x008B /* G */, 
			0x00A3 /* H */,      0x0223 /* I */,      0x00ED /* J */,      0x038D /* K */, 
			0x03B1 /* L */,      0x00DD /* M */,      0x031D /* N */,      0x0371 /* O */, 
			0x0377 /* P */,      0x038B /* Q */,      0x03A3 /* R */,      0x00BB /* S */, 
			0x023B /* T */,      0x03BB /* U */,      0x00D7 /* V */,      0x0317 /* W */, 
			0x0347 /* X */,      0x00B7 /* Y */,      0x0237 /* Z */,      0x02C7 /* [ */, 
			0x02F7 /* 5C */,     0x0213 /* ] */,      0x028F /* ^ */,      0x0065 /* _ */, 
			0x0185 /* ` */,      0x0069 /* a */,      0x0309 /* b */,      0x01A1 /* c */, 
			0x0321 /* d */,      0x004D /* e */,      0x010D /* f */,      0x0059 /* g */, 
			0x0219 /* h */,      0x0161 /* i */,      0x0261 /* j */,      0x0243 /* k */, 
			0x0053 /* l */,      0x02EF /* m */,      0x0143 /* n */,      0x02F1 /* o */, 
			0x01E5 /* p */,      0x01E9 /* q */,      0x03C9 /* r */,      0x013D /* s */, 
			0x0179 /* t */,      0x0279 /* u */,      0x012F /* v */,      0x014F /* w */, 
			0x024F /* x */,      0x03DB /* y */,      0x037B /* z */,      0x036F /* { */, 
			0x00F5 /* | */,      0x03C5 /* } */,      0x03D1 /* ~ */,      0x00BD /* 7F */, 
			0x023D /* 80 */,     0x00AF /* 81 */,     0x022F /* 82 */,     0x03DD /* 83 */, 
			0x03BD /* 84 */,     0x03D7 /* 85 */,     0x03AF /* 86 */,     0x010B /* 87 */, 
			0x004B /* 88 */,     0x01CB /* 89 */,     0x02E3 /* 8A */,     0x0003 /* 8B */, 
			0x0000 /* 8C */,     0x0000 /* 8D */,     0x0000 /* 8E */,     0x0000 /* 8F */, 
			0x0000 /* 90 */,     0x0000 /* 91 */,     0x0000 /* 92 */,     0x0000 /* 93 */, 
			0x0000 /* 94 */,     0x0000 /* 95 */,     0x0000 /* 96 */,     0x0000 /* 97 */, 
			0x0000 /* 98 */,     0x0000 /* 99 */,     0x0000 /* 9A */,     0x0000 /* 9B */, 
			0x0000 /* 9C */,     0x0000 /* 9D */,     0x0000 /* 9E */,     0x0000 /* 9F */, 
			0x0000 /* A0 */,     0x0000 /* A1 */,     0x0000 /* A2 */,     0x0000 /* A3 */, 
			0x0000 /* A4 */,     0x0000 /* A5 */,     0x0000 /* A6 */,     0x0000 /* A7 */, 
			0x0000 /* A8 */,     0x0000 /* A9 */,     0x0000 /* AA */,     0x0000 /* AB */, 
			0x0000 /* AC */,     0x0000 /* AD */,     0x0000 /* AE */,     0x0000 /* AF */, 
			0x0000 /* B0 */,     0x0000 /* B1 */,     0x0000 /* B2 */,     0x0000 /* B3 */, 
			0x0000 /* B4 */,     0x0000 /* B5 */,     0x0000 /* B6 */,     0x0000 /* B7 */, 
			0x0000 /* B8 */,     0x0000 /* B9 */,     0x0000 /* BA */,     0x0000 /* BB */, 
			0x0000 /* BC */,     0x0000 /* BD */,     0x0000 /* BE */,     0x0000 /* BF */, 
			0x0000 /* C0 */,     0x0000 /* C1 */,     0x0000 /* C2 */,     0x0000 /* C3 */, 
			0x0000 /* C4 */,     0x0000 /* C5 */,     0x0000 /* C6 */,     0x0000 /* C7 */, 
			0x0000 /* C8 */,     0x0000 /* C9 */,     0x0000 /* CA */,     0x0000 /* CB */, 
			0x0000 /* CC */,     0x0000 /* CD */,     0x0000 /* CE */,     0x0000 /* CF */, 
			0x0000 /* D0 */,     0x0000 /* D1 */,     0x0000 /* D2 */,     0x0000 /* D3 */, 
			0x0000 /* D4 */,     0x0000 /* D5 */,     0x0000 /* D6 */,     0x0000 /* D7 */, 
			0x0000 /* D8 */,     0x0000 /* D9 */,     0x0000 /* DA */,     0x0000 /* DB */, 
			0x0000 /* DC */,     0x0000 /* DD */,     0x0000 /* DE */,     0x0000 /* DF */, 
			0x0000 /* E0 */,     0x0000 /* E1 */,     0x0000 /* E2 */,     0x0000 /* E3 */, 
			0x0000 /* E4 */,     0x0000 /* E5 */,     0x0000 /* E6 */,     0x0000 /* E7 */, 
			0x0000 /* E8 */,     0x0000 /* E9 */,     0x0000 /* EA */,     0x0000 /* EB */, 
			0x0000 /* EC */,     0x0000 /* ED */,     0x0000 /* EE */,     0x0000 /* EF */, 
			0x0000 /* F0 */,     0x0000 /* F1 */,     0x0000 /* F2 */,     0x0000 /* F3 */, 
			0x0000 /* F4 */,     0x0000 /* F5 */,     0x0000 /* F6 */,     0x0000 /* F7 */, 
			0x0000 /* F8 */,     0x0000 /* F9 */,     0x0000 /* FA */,     0x0000 /* FB */, 
			0x0000 /* FC */,     0x0000 /* FD */,     0x0000 /* FE */,     0x0000 /* FF */,
			};

        /*******************************************************************************/
        // Pixel elements to plot for BarCode 3of9
        ushort[] PrinterFontBC3of9Table = new ushort[256] {
            0x0000 /* 00 */,  0x0000 /* 01 */,  0x0000 /* 02 */,  0x0000 /* 03 */,
            0x0000 /* 04 */,  0x0000 /* 05 */,  0x0000 /* 06 */,  0x0000 /* 07 */,
            0x0000 /* 08 */,  0x0000 /* 09 */,  0x0000 /* 0A */,  0x0000 /* 0B */,
            0x0000 /* 0C */,  0x0000 /* 0D */,  0x0000 /* 0E */,  0x0000 /* 0F */,
            0x0000 /* 10 */,  0x0000 /* 11 */,  0x0000 /* 12 */,  0x0000 /* 13 */,
            0x0000 /* 14 */,  0x0000 /* 15 */,  0x0000 /* 16 */,  0x0000 /* 17 */,
            0x0000 /* 18 */,  0x0000 /* 19 */,  0x0000 /* 1A */,  0x0000 /* 1B */,
            0x0000 /* 1C */,  0x0000 /* 1D */,  0x0000 /* 1E */,  0x0000 /* 1F */,
            0x5D71 /*   */,   0x0000 /* ! */,   0x0000 /* " */,   0x0000 /* # */,
            0x5111 /* $ */,   0x4445 /* % */,   0x0000 /* & */,   0x0000 /* ' */,
	        0x0000 /* ( */,   0x0000 /* ) */,   0x5DD1 /* * */,   0x4451 /* + */,
	        0x0000 /* , */,   0x7751 /* - */,   0x5D47 /* . */,   0x4511 /* / */,
            0x5DC5 /* 0 */,   0x7517 /* 1 */,   0x751D /* 2 */,   0x5477 /* 3 */,
            0x75C5 /* 4 */,   0x5717 /* 5 */,   0x571D /* 6 */,   0x7745 /* 7 */,
            0x5D17 /* 8 */,   0x5D1D /* 9 */,   0x0000 /* : */,   0x0000 /* ; */,
            0x0000 /* < */,   0x0000 /* = */,   0x0000 /* > */,   0x0000 /* ? */,
            0x0000 /* @ */,   0x7457 /* A */,   0x745D /* B */,   0x5177 /* C */,
            0x7475 /* D */,   0x51D7 /* E */,   0x51DD /* F */,   0x7715 /* G */,
            0x5C57 /* H */,   0x5C5D /* I */,   0x5C75 /* J */,   0x7157 /* K */,
            0x715D /* L */,   0x4577 /* M */,   0x7175 /* N */,   0x45D7 /* O */,
            0x45DD /* P */,   0x71D5 /* Q */,   0x4757 /* R */,   0x475D /* S */,
            0x4775 /* T */,   0x7547 /* U */,   0x7571 /* V */,   0x55C7 /* W */,
            0x75D1 /* X */,   0x5747 /* Y */,   0x5771 /* Z */,   0x0000 /* [ */,
            0x0000 /* 5C */,  0x0000 /* ] */,   0x0000 /* ^ */,   0x0000 /* _ */,
            0x0000 /* ` */,   0x0000 /* a */,   0x0000 /* b */,   0x0000 /* c */,
            0x0000 /* d */,   0x0000 /* e */,   0x0000 /* f */,   0x0000 /* g */,
            0x0000 /* h */,   0x0000 /* i */,   0x0000 /* j */,   0x0000 /* k */,
            0x0000 /* l */,   0x0000 /* m */,   0x0000 /* n */,   0x02F1 /* o */,
            0x0000 /* p */,   0x0000 /* q */,   0x0000 /* r */,   0x0000 /* s */,
            0x0000 /* t */,   0x0000 /* u */,   0x0000 /* v */,   0x0000 /* w */,
            0x0000 /* x */,   0x0000 /* y */,   0x0000 /* z */,   0x0000 /* { */,
            0x0000 /* | */,   0x0000 /* } */,   0x0000 /* ~ */,   0x0000 /* 7F */,
            0x0000 /* 80 */,  0x0000 /* 81 */,  0x0000 /* 82 */,  0x0000 /* 83 */,
            0x0000 /* 84 */,  0x0000 /* 85 */,  0x0000 /* 86 */,  0x0000 /* 87 */,
            0x0000 /* 88 */,  0x0000 /* 89 */,  0x0000 /* 8A */,  0x0000 /* 8B */,
            0x0000 /* 8C */,  0x0000 /* 8D */,  0x0000 /* 8E */,  0x0000 /* 8F */,
            0x0000 /* 90 */,  0x0000 /* 91 */,  0x0000 /* 92 */,  0x0000 /* 93 */,
            0x0000 /* 94 */,  0x0000 /* 95 */,  0x0000 /* 96 */,  0x0000 /* 97 */,
            0x0000 /* 98 */,  0x0000 /* 99 */,  0x0000 /* 9A */,  0x0000 /* 9B */,
            0x0000 /* 9C */,  0x0000 /* 9D */,  0x0000 /* 9E */,  0x0000 /* 9F */,
            0x0000 /* A0 */,  0x0000 /* A1 */,  0x0000 /* A2 */,  0x0000 /* A3 */,
            0x0000 /* A4 */,  0x0000 /* A5 */,  0x0000 /* A6 */,  0x0000 /* A7 */,
            0x0000 /* A8 */,  0x0000 /* A9 */,  0x0000 /* AA */,  0x0000 /* AB */,
            0x0000 /* AC */,  0x0000 /* AD */,  0x0000 /* AE */,  0x0000 /* AF */,
            0x0000 /* B0 */,  0x0000 /* B1 */,  0x0000 /* B2 */,  0x0000 /* B3 */,
            0x0000 /* B4 */,  0x0000 /* B5 */,  0x0000 /* B6 */,  0x0000 /* B7 */,
            0x0000 /* B8 */,  0x0000 /* B9 */,  0x0000 /* BA */,  0x0000 /* BB */,
            0x0000 /* BC */,  0x0000 /* BD */,  0x0000 /* BE */,  0x0000 /* BF */,
            0x0000 /* C0 */,  0x0000 /* C1 */,  0x0000 /* C2 */,  0x0000 /* C3 */,
            0x0000 /* C4 */,  0x0000 /* C5 */,  0x0000 /* C6 */,  0x0000 /* C7 */,
            0x0000 /* C8 */,  0x0000 /* C9 */,  0x0000 /* CA */,  0x0000 /* CB */,
            0x0000 /* CC */,  0x0000 /* CD */,  0x0000 /* CE */,  0x0000 /* CF */,
            0x0000 /* D0 */,  0x0000 /* D1 */,  0x0000 /* D2 */,  0x0000 /* D3 */,
            0x0000 /* D4 */,  0x0000 /* D5 */,  0x0000 /* D6 */,  0x0000 /* D7 */,
            0x0000 /* D8 */,  0x0000 /* D9 */,  0x0000 /* DA */,  0x0000 /* DB */,
            0x0000 /* DC */,  0x0000 /* DD */,  0x0000 /* DE */,  0x0000 /* DF */,
            0x0000 /* E0 */,  0x0000 /* E1 */,  0x0000 /* E2 */,  0x0000 /* E3 */,
            0x0000 /* E4 */,  0x0000 /* E5 */,  0x0000 /* E6 */,  0x0000 /* E7 */,
            0x0000 /* E8 */,  0x0000 /* E9 */,  0x0000 /* EA */,  0x0000 /* EB */,
            0x0000 /* EC */,  0x0000 /* ED */,  0x0000 /* EE */,  0x0000 /* EF */,
            0x0000 /* F0 */,  0x0000 /* F1 */,  0x0000 /* F2 */,  0x0000 /* F3 */,
            0x0000 /* F4 */,  0x0000 /* F5 */,  0x0000 /* F6 */,  0x0000 /* F7 */,
            0x0000 /* F8 */,  0x0000 /* F9 */,  0x0000 /* FA */,  0x0000 /* FB */,
            0x0000 /* FC */,  0x0000 /* FD */,  0x0000 /* FE */,  0x0000 /* FF */,
        };


        public enum TRotation
        {
            Rotate0,
            Rotate90,
            Rotate180,
            Rotate270
        }

        public enum TJustification
        {
            Left,
            Center,
            Right
        }

        #region Properties and Members
        protected TRotation _Rotation = TRotation.Rotate0;
        [System.ComponentModel.DefaultValue(TRotation.Rotate0)] // This prevents serialization of default values
        public TRotation Rotation
        {
            get { return _Rotation; }
            set { _Rotation = value; }
        }

        protected TJustification _Justification = TJustification.Left;
        [System.ComponentModel.DefaultValue(TJustification.Left)] // This prevents serialization of default values
        public TJustification Justification
        {
            get { return _Justification; }
            set { _Justification = value; }
        }


        protected bool _Hidden = false;
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool Hidden
        {
            get { return _Hidden; }
            set { _Hidden = value; }
        }

        /// <summary>
        /// This value can be changed to increase or decrease the intercharacter spacing.
        /// </summary>
        [XmlIgnoreAttribute]
        public static int OCRExtraDotsWidth = 1;
        #endregion


        public TWinPrnBase()
            : base()
        {
        }

        #region Implementation code
        // Parent needs to be publicly accessible, but we don't want it to be a true property for XML Serialization reasons
        protected TWinPrnBase _Parent = null;
        public TWinPrnBase GetParent()
        {
            return _Parent;
        }
        public void SetParent(TWinPrnBase pValue)
        {
            _Parent = pValue;
        }

        static protected List<PrintTextInfo> ticketStrings = new List<PrintTextInfo>();
		public List<PrintTextInfo> GetAllStringsInCurrentTicket()
		{
			return ticketStrings;
		}

        public void ResolveObjectReferences(TWinPrnBase pParent, TIssPrnFormRev pIssPrnFormRev)
        {
            // Set our own parent to whatever was passed to us
            _Parent = pParent;

            // Set our coordinates
            fCoordinates = new Rectangle(this.Left, this.Top, this.Width, this.Height);


            // If its a TWinBasePrnData, lets try to find an associated prompt object
            if (this is TWinBasePrnData)
            {

                // Alan likes to alter the date and time masks for host-side reasons, so unfortunately
                // we end up getting incorrect date and time masks for use by the .NET Handheld software and
                // PatrolCar/Emulator. For those platforms we'll try to translate them back the way 
                // they're supposed to be here and hopefully save ourselves hours of debugging in the future.
                // Handheld code can be identified by the Windows platform, but we'll have to use
                // TClientDef.GuaranteedThreadSafe to identify PatrolCar since its the only application
                // that should have this static variable set to true.
                // Note: Alan's conversions do not preserve character casing and so cannot be reversed with 
                // 100% reliability. So instead of using GetAutoISSUEMaskForDotNetMask_Date and
                // GetAutoISSUEMaskForDotNetMask_Time we are forced to carry our desired mask in MaskForHH.
                // (To be backward compatible with older XML files, we'll set MaskForHH equal to Mask if it is blank)
                if ((Environment.OSVersion.Platform == PlatformID.WinCE) || (TClientDef.GuaranteedThreadSafe == true))
                {
                    // If MaskForHH exists, copy it to regular mask. If doesn't exist, then set equal to existing mask
                    if (((this as TWinBasePrnData).MaskForHH == "") && ((this as TWinBasePrnData).Mask != ""))
                    {
                        ((TWinBasePrnData)(this)).MaskForHH = ((TWinBasePrnData)(this)).Mask;
                    }
                    else
                    {
                        ((TWinBasePrnData)(this)).Mask = ((TWinBasePrnData)(this)).MaskForHH;
                    }
                }

                if (((TWinBasePrnData)(this)).PromptWin != "")
                {
                    // Start at the top-level TIssPrnFormRev to find the prompt
                    TWinPrnPrompt loPromptWin = pIssPrnFormRev.GetWinPromptByName(((TWinBasePrnData)(this)).PromptWin);
                    if (loPromptWin != null)
                        ((TWinBasePrnData)(this)).PromptWinCtrl = loPromptWin;
                }
            }

            // If we descend from TWinPrnPanel, then make ourself the parent of all our children.
            if (this is TWinPrnPanel)
            {
                foreach (TWinPrnBase loChild in ((TWinPrnPanel)(this)).Children)
                    loChild.ResolveObjectReferences(this, pIssPrnFormRev);
            }

            // We need to also find Page Breaks in the print picture
            if (this is TWinPrnPrompt)
            {
                // If our name begins with 'FORMFEED', hide ourself and alert the owning form 
                // that we are indeed a formfeed
                if (this.Name.StartsWith("FORMFEED") == true)
                {
                    // Recurse parents until we find the parent form
                    TWinPrnForm loParentForm = null;
                    TWinPrnBase loParent = _Parent;
                    while (loParent != null)
                    {
                        // If this parent is a TWinPrnForm, its our final destination
                        if (loParent is TWinPrnForm)
                        {
                            loParentForm = loParent as TWinPrnForm;
                            break;
                        }
                        // Get next parent and move to next loop iteration
                        loParent = loParent._Parent;
                    }

                    // If we found the parent form, we can add the page break info
                    if (loParentForm != null)
                    {
                        // need to give our absolute top position, not a position relative to our immediate parent
                        AbsoluteCoordinates();
                        loParentForm.AddPageBreak(fAbsCoordinates.Top);
                        // We're just a helper item, not meant to be visible
                        this.Hidden = true;
                    }
                }
            }
        }

        private TWinPrnPrompt GetWinPromptByName(string CompareName)
        {
            // Return ourself if we are the correct class and names match
            if ((this is TWinPrnPrompt) && (this.Name.Equals(CompareName.ToUpper())))
                return this as TWinPrnPrompt;

            // If we descend from TWinPrnPanel then recurse through children
            TWinPrnPrompt loResult = null;
            if (this is TWinPrnPanel)
            {
                foreach (TWinPrnBase loChild in (this as TWinPrnPanel).Children)
                {
                    loResult = loChild.GetWinPromptByName(CompareName);
                    // If we found a result, return this (recursively up the chain)
                    if (loResult != null)
                        return loResult;
                }
            }
            // We're not the desired prompt object so return null
            return null;
        }

        public void AddDataElementsToList(List<TWinBasePrnData> iFldList)
        {
            if (this is TWinBasePrnData)
                iFldList.Add(this as TWinBasePrnData);

            // If we descend from TWinPrnPanel then recurse through children
            if (this is TWinPrnPanel)
            {
                foreach (TWinPrnBase loChild in (this as TWinPrnPanel).Children)
                    loChild.AddDataElementsToList(iFldList);
            }
        }

        protected Rectangle fCoordinates;
        protected Rectangle fAbsCoordinates;
        protected Rectangle fVisCoordinates;



        protected int AbsoluteCoordinates()
        {
            TWinPrnBase loWin = null;

            // Get starting coordinates relative to our parent
            int AbsLeft = this.Left;
            int AbsTop = this.Top;
            int AbsWidth = this.Width;
            int AbsHeight = this.Height;

            // Convert relative coordinates to absolute coordinates based on level of nesting
            for (loWin = this._Parent; loWin != null; loWin = loWin._Parent)
            {
                AbsLeft = AbsLeft + loWin.Left;
                AbsTop = AbsTop + loWin.Top;
            }

            // Make rectangle based on converted absolute position
            fAbsCoordinates = new Rectangle(AbsLeft, AbsTop, AbsWidth, AbsHeight);

            // Make adjustments for any obscured portion
            for (loWin = this._Parent; loWin != null; loWin = loWin._Parent)
            {
                // is this window big enough? 
                if ((this.Top > loWin.Height) || (this.Left > loWin.Width))
                {
                    // this window is completely obscured 
                    fAbsCoordinates = new Rectangle(fAbsCoordinates.Left, fAbsCoordinates.Top, 0, 0);
                }
                else
                {
                    // is this window partially obscured? 
                    if ((this.Height + this.Top) > loWin.Height)
                        fAbsCoordinates = new Rectangle(fAbsCoordinates.Left, fAbsCoordinates.Top, fAbsCoordinates.Width,
                            loWin.Height - /*fAbsCoordinates*/this.Top);
                    if ((this.Width + this.Left) > loWin.Width)
                        fAbsCoordinates = new Rectangle(fAbsCoordinates.Left, fAbsCoordinates.Top,
                            loWin.Width - /*fAbsCoordinates*/this.Left, fAbsCoordinates.Height);
                }
            }
            return 0;
        }

#if WindowsCE
		// On WindowsCE we need to use a 2-color bitmap because its the most memory efficient and also 
		// the easiest to transfer to our thermal printer. The .NET Compact Framework doesn't support 
		// this type of bitmap, so we will use API functions to do it instead.
		[XmlIgnoreAttribute]
		public static IntPtr glDrawBitmap = IntPtr.Zero; // 2-color Bitmap created by CreateDIBSection API call
		[XmlIgnoreAttribute]
		public static IntPtr glDrawBitmapHDC = IntPtr.Zero; // Handle to device context for glDrawBitmap
		[XmlIgnoreAttribute]
		public static IntPtr glDrawBitmapDataPtr = IntPtr.Zero; // Location of bits representing the data of glDrawBitmap
#endif
#if !WindowsCE && !__ANDROID__   
        // On the Full .NET framework, we can use managed bitmaps because they are easier to work with
        // and possibly faster too since we couldn't figure out how to properly BitBlit a 2-color bitmap
        // to another drawing canvas.
        [XmlIgnoreAttribute]
        public static Bitmap OffscreenBitmapWin32 = null;
#endif

#if __ANDROID__
        [XmlIgnoreAttribute]
        public static Android.Graphics.Bitmap OffscreenBitmapWin32 = null;

        [XmlIgnoreAttribute]
        public static Graphics OffScreenGraphics = null;
#endif


        // Managed and Unmanaged fonts we will be using
        [XmlIgnoreAttribute]
        public static Font gl8x8PrinterFont = null;
        [XmlIgnoreAttribute]
        public static Font gl12x12PrinterFont = null;
        [XmlIgnoreAttribute]
        public static Font gl16x16PrinterFont = null;
        [XmlIgnoreAttribute]
        public static Font gl16x16PrinterFontBold = null;
        [XmlIgnoreAttribute]
        public static Font gl20x20PrinterFont = null;
        [XmlIgnoreAttribute]
        public static Font gl24x24PrinterFont = null;
        [XmlIgnoreAttribute]
        public static Font gl24x24VerticalPrinterFont = null;
        [XmlIgnoreAttribute]
        public static IntPtr gl24x24VerticalPrinterFontForAPI = IntPtr.Zero;

        [XmlIgnoreAttribute]
        public static bool ScaleTestToFit = false;


        public Android.Graphics.Bitmap GetOffscreenBitmapWin32()
        {
            return OffscreenBitmapWin32;
        }


        public void PrepareForPrint()
        {
            if (this is TWinPrnPanel)
            {
                //Reset our string collector array
                ticketStrings.Clear();

                TWinPrnPanel ThisPanel = this as TWinPrnPanel;
                foreach (TWinPrnBase Child in ThisPanel.Children)
                    Child.PrepareForPrint();
                return;
            }

            if (this is TWinBasePrnData) //TWinPrnData
            {
                TWinBasePrnData ThisData = this as TWinBasePrnData;

                // Set visible property of prompt. Will be hidden if data is blank and fHidePromptIsBlank is true.
                if (ThisData.PromptWinCtrl != null)
                {
                    // Start with the prompt not hidden
                    ThisData.PromptWinCtrl.Hidden = false;
                    if ((ThisData.HidePromptIfBlank == true) && (this.TextBuf == ""))
                        ThisData.PromptWinCtrl.Hidden = true;
                }

                // Is this a barcode? If so, translate it 
                
                switch (ThisData.Font)
                {
                    case TFont.FontBC128:  
                        {
                            AddPrintBarCodeInfo(ThisData.TextBuf, ThisData.Font); //For PCL format we need to use the text before we transalt it 
                            TranslateTextToBC128();
                            break; 
                        }
                    case TFont.FontBC3of9:
                    case TFont.FontBC39:
                        {
                            AddPrintBarCodeInfo(ThisData.TextBuf, ThisData.Font); //For PCL format we need to use the text before we transalt it 
                            TranslateTextToBC3of9();
                            break;
                        }
                };
            }
        }

        public int PaintDescendants()
        {
            this.Paint();

            if (this is TWinPrnPanel)
            {
                TWinPrnPanel ThisPanel = this as TWinPrnPanel;
                foreach (TWinPrnBase Child in ThisPanel.Children)
                    Child.PaintDescendants();
            }
            return 0;
        }

        public Android.Graphics.Bitmap WriteBitmapData()
        {
            String storagePath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

            try
            {

                // debug - print to Zebra - save two versions

                String filePath = storagePath + "/APPNAME2.png";
                using (var os = new FileStream(filePath, FileMode.Create))
                {
                    OffscreenBitmapWin32.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 95, os);
                    os.Close();
                }

                String filePath2 = storagePath + "/APPNAME2.jpg";
                using (var os2 = new FileStream(filePath2, FileMode.Create))
                {
                    OffscreenBitmapWin32.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 95, os2);
                    os2.Close();
                }

            }
            catch (Java.IO.FileNotFoundException e1)
            {
                System.Console.WriteLine("FILENOTFOUND");
            }
            catch (Java.IO.IOException e2)
            {
                System.Console.WriteLine("IOEXCEPTION");
            }
            return OffscreenBitmapWin32;
        }

        protected enum TBC128CharSet { CharSetNone = 0, CharSetA, CharSetB, CharSetC };

        // Returns True if code C should be used. Code C is used if the current code is c and
        // the next 2 chars are numeric OR if the current code is not C and the next 4 chars
        // are numeric
        private bool UseCodeC(string pSrcText, int pStartIdx, TBC128CharSet iCharSet)
        {
            int loNumericCnt = 0;
            for (int loNdx = 0; loNdx < 4; loNdx++)
            {
                // Avoid accessing invalid character index within the string
                if ((pSrcText.Length - 1) < (pStartIdx + loNdx))
                    break;
                // Break out of loop if not numeric character
                if ((pSrcText[pStartIdx + loNdx] < '0') || (pSrcText[pStartIdx + loNdx] > '9'))
                    break;
                loNumericCnt++;
            }

            // We will use Code C if there are 4 numeric characters or if we're already in Code C and
            // there are at least 2 more numeric characters.

            if (loNumericCnt == 4)
                return true;
            else if ((loNumericCnt > 1) && (iCharSet == TBC128CharSet.CharSetC))
                return true;
            else
                return false;
        }

        protected int DrawBarcode(TFont iFont, Rectangle iRectangle, Justification_Android pJustification, Rotation_Android iRotation)
        {
     
            int loStrLen = this.TextBuf.Length;
            ushort loIdx;
            ushort loBitIdx;
            ushort loPixelIdx;
            ushort loRowIdx;
            ushort loCharBitmap;
            int loFontMult = 2; // iFontHorzMult; //2;
            int loFontMultIdx;
            int loJustificationOffset;
            int loBarCodeWidthJustification;
            const int FontBC128Width = 10;
            const int FontBC3of9Width = 15;
            const int FontBC3of9Height = 1;
            int iFontVertMult;
            Rectangle loRectangle;
       
            //Adjust the font Vert mult based on the orintation 
            if (iRotation == Rotation_Android.Rotate90 || iRotation == Rotation_Android.Rotate270)
            {
                iFontVertMult = iRectangle.Width;
                loRectangle = new Rectangle(iRectangle.Left, iRectangle.Top, iRectangle.Width, iRectangle.Height); //We will set the top cord to 0 as we will calc it based on the justification 
            }else{
                iFontVertMult = iRectangle.Height;
                loRectangle = new Rectangle(0, iRectangle.Top, iRectangle.Width, iRectangle.Height); //We will set the Left cord to 0 as we will calc it based on the justification 
            }
            // Adjust string length if necessary 
            if (loStrLen >= 80) loStrLen = 80;
            
            //Printing the border for debugging 
            //Pen loPen = new Pen(Color.Black);
            //OffScreenGraphics.DrawRectangle(loPen, iRectangle.X, iRectangle.Y, iRectangle.Width, iRectangle.Height);

            // Minimum Font Multiplier is 1
            if (loFontMult < 1) loFontMult = 1;

            // Init horizontal offset 
            loJustificationOffset = 0;

            // init value for justification calculations
            loBarCodeWidthJustification = FontBC128Width + 1;
            if (iFont == TFont.FontBC39 || iFont == TFont.FontBC3of9)
            {
                loBarCodeWidthJustification = FontBC3of9Width + 1;
            }
   
    		// Are we justifying the barcode?
    		
            switch ( pJustification )
    		{
                case Justification_Android.Center:
	    		{
		    		switch (iRotation)
		    		{
						case Rotation_Android.Rotate270:
						{
                            loJustificationOffset = (loRectangle.Height / 2) - ((loStrLen * loBarCodeWidthJustification * loFontMult) / 2);
                            break;
						}

						case Rotation_Android.Rotate90:
						{
                            loJustificationOffset = (loRectangle.Height / 2) - ((loStrLen * loBarCodeWidthJustification * loFontMult) / 2);
							break;
						}

			            default:
			            {
                            loJustificationOffset = (OffscreenBitmapWin32.Width / 2) - ((loStrLen * loBarCodeWidthJustification * loFontMult) / 2);
				            break;
			            }
		            }
		            break;
	            }

                case Justification_Android.Right:
	            {
		            // right justify but bump the length for a bit more space so we dont truncate the barcode
		            int loStrLenForRightJustify = (loStrLen + 1);
		            if ( loStrLenForRightJustify > 80) 
		            {
			            loStrLenForRightJustify = 80;
		            }
		            switch (iRotation)
		            {
			            case Rotation_Android.Rotate270:
			            {
                            loJustificationOffset = (loRectangle.Height) - ((loStrLenForRightJustify * loBarCodeWidthJustification * loFontMult));
				            break;
			            }

			            case Rotation_Android.Rotate90:
			            {
                            loJustificationOffset = (loRectangle.Height) - ((loStrLenForRightJustify * loBarCodeWidthJustification * loFontMult));
				            break;
			            }

			            default:
			            {
                            loJustificationOffset = (OffscreenBitmapWin32.Width) - ((loStrLenForRightJustify * loBarCodeWidthJustification * loFontMult));
				            break;
			            }
		            }
		            break;
	            }

	            default :
		        {
			        // no justifications
			        break;
		        }
	        }
            
            // Are we drawing the 128-Barcode?
            if (iFont == TFont.FontBC128)
            {
                for (loIdx = 0; loIdx < loStrLen; loIdx++)
                {   // loop through all code words
                    loCharBitmap = (ushort)PrinterFontBC128Table[(int)this.TextBuf[loIdx]];
                    for (loRowIdx = 0; loRowIdx < iFontVertMult/*FontBC128DesiredHeight*/; loRowIdx++)
                    { // repeat for each horizontal row
                        loPixelIdx = 1;
                        for (loBitIdx = 1; loBitIdx <= 0x200; loBitIdx <<= 1)
                        { // process each bit in a code word
                            for (loFontMultIdx = 1; loFontMultIdx <= loFontMult; loFontMultIdx++)
                            {
                                if ((loCharBitmap & loBitIdx) > 0)
                                {

                                    switch (iRotation)
                                    {
                                        case Rotation_Android.Rotate270:
                                            {
                                                // Output adjusted for Vertical Font Angle
                                                OffscreenBitmapWin32.SetPixel(loRectangle.Left + loRowIdx,
                                                        (loRectangle.Bottom - loJustificationOffset) - ((loIdx * ((FontBC128Width + 1) * loFontMult)) + (loPixelIdx * loFontMult) + (loFontMultIdx - 1)),
                                                        Color.Black);
                                                break;
                                            }

                                        case Rotation_Android.Rotate90:
                                            {
                                                // Output adjusted for Vertical Font Angle
                                                OffscreenBitmapWin32.SetPixel(loRectangle.Right - loRowIdx,
                                                        (loRectangle.Top + loJustificationOffset) + (loIdx * ((FontBC128Width + 1) * loFontMult)) + (loPixelIdx * loFontMult) + (loFontMultIdx - 1),
                                                        Color.Black);
                                                break;
                                            }

                                        default:
                                            {
                                                // Output in normal orientation  						        
                                                OffscreenBitmapWin32.SetPixel(loRectangle.Left + loJustificationOffset + (loIdx * ((FontBC128Width + 1) * loFontMult)) + (loPixelIdx * loFontMult) + (loFontMultIdx - 1),
                                                        loRectangle.Top + loRowIdx,
                                                        Color.Black);
                                                break;
                                            }
                                    }
                                }
                            }
                            loPixelIdx++;
                        } // for loBitIdx
                    } // for loRowIdx
                } // for loIdx
                return 0;
            } //BC128 font

            if (iFont == TFont.FontBC3of9 || iFont == TFont.FontBC39)
            {
                loBitIdx = 1;
                for (loIdx = 0; loIdx < loStrLen; loIdx++)  
		        {
                    loCharBitmap = (ushort)PrinterFontBC3of9Table[(int)this.TextBuf[loIdx]]; ;
			        for (loRowIdx = 0; loRowIdx < iFontVertMult; loRowIdx++)
			        { 
				        loPixelIdx = 1;
                        for (loBitIdx = 1; loBitIdx <= 16384; loBitIdx = (ushort)(loBitIdx << 1))
				        {
                            for (loFontMultIdx = 1; loFontMultIdx <= loFontMult; loFontMultIdx++)
					        {
						        if ((loCharBitmap & loBitIdx) > 0)
						        {
  							        switch (iRotation)
							        {
								        case Rotation_Android.Rotate270:
								        {
									        // Output adjusted for Vertical Font Angle
                                            OffscreenBitmapWin32.SetPixel(loRectangle.Left + loRowIdx,
										            (loRectangle.Bottom - loJustificationOffset) - ((loIdx * ((FontBC3of9Width + 1) * loFontMult)) + (loPixelIdx * loFontMult) + (loFontMultIdx - 1)), 
			  							            Color.Black);
									        break;
								        }

								        case Rotation_Android.Rotate90:
								        {
									        // Output adjusted for Vertical Font Angle
                                            OffscreenBitmapWin32.SetPixel(loRectangle.Right - loRowIdx,
		  								            (loRectangle.Top + loJustificationOffset) + (loIdx * ((FontBC3of9Width + 1) * loFontMult)) + (loPixelIdx * loFontMult) + (loFontMultIdx - 1), 
			  							            Color.Black);
									        break;
								        }
          
								    default:
								        {
									    // Output in normal orientation
                                        OffscreenBitmapWin32.SetPixel(loRectangle.Left + loJustificationOffset + 
		  								            (loIdx * ((FontBC3of9Width + 1) * loFontMult)) + (loPixelIdx * loFontMult) + (loFontMultIdx - 1), 
			  							            loRectangle.Top + loRowIdx, 
			  							            Color.Black);
									break;
								        }
							        } // switch iRotation
                                }
					        } // for loFontMultIdx
					        loPixelIdx++;
				        } // for loBitIdx
			        } // for loRowIdx
		        } // for loIdx
            }

            //Font is not supported, just return now
            return 0;   
        }

        protected int TranslateTextToBC128()
        {
            /*const int BC128_1stChar = 0x20;*/
            /*const int BC128_LastChar = 0x7E;*/
            const int bc128StartCharSetA = 103; // we won't use set A, smAlpha1 
            const int bc128StartCharSetB = 104; // symbol start char indicating code set B in use 
            const int bc128StartCharSetC = 105; //  symbol start char indicating code set C in use 
            const int bc128StopCharA = 106;   // symbol stop char (1st 11 modules)
            const int bc128StopCharB = 107;   // symbol stop char (last 2 modules)
            // code set switching chars. Used anywhere in symbol to switch code sets 
            const int bc128SwitchToSetAChar = 101;
            const int bc128SwitchToSetBChar = 100;
            const int bc128SwitchToSetCChar = 99;

            StringBuilder loBuf = new StringBuilder("");

            // Replace tab indicators with actual tab data
            this.TextBuf = this.TextBuf.Replace("\\t", "\t");

            int loLen = this.TextBuf.Length;
            int loFromNdx = 0;
            int loToNdx = 0;
            int loCheckDigit = 0;
            TBC128CharSet loCharSet = TBC128CharSet.CharSetNone;

            if (loLen == 0) return 0; // nothing to bar code

            // We will not use code set A. Code Set B provides all ascii characters between 0x20 &
            //   0x7E, and Code Set C provides 2 digits per byte from "00" to "99".  Code set A has
            //   control characters and such. 

            // loop through the source string
            for (; loFromNdx < loLen; )
            {
                // are the next 2 digits numerics? If so, switch to code C (if necessary)
                if (UseCodeC(TextBuf, loFromNdx, loCharSet) == true)
                { // code "c" it is!
                    // are we already at code "c"??
                    if (loCharSet != TBC128CharSet.CharSetC)
                    { // nope. If beginning of symbol, use StartC, otherwise use SwitchToC
                        if (loCharSet == TBC128CharSet.CharSetNone)
                            loBuf.Append((char)(bc128StartCharSetC + ' '));
                        else
                            loBuf.Append((char)(bc128SwitchToSetCChar + ' '));
                        loCharSet = TBC128CharSet.CharSetC;
                    }

                    // now record the code word.
                    int loChar = 10 * (TextBuf[loFromNdx] - '0') + TextBuf[loFromNdx + 1] - '0' + ' ';
                    loBuf.Append(Convert.ToChar(loChar));

                    // We just encoded 2 digits, so advance loFromNdx by 2
                    loFromNdx += 2;
                }
                else if ((TextBuf[loFromNdx] >= 'a') && (TextBuf[loFromNdx] <= 'z'))
                {
                    // lower case letter, shift to code "b"
                    if (loCharSet != TBC128CharSet.CharSetB)
                    { // nope. If beginning of symbol, use StartB, otherwise use SwitchToB
                        if (loCharSet == TBC128CharSet.CharSetNone)
                            loBuf.Append((char)(bc128StartCharSetB + ' '));
                        else
                            loBuf.Append((char)(bc128SwitchToSetBChar + ' '));
                        loCharSet = TBC128CharSet.CharSetB;
                    }

                    // now record the code word. And advance both "to" and "from"
                    loBuf.Append(TextBuf[loFromNdx++]);
                }
                else
                {
                    // everything else is code A
                    if (loCharSet != TBC128CharSet.CharSetA)
                    {
                        // nope. If beginning of symbol, use StartA, otherwise use SwitchToA
                        // hold on. If we are in B and this is not control character, we can stay in B
                        if ((loCharSet != TBC128CharSet.CharSetB) || (TextBuf[loFromNdx] < ' '))
                        {
                            if (loCharSet == TBC128CharSet.CharSetNone)
                                loBuf.Append((char)(bc128StartCharSetA + ' '));
                            else
                                loBuf.Append((char)(bc128SwitchToSetAChar + ' '));
                            loCharSet = TBC128CharSet.CharSetA;
                        }
                    }

                    // now record the code word. And advance both "to" and "from"
                    if (TextBuf[loFromNdx] < ' ')
                        loBuf.Append((char)(TextBuf[loFromNdx++] + 64 + ' ')); // 64 is offset to control char symbols.
                    else
                        loBuf.Append(TextBuf[loFromNdx++]);
                }

            } // for loop that encodes the data.

            // After the data comes the check digit. It is calculated by summing the product of each
            //   code byte and its position, then taking the remainder modulo 103.  (Position 0, the start
            //   character, has weight 1). 
            // The "To" index needs to be the length of the current string
            string loTempStr = loBuf.ToString();
            loToNdx = loTempStr.Length;
            loCheckDigit = ((char)(loBuf[0] - ' ' + loBuf[1] - ' '));
            for (loFromNdx = 2; loFromNdx < loToNdx; loFromNdx++)
                loCheckDigit += ((int)loBuf[loFromNdx] - ' ') * loFromNdx;

            loCheckDigit %= 103;
            // Add the check digit 
            loBuf.Append((char)(loCheckDigit + ' '));
            // Finally, add the stop character.  The stop character is actually 13 modules (the rest of 
            //   the character set is 11 modules).  Due to the way the characters are stored in the font,
            //   the stop character is stored across two characters.  Luckily, position 11 in the stop char
            //   is a space, so when we print (each character has a space appended to it) we will be OK. 
            loBuf.Append((char)(bc128StopCharA + ' '));
            loBuf.Append((char)(bc128StopCharB + ' '));

            // All done.  Write it back to fTextBuf 
            this.TextBuf = loBuf.ToString();
            return loToNdx;
        }

        /*
          Takes the contents of the window (in fTextBuf) and translates it into BC3of9

             3of9 is pretty straight forward: a simple sequence with a start char,
             the actual data, followed by the stop char. The start and stop char are the same
             There is no check digit... although a summary found on the 'net may differ:

          "A check digit is not often used with Code 39 but a few critical applications
          may require one. The check digit is the modulus 43 sum of all the character
          values in the message. It is printed as the last data character.
          The following table shows the character and value used for the calculation..."

          Char Value        Char Value         Char Value         Char Value
           0     0           B    11            M    22            X    33
           1     1           C    12            N    23            Y    34
           2     2           D    13            O    24            Z    35
           3     3           E    14            P    25            -    36
           4     4           F    15            Q    26            .    37
           5     5           G    16            R    27          space  38
           6     6           H    17            S    28            $    39
           7     7           I    18            T    29            /    40
           8     8           J    19            U    30            +    41
           9     9           K    20            V    31            %    42
           A    10           L    21            W    32

          Example calculation
          Data: 12345ABCDE/
          Sum of values: 1 + 2 + 3 + 4 + 5 + 10 + 11 + 12 + 13 + 14 + 40 = 115
          115 divided by 43 = 2 remainder 29. Therefore T is the check digit.
          Data with check digit: 12345ABCDE/T
         */
        protected int TranslateTextToBC3of9()
        {
            const int BC3of9_StartChar = 0x2A;  // start and stop are the same char
            const int BC3of9_StopChar = 0x2A;

            StringBuilder loBuf = new StringBuilder("");
            int loLen = this.TextBuf.Length;
            int loFromNdx = 0;
            int loToNdx = 0;

            if (loLen == 0) return 0; // nothing to bar code

            // start with the start char
            loBuf.Append((char)(BC3of9_StartChar));

            // loop through source and copy the chars over
            for (; loFromNdx < loLen; )
            {
                // copy the char, and advance both "to" and "from"
                loBuf.Append(TextBuf[loFromNdx++]);
            }

            // add the stop char
            loBuf.Append((char)(BC3of9_StopChar));

            // all done.  Write it back to fTextBuf 
            this.TextBuf = loBuf.ToString();
            return loToNdx;
        }

        protected void CreatePrinterFonts()
        {
#if WindowsCE  // this is all WindowsCE specific, no ANDROID code within
            #region This is a mess of platform dependent stuff, so it looks pretty ugly


            // Create all TrueType fonts that may be used. Certain fonts such as OCR and BarCodes will
            // not have a TrueType font associated with them (they will be drawn with our own bitmaps)
#if WindowsCE
            // Create a logical font object.
			// For Compact Framework on WinCE, we have to use the .NET supplied LogFont class
			Microsoft.WindowsCE.Forms.LogFont loLogFont = new LogFont();
#else
            // Create a logical font object.
            // .NET didn't supply a LogFont class for full framework, so we have to use LOGFONT structure,
            // which we defined with the same member names as the LogFont class used by WinCE
            LOGFONT loLogFont = new LOGFONT();
#endif
            // Set most of the common attributes for our fonts
            loLogFont.Escapement = 0;
            loLogFont.Orientation = loLogFont.Escapement;
            loLogFont.Italic = 0;
            loLogFont.Underline = 0;
            loLogFont.StrikeOut = 0;
#if WindowsCE
            loLogFont.CharSet = LogFontCharSet.ANSI;
			loLogFont.OutPrecision = LogFontPrecision.Default;
			loLogFont.Quality = LogFontQuality.Default;
			loLogFont.PitchAndFamily = LogFontPitchAndFamily.Variable | LogFontPitchAndFamily.DontCare;
#else
            loLogFont.CharSet = (byte)LogFontCharSet.ANSI;
            loLogFont.OutPrecision = (byte)LogFontPrecision.Default;
            loLogFont.Quality = (byte)LogFontQuality.Default;
            loLogFont.PitchAndFamily = (byte)(LogFontPitchAndFamily.Variable | LogFontPitchAndFamily.DontCare);
#endif
            // Create an 8x8 font
            // This is original code for printer 8x8, but found other settings that work better?
            /*
            loLogFont.FaceName = "Arial";
            loLogFont.Height = 10;
            loLogFont.Width = 5;
            loLogFont.Weight = LogFontWeight.SemiBold;
            */
            loLogFont.FaceName = "Tahoma";
            loLogFont.Height = -10;
            loLogFont.Width = 0;
#if WindowsCE
            loLogFont.Weight = LogFontWeight.Light; // 700;
#else
            loLogFont.Weight = (int)LogFontWeight.Light; // 700;
#endif
            // Create font if we don't already have it
            if (gl8x8PrinterFont == null)
                gl8x8PrinterFont = System.Drawing.Font.FromLogFont(loLogFont);

            // Create a 12x12 font
            loLogFont.Height = 18;
            loLogFont.Width = 0;
#if WindowsCE
            loLogFont.Weight = LogFontWeight.Heavy; 
#else
            loLogFont.Weight = (int)LogFontWeight.Heavy;
#endif
            // Create font if we don't already have it
            if (gl12x12PrinterFont == null)
                gl12x12PrinterFont = System.Drawing.Font.FromLogFont(loLogFont);

            // Create a 16x16 font
            loLogFont.FaceName = "Tahoma";
            loLogFont.Width = 0;
            loLogFont.Height = -18;
            loLogFont.Width = 0;
#if WindowsCE
            loLogFont.Weight = LogFontWeight.Thin; 
			loLogFont.Quality = LogFontQuality.Draft; 
#else
            loLogFont.Weight = (int)LogFontWeight.Thin;
            loLogFont.Quality = (byte)LogFontQuality.Draft;
#endif
            // Create font if we don't already have it
            if (gl16x16PrinterFont == null)
                gl16x16PrinterFont = System.Drawing.Font.FromLogFont(loLogFont);

            // Create a 16x16 Bold font
            loLogFont.FaceName = "Tahoma";
            loLogFont.Width = 0;
            loLogFont.Height = 20;
            loLogFont.Width = 0;
#if WindowsCE
            loLogFont.Weight = LogFontWeight.Heavy; 
			loLogFont.Quality = LogFontQuality.Draft; 
#else
            loLogFont.Weight = (int)LogFontWeight.Heavy;
            loLogFont.Quality = (byte)LogFontQuality.Draft;
#endif
            // Create font if we don't already have it
            if (gl16x16PrinterFontBold == null)
                gl16x16PrinterFontBold = System.Drawing.Font.FromLogFont(loLogFont);

            // Create a 20x20 font
            loLogFont.Width = 0;
            loLogFont.Height = 26;
#if WindowsCE
            loLogFont.Weight = LogFontWeight.SemiBold; 
#else
            loLogFont.Weight = (int)LogFontWeight.SemiBold;
#endif
            // Create font if we don't already have it
            if (gl20x20PrinterFont == null)
                gl20x20PrinterFont = System.Drawing.Font.FromLogFont(loLogFont);

            // Create a 24x24 font
            loLogFont.Height = -24; // -25; 2008.07.24 - made font smaller for better fit in printpictures
            loLogFont.Width = 0;
#if WindowsCE
            loLogFont.Weight = LogFontWeight.ExtraBold; //Heavy; // ExtraBold
#else
            loLogFont.Weight = (int)LogFontWeight.ExtraBold; // Heavy; //ExtraBold
#endif
            // Create font if we don't already have it
            if (gl24x24PrinterFont == null)
                gl24x24PrinterFont = System.Drawing.Font.FromLogFont(loLogFont);

            // Create a 24x24 VERTICAL font
            // When rendering fonts for the emulator, when it comes
            // to the 24x24Vert font, we will not rotate the font -
            // this is because the emulator does the rotation during the printing
            loLogFont.Escapement = 90 * 10;
            loLogFont.Orientation = loLogFont.Escapement;
            // Create font if we don't already have it
            if (gl24x24VerticalPrinterFont == null)
                gl24x24VerticalPrinterFont = System.Drawing.Font.FromLogFont(loLogFont);


#if !WindowsCE
            // DEBUG -- CreateFontIndirect is not supported in our handheld?

            // The managed font doesn't seem to rotate, so verticle font will need to be drawn via API call.
            // Create a 24x24 vertical font suitable for use with API drawing functions
            IntPtr pLF = WinGDI.LocalAlloc(0x40, 92);
            Marshal.StructureToPtr(loLogFont, pLF, false);
            // Create font if we don't already have it
            if (gl24x24VerticalPrinterFontForAPI == IntPtr.Zero)
                gl24x24VerticalPrinterFontForAPI = WinGDI.CreateFontIndirect(pLF);
            WinGDI.LocalFree(pLF);
#endif
            #endregion
#endif
        }

        /*
         * Draws a complete Window on the screen.  
         * - Erases the window first (Fills it with the background color).
         * - Draws the inner and outer frames (if the window has them).
         * - Draws the text in the window. The text can scroll both vertically and horizontally in
         *   the window.  The 1st character of text painted for non-console windows 
         *   is held in the property fFirstVisibleChar.
         */
        protected int Paint()
        {
            // Don't paint if we're marked as "hidden"
            if (this.Hidden == true)
                return 0;

            Rectangle loLinePoints;

            // Get our absolute coordinates 
            AbsoluteCoordinates();

            // Always clear the background first since print picture might be reliant on this behavior to 
            // essentially erase something underneath. (Basically we need to be opaque rather than transparent)
            Printer_FillBox(fAbsCoordinates.Top, fAbsCoordinates.Left, fAbsCoordinates.Height, fAbsCoordinates.Width, Color.White);

            // Draw the window frame if necessary 
            if (this._FrameThickness > 0)
            {
                Printer_DrawBox(fAbsCoordinates.Top, fAbsCoordinates.Left, fAbsCoordinates.Height,
                    fAbsCoordinates.Width, this._FrameThickness, Color.Black);
            }

            // Paint the text unless its a TWinPrnDraw (used for signatures and diagrams) or a TWinPrnImage (used for embedded images)
            if ((!(this is TWinPrnDraw)) && (!(this is TWinPrnImage)))
			{
                PaintText();
			}
			else
            {
                PaintImage();
            }
				

            return 0;
        }

		public void PaintImage()
        {
            AbsoluteCoordinates();
            //Extract the image point from TextBuf
            string loTextBuf = this.TextBuf.Replace("\n", ","); //Replace all line breaks so we will get one line of text not multilines
            string[] loPoints = loTextBuf.Split(',');
            if (loPoints.Length <= 0) return; //no points to draw
            Pen loPen = new Pen(Color.Black);
            for (int i=0; i<loPoints.Length; i++)
            {
                string[] loPoint = loPoints[i].Split(' ');
                if (loPoint.Length < 2) continue; //Not valid point, continue
                int loX = Convert.ToInt32(loPoint[0], 16) + fAbsCoordinates.Left;
                int loY = Convert.ToInt32(loPoint[1], 16) + fAbsCoordinates.Top;
                OffScreenGraphics.DrawPoint(loX, loY, loPen);
            }
        }

        public int Series3CE_ClearPrintCanvas(int iDotHeight, int iDotWidth)
        {
            // On the full .NET Framework, we will use bitmaps that are 24-bits per pixel,
            // because they are the easiest to work with.
            OffscreenBitmapWin32 = Android.Graphics.Bitmap.CreateBitmap(iDotWidth, iDotHeight,
                    Android.Graphics.Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(OffscreenBitmapWin32);
            OffScreenGraphics = new Graphics(canvas);
            OffScreenGraphics.Clear(Color.White);
            return 0;
        }

        protected int Printer_FillBox(int iTop, int iLeft, int iHeight, int iWidth, Color iWriteColor)
        {

            // Calculated rectangle dimensions
            Rectangle loRect = new Rectangle(iLeft, iTop, iWidth, iHeight);
            OffScreenGraphics.FillRectangle(new Brush(Color.White), iLeft, iTop, iWidth, iHeight);
            return 0;

        }

        protected int Printer_DrawBox(int iTop, int iLeft, int iHeight, int iWidth,
            int iLineThickness, Color iWriteColor)
        {
            Pen DrawPen = new Pen(Color.Black, iLineThickness);


            // AJW - review - this assumes 3in printer
            // We have a problem making vertical line on last pixel, so adjust as needed
            if (iLeft + iWidth == 576)
                iWidth = iWidth - 1;

            OffScreenGraphics.DrawRectangle(DrawPen, iLeft, iTop, iWidth, iHeight);

            return 0;
        }

        // Helper function to convert type def
        private Justification_Android GetAndroidJustificationForTJustification(TJustification iTJustification)
        {
            switch (iTJustification)
            {
                //case TJustification.Left: { return Justification_Android.Left; }
                case TJustification.Center: { return Justification_Android.Center; }
                case TJustification.Right: { return Justification_Android.Right; }

                // default - same as left
                default: { return Justification_Android.Left; }
            }
        }

        // Helper function to convert type def
        private Rotation_Android GetAndroidRotationForTRotation(TRotation iTRotation)
        {
            switch (iTRotation)
            {
                //case TRotation.Rotate0: { return Rotation_Android.Rotate0; }
                case TRotation.Rotate90: { return Rotation_Android.Rotate90; }
                case TRotation.Rotate180: { return Rotation_Android.Rotate180; }
                case TRotation.Rotate270: { return Rotation_Android.Rotate270; }

                // default - same as 0
                default: { return Rotation_Android.Rotate0; }
            }

        }

		protected void AddPrintTextInfo(string text, Font font, TWinBase.TFont iDuncanFont, RectangleF rect)
		{
			if(text.Length <= 0) return; //nothing to do

            PrintTextInfo loPrintTextInfoObj = new PrintTextInfo();
            loPrintTextInfoObj.text = text;
            loPrintTextInfoObj.y = (int)rect.Top;
            loPrintTextInfoObj.font = font;
            loPrintTextInfoObj.duncanFont = iDuncanFont;

            //Calc the x based on the justification
            switch (font.Justification)
            {
                case Justification_Android.Center:
                    {
                        loPrintTextInfoObj.x = (int)(rect.Left + rect.Width / 2);
                        break;
                    }
                case Justification_Android.Right:
                    {
                        int loTextSize = text.Length * font.Size;
                        if (loTextSize < rect.Width)
                        {
                            loPrintTextInfoObj.x = (int)(rect.Left + rect.Width - loTextSize);
                        }
                        else
                        {
                            loPrintTextInfoObj.x = (int)(rect.Left);
                        }
                        break;
                    }
                case Justification_Android.Left:
                default:
                    {
                        loPrintTextInfoObj.x = (int)(rect.Left);
                        break;
                    }
            }

            // keep a copy for calculations
            loPrintTextInfoObj.sourceRect = new Rectangle(fAbsCoordinates.Left, fAbsCoordinates.Top,
                                             fAbsCoordinates.Width, fAbsCoordinates.Height);


            ticketStrings.Add(loPrintTextInfoObj);
		}

        protected void AddPrintBarCodeInfo(string text, TWinBase.TFont iDuncanFont)
        {
            if (text.Length <= 0) return; //nothing to do
            //Build the barcode border rect
            AbsoluteCoordinates();
            Rectangle loRect = new Rectangle(fAbsCoordinates.Left, fAbsCoordinates.Top,
                                             fAbsCoordinates.Width, fAbsCoordinates.Height);

            PrintTextInfo loPrintTextInfoObj = new PrintTextInfo();
            loPrintTextInfoObj.text = text;

            // keep a copy for calculations
            loPrintTextInfoObj.sourceRect  = new Rectangle(fAbsCoordinates.Left, fAbsCoordinates.Top,
                                             fAbsCoordinates.Width, fAbsCoordinates.Height);

            loPrintTextInfoObj.y = (int)loRect.Top;
            loPrintTextInfoObj.duncanFont = iDuncanFont;

            //We will set the justification to the left side as PCL doesn't support barcode justification
            loPrintTextInfoObj.x = (int)(loRect.Left);
            //Add the barcode object to the list         
            ticketStrings.Add(loPrintTextInfoObj);
        }

        protected int PaintText()
        {
            string loText = this.TextBuf;

            AbsoluteCoordinates();
            RectangleF loTempRectangleF = new RectangleF(fAbsCoordinates.Left, fAbsCoordinates.Top,
                fAbsCoordinates.Width, fAbsCoordinates.Height);
            //Create one with int dimensions 
            Rectangle loTempRectangle = new Rectangle(fAbsCoordinates.Left, fAbsCoordinates.Top,
                fAbsCoordinates.Width, fAbsCoordinates.Height);

            // AJW - for review - we need to zero in on accurate reproduction

            /*
            Font loFontToPrint = new Font("serif", 12, FontStyle.Italic);

            switch (this.Font)
            {
                case TFont.Font8x8: { loFontToPrint = new Font("sansserif", 10, FontStyle.Regular); break; }
                case TFont.Font16x16: { loFontToPrint = new Font("sansserif", 12, FontStyle.Regular); break; }
                case TFont.Font24x24: { loFontToPrint = new Font("sansserif", 14, FontStyle.Bold); break; }
                case TFont.FontBC128: { TranslateTextToBC128(); break; }  // Barcode
            };
			*/


            // translate justification and rotation
            Justification_Android loJustification = GetAndroidJustificationForTJustification(this.Justification);
            Rotation_Android loRotation = GetAndroidRotationForTRotation(this.Rotation);

            // select font 
            Font loFontToPrint = null;

            //// Common Font characteristics
            //loLogFont.lfEscapement = 0;
            //loLogFont.lfOrientation = loLogFont.lfEscapement;
            //loLogFont.lfItalic = 0;
            //loLogFont.lfUnderline = 0;
            //loLogFont.lfStrikeOut = 0;
            //loLogFont.lfCharSet = ANSI_CHARSET;
            //loLogFont.lfOutPrecision = OUT_DEFAULT_PRECIS;
            //loLogFont.lfQuality = DEFAULT_QUALITY;
            //loLogFont.lfPitchAndFamily = VARIABLE_PITCH | FF_DONTCARE;


            //(Droid Sans), serif (Droid Serif), and monospace (Droid Sans Mono)
            string loPrinterFontFamilyToUse = "sansserif";
                   

            switch (this.Font)
            {


                //// Create an 8x8 font
                //lstrcpy(loLogFont.lfFaceName, _T("Tahoma"));
                //loLogFont.lfHeight = 10;
                //loLogFont.lfWidth = 5;
                //loLogFont.lfWeight = FW_SEMIBOLD; // 600
                //gl8x8PrinterFont = CreateFontIndirect(&loLogFont);
                case TFont.Font8x8:
                    {
                        loFontToPrint = new Font(loPrinterFontFamilyToUse, 10, FontStyle.Regular, loJustification, loRotation);
                        break;
                    }


                //// Create a 12x12 font
                //lstrcpy(loLogFont.lfFaceName, _T("Tahoma"));
                //loLogFont.lfHeight = 18; //14;
                //loLogFont.lfWidth = 0;   //7;
                //loLogFont.lfWeight = FW_NORMAL;
                //gl12x12PrinterFont = CreateFontIndirect(&loLogFont);
                case TFont.Font12x12: { loFontToPrint = new Font(loPrinterFontFamilyToUse, 12, FontStyle.Regular, loJustification, loRotation); break; }

                //// Create a 12x12 Bold font
                //lstrcpy(loLogFont.lfFaceName, _T("Tahoma"));
                //loLogFont.lfHeight = 18; //14;
                //loLogFont.lfWidth = 0;   //7;
                //loLogFont.lfWeight = FW_NORMAL;
                //gl12x12PrinterFont = CreateFontIndirect(&loLogFont);
                case TFont.Font12x12Bold: { loFontToPrint = new Font(loPrinterFontFamilyToUse, 12, FontStyle.Regular, loJustification, loRotation); break; }


                //// Create a 16x16 font
                //lstrcpy(loLogFont.lfFaceName, TEXT("Tahoma"));
                //loLogFont.lfWidth = 0;
                //loLogFont.lfHeight = -18; //18; //25;
                //loLogFont.lfWidth = 0; //7;
                //loLogFont.lfWeight = FW_THIN; //FW_LIGHT; //FW_NORMAL; //FW_SEMIBOLD; //FW_HEAVY; //FW_BOLD;
                //loLogFont.lfQuality = DRAFT_QUALITY; //DEFAULT_QUALITY;
                //gl16x16PrinterFont = CreateFontIndirect(&loLogFont);
                //case TFont.Font16x16: { loFontToPrint = new Font(loPrinterFontFamilyToUse, 16, FontStyle.Regular, loJustification, loRotation); break; }
                case TFont.Font16x16: { loFontToPrint = new Font(loPrinterFontFamilyToUse, 14, FontStyle.Regular, loJustification, loRotation); break; }


                //// Create a 16x16 Bold font
                //lstrcpy(loLogFont.lfFaceName, TEXT("Tahoma"));
                //loLogFont.lfWidth = 0;
                //loLogFont.lfHeight = 20; //-18; 
                //loLogFont.lfWidth = 0; //7;
                //loLogFont.lfWeight = 600; //FW_SEMIBOLD; 
                //loLogFont.lfQuality = DRAFT_QUALITY; //DEFAULT_QUALITY;
                //gl16x16PrinterFontBold = CreateFontIndirect(&loLogFont);
                //case TFont.Font16x16Bold: { loFontToPrint = new Font(loPrinterFontFamilyToUse, 16, FontStyle.Bold, loJustification, loRotation); break; }
                case TFont.Font16x16Bold: { loFontToPrint = new Font(loPrinterFontFamilyToUse, 14, FontStyle.Bold, loJustification, loRotation); break; }

                //  // Create a 20x20 font
                //  loLogFont.lfWidth = 0; //11; 
                //  loLogFont.lfHeight = 26; //24; //22; //31;
                //  loLogFont.lfWeight = FW_SEMIBOLD; //FW_BOLD;
                //  gl20x20PrinterFont = CreateFontIndirect( &loLogFont );
                //case TFont.Font20x20: { loFontToPrint = new Font(loPrinterFontFamilyToUse, 20, FontStyle.Regular, loJustification, loRotation); break; }
                //case TFont.Font20x20: { loFontToPrint = new Font(loPrinterFontFamilyToUse, 19, FontStyle.Regular, loJustification, loRotation); break; }
                case TFont.Font20x20: { loFontToPrint = new Font(loPrinterFontFamilyToUse, 18, FontStyle.Regular, loJustification, loRotation); break; }

                //  // Create a 24x24 font
                //  loLogFont.lfHeight = -25; //30; Good = 30
                //  loLogFont.lfWidth = 0; //11; 
                //  loLogFont.lfWeight = 600; //FW_SEMIBOLD; //1000; Good = 1000
                //  gl24x24PrinterFont = CreateFontIndirect( &loLogFont );
                //case TFont.Font24x24: { loFontToPrint = new Font(loPrinterFontFamilyToUse, 24, FontStyle.Regular, loJustification, loRotation); break; }
                case TFont.Font24x24: { loFontToPrint = new Font(loPrinterFontFamilyToUse, 22, FontStyle.Regular, loJustification, loRotation); break; }

                //   // Create a 24x24 VERTICAL font
                //  // When rendering fonts for the emulator, when it comes
                //  // to the 24x24Vert font, we will not rotate the font -
                //  // this is because the emulator does the rotation during the printing
                //#ifndef RenderCEPrinterFontBitmapsForEmulator
                //  loLogFont.lfEscapement = 90 * 10;
                //#endif
                //  gl24x24VerticalPrinterFont = CreateFontIndirect( &loLogFont );
                //  loLogFont.lfEscapement = 0;
                case TFont.Font36x36: { loFontToPrint = new Font(loPrinterFontFamilyToUse, 36, FontStyle.Regular, loJustification, loRotation); break; }




                #region BarCode_and_OCR_need_implementation
                // AJW - TODO barcodes/ TRUE OCR need to be implemented

                case TFont.FontOCR:
                    {
                        return 0;

                        loFontToPrint = new Font(loPrinterFontFamilyToUse, 12, FontStyle.Italic, loJustification, loRotation);
                        break;
                    }

                case TFont.FontBC128:
                case TFont.FontBC39:
                case TFont.FontBC3of9:
                    {
                        return DrawBarcode(this.Font, loTempRectangle, loJustification, loRotation);
                    }

                #endregion


                default:
                    {
                        loFontToPrint = new Font("serif", 12, FontStyle.Italic, loJustification, loRotation);
                        break;
                    }
            };

			//Add the string object to our collector array
			AddPrintTextInfo(loText, loFontToPrint, this.Font, loTempRectangleF);

            OffScreenGraphics.DrawString(loText, loFontToPrint, new Brush(Color.Black), loTempRectangleF);

            return 0;
        }

        #endregion
    }

    /// <summary>
    /// Summary description for TPrnControl.
    /// </summary>

    public class TPrnControl : Reino.ClientConfig.TWinPrnBase
    {
        #region Properties and Members
        #endregion

        public TPrnControl()
            : base()
        {
        }
    }

    /// <summary>
    /// Summary description for TWinPrnPrompt.
    /// </summary>

    public class TWinPrnPrompt : Reino.ClientConfig.TWinPrnBase
    {
        #region Properties and Members
        #endregion

        public TWinPrnPrompt()
            : base()
        {
            // Set defaults that Layout Tool uses
            this._Height = 20;
            this._Width = 5;
            this._Font = TFont.Font16x16;
        }
    }

    /// <summary>
    /// Summary description for TWinBasePrnData.
    /// </summary>

    public class TWinBasePrnData : Reino.ClientConfig.TPrnControl
    {
        #region Properties and Members
        protected string _PromptWin = "";
        [System.ComponentModel.DefaultValue("")] // This prevents serialization of default values
        public string PromptWin
        {
            get { return _PromptWin; }
            set { _PromptWin = value; }
        }

        protected TWinPrnPrompt _PromptWinCtrl = null;
        [XmlIgnoreAttribute]
        public TWinPrnPrompt PromptWinCtrl
        {
            get { return _PromptWinCtrl; }
            set { _PromptWinCtrl = value; }
        }

        protected bool _HidePromptIfBlank = false;
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public bool HidePromptIfBlank
        {
            get { return _HidePromptIfBlank; }
            set { _HidePromptIfBlank = value; }
        }

        protected int _MaxLength = 0;
        [System.ComponentModel.DefaultValue(0)] // This prevents serialization of default values
        public int MaxLength
        {
            get { return _MaxLength; }
            set { _MaxLength = value; }
        }

        protected string _Mask = "";
        [System.ComponentModel.DefaultValue("")] // This prevents serialization of default values
        public string Mask
        {
            get { return _Mask; }
            set { _Mask = value; }
        }

        protected string _MaskForHH = "";
        /// <summary>
        /// MaskForHH was added because Alan likes to translate the mask for his host-side needs, but
        /// that screws up the .NET Handheld and PatrolCar/Emulator. His conversions cannot be reversed,
        /// so in order to play nice with his needs and also save us many hours of debugging, it is 
        /// necessary to add extra bloat and keep a virgin copy of the mask that (hopefully) won't 
        /// get altered by anybody.
        /// </summary>
        [System.ComponentModel.DefaultValue("")] // This prevents serialization of default values
        public string MaskForHH
        {
            get { return _MaskForHH; }
            set { _MaskForHH = value; }
        }
        #endregion

        public TWinBasePrnData()
            : base()
        {
            // Set defaults that Layout Tool uses
            this._Height = 30;
            this._Width = 5;
            this._Font = TFont.Font24x24;
        }
    }

    /// <summary>
    /// Summary description for TWinPrnMemo.
    /// </summary>

    public class TWinPrnMemo : Reino.ClientConfig.TWinBasePrnData
    {
        #region Properties and Members
        #endregion

        public TWinPrnMemo()
            : base()
        {
        }
    }

    /// <summary>
    /// Summary description for TWinPrnImage.
    /// </summary>

    public class TWinPrnImage : Reino.ClientConfig.TWinBasePrnData
    {
        public enum TImageSourceType
        {
            imgStaticImageFromFile = 0,
            imgSelectedDetailRecord
        }

        #region Properties and Members

        protected int _ZOrder = 0;
        [System.ComponentModel.DefaultValue(0)] // This prevents serialization of default values
        public int ZOrder
        {
            get { return _ZOrder; }
            set { _ZOrder = value; }
        }

        protected string _ImageSourceParameter = "";
        [System.ComponentModel.DefaultValue("")] // This prevents serialization of default values
        public string ImageSourceParameter
        {
            get { return _ImageSourceParameter; }
            set { _ImageSourceParameter = value; }
        }

        protected TImageSourceType _ImageSourceType = TImageSourceType.imgSelectedDetailRecord;
        public TImageSourceType ImageSourceType
        {
            get { return _ImageSourceType; }
            set { _ImageSourceType = value; }
        }

        // This field should NOT be serialized. It will be populated by another process with data to use
        // for drawing an image. It might be populated by an issuance application, AI.NET inquiry screen,
        // or AI.NET export library.
        [XmlIgnoreAttribute]
        public string ImageDataAsBase64 = "";

        #endregion

        public TWinPrnImage()
            : base()
        {
        }
    }

    /// <summary>
    /// Summary description for TWinPrnDraw.
    /// </summary>

    public class TWinPrnDraw : Reino.ClientConfig.TWinBasePrnData
    {
        #region Properties and Members
        #endregion

        public TWinPrnDraw()
            : base()
        {
        }
    }

    /// <summary>
    /// Summary description for TWinPrnPanel.
    /// </summary>

    public class TWinPrnPanel : Reino.ClientConfig.TPrnControl
    {
        #region Properties and Members
        protected List<TWinPrnBase> _Children;
        /// <summary>
        /// A collection of TWinPrnBase objects
        /// </summary>
        public List<TWinPrnBase> Children
        {
            get { return _Children; }
            set { _Children = value; }
        }
        #endregion

        public TWinPrnPanel()
            : base()
        {
            _Children = new List<TWinPrnBase>();

            // Set defaults that Layout Tool uses
            this._Height = 50;
            this._Width = 100;
        }
    }

    /// <summary>
    /// Summary description for TWinPrnForm.
    /// </summary>

    public class TWinPrnForm : Reino.ClientConfig.TWinPrnPanel
    {
        #region Properties and Members
        public const int MAX_PAGE_BREAKS = 10;
        private int[] fPageBreaks = new int[MAX_PAGE_BREAKS];
        private int fPageBreakCnt = 0;
        #endregion

        public TWinPrnForm()
            : base()
        {
        }

        public int PageBreakCnt()
        {
            return fPageBreakCnt;
        }

        public int GetPageBreak(int iNdx)
        {
            if (iNdx >= fPageBreakCnt)
                return -1;
            return fPageBreaks[iNdx];
        }

        public void AddPageBreak(int iPageBreakPos)
        {
            if (fPageBreakCnt >= MAX_PAGE_BREAKS) return;
            if (iPageBreakPos < 0) return;
            // find where it belongs
            int loInsertNdx;

            for (loInsertNdx = 0; loInsertNdx < fPageBreakCnt; loInsertNdx++)
            {
                if (iPageBreakPos <= fPageBreaks[loInsertNdx])
                    break;
            }

            if ((loInsertNdx < fPageBreakCnt) && (fPageBreaks[loInsertNdx] == iPageBreakPos))
                return; // duplicate.

            // make space
            int loShuffleNdx;
            for (loShuffleNdx = fPageBreakCnt - 1; loShuffleNdx >= loInsertNdx; loShuffleNdx--)
                fPageBreaks[loShuffleNdx + 1] = fPageBreaks[loShuffleNdx];

            fPageBreaks[loInsertNdx] = iPageBreakPos; // add it
            fPageBreakCnt++; // increment count
        }

    }

    /// <summary>
    /// Summary description for TWinPrnData.
    /// </summary>

    public class TWinPrnData : Reino.ClientConfig.TWinBasePrnData
    {
        #region Properties and Members
        #endregion

        public TWinPrnData()
            : base()
        {
        }
    }

    /// <summary>
    /// Summary description for TIssPrnForm.
    /// </summary>

    public class TIssPrnForm : Reino.ClientConfig.TObjBase
    {
        #region Properties and Members
        protected List<TIssPrnFormRev> _Revisions;
        /// <summary>
        /// A collection of TIssPrnFormRev objects
        /// </summary>
        public List<TIssPrnFormRev> Revisions
        {
            get { return _Revisions; }
            set { _Revisions = value; }
        }

        public TIssPrnFormRev HighFormRevision
        {
            get
            {
                // Will return null if no forms exist.
                TIssPrnFormRev highForm = null;
                // Find print form with highest Revision.
                foreach (TIssPrnFormRev oneForm in Revisions)
                {
                    // If haven't found a high form yet or if this form
                    // has a higher revision than our high form so far
                    // then make this form the new high form.
                    if ((highForm == null) || (oneForm.Revision > highForm.Revision))
                    {
                        highForm = oneForm;
                    }
                }

                // Return the high form we found.
                return highForm;
            }
        }
        #endregion

        public TIssPrnForm()
            : base()
        {
            // Create list of Print Picture Revisions
            this._Revisions = new List<TIssPrnFormRev>();
        }
    }

#if !WindowsCE || __ANDROID__   // defined for host AND android, not an often combo!
    /// <summary>
    /// Predicate class for locating a TPrnDataElement
    /// </summary>
    public class TPrnDataElementFindPredicate : object
    {
        private string fCompareName;

        public TPrnDataElementFindPredicate(string iCompareName)
        {
            fCompareName = iCompareName;
        }

        public bool CompareByElementName(TWinBasePrnData pObject)
        {
            return (((TWinBasePrnData)pObject).Name == fCompareName);
        }
    }
#endif



    /// <summary>
    /// Summary description for TIssPrnFormRev.
    /// </summary>
    public class TIssPrnFormRev : Reino.ClientConfig.TWinPrnForm
    {
        #region Properties and Members
        protected int _Revision = 0;
        //Always write this to XML so easy to read/know what revision it is.        [System.ComponentModel.DefaultValue(0)] // This prevents serialization of default values
        public int Revision
        {
            get { return _Revision; }
            set { _Revision = value; }
        }

        protected int _CurrentCFGRev = 0;
        [System.ComponentModel.DefaultValue(0)] // This prevents serialization of default values
        public int CurrentCFGRev
        {
            get { return _CurrentCFGRev; }
            set { _CurrentCFGRev = value; }
        }

        protected List<TWinBasePrnData> _AllPrnDataElements;
        [XmlIgnoreAttribute]
        public List<TWinBasePrnData> AllPrnDataElements
        {
            get { return _AllPrnDataElements; }
            set { _AllPrnDataElements = value; }
        }
        #endregion

        public TIssPrnFormRev()
            : base()
        {
            _AllPrnDataElements = new List<TWinBasePrnData>();
        }
    }

#if !WindowsCE && !__ANDROID__  
    // This class is based on Stucki algo in cxImage from CodeProject
    public class Dithering_Stucki
    {
        private byte[,] GrayMap;

        private byte GrayLevelFromColor(Color fromColor)
        {
            byte divisor = 3;
            return (byte)(((fromColor.R + fromColor.G + fromColor.B) / 3));
        }

        private void Set1BPPIndexedPixel(int bmpDataStride, int columnIdx, int rowIdx, bool pixel, byte[] destBytes)
        {
            int index = rowIdx * bmpDataStride + (columnIdx >> 3);
            byte p = destBytes[index];

            byte mask = (byte)(0x80 >> (columnIdx & 0x7));
            if (pixel)
                p = (byte)(p | mask);
            else
                p = (byte)(p & (mask ^ 0xFF));
            destBytes[index] = p;
        }

        public Android.Graphics.Bitmap Dither(Android.Graphics.Bitmap originalImage)
        {
            return null;
        }
    }

    // This class is based on Floyd-Steinberg algo found at: http://dotnet-snippets.de/dns/floyd-steinberg-dithering-SID94.aspx
    public class Dithering_FloydSteinberg
    {
    }
#endif
}
