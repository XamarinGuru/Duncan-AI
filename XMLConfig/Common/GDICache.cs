// VSS Keyword information
// File last modified by:   $Author: James $
//                    on:     $Date: 12/04/13 8:15a $
//               Archive:  $Archive: /AutoISSUE Solution.root/AutoISSUE Solution/Common/ClientConfig/GDICache.cs $
//              Revision: $Revision: 4 $

using System;
using System.Security;
using System.Collections;
using System.Collections.Generic;
//using System.Windows.Forms; - CSM
using System.Drawing;
using System.Text;
using System.Runtime.InteropServices;

#if WindowsCE
using Microsoft.WindowsCE.Forms;
#endif

namespace Reino.ClientConfig
{
    #region LogFontGdi
    /// <summary>
    /// LogFontGdi is a LOGFONT structure for use in both WindowsCE and Win32.
    /// It is compatible with CreateFontIndirect API call on both platforms.
    /// On Win32, it is also compatible with Font.FromLogFont(), however on
    /// WindowsCE, Font.FromLogFont() should be used with the LOGFONT class in 
    /// Microsoft.WindowsCE.Forms.LogFont instead.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    public struct LogFontGdi
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
    public enum LogFontCharSetGdi
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
    public enum LogFontPrecisionGdi
    {
        Default = 0,
        String = 1,
        Raster = 6,
    }

    // Specifies the quality of a font. (This definition came from the Compact Framework)
    public enum LogFontQualityGdi
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
    public enum LogFontPitchAndFamilyGdi
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
    public enum LogFontWeightGdi
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
    #endregion

    #region FontCache
    /// <summary>
    /// Caches both managed and unmanged fonts. To free all underlying unmanaged resources at application exit, 
    /// you can call FontCache.ReleaseGdiResources()
    /// </summary>
    public class FontCache : IDisposable
    {
        #region Protected Classes
        protected class FontVariation
        {
            private const int _maxSize = (int)(FontStyle.Bold | FontStyle.Regular | FontStyle.Italic | FontStyle.Strikeout | FontStyle.Underline);
            internal IntPtr[] _fontsGdi = new IntPtr[_maxSize];
            internal Font[] _fontsManaged = new Font[_maxSize];

            public IntPtr GetGdiFont(FontStyle style)
            {
                return _fontsGdi[(int)style];
            }

            public void SetGdiFont(FontStyle style, IntPtr value)
            {
                _fontsGdi[(int)style] = value;
            }

            public Font GetManagedFont(FontStyle style)
            {
                return _fontsManaged[(int)style];
            }

            public void SetManagedFont(FontStyle style, Font value)
            {
                _fontsManaged[(int)style] = value;
            }
        }

        protected class FontFamilyItem
        {
            internal Hashtable SizeList = new Hashtable();

            public FontVariation this[float val]
            {
                get
                {
                    if (!SizeList.ContainsKey(val))
                        SizeList.Add(val, new FontVariation());
                    return (FontVariation)SizeList[val];
                }
            }
        }
        #endregion

        #region Protected Members
        static protected FontCache _cache = new FontCache();
        static protected Hashtable _fontFamilies = new Hashtable();
        static protected bool _UseGdiFonts = false;
        #endregion

        #region Constructor / Destructor
        /// <summary>
        /// Don't allow an instance of this class to be created. 
        /// The only access is via the static methods.
        /// </summary>
        private FontCache()
        {
        }

        ~FontCache()
        {
            Release(); // Make sure all unmanaged resources are freed
        }
        #endregion

        #region Private Methods
        private FontFamilyItem this[string Family]
        {
            get
            {
                if (!_fontFamilies.ContainsKey(Family))
                {
                    _fontFamilies.Add(Family, new FontFamilyItem());
                }
                return (FontFamilyItem)_fontFamilies[Family];
            }
        }

        private void Release()
        {
            IPlatformDependentGDI WinGDI;
            if (Environment.OSVersion.Platform == PlatformID.WinCE)
                WinGDI = new WinCE_GDI();
            else
                WinGDI = new Win32_GDI();

            foreach (FontFamilyItem FamilyItem in _fontFamilies.Values)
            {
                foreach (FontVariation Variation in FamilyItem.SizeList.Values)
                {
                    foreach (Font loFont in Variation._fontsManaged)
                    {
                        if (loFont != null)
                        {
                            loFont.Dispose();
                        }
                    }
                    foreach (IntPtr loGdiFont in Variation._fontsGdi)
                    {
                        if (loGdiFont != IntPtr.Zero)
                            WinGDI.DeleteObject(loGdiFont);
                    }
                }
            }
        }
        #endregion

        #region Public Methods
        public static FontGdi CreateFontGdi(Font fromFont)
        {
            return CreateFontGdi(fromFont.Name, fromFont.Size, fromFont.Style);
        }

