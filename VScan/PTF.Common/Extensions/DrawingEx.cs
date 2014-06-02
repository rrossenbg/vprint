/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Collections.Generic;
using System.Drawing.Drawing2D;

namespace PremierTaxFree.PTFLib
{
    public static class DrawingEx
    {
        private static Random ms_Rnd = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// Creates array of points by rectangle
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Point[] ToPoints(this Rectangle r)
        {
            return new Point[] { 
                r.Location, 
                new Point(r.X + r.Width, r.Y), 
                new Point(r.X + r.Width, r.Y + r.Height), 
                new Point(r.X, r.Y + r.Height)};
        }

        /// <summary>
        /// Fills a rectangle with hatch brush
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        /// <param name="foreColor"></param>
        /// <param name="backColor"></param>
        public static void FillRectangleHatch(this Graphics g, Rectangle rect, Color foreColor, Color backColor)
        {
            using (HatchBrush b = new HatchBrush(HatchStyle.BackwardDiagonal, foreColor, backColor))
            {
                g.FillRectangle(b, rect);
            }
        }

        /// <summary>
        /// Draws a string into rectangle
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="brush"></param>
        public static void DrawStringInRectangle(this Graphics g, Rectangle rect, string text, Font font, Brush brush)
        {
            var size = g.MeasureString(text, font).ToSize();
            var offset = rect.Size.Center(size);
            var r = rect.Location;
            r.Offset(offset);
            g.DrawString(text, font, brush, r);
        }

        /// <summary>
        /// Fills rectangle by hatch brush
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        /// <param name="foreColor"></param>
        /// <param name="backColor"></param>
        /// <param name="style"></param>
        public static void FillRectangleHatch(this Graphics g, Rectangle rect, Color foreColor, Color backColor, HatchStyle style)
        {
            using (HatchBrush b = new HatchBrush(style, foreColor, backColor))
            {
                g.FillRectangle(b, rect);
            }
        }

        /// <summary>
        /// Inverts the point coordinates
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Point Invert(this Point point)
        {
            return new Point(-point.X, -point.Y);
        }

        /// <summary>
        /// Finds the center
        /// </summary>
        /// <param name="size1"></param>
        /// <param name="size2"></param>
        /// <returns></returns>
        public static Point Center(this Size size1, Size size2)
        {
            Point p = new Point(size1.Width / 2 - size2.Width / 2, size1.Height / 2 - size2.Height / 2);
            return p;
        }

        /// <summary>
        /// Computes the length of segment
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static float LengthTo(this Point from, Point to)
        {
            float result = Convert.ToSingle(Math.Sqrt((from.X - to.X) ^ 2 + (from.Y - to.Y) ^ 2));
            if (float.IsNaN(result))
                return 0;
            return result;
        }

        /// <summary>
        /// Computes the angle between two points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static float ToAngle(this PointF p1, PointF p2)
        {
            return Convert.ToSingle(Math.Atan((p1.Y - p2.Y) / (p1.X - p2.X)));
        }

        /// <summary>
        /// Casts a point to pointf
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static PointF Cast(this Point point)
        {
            return new PointF(point.X, point.Y);
        }

        /// <summary>
        /// Computes a random point arround center which lays in a ring
        /// </summary>
        /// <param name="p"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Point Random(this Point p, int min, int max)
        {
            int x = ms_Rnd.Next(min, max);
            int y = ms_Rnd.Next(min, max);
            return new Point(x, y);
        }

        /// <summary>
        /// Computes a random color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color Random(this Color color)
        {
            return Color.FromKnownColor((KnownColor)ms_Rnd.Next(1, 150));
        }

        /// <summary>
        /// Keep value between 1 and 174
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Color ToKnownColor(this Int32 value)
        {
            Debug.Assert(value > 0 && value <= 174, "Keep value in between 1 and 174");
            return Color.FromKnownColor((KnownColor)value);
        }

        /// <summary>
        /// Initialize Random(2)
        /// </summary>
        /// <param name="rnd"></param>
        /// <returns></returns>
        public static Color ToColor(this Random rnd)
        {
            // to create lighter colours:
            // take a random integer between 0 & 128 (rather than between 0 and 255)
            // and then add 127 to make the colour lighter
            byte[] colorBytes = new byte[3];
            colorBytes[0] = (byte)(rnd.Next(128) + 127);
            colorBytes[1] = (byte)(rnd.Next(128) + 127);
            colorBytes[2] = (byte)(rnd.Next(128) + 127);

            Color color = Color.FromArgb(255, colorBytes[0], colorBytes[1],colorBytes[2]);
            return color;
        }

        /// <summary>
        /// Gets colors of a palette
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static List<Color> GetPalette(this Color color)
        {
            PropertyInfo[] properties = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static);

