// VSS Keyword information
// File last modified by:   $Author: James $
//                    on:     $Date: 4/19/07 4:38p $
//               Archive:  $Archive: /AutoISSUE Solution.root/AutoISSUE Solution/Common/ClientConfig/GDICommon.cs $
//              Revision: $Revision: 3 $

using System;
using System.Security;
using System.Collections;
using System.Collections.Generic;
//using System.Windows.Forms; - CSM
using System.Drawing;
using System.Text;
using System.Runtime.InteropServices;

namespace Reino.ClientConfig
{
    #region FontGdi
    /// <summary>
    /// This class provides a type-safe wrapper for GDI Font handles.
    /// </summary>
    public struct FontGdi
    {
        private IntPtr _hfont;

        public FontGdi(IntPtr hfont)
        {
            _hfont = hfont;
        }

        public static FontGdi Empty
        {
            get { return (FontGdi)IntPtr.Zero; }
        }

        public bool IsEmpty
        {
            get { return (_hfont == IntPtr.Zero); }
        }

        public override bool Equals(object obj)
        {
            return ((IntPtr)this == (IntPtr)obj);
        }

        public override int GetHashCode()
        {
            return _hfont.GetHashCode();
        }

        public static bool operator !=(FontGdi a, FontGdi b)
        {
            return (a._hfont != b._hfont);
        }

        public static bool operator ==(FontGdi a, FontGdi b)
        {
            return (a._hfont == b._hfont);
        }

        public static explicit operator IntPtr(FontGdi font)
        {
            return font._hfont;
        }

        public static implicit operator FontGdi(IntPtr hfont)
        {
            return new FontGdi(hfont);
        }
    }
    #endregion

    #region GraphicsGdi
    /// <summary>
    /// Managed text drawing via Graphics object (GDI+) is slow. We can draw
    /// text about 5 times faster via GDI using API calls. This is a helper
    /// class to ease the process of mixed mode painting using GDI & GDI+ together.
    /// </summary>
    public class GraphicsGdi : IDisposable
    {
        #region Private Members
        private IntPtr m_hdc;
        private IntPtr m_hwnd;
        private bool m_fontChanged = false;
        private IntPtr m_oldFont;
        private Color m_textColor = Color.Black;
        private FontGdi m_font;
        private bool m_ownDc = true;			// True means we'll release DC in desctructor

        private ReinoControls.IPlatformDependent WinAPI;
        private IPlatformDependentGDI WinGDI;
        #endregion

        #region Public Properties
        public bool Transparent
        {
            set
            {
                const int bkModeTransparent = 1;
                const int bkModeOpaque = 2;
                int mode = (value) ? bkModeTransparent : bkModeOpaque;
                WinGDI.SetBkMode(m_hdc, mode);
            }
        }

        public IntPtr ClipRegion
        {
            set { WinGDI.SelectClipRgn(m_hdc, value); }
        }

        public FontGdi Font
        {
            set
            {
                if (value.IsEmpty && m_fontChanged)
                {
                    WinGDI.SelectObject(m_hdc, (IntPtr)m_oldFont);
                    m_oldFont = IntPtr.Zero;
                    m_font = FontGdi.Empty;
                }

                IntPtr oldFont = WinGDI.SelectObject(m_hdc, (IntPtr)value);

                if (!m_fontChanged)
                {
                    m_oldFont = oldFont;
                    m_fontChanged = true;
                    m_font = value;
                }
            }
            get { return m_font; }
        }

        public Color TextColor
        {
            get { return m_textColor; }
            set
            {
                if (value == m_textColor)
                    return;

                int color = (value.B << 16) + (value.G << 8) + value.R;
                WinGDI.SetTextColor(m_hdc, color);
                m_textColor = value;
            }
        }
        #endregion

        #region Constructors / Destructor
        private GraphicsGdi()
        {
            // Don't allow public creation without parameters
        }

        public GraphicsGdi(IntPtr hwnd)
        {
            Init(hwnd, Rectangle.Empty);
        }

        public GraphicsGdi(IntPtr hwnd, Rectangle clipRect)
        {
            Init(hwnd, clipRect);
        }