        public static FontGdi CreateFontGdi(string Family, float Size, FontStyle Style)
        {
            FontVariation loFontVariation = _cache[Family][Size];
            IntPtr hFont = loFontVariation.GetGdiFont(Style);
            if (hFont != IntPtr.Zero)
                return new FontGdi(hFont);

            IPlatformDependentGDI WinGDI;
            if (Environment.OSVersion.Platform == PlatformID.WinCE)
                WinGDI = new WinCE_GDI();
            else
                WinGDI = new Win32_GDI();

#if WindowsCE
            // The structure included in Microsoft.WindowsCE.Forms.LogFont doesn't work with CreateFontIndirect,
            // so we will use our LogFontGdi structure that has the correct definition.
            LogFontGdi loLogFont = new LogFontGdi();
            loLogFont.Escapement = 0;
            loLogFont.Orientation = loLogFont.Escapement;
            loLogFont.Italic = (Style & FontStyle.Italic) == FontStyle.Italic ? (byte)1 : (byte)0;
            loLogFont.Underline = (Style & FontStyle.Underline) == FontStyle.Underline ? (byte)1 : (byte)0;
            loLogFont.StrikeOut = (Style & FontStyle.Strikeout) == FontStyle.Strikeout ? (byte)1 : (byte)0;
            loLogFont.CharSet = (byte)LogFontCharSetGdi.ANSI;
            loLogFont.OutPrecision = (byte)LogFontPrecisionGdi.Default;
            loLogFont.Quality = (byte)LogFontQualityGdi.Default;
            loLogFont.PitchAndFamily = (byte)(LogFontPitchAndFamilyGdi.Variable | LogFontPitchAndFamilyGdi.DontCare);
            loLogFont.ClipPrecision = 0;
            loLogFont.FaceName = Family;
            
            //JLA debug: 8.0 seems too small under GDI on the full framework (8.25 turns into 8.0 on WinCE?)
            if ((Size == 8.0F) && ((Style & FontStyle.Bold) == FontStyle.Regular))
                Size = 9.0F;
            
            loLogFont.Height = (int)(Size / 8 * 13);
            // Arial bold doesn't work too well, so do slight adjustment when dealing with Arial
            if (Family.Equals("Arial"))
                loLogFont.Weight = (Style & FontStyle.Bold) == FontStyle.Bold ? (int)LogFontWeightGdi.SemiBold : (int)LogFontWeightGdi.Normal;
            else
                loLogFont.Weight = (Style & FontStyle.Bold) == FontStyle.Bold ? (int)LogFontWeightGdi.Bold : (int)LogFontWeightGdi.Normal;
            loLogFont.Width = 0;
#else
            // Create a logical font object.
            // .NET didn't supply a LogFont class for full framework, so we have to use LogFontGdi structure,
            // which we defined with the same member names as the LogFont class used by WinCE
            LogFontGdi loLogFont = new LogFontGdi();
            loLogFont.Escapement = 0;
            loLogFont.Orientation = loLogFont.Escapement;
            loLogFont.Italic = (Style & FontStyle.Italic) == FontStyle.Italic ? (byte)1 : (byte)0;
            loLogFont.Underline = (Style & FontStyle.Underline) == FontStyle.Underline ? (byte)1 : (byte)0;
            loLogFont.StrikeOut = (Style & FontStyle.Strikeout) == FontStyle.Strikeout ? (byte)1 : (byte)0;
            loLogFont.CharSet = (byte)LogFontCharSetGdi.ANSI;
            loLogFont.OutPrecision = (byte)LogFontPrecisionGdi.Default;
            loLogFont.Quality = (byte)LogFontQualityGdi.Default;
            loLogFont.PitchAndFamily = (byte)(LogFontPitchAndFamilyGdi.Variable | LogFontPitchAndFamilyGdi.DontCare);
            loLogFont.ClipPrecision = 0;
            loLogFont.FaceName = Family;

            //JLA debug: 8.25 seems too small under GDI on the full framework
            if ((Size == 8.25F) && ((Style & FontStyle.Bold) == FontStyle.Regular))
                Size = 9.0F;

            loLogFont.Height = (int)(Size / 8 * 13);
            // Arial bold doesn't work too well, so do slight adjustment when dealing with Arial
            if (Family.Equals("Arial"))
                loLogFont.Weight = (Style & FontStyle.Bold) == FontStyle.Bold ? (int)LogFontWeightGdi.SemiBold : (int)LogFontWeightGdi.Normal;
            else
                loLogFont.Weight = (Style & FontStyle.Bold) == FontStyle.Bold ? (int)LogFontWeightGdi.Bold : (int)LogFontWeightGdi.Normal;

            loLogFont.Width = 0;
#endif

            // Call native function to create the GDI font
            IntPtr pLF = WinGDI.LocalAlloc(0x40, 92);
            Marshal.StructureToPtr(loLogFont, pLF, false);
            hFont = WinGDI.CreateFontIndirect(pLF);
            WinGDI.LocalFree(pLF);

            // Add GDI font to the cache and return it
            loFontVariation.SetGdiFont(Style, hFont);
            return new FontGdi(hFont);
        }

