using System;
using System.Linq;
using System.Reflection;
using System.IO;
using Android.Graphics;

namespace System.Duncan.Drawing
{
    public class ImageAttributes
    {
        ColorMatrix cm;
        public void SetColorMatrix(ColorMatrix cm)
        {
            this.cm = cm;
        }
        public ColorMatrix GetColorMatrix()
        {
            return cm;
        }
    }
    public class ColorMatrix
    {
        float[][] matrix;
        public float[][] Matrix{
            get{ return matrix; }
        }
        public ColorMatrix(float[][] matrix)
        {
            this.matrix = matrix;
        }
    }

    
    public enum GraphicsUnit
    {
        Pixel,
    }


    
    public class MouseEventArgs
    {
        public int X{get; set;}
        public int Y{get; set;}

        public MouseEventArgs(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }

    public abstract class Image : IDisposable
    {
        public abstract int Width { get; }
        public abstract int Height { get; }

        public System.Drawing.Size Size { get { return new System.Drawing.Size(Width, Height); } }

        public virtual void Dispose()
        {
        }
    }


    public class Bitmap : Image
    {
        public Android.Graphics.Bitmap ABitmap;

        public Bitmap(int w, int h)
        {
            ABitmap = Android.Graphics.Bitmap.CreateBitmap(w,h, Android.Graphics.Bitmap.Config.Argb8888);
        }
        public Bitmap(MemoryStream ms)
        {
            ABitmap = Android.Graphics.BitmapFactory.DecodeStream(ms);
        }
        public Bitmap(Stream rs)
        {
            ABitmap = Android.Graphics.BitmapFactory.DecodeStream(rs);
        }
        
        public Bitmap(string filename)
        {
            ABitmap = Android.Graphics.BitmapFactory.DecodeFile (filename);
        }

        public Bitmap(string filename, BitmapFactory.Options options)
        {
            ABitmap = Android.Graphics.BitmapFactory.DecodeFile(filename, options);
        }

        public void Clear(Color color){
            //ABitmap.EraseColor(color.ToArgb());
        }

        public override int Width{
            get{ return ABitmap.Width; }
        }
        public override int Height{
            get{ return ABitmap.Height; }
        }

        public override void Dispose(){
            if (ABitmap != null)
            {
                ABitmap.Dispose();
                ABitmap = null;
            }
        }

        public void RotateBitmap(int angle)
        {
            try
            {
                Android.Graphics.Bitmap rotatedBitmap = null;
                Android.Graphics.Matrix matrix = new Android.Graphics.Matrix();
                matrix.PostRotate(angle);
                rotatedBitmap = Android.Graphics.Bitmap.CreateBitmap(this.ABitmap, 0, 0, this.ABitmap.Width, this.ABitmap.Height, matrix, false);
                this.ABitmap.Dispose();
                this.ABitmap = rotatedBitmap;
                matrix.Dispose();
            }
            catch (Exception ex)
            {               
                System.Console.WriteLine("Exception source {0}: {1}", ex.Source, ex.ToString());
            } 
        }
        
    }

    public class Pen : IDisposable
    {
        public Color Color;
        public int Width;

        public Pen(Color c)
        {
            this.Color = c;
            this.Width = 1;
        }

        public Pen(Color c, int width)
        {
            this.Color = c;
            this.Width = width;
        }
        public void Dispose()
        {
        }
    }


    public class Brush : IDisposable
    {
        public Color Color;

        public Brush(Color c)
        {
            this.Color = c;
        }
        public void Dispose()
        {
        }
    }

    public class SolidBrush : Brush
    {
        public SolidBrush(Color c) : base(c)
        {
        }
    }

    public enum FontStyle
    {
        Regular,
        Bold,
        Italic,
    }

    public enum Rotation_Android
    {
        Rotate0,
        Rotate90,
        Rotate180,
        Rotate270
    }

    public enum Justification_Android
    {
        Left,
        Center,
        Right
    }


    public class Font : IDisposable
    {
        public string Face;
        public string Name;
        public int Size;
        public FontStyle Style;
        public Justification_Android Justification = Justification_Android.Left;
        public Rotation_Android Rotation = Rotation_Android.Rotate0;


        public Font(string face, int size, FontStyle style)
        {
            Size = size;
            Face = face;
            Style = style;
            Name = Face;
            Justification = Justification_Android.Left;
            Rotation = Rotation_Android.Rotate0;
        }

        public Font(string face, int size, FontStyle style, Justification_Android iJustification, Rotation_Android iRotation)
        {
            Size = size;
            Face = face;
            Style = style;
            Name = Face;
            Justification = iJustification;
            Rotation = iRotation;
        }


        public void Dispose()
        {
        }
    }


    public static class ColorHelper
    {
        public static Android.Graphics.Color AColor(this Color c)
        {
            return new Android.Graphics.Color(c.ToArgb());
        }
    }

    public struct Rectangle
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public int X { get { return Left; } set { Left = value; } }
        public int Y { get { return Top; } set { Top = value; } }

        public int Right { get { return Left + Width; } }
        public int Bottom { get { return Top + Height; } }

        public Point Location { get { return new Point(Left, Top); } set { this.X = value.X; this.Y = value.Y; } }

        public System.Drawing.Size Size { get { return new System.Drawing.Size(Width, Height); } }

        public Rectangle(int Left, int Top, int Width, int Height)
            : this()
        {
            this.Left = Left;
            this.Top = Top;
            this.Width = Width;
            this.Height = Height;
        }

        public bool Contains(Rectangle r)
        {
            return this.Contains(r.Location) && r.Contains(r.Right, r.Bottom);
        }

        public bool Contains(Point p)
        {
            return Contains(p.X, p.Y);
        }

        public bool Contains(int x, int y)
        {
            return this.Left <= x && this.Right >= x && this.Top <= y && this.Bottom >= y;
        }

        public bool IntersectsWith(Rectangle r)
        {
            return this.Contains(r.Left, r.Top) || this.Contains(r.Right, r.Bottom) || r.Contains(this.Left, this.Top) || r.Contains(this.Right, this.Bottom);
        }

    }

    public struct RectangleF
    {
        public float Left { get; set; }
        public float Top { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public float X { get { return Left; } set { Left = value; } }
        public float Y { get { return Top; } set { Top = value; } }

        public float Right { get { return Left + Width; } }
        public float Bottom { get { return Top + Height; } }

        public Point Location { get { return new Point((int)Left, (int)Top); } }

        public RectangleF(float Left, float Top, float Width, float Height)
            : this()
        {
            this.Left = Left;
            this.Top = Top;
            this.Width = Width;
            this.Height = Height;
        }

        public bool Contains(RectangleF r)
        {
            return this.Contains(r.Location) && r.Contains(new Point((int)r.Right, (int)r.Bottom));
        }

        public bool Contains(Point p)
        {
            return this.Left <= p.X && this.Right >= p.X && this.Top <= p.Y && this.Bottom >= p.Y;
        }

    }


    public static class RectHelpers
    {

        public static Android.Graphics.Rect ToA(this Rectangle r)
        {
            return new Android.Graphics.Rect(r.Left, r.Top, r.Left + r.Width, r.Top + r.Height);
        }

        public static Android.Graphics.RectF ToA(this RectangleF r)
        {
            return new Android.Graphics.RectF(r.Left, r.Top, r.Left + r.Width, r.Top + r.Height);
        }
    }

}