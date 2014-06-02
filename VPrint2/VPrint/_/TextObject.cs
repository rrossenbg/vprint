/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Drawing;
using System.Linq;
using VPrinting.Extentions;

namespace VPrinting
{
    [Serializable]
    public class TextObject : IImageObject
    {
        public Point Location { get; set; }
        public Size Size { get; set; }
        public string Text { get; set; }
        public string Format { get; set; }
        public Font Font { get; set; }
        public bool Selected { get; set; }
        public string BoundColumn { get; set; }

        public void Draw(Graphics g, Point offset, DrawingSurface surface)
        {
            Point p = Location;
            p.Offset(offset);

            Rectangle r = new Rectangle(p,Size);

            if (surface == DrawingSurface.Screen)
            {
                g.FillRectangle(Brushes.White, r);
                g.DrawRectangle(Pens.Black, r);
            }

            string text = string.IsNullOrEmpty(Format) ? Text : string.Format(Format, Text);
            g.DrawString(text, Font, Brushes.Black, p);

            if (Selected && surface == DrawingSurface.Screen)
                g.DrawRectangles(Pens.Black, r.ToRectArray(new Size(5, 5)).ToArray());
        }

        public void Measure(Graphics g)
        {
            string text = string.IsNullOrEmpty(Format) ? Text : string.Format(Format, Text);
            var s = g.MeasureString(text, Font);
            Size = s.ToSize();
        }

        public bool Contains(Point p)
        {
            return new Rectangle(Location, Size).Contains(p);
        }
    }
}