        //public GraphicsGdi(Control control)
        //{
        //    control.Capture = true;
        //    IntPtr hwnd = WinAPI.GetCapture();
        //    control.Capture = false;
        //    Init(hwnd, Rectangle.Empty);
        //}

        public static GraphicsGdi FromHdc(IntPtr hdc)
        {
            GraphicsGdi GfxGdi = new GraphicsGdi();
            GfxGdi.InitWithHdc(hdc, Rectangle.Empty, false);
            return GfxGdi;
        }

        ~GraphicsGdi()
        {
            Release();	// Make sure we release the DC
        }
        #endregion

        #region Private Methods
        private void Init(IntPtr hwnd, Rectangle clipRect)
        {
            m_hwnd = hwnd;
            InitWithHdc(WinGDI.GetDC(hwnd), clipRect, true);
        }

        private void InitWithHdc(IntPtr hdc, Rectangle clipRect, bool ownDc)
        {
            //CSM
            //// Get object for various API calls
            //if (Environment.OSVersion.Platform == PlatformID.WinCE)
            //    WinAPI = new ReinoControls.WinCEAPI();
            //else
            //    WinAPI = new ReinoControls.Win32API();

            // Get object for various GDI-related API calls
            if (Environment.OSVersion.Platform == PlatformID.WinCE)
                WinGDI = new WinCE_GDI();
            else
                WinGDI = new Win32_GDI();

            m_hdc = hdc;
            this.Transparent = true;				// Use transparent drawing by default
            m_ownDc = ownDc;

            if (!clipRect.IsEmpty)					// If there is a clip rectangle, use it.
            {
                int l, t, r, b;
                l = Convert.ToInt16(clipRect.Left);
                t = Convert.ToInt16(clipRect.Top);
                r = Convert.ToInt16(clipRect.Right);
                b = Convert.ToInt16(clipRect.Bottom);
                IntPtr hrgn = WinGDI.CreateRectRgn(l, t, r, b);
                int i = WinGDI.SelectClipRgn(m_hdc, hrgn);
                WinGDI.DeleteObject(hrgn);
            }
        }

        private void Release()
        {
            if (m_hdc != IntPtr.Zero)
            {
                // If we ever changed the font, then reselect the orignal font back into the device context
                if (m_fontChanged)
                    WinGDI.SelectObject(m_hdc, m_oldFont);

                // If we own the device context, then its time to release it
                if (m_ownDc)
                {
                    WinGDI.ReleaseDC(m_hwnd, m_hdc);
                    m_hdc = IntPtr.Zero;
                }
            }
        }
        #endregion

        #region Public Methods
        public void Dispose()
        {
            Release();								// Release the DC
            GC.SuppressFinalize(this);				// Don't call the destructor
        }

        public static int ToGdiColor(Color color)
        {
            return (color.B << 16) + (color.G << 8) + color.R;
        }

        public bool DeleteObject(IntPtr obj)
        {
            return WinGDI.DeleteObject(obj);
        }

        public int DrawText(string text, Rectangle rect, uint format)
        {
            Rectangle loRect = new Rectangle(rect.X, rect.Y, rect.Width + rect.X, rect.Height + rect.Y);
            if (text != null)
                return WinGDI.DrawText(m_hdc, text, text.Length, ref loRect, format);
            else
                return 0;
        }

        public int DrawText(string text, FontGdi font, Color textColor, Rectangle rect)
        {
            // Select desired font into device context
            IntPtr oldFont = WinGDI.SelectObject(m_hdc, (IntPtr)font);
            // Translate color to GDI color and set as text color for device context
            int color = (textColor.B << 16) + (textColor.G << 8) + textColor.R;
            WinGDI.SetTextColor(m_hdc, color);
            // Adjust passed rectangle because API assumes right and bottom in bytes occupied by width and height
            Rectangle loRect = new Rectangle(rect.X, rect.Y, rect.Width + rect.X, rect.Height + rect.Y);
            int Result = 0;
            // Draw text to device context
            if (text != null)
                Result = WinGDI.DrawText(m_hdc, text, text.Length, ref loRect, 0);
            // Restore previous font in device context
            WinGDI.SelectObject(m_hdc, (IntPtr)oldFont);
            return Result;
        }