            List<Color> colors = new List<Color>();
            foreach (PropertyInfo prop in properties)
            {
                color = (Color)prop.GetValue(null, null);
                if (color == Color.Transparent) 
                    continue;
                if (color == Color.Empty) 
                    continue;
                colors.Add(color);
            }
            return colors;
        }

        /// <summary>
        /// Inflates a rectangle by 20, 20
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Rectangle InflateEx(this Rectangle rect)
        {
            return rect.InflateEx(20, 20);
        }

        /// <summary>
        /// Inflates a rectangle
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Rectangle InflateEx(this Rectangle rect, int width, int height)
        {
            Rectangle r = rect;
            r.Inflate(width, height);
            return r;
        }

        /// <summary>
        /// Rotates a point around a center by a angle
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static PointF Rotate(this PointF center, SizeF radius, double angle)
        {
            float x = Convert.ToSingle(center.X + (radius.Width * Math.Cos(angle)));
            float y = Convert.ToSingle(center.Y + (radius.Height * Math.Sin(angle)));
            return new PointF(x, y);
        }

        /// <summary>
        /// Rotates a point around a center by a angle
        /// </summary>
        /// <param name="center"></param>
        /// <param name="point"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static PointF Rotate(this PointF center, PointF point, float angle)
        {
            float w = point.X - center.X;
            float h = point.Y - center.Y;
            double r = Math.Sqrt(w * w + h * h);
            double value = Math.Atan(h / w) + angle;
            Debug.WriteLine(value, " value ");
            return new PointF(
                Convert.ToSingle(center.X + (r * Math.Cos(value))),
                Convert.ToSingle(center.Y + (r * Math.Sin(value))));
        }

        /// <summary>
        /// Computes the center of rectangle
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static PointF Center(this Rectangle rect)
        {
            return new PointF(rect.X + rect.Width / 2.0f, rect.Y + rect.Height / 2.0f);
        }

        public static Rectangle Bounds(this Rectangle rect, float angle)
        {
            PointF p0 = new PointF(rect.X + rect.Width / 2.0f, rect.Y + rect.Height / 2.0f);
            PointF p1 = p0.Rotate(new PointF(rect.Left, rect.Top), angle);
            PointF p2 = p0.Rotate(new PointF(rect.Right, rect.Top), angle);
            PointF p3 = p0.Rotate(new PointF(rect.Right, rect.Bottom), angle);
            PointF p4 = p0.Rotate(new PointF(rect.Left, rect.Bottom), angle);
            return Bounds(p1, p2, p3, p4);
        }

        /// <summary>
        /// Creates a rectangle structure by array of points
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Rectangle Bounds(params PointF[] points)
        {
            const int MIN = 10;
            float x1 = float.MaxValue;
            float x2 = float.MinValue;
            float y1 = float.MaxValue;
            float y2 = float.MinValue;

            foreach (PointF p in points)
            {
                if (x1 > p.X)
                    x1 = p.X;
                if (x2 < p.X)
                    x2 = p.X;
                if (y1 > p.Y)
                    y1 = p.Y;
                if (y2 < p.Y)
                    y2 = p.Y;
            }

            return
                Rectangle.Ceiling(RectangleF.FromLTRB(x1, y1, (x1 == x2) ? x1 + MIN : x2, (y1 == y2) ? y2 + MIN : y2));
        }

        /// <summary>
        /// Computes the area covered by two rectangle structures
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static Rectangle Area(this Rectangle r1, Rectangle r2)
        {
            return Rectangle.FromLTRB(Math.Min(r1.Left, r2.Left), Math.Min(r1.Top, r2.Top),
                Math.Max(r1.Right, r2.Right), Math.Max(r1.Bottom, r2.Bottom));
        }

        /// <summary>
        /// Returns invalid rectangle structure
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Rectangle Invalid(this Rectangle r)
        {
            return Rectangle.FromLTRB(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
        }        

        /// <summary>
        /// Subtracts size from point structure
        /// </summary>
        /// <param name="p"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Point Sub(this Point p, Size s)
        {
            return Point.Subtract(p, s);
        }

        /// <summary>
        /// Checks whether size of rectangle is betweeen min size and max size
        /// </summary>
        /// <param name="r"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsBetween(this Rectangle r, Size min, Size max)
        {
            return min.LE(r.Size) && r.Size.LE(max);
        }

        /// <summary>
        /// Compares two size structures (less or equal)
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool LE(this Size s1, Size s2)
        {
            return s1.Width <= s2.Width && s1.Height <= s2.Height;
        }

        /// <summary>
        /// Compares two size structures (greater or equal)
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool GE(this Size s1, Size s2)
        {
            return s1.Width >= s2.Width && s1.Height >= s2.Height;
        }

        /// <summary>
        /// Checks whether a rectangle structure is valid
        /// </summary>
        /// <param name="re"></param>
        /// <returns></returns>
        public static bool IsValid(this Rectangle re)
        {
            return re.X >= 0 && re.Y >= 0 && re.Height > 0 && re.Width > 0;
        }
    }
}
