/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System.Drawing;

namespace VPrinting
{
    public enum DrawingSurface
    {
        Screen,
        Printer,
    }

    public interface IImageObject
    {
        Point Location { get; set; }
        Size Size { get; set; }
        string Text { get; set; }
        string Format { get; set; }
        string BoundColumn { get; set; }
        Font Font { get; set; }
        bool Selected { get; set; }
        void Draw(Graphics g, Point offset, DrawingSurface surface);
        void Measure(Graphics g);
        bool Contains(Point p);
    }
}