        public static Font CreateFontManaged(Font fromFont)
        {
            return CreateFontManaged(fromFont.Name, fromFont.Size, fromFont.Style);
        }

        public static Font CreateFontManaged(string Family, float Size, FontStyle Style)
        {
            FontVariation loFontVariation = _cache[Family][Size];
            Font loFont = loFontVariation.GetManagedFont(Style);
            if (loFont != null)
                return loFont;

            // Lets make the font from managed routine instead of based on LogFont structure,
            // because the managed routine works better for this purpose.
            loFont = new System.Drawing.Font(Family, Size, Style);
            // Add managed font to the cache and return it
            loFontVariation.SetManagedFont(Style, loFont);
            return loFont;

            // This code section would create managed font based on LogFont. It doesn't work well for bold fonts?
            /*
            IPlatformDependentGDI WinGDI;
            if (Environment.OSVersion.Platform == PlatformID.WinCE)
                WinGDI = new WinCE_GDI();
            else
                WinGDI = new Win32_GDI();

#if WindowsCE
            // For Compact Framework on WinCE, we have to use the .NET supplied LogFont class
            Microsoft.WindowsCE.Forms.LogFont loLogFont = new LogFont();
            loLogFont.Escapement = 0;
            loLogFont.Orientation = loLogFont.Escapement;
            loLogFont.Italic = (Style & FontStyle.Italic) == FontStyle.Italic ? (byte)1 : (byte)0;
            loLogFont.Underline = (Style & FontStyle.Underline) == FontStyle.Underline ? (byte)1 : (byte)0;
            loLogFont.StrikeOut = (Style & FontStyle.Strikeout) == FontStyle.Strikeout ? (byte)1 : (byte)0;
            loLogFont.CharSet = LogFontCharSet.ANSI;
            loLogFont.OutPrecision = LogFontPrecision.Default;
            loLogFont.Quality = LogFontQuality.Default;
            loLogFont.PitchAndFamily = LogFontPitchAndFamily.Variable | LogFontPitchAndFamily.DontCare;
            loLogFont.ClipPrecision = 0;
            loLogFont.FaceName = Family;
            loLogFont.Height = (int)(Size / 8 * 13);
            loLogFont.Weight = (Style & FontStyle.Bold) == FontStyle.Bold ? LogFontWeight.Bold : LogFontWeight.Normal;
            loLogFont.Width = 0;
#else
            // Create a logical font object.
            // .NET didn't supply a LogFont class for full framework, so we have to use LogFontGdi structure,
            // which we defined with the same member names as the LogFont class used by WinCE
            LogFontGdi loLogFont = new LogFontGdi();
            loLogFont.Escapement = 0;
            loLogFont.Orientation = loLogFont.Escapement;
            loLogFont.Italic = (Style & FontStyle.Italic) == FontStyle.Italic ? (byte)1 : (byte)0;
            loLogFont.Underline = (Style & FontStyle.Underline) == FontStyle.Underline ? (byte)1 : (byte)0;
            loLogFont.StrikeOut = (Style & FontStyle.Strikeout) == FontStyle.Strikeout ? (byte)1 : (byte)0;
            loLogFont.CharSet = (byte)LogFontCharSetGdi.ANSI;
            loLogFont.OutPrecision = (byte)LogFontPrecisionGdi.Default;
            loLogFont.Quality = (byte)LogFontQualityGdi.Default;
            loLogFont.PitchAndFamily = (byte)(LogFontPitchAndFamilyGdi.Variable | LogFontPitchAndFamilyGdi.DontCare);
            loLogFont.ClipPrecision = 0;
            loLogFont.FaceName = Family;
            loLogFont.Height = (int)(Size / 8 * 13);
            loLogFont.Weight = (Style & FontStyle.Bold) == FontStyle.Bold ? (int)LogFontWeightGdi.Bold : (int)LogFontWeightGdi.Normal;
            loLogFont.Width = 0;
#endif

            // Create font based on the log font
            loFont = System.Drawing.Font.FromLogFont(loLogFont);

            // Add managed font to the cache and return it
            loFontVariation.SetManagedFont(Style, loFont);
            return loFont;
            */
        }

        public static bool UseGdiFonts
        {
            get { return _UseGdiFonts; }
            set { _UseGdiFonts = value; }
        }

        public void Dispose()
        {
            Release();	                            // Make sure all unmanaged resources are freed
            GC.SuppressFinalize(this);				// Don't call the destructor
        }