        public int ExtTextOut(int x, int y, string text)
        {
            Rectangle loRect = Rectangle.Empty;
            return WinGDI.ExtTextOut(m_hdc, x, y, 0, ref loRect, text,
                Convert.ToUInt16(text.Length), null);
        }

        public Size GetTextExtent(string text)
        {
            if (text == null || text.Length == 0)
                return Size.Empty;

            Size size = Size.Empty;
            int fit;
            WinGDI.GetTextExtentExPoint(m_hdc, text, text.Length, 10000, out fit, null, ref size);
            return size;
        }

        public IntPtr SelectObject(IntPtr obj)
        {
            return WinGDI.SelectObject(m_hdc, obj);
        }
        #endregion
    }
    #endregion

    #region CommonDraw
    /// <summary>
    /// CommonDraw simplifies some of the most common drawing routines. It uses cached brushes, pens and fonts
    /// and will draw text using the speedier unmanaged GDI if desired
    /// </summary>
    public class CommonDraw
    {
        #region Constructor
        private CommonDraw()
        {
            // Don't allow public creation of this object type
        }
        #endregion

        #region Static Public Methods
        static public void FillRectangle(Graphics gfx, Brush brush, Rectangle rect)
        {
            gfx.FillRectangle(brush, rect);
        }

        static public void FillRectangle(Graphics gfx, Color color, Rectangle rect)
        {
            Brush brush = BrushCache.CreateBrushManaged(color);
            gfx.FillRectangle(brush, rect);
        }

        static public void DrawRectangle(Graphics gfx, Pen pen, Rectangle rect)
        {
            gfx.DrawRectangle(pen, rect);
        }

        static public void DrawRectangle(Graphics gfx, Color color, Rectangle rect)
        {
            Pen pen = PenCache.CreateSolidPen(color);
            gfx.DrawRectangle(pen, rect);
        }

        static public void DrawLine(Graphics gfx, Pen pen, int x1, int y1, int x2, int y2)
        {
            gfx.DrawLine(pen, x1, y1, x2, y2);
        }

        static public void DrawLine(Graphics gfx, Color color, int x1, int y1, int x2, int y2)
        {
            Pen pen = PenCache.CreateSolidPen(color);
            gfx.DrawLine(pen, x1, y1, x2, y2);
        }

        static public void DrawText(Graphics gfx, Bitmap DoubleBuffer, string text, Font font, Color textColor, Rectangle rect)
        {
            if (FontCache.UseGdiFonts == true)
            {
                IntPtr _OffScreenHdc;
                GraphicsGdi _OffScreenGfxGdi;
                FontGdi loFontGdi;
#if !WindowsCE
                // Was an image for double-buffering passed to us?
                if (DoubleBuffer != null)
                {
                    // For some strange reason, GDI draws text to the offscreen bitmap in a very ugly manner.
                    // To fix, we need to go through a few hoops to use a compatible memory device context instead
                    // of using the device context the graphics object gives us....

                    // Get object for various GDI-related API calls
                    IPlatformDependentGDI WinGDI;
                    if (Environment.OSVersion.Platform == PlatformID.WinCE)
                        WinGDI = new WinCE_GDI();
                    else
                        WinGDI = new Win32_GDI();

                    _OffScreenHdc = gfx.GetHdc(); // Get handle to device context of offscreen graphics
                    IntPtr memDC = WinGDI.CreateCompatibleDC(_OffScreenHdc); // Create memory device context compatible with offscreen graphics
                    IntPtr hGdiBmp = DoubleBuffer.GetHbitmap(); // Create a GDI compatible version of the bitmap
                    WinGDI.SelectObject(memDC, hGdiBmp); // Select the GDI bitmap in the memory device context

                    // Draw text to memory device context
                    _OffScreenGfxGdi = GraphicsGdi.FromHdc(memDC);
                    loFontGdi = FontCache.CreateFontGdi(font);
                    _OffScreenGfxGdi.DrawText(text, loFontGdi, textColor, rect);

                    // Copy color information from desired source area to same area in destination
                    WinGDI.BitBlt(_OffScreenHdc, rect.X, rect.Y, rect.Width, rect.Height, memDC,
                        rect.X, rect.Y, TernaryRasterOperations.SRCCOPY);

                    WinGDI.DeleteObject(hGdiBmp); // Delete the GDI bitmap
                    WinGDI.DeleteDC(memDC); // Delete the memory device context
                    gfx.ReleaseHdc(_OffScreenHdc); // Release device context of offscreen graphics

                    // Finished with GDI drawing for double buffered technique
                    return;
                }
#endif
                // Text draws much faster with GDI rather than managed GDI+, so we need to switch to
                // pure GDI mode by getting a handle to the device context. Note that the source Graphics
                // object cannot be used while we have the device context handle!
                _OffScreenHdc = gfx.GetHdc();
                _OffScreenGfxGdi = GraphicsGdi.FromHdc(_OffScreenHdc);
                loFontGdi = FontCache.CreateFontGdi(font);
                _OffScreenGfxGdi.DrawText(text, loFontGdi, textColor, rect);

                // Calls to the GetHdc and ReleaseHdc methods must appear in pairs. 
                // During the scope of a GetHdc and ReleaseHdc method pair, you usually 
                // make only calls to GDI functions. Calls in that scope made to GDI+ 
                // methods of the Graphics that produced the hdc parameter fail with an 
                // ObjectBusy error. Also, GDI+ ignores any state changes made to the 
                // Graphics of the hdc parameter in subsequent operations.
                gfx.ReleaseHdc(_OffScreenHdc);
                _OffScreenGfxGdi.Dispose();
            }
            else
            {
                // Draw text with managed routines via GDI+ (Not as fast as pure GDI via Windows API)
                StringFormat strFormat = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.NoClip);
                Brush textBrush = BrushCache.CreateBrushManaged(textColor);
                gfx.DrawString(text, font, textBrush, rect, strFormat);
                strFormat.Dispose();
            }
        }

