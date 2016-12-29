// VSS Keyword information
// File last modified by:   $Author: James $
//                    on:     $Date: 4/19/07 4:38p $
//               Archive:  $Archive: /AutoISSUE Solution.root/AutoISSUE Solution/Common/ClientConfig/Win32GDI.cs $
//              Revision: $Revision: 2 $

using System;
using System.Security;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Runtime.InteropServices;

namespace Reino.ClientConfig
{
	// GDI functions for use on FULL .NET Framework
	sealed class Win32_GDI : IPlatformDependentGDI
	{
		// We won't use this API calls maliciously, so let's avoid incurring a performance loss 
		// of a run-time security check by using the "SuppressUnmanagedCodeSecurity" attribute.
		// (This attribute isn't available on Compact Framework)

		// GetDC
		[System.Security.SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr GetDC(IntPtr hWnd);
		IntPtr IPlatformDependentGDI.GetDC(IntPtr hWnd)
		{
			return GetDC(hWnd);
		}

		// ReleaseDC
		[System.Security.SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", ExactSpelling = true)]
		private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
		int IPlatformDependentGDI.ReleaseDC(IntPtr hWnd, IntPtr hDC)
		{
			return ReleaseDC(hWnd, hDC);
		}

		// CreateCompatibleDC
		[System.Security.SuppressUnmanagedCodeSecurity]
		[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr CreateCompatibleDC(IntPtr hDC);
		IntPtr IPlatformDependentGDI.CreateCompatibleDC(IntPtr hDC)
		{
			return CreateCompatibleDC(hDC);
		}

		// DeleteDC
		[System.Security.SuppressUnmanagedCodeSecurity]
		[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
		private static extern bool DeleteDC(IntPtr hdc);
		bool IPlatformDependentGDI.DeleteDC(IntPtr hdc)
		{
			return DeleteDC(hdc);
		}

		// SelectObject
		[System.Security.SuppressUnmanagedCodeSecurity]
		[DllImport("gdi32.dll", ExactSpelling = true)]
		private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
		IntPtr IPlatformDependentGDI.SelectObject(IntPtr hDC, IntPtr hObject)
		{
			return SelectObject(hDC, hObject);
		}

		// DeleteObject
		[System.Security.SuppressUnmanagedCodeSecurity]
		[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
		private static extern bool DeleteObject(IntPtr hObject);
		bool IPlatformDependentGDI.DeleteObject(IntPtr hObject)
		{
			return DeleteObject(hObject);
		}

		// CreateDIBSection
		[System.Security.SuppressUnmanagedCodeSecurity]
		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern IntPtr CreateDIBSection(IntPtr hdc, IntPtr hdr, uint colors, ref IntPtr pBits, IntPtr hFile, uint offset);
		IntPtr IPlatformDependentGDI.CreateDIBSection(IntPtr hdc, IntPtr hdr, uint colors, ref IntPtr pBits, IntPtr hFile, uint offset) 
		{ 
			return CreateDIBSection(hdc, hdr, colors, ref pBits, hFile, offset);
		}

		// LocalAlloc
		[System.Security.SuppressUnmanagedCodeSecurity]
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr LocalAlloc(int flags, int size);
		IntPtr IPlatformDependentGDI.LocalAlloc(int flags, int size)
		{
			return LocalAlloc(flags, size);
		}

		// LocalFree
		[System.Security.SuppressUnmanagedCodeSecurity]
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern void LocalFree(IntPtr p);
		void IPlatformDependentGDI.LocalFree(IntPtr p)
		{
			LocalFree(p);
		}

		// Polyline
		[System.Security.SuppressUnmanagedCodeSecurity]
		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern int Polyline(IntPtr hdc, int[] lppt, int cPoints);
		int IPlatformDependentGDI.Polyline(IntPtr hdc, int[] lppt, int cPoints)
		{
			return Polyline(hdc, lppt, cPoints);
		}

		// FillRect
		[System.Security.SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		private static extern int FillRect(IntPtr hDC, ref Rectangle lprc, IntPtr hbr);
		int IPlatformDependentGDI.FillRect(IntPtr hDC, ref Rectangle lprc, IntPtr hbr)
		{
			return FillRect(hDC, ref lprc, hbr);
		}

		// CreatePen
		[System.Security.SuppressUnmanagedCodeSecurity]
		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern IntPtr CreatePen(int fnPenStyle, int nWidth, int crColor);
		IntPtr IPlatformDependentGDI.CreatePen(int fnPenStyle, int nWidth, int crColor)
		{
			return CreatePen(fnPenStyle, nWidth, crColor);
		}

		// CreateSolidBrush
		[System.Security.SuppressUnmanagedCodeSecurity]
		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern IntPtr CreateSolidBrush(int color);
		IntPtr IPlatformDependentGDI.CreateSolidBrush(int color)
		{
			return CreateSolidBrush(color);
		}

		// CreateFontIndirect
		[System.Security.SuppressUnmanagedCodeSecurity]
		[DllImport("gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		private static extern IntPtr CreateFontIndirect(IntPtr pLogFont);
		IntPtr IPlatformDependentGDI.CreateFontIndirect(IntPtr pLogFont)
		{
			return CreateFontIndirect(pLogFont);
		}

		// ExtTextOut
		[System.Security.SuppressUnmanagedCodeSecurity]
		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern int ExtTextOut(IntPtr hdc, int X, int Y, uint fuOptions,
			ref Rectangle lprc, string lpString, int cbCount, int[] lpDx);
		int IPlatformDependentGDI.ExtTextOut(IntPtr hdc, int X, int Y, uint fuOptions,
			ref Rectangle lprc, string lpString, int cbCount, int[] lpDx)
		{
			return ExtTextOut(hdc, X, Y, fuOptions,	ref lprc, lpString, cbCount, lpDx);
		}

		// SetTextAlign
		[System.Security.SuppressUnmanagedCodeSecurity]
		[DllImport("gdi32.dll")]
		private static extern uint SetTextAlign(IntPtr hdc, uint fMode);
		uint IPlatformDependentGDI.SetTextAlign(IntPtr hdc, uint fMode)
		{
			return SetTextAlign(hdc, fMode);
		}

		// SetTextColor
		[System.Security.SuppressUnmanagedCodeSecurity]
		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern int SetTextColor(IntPtr hDC, int cColor);
		int IPlatformDependentGDI.SetTextColor(IntPtr hDC, int cColor)
		{
			return SetTextColor(hDC, cColor);
		}

		// SetBkMode
		[System.Security.SuppressUnmanagedCodeSecurity]
		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern int SetBkMode(IntPtr hDC, int nMode);
		int IPlatformDependentGDI.SetBkMode(IntPtr hDC, int nMode)
		{
			return SetBkMode(hDC, nMode);
		}

		// SetPixel
		[System.Security.SuppressUnmanagedCodeSecurity]
		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern uint SetPixel(IntPtr hdc, int X, int Y, uint crColor);
		uint IPlatformDependentGDI.SetPixel(IntPtr hdc, int X, int Y, uint crColor)
		{
			return SetPixel(hdc, X, Y, crColor);
		}

		// GdiFlush
		[System.Security.SuppressUnmanagedCodeSecurity]
		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern bool GdiFlush();
		bool IPlatformDependentGDI.GdiFlush()
		{
			return GdiFlush();
		}

        // DrawText
        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int DrawText(IntPtr hDC, string Text, int nLen, IntPtr pRect, uint uFormat);
        int IPlatformDependentGDI.DrawText(IntPtr hDC, string Text, int nLen, IntPtr pRect, uint uFormat)
        {
            return DrawText(hDC, Text, nLen, pRect, uFormat);
        }

        // DrawText
        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int DrawText(IntPtr hDC, string Text, int nLen, ref Rectangle rect, uint uFormat);
        int IPlatformDependentGDI.DrawText(IntPtr hDC, string Text, int nLen, ref Rectangle rect, uint uFormat)
        {
            return DrawText(hDC, Text, nLen, ref rect, uFormat);
        }

        // CreateRectRgn
        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);
        IntPtr IPlatformDependentGDI.CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect)
        {
            return CreateRectRgn(nLeftRect, nTopRect, nRightRect, nBottomRect);
        }

        // SelectClipRgn
        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("gdi32.dll")]
        private static extern int SelectClipRgn(IntPtr hDC, IntPtr hRgn);
        int IPlatformDependentGDI.SelectClipRgn(IntPtr hDC, IntPtr hRgn)
        {
            return SelectClipRgn(hDC, hRgn);
        }

        // Rectangle
        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("gdi32.dll")]
        private static extern bool Rectangle(IntPtr hdc, int left, int top, int right, int bottom);
        bool IPlatformDependentGDI.Rectangle(IntPtr hdc, int left, int top, int right, int bottom)
        {
            return Rectangle(hdc, left, top, right, bottom);
        }

        // GetTextExtentExPoint
        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("gdi32.dll")]
        private static extern bool GetTextExtentExPoint(IntPtr hdc, string lpString, int cchString,
            int nMaxExtent, out int lpnFit, int[] alpDx, ref Size size);
        bool IPlatformDependentGDI.GetTextExtentExPoint(IntPtr hdc, string lpString, int cchString,
            int nMaxExtent, out int lpnFit, int[] alpDx, ref Size size)
        {
            return GetTextExtentExPoint(hdc, lpString, cchString, nMaxExtent, out lpnFit, alpDx, ref size);
        }

        // BitBlt
        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
            int nWidth, int nHeight, IntPtr hObjSource, int nXSrc, int nYSrc, 
            Reino.ClientConfig.TernaryRasterOperations dwRop);
        bool IPlatformDependentGDI.BitBlt(IntPtr hObject, int nXDest, int nYDest,
            int nWidth, int nHeight, IntPtr hObjSource, int nXSrc, int nYSrc,
            Reino.ClientConfig.TernaryRasterOperations dwRop)
        {
            return BitBlt(hObject, nXDest, nYDest, nWidth, nHeight, hObjSource, nXSrc, nYSrc, dwRop);
        }
    }

	// For compilation reasons, a WinCE_GDI must exist, even though its never used in the Full Framework
	sealed class WinCE_GDI : IPlatformDependentGDI
	{
		IntPtr IPlatformDependentGDI.GetDC(IntPtr hWnd) { return IntPtr.Zero; }
		int IPlatformDependentGDI.ReleaseDC(IntPtr hWnd, IntPtr hDC) { return 0; }
		IntPtr IPlatformDependentGDI.CreateCompatibleDC(IntPtr hDC) { return IntPtr.Zero; }
		bool IPlatformDependentGDI.DeleteDC(IntPtr hdc) { return false; }
		IntPtr IPlatformDependentGDI.SelectObject(IntPtr hDC, IntPtr hObject) { return IntPtr.Zero; }
		bool IPlatformDependentGDI.DeleteObject(IntPtr hObject) { return false; }
		IntPtr IPlatformDependentGDI.CreateDIBSection(IntPtr hdc, IntPtr hdr, uint colors, ref IntPtr pBits, IntPtr hFile, uint offset) { return IntPtr.Zero; }
		IntPtr IPlatformDependentGDI.LocalAlloc(int flags, int size) { return IntPtr.Zero; }
		void IPlatformDependentGDI.LocalFree(IntPtr p) { return; }
		int IPlatformDependentGDI.Polyline(IntPtr hdc, int[] lppt, int cPoints) { return 0; }
		int IPlatformDependentGDI.FillRect(IntPtr hDC, ref Rectangle lprc, IntPtr hbr) { return 0; }
		IntPtr IPlatformDependentGDI.CreatePen(int fnPenStyle, int nWidth, int crColor) { return IntPtr.Zero; }
		IntPtr IPlatformDependentGDI.CreateSolidBrush(int color) { return IntPtr.Zero; }
		IntPtr IPlatformDependentGDI.CreateFontIndirect(IntPtr pLogFont) { return IntPtr.Zero; }
		int IPlatformDependentGDI.ExtTextOut(IntPtr hdc, int X, int Y, uint fuOptions,
			ref Rectangle lprc, string lpString, int cbCount, int[] lpDx) { return 0; }
		uint IPlatformDependentGDI.SetTextAlign(IntPtr hdc, uint fMode) { return 0; }
		int IPlatformDependentGDI.SetTextColor(IntPtr hDC, int cColor) { return 0; }
		int IPlatformDependentGDI.SetBkMode(IntPtr hDC, int nMode) { return 0; }
		uint IPlatformDependentGDI.SetPixel(IntPtr hdc, int X, int Y, uint crColor) { return 0; }
		bool IPlatformDependentGDI.GdiFlush() { return true; }

        int IPlatformDependentGDI.DrawText(IntPtr hDC, string Text, int nLen, IntPtr pRect, uint uFormat) { return 0; }
        int IPlatformDependentGDI.DrawText(IntPtr hDC, string Text, int nLen, ref Rectangle rect, uint uFormat) { return 0; }
        IntPtr IPlatformDependentGDI.CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect) { return IntPtr.Zero; }
        int IPlatformDependentGDI.SelectClipRgn(IntPtr hDC, IntPtr hRgn) { return 0; }
        bool IPlatformDependentGDI.Rectangle(IntPtr hdc, int left, int top, int right, int bottom) { return false; }
        bool IPlatformDependentGDI.GetTextExtentExPoint(IntPtr hdc, string lpString, int cchString,
            int nMaxExtent, out int lpnFit, int[] alpDx, ref Size size) 
        { 
            lpnFit = 0;  
            return false; 
        }
        bool IPlatformDependentGDI.BitBlt(IntPtr hObject, int nXDest, int nYDest,
            int nWidth, int nHeight, IntPtr hObjSource, int nXSrc, int nYSrc,
            Reino.ClientConfig.TernaryRasterOperations dwRop) { return false; }
    }

}