        /// <summary>
        /// Explicitly release all unmanaged resources used by the static instance of FontCache.
        /// It is not required to call this because the class has a destructor, however its good practice.
        /// </summary>
        public static void ReleaseGdiResources()
        {
            _cache.Dispose();
        }
        #endregion
    }
    #endregion

    #region BrushCache
    /// <summary>
    /// Caches managed solid brushes. To free all underlying unmanaged resources at application exit, 
    /// you can call BrushCache.ReleaseGdiResources()
    /// </summary>
    public class BrushCache : IDisposable
    {
        #region Protected Members
        static protected BrushCache _cache = new BrushCache();
        static protected Hashtable _brushesManaged = new Hashtable();
        #endregion

        #region Constructor / Destructor
        /// <summary>
        /// Don't allow an instance of this class to be created. 
        /// The only access is via the static methods.
        /// </summary>
        private BrushCache()
        {
        }

        ~BrushCache()
        {
            Release();	 // Make sure all unmanaged resources are freed
        }
        #endregion

        #region PrivateMethods
        private void Release()
        {
            foreach (SolidBrush loBrush in _brushesManaged.Values)
            {
                if (loBrush != null)
                    loBrush.Dispose();
            }
        }
        #endregion

        #region Public Methods
        public static SolidBrush CreateBrushManaged(Color BrushColor)
        {
            // If a brush for this color is already cached, just return it
            if (_brushesManaged.ContainsKey(BrushColor))
                return (_brushesManaged[BrushColor] as SolidBrush);

            // Lets make a new solid brush for the passed color
            SolidBrush loBrush = new SolidBrush(BrushColor);
            // Add managed brush to the cache and return it
            _brushesManaged.Add(BrushColor, loBrush);
            return loBrush;
        }

        public void Dispose()
        {
            Release();	                            // Make sure all unmanaged resources are freed
            GC.SuppressFinalize(this);				// Don't call the destructor
        }

        /// <summary>
        /// Explicitly release all unmanaged resources used by the static instance of BrushCache.
        /// It is not required to call this because the class has a destructor, however its good practice.
        /// </summary>
        public static void ReleaseGdiResources()
        {
            _cache.Dispose();
        }
        #endregion
    }
    #endregion

    #region PenCache
    /// <summary>
    /// Caches managed pens (supports both Solid and Dotted). To free all underlying unmanaged resources 
    /// at application exit, you can call BrushCache.ReleaseGdiResources()
    /// </summary>
    public class PenCache : IDisposable
    {
        #region Protected Members
        static protected PenCache _cache = new PenCache();
        static protected Hashtable _SolidPens = new Hashtable();
        static protected Hashtable _DottedPens = new Hashtable();
        #endregion

        #region Constructor / Destructor
        /// <summary>
        /// Don't allow an instance of this class to be created. 
        /// The only access is via the static methods.
        /// </summary>
        private PenCache()
        {
        }

        ~PenCache()
        {
            Release();	 // Make sure all unmanaged resources are freed
        }
        #endregion

        #region PrivateMethods
        private void Release()
        {
            foreach (Pen loSolidPen in _SolidPens.Values)
            {
                if (loSolidPen != null)
                    loSolidPen.Dispose();
            }
            foreach (Pen loDottedPen in _DottedPens.Values)
            {
                if (loDottedPen != null)
                    loDottedPen.Dispose();
            }
        }
        #endregion

        #region Public Methods
        public static Pen CreateSolidPen(Color PenColor)
        {
            // If a pen for this color is already cached, just return it
            if (_SolidPens.ContainsKey(PenColor))
                return (_SolidPens[PenColor] as Pen);

            // Lets make a new solid pen for the passed color
            Pen loPen = new Pen(PenColor);
            // Add managed pen to the cache and return it
            _SolidPens.Add(PenColor, loPen);
            return loPen;
        }

        public static Pen CreateDottedPen(Color PenColor)
        {
            // If a pen for this color is already cached, just return it
            if (_DottedPens.ContainsKey(PenColor))
                return (_DottedPens[PenColor] as Pen);

            // Lets make a new dotted pen for the passed color
            Pen loPen = new Pen(PenColor);
            loPen.Width = 1;
            loPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash; // Dot would be nicer -- but seems to be missing from .NET CF!
            // Add managed pen to the cache and return it
            _DottedPens.Add(PenColor, loPen);
            return loPen;
        }

        public void Dispose()
        {
            Release();	                            // Make sure all unmanaged resources are freed
            GC.SuppressFinalize(this);				// Don't call the destructor
        }

        /// <summary>
        /// Explicitly release all unmanaged resources used by the static instance of PenCache.
        /// It is not required to call this because the class has a destructor, however its good practice.
        /// </summary>
        public static void ReleaseGdiResources()
        {
            _cache.Dispose();
        }
        #endregion
    }
    #endregion
}