        static public void DrawTextDisabled(Graphics gfx, Bitmap DoubleBuffer, string text, Font font, Rectangle rect)
        {
            if (FontCache.UseGdiFonts == true)
            {
                IntPtr _OffScreenHdc;
                GraphicsGdi _OffScreenGfxGdi;
                FontGdi loFontGdi;
#if !WindowsCE
                // Was an image for double-buffering passed to us?
                if (DoubleBuffer != null)
                {
                    // For some strange reason, GDI draws text to the offscreen bitmap in a very ugly manner.
                    // To fix, we need to go through a few hoops to use a compatible memory device context instead
                    // of using the device context the graphics object gives us....

                    // Get object for various GDI-related API calls
                    IPlatformDependentGDI WinGDI;
                    if (Environment.OSVersion.Platform == PlatformID.WinCE)
                        WinGDI = new WinCE_GDI();
                    else
                        WinGDI = new Win32_GDI();

                    _OffScreenHdc = gfx.GetHdc(); // Get handle to device context of offscreen graphics
                    IntPtr memDC = WinGDI.CreateCompatibleDC(_OffScreenHdc); // Create memory device context compatible with offscreen graphics
                    IntPtr hGdiBmp = DoubleBuffer.GetHbitmap(); // Create a GDI compatible version of the bitmap
                    WinGDI.SelectObject(memDC, hGdiBmp); // Select the GDI bitmap in the memory device context

                    /*
                    // Copy desired area of current double buffer image into memory device context
                    WinGDI.BitBlt(memDC, rect.X - 1, rect.Y, rect.Width + 1, rect.Height + 1, _OffScreenHdc,
                        rect.X - 1, rect.Y, TernaryRasterOperations.SRCCOPY);
                    */

                    // Draw text to memory device context
                    _OffScreenGfxGdi = GraphicsGdi.FromHdc(memDC);
                    loFontGdi = FontCache.CreateFontGdi(font);
                    _OffScreenGfxGdi.DrawText(text, loFontGdi, SystemColors.ControlLightLight, rect);
                    // Draw dark text offset by 1-pixel
                    Rectangle rectOffset = new Rectangle(rect.Left - 1, rect.Top + 1, rect.Width, rect.Height);
                    _OffScreenGfxGdi.DrawText(text, loFontGdi, SystemColors.ControlDarkDark, rectOffset);

                    // Copy color information from desired source area to same area in destination
                    WinGDI.BitBlt(_OffScreenHdc, rect.X - 1, rect.Y, rect.Width + 1, rect.Height + 1, memDC,
                        rect.X - 1, rect.Y, TernaryRasterOperations.SRCCOPY);

                    WinGDI.DeleteObject(hGdiBmp); // Delete the GDI bitmap
                    WinGDI.DeleteDC(memDC); // Delete the memory device context
                    gfx.ReleaseHdc(_OffScreenHdc); // Release device context of offscreen graphics

                    // Finished with GDI drawing for double buffered technique
                    return;
                }
#endif
                // Text draws much faster with GDI rather than managed GDI+, so we need to switch to
                // pure GDI mode by getting a handle to the device context. Note that the source Graphics
                // object cannot be used while we have the device context handle!
                _OffScreenHdc = gfx.GetHdc();
                _OffScreenGfxGdi = GraphicsGdi.FromHdc(_OffScreenHdc);
                loFontGdi = FontCache.CreateFontGdi(font);
                // Draw light text first
                _OffScreenGfxGdi.DrawText(text, loFontGdi, SystemColors.ControlLightLight, rect);
                // Draw dark text offset by 1-pixel
                rect = new Rectangle(rect.Left - 1, rect.Top + 1, rect.Width, rect.Height);
                _OffScreenGfxGdi.DrawText(text, loFontGdi, SystemColors.ControlDarkDark, rect);

                // Calls to the GetHdc and ReleaseHdc methods must appear in pairs. 
                // During the scope of a GetHdc and ReleaseHdc method pair, you usually 
                // make only calls to GDI functions. Calls in that scope made to GDI+ 
                // methods of the Graphics that produced the hdc parameter fail with an 
                // ObjectBusy error. Also, GDI+ ignores any state changes made to the 
                // Graphics of the hdc parameter in subsequent operations.
                gfx.ReleaseHdc(_OffScreenHdc);
                _OffScreenGfxGdi.Dispose();
            }
            else
            {
                // Draw text with managed routines via GDI+ (Not as fast as pure GDI via Windows API)
                StringFormat strFormat = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.NoClip);
                Brush textBrush;
                // Draw light text first
                textBrush = BrushCache.CreateBrushManaged(SystemColors.ControlLightLight);
                gfx.DrawString(text, font, textBrush, rect, strFormat);
                // Draw dark text offset by 1-pixel
                rect = new Rectangle(rect.Left - 1, rect.Top + 1, rect.Width, rect.Height);
                textBrush = BrushCache.CreateBrushManaged(SystemColors.ControlDarkDark);
                gfx.DrawString(text, font, textBrush, rect, strFormat);
                // StringFormat is IDisposable, so lets dispose of it
                strFormat.Dispose();
            }
        }

        static public void DrawFocusFrame(Graphics gfx, Rectangle rect, Boolean HiContrast)
        {
            Pen pen;
            // WindowsCE doesn't have a built-in DrawFocusRectangle function,
            // so we'll have to settle for a dotted line
            // First, draw solid light-colored rectangle if HiContrast is desired
            if (HiContrast == true)
            {
                pen = PenCache.CreateSolidPen(Color.FromArgb(227, 237, 255/*255, 255, 192*//*227, 237, 255*/)); //Color.White
                gfx.DrawRectangle(pen, rect);
            }

            // Then draw dotted black rectangle on top
            pen = PenCache.CreateDottedPen(Color.Black);
            gfx.DrawRectangle(pen, rect);
     
        }

        static public SizeF ConvertSizeToSizeF(Size size)
        {
            return new SizeF((float)size.Width, (float)size.Height);
        }

        static public SizeF MeasureString(Graphics gfx, string text, Font font)
        {
            SizeF strExtent;
            if (FontCache.UseGdiFonts == true)
            {
                // Text draws much faster with GDI rather than managed GDI+, so we need to switch to
                // pure GDI mode by getting a handle to the device context. Note that the source Graphics
                // object cannot be used while we have the device context handle!
                IntPtr _OffScreenHdc = gfx.GetHdc();
                GraphicsGdi _OffScreenGfxGdi = GraphicsGdi.FromHdc(_OffScreenHdc);
                _OffScreenGfxGdi.Font = FontCache.CreateFontGdi(font);
                // Get text extent, then convert from Size to SizeF
                Size sfGdi = _OffScreenGfxGdi.GetTextExtent(text);
                strExtent = ConvertSizeToSizeF(sfGdi); //(SizeF)sfGdi; //Implicit operator is only on Full Framework

                // After the text measurement has been made, we need to restore the previous GDI font
                // by assigning an empty font to the device context
                _OffScreenGfxGdi.Font = FontGdi.Empty;

                // Calls to the GetHdc and ReleaseHdc methods must appear in pairs. 
                // During the scope of a GetHdc and ReleaseHdc method pair, you usually 
                // make only calls to GDI functions. Calls in that scope made to GDI+ 
                // methods of the Graphics that produced the hdc parameter fail with an 
                // ObjectBusy error. Also, GDI+ ignores any state changes made to the 
                // Graphics of the hdc parameter in subsequent operations.
                gfx.ReleaseHdc(_OffScreenHdc);
                _OffScreenGfxGdi.Dispose();
            }
            else
            {
                // Get text extent using managed GDI+ call
                strExtent = gfx.MeasureString(text, font);
            }

            // Now return the final SizeF structure
            return strExtent;
        }
        #endregion
    }
    #endregion

    #region CommonDrawTextBatch
    /// <summary>
    /// CommonDrawTextBatch is to be used when drawing several text elements sequentially. It should NOT be
    /// used if and GDI+ drawing needs to be performed between text elements because the Device Context
    /// will be locked until this object is disposed.
    /// </summary>
    public class CommonDrawTextBatch : IDisposable
    {
        #region Private Members
        IntPtr _OffScreenHdc = IntPtr.Zero;
        GraphicsGdi _OffScreenGfxGdi = null;
        Graphics _OffScreenGfx = null;
        Bitmap _DoubleBuffer = null;
        IntPtr memDC = IntPtr.Zero;
        IntPtr hGdiBmp = IntPtr.Zero;
        #endregion

        #region Constructors / Destructor
        private CommonDrawTextBatch()
        {
            // Don't allow public creation of this object type
        }

        public CommonDrawTextBatch(Graphics gfx, Bitmap DoubleBuffer)
        {
            // Keep reference to the passed graphics objects
            _OffScreenGfx = gfx;
            _DoubleBuffer = DoubleBuffer;

            // If unmanaged GDI text is desired, we need to get device context
            if (FontCache.UseGdiFonts == true)
            {
#if !WindowsCE
                // Was an image for double-buffering passed to us?
                if (_DoubleBuffer != null)
                {
                    // For some strange reason, GDI draws text to the offscreen bitmap in a very ugly manner.
                    // To fix, we need to go through a few hoops to use a compatible memory device context instead
                    // of using the device context the graphics object gives us....

                    // Get object for various GDI-related API calls
                    IPlatformDependentGDI WinGDI;
                    if (Environment.OSVersion.Platform == PlatformID.WinCE)
                        WinGDI = new WinCE_GDI();
                    else
                        WinGDI = new Win32_GDI();

                    _OffScreenHdc = _OffScreenGfx.GetHdc(); // Get handle to device context of offscreen graphics
                    memDC = WinGDI.CreateCompatibleDC(_OffScreenHdc); // Create memory device context compatible with offscreen graphics
                    hGdiBmp = _DoubleBuffer.GetHbitmap(); // Create a GDI compatible version of the bitmap
                    WinGDI.SelectObject(memDC, hGdiBmp); // Select the GDI bitmap in the memory device context
                    _OffScreenGfxGdi = GraphicsGdi.FromHdc(memDC);

                    /*
                    // Copy desired area of current double buffer image into memory device context
                    WinGDI.BitBlt(memDC, 0, 0, _DoubleBuffer.Width, _DoubleBuffer.Height, _OffScreenHdc,
                        0, 0, TernaryRasterOperations.SRCCOPY);
                    */
                    return;
                }
#endif
                // Text draws much faster with GDI rather than managed GDI+, so we need to switch to
                // pure GDI mode by getting a handle to the device context. Note that the source Graphics
                // object cannot be used while we have the device context handle!
                _OffScreenHdc = _OffScreenGfx.GetHdc();
                _OffScreenGfxGdi = GraphicsGdi.FromHdc(_OffScreenHdc);
            }
        }

        ~CommonDrawTextBatch()
        {
            Release();
        }
        #endregion

        #region Private Methods
        private void Release()
        {
#if !WindowsCE
            // Was an image for double-buffering passed to us?
            if (_DoubleBuffer != null)
            {
                // Get object for various GDI-related API calls
                IPlatformDependentGDI WinGDI;
                if (Environment.OSVersion.Platform == PlatformID.WinCE)
                    WinGDI = new WinCE_GDI();
                else
                    WinGDI = new Win32_GDI();

                // Copy GDI bitmap color information into the double buffer image
                WinGDI.BitBlt(_OffScreenHdc, 0, 0, _DoubleBuffer.Width, _DoubleBuffer.Height, memDC,
                    0, 0, TernaryRasterOperations.SRCCOPY);

                WinGDI.DeleteObject(hGdiBmp); // Delete the GDI bitmap
                WinGDI.DeleteDC(memDC); // Delete the memory device context
                if (_OffScreenHdc != IntPtr.Zero)
                    _OffScreenGfx.ReleaseHdc(_OffScreenHdc); // Release device context of offscreen graphics
                if (_OffScreenGfxGdi != null)
                    _OffScreenGfxGdi.Dispose();
                return;
            }
#endif
            // Calls to the GetHdc and ReleaseHdc methods must appear in pairs. 
            // During the scope of a GetHdc and ReleaseHdc method pair, you usually 
            // make only calls to GDI functions. Calls in that scope made to GDI+ 
            // methods of the Graphics that produced the hdc parameter fail with an 
            // ObjectBusy error. Also, GDI+ ignores any state changes made to the 
            // Graphics of the hdc parameter in subsequent operations.
            if (_OffScreenHdc != IntPtr.Zero)
                _OffScreenGfx.ReleaseHdc(_OffScreenHdc);
            if (_OffScreenGfxGdi != null)
                _OffScreenGfxGdi.Dispose();
        }
        #endregion

        #region Public Methods
        public void Dispose()
        {
            Release();								// Release resources
            GC.SuppressFinalize(this);				// Don't call the destructor
        }

        public void DrawText(string text, Font font, Color textColor, Rectangle rect)
        {
            if (FontCache.UseGdiFonts == true)
            {
                FontGdi loFontGdi = FontCache.CreateFontGdi(font);
                _OffScreenGfxGdi.DrawText(text, loFontGdi, textColor, rect);
            }
            else
            {
                // Draw text with managed routines via GDI+ (Not as fast as pure GDI via Windows API)
                StringFormat sf = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.NoClip);
                Brush textBrush = BrushCache.CreateBrushManaged(textColor);
                _OffScreenGfx.DrawString(text, font, textBrush, rect, sf);
                // StringFormat is IDisposable, so lets dispose of it
                sf.Dispose();
            }
        }
        #endregion
    }
    #endregion

    #region TernaryRasterOperations
    public enum TernaryRasterOperations
    {
        SRCCOPY = 0x00CC0020,     // dest = source 
        SRCPAINT = 0x00EE0086,    // dest = source OR dest           
        SRCAND = 0x008800C6,      // dest = source AND dest          
        SRCINVERT = 0x00660046,   // dest = source XOR dest       
        SRCERASE = 0x00440328,    // dest = source AND (NOT dest ) 
        NOTSRCCOPY = 0x00330008,  // dest = (NOT source)         
        NOTSRCERASE = 0x001100A6, // dest = (NOT src) AND (NOT dest)
        MERGECOPY = 0x00C000CA,   // dest = (source AND pattern)     
        MERGEPAINT = 0x00BB0226,  // dest = (NOT source) OR dest    
        PATCOPY = 0x00F00021,     // dest = pattern                  
        PATPAINT = 0x00FB0A09,    // dest = DPSnoo                   
        PATINVERT = 0x005A0049,   // dest = pattern XOR dest         
        DSTINVERT = 0x00550009,   // dest = (NOT dest)               
        BLACKNESS = 0x00000042,   // dest = BLACK                    
        WHITENESS = 0x00FF0062,   // dest = WHITE                    
    };
    #endregion
}
