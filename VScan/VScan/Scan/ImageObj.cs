/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;
using PremierTaxFree.PTFLib;

namespace PremierTaxFree
{
    public enum TestType
    {
        None = 0,
        LeftTop,
        RightTop,
        RightBottom,
        LeftBottom,
    }

    public enum SelectionType
    {
        None = 0,
        Resize = 1,
        Rotate = 2,
    }

    public class ImageObjComparer : IComparer<ImageObj>
    {
        public static readonly ImageObjComparer Default = new ImageObjComparer();

        public int Compare(ImageObj x, ImageObj y)
        {
            return x == y ? 0 : 1;
        }
    }

    /// <summary>
    /// Image object abstraction.
    /// Used to select, unselect, edit, move, resize images over the window.
    /// </summary>
    [Serializable]
    public class ImageObj : IDisposable
    {
        private Rectangle m_R1, m_R2, m_R3, m_R4;

        public Bitmap Bmp { get; set; }
        public Rectangle Rect { get; private set; }
        public Point Offset { get; set; }
        public SelectionType Selection { get; set; }
        public Guid Id { get; private set; }

        public ImageObj(Bitmap org, Rectangle rec)
        {
            Id = Guid.NewGuid();

            Debug.Assert(org != null);

            Resize(rec);

            Bmp = new Bitmap(rec.Width, rec.Height);

            for (int x = rec.Left; x < rec.Right; x++)
            {
                for (int y = rec.Top; y < rec.Bottom; y++)
                {
                    if (org.Width > x && org.Height > y)
                    {
                        Color c = org.GetPixel(x, y);
                        Bmp.SetPixel(x - rec.Left, y - rec.Top, c);
                        org.SetPixel(x, y, Color.White);
                    }
                }
            }
        }

        ~ImageObj()
        {
            Bmp.Dispose();
        }

        public void Resize(Rectangle rect)
        {
            Resize(rect, true);
        }

        public void Resize(Rectangle rect, bool setRect)
        {
            if (setRect)
                Rect = rect;

            CreateBoundRectangle();

            if (Bmp != null && Rect.Width > 0 && Rect.Height > 0)
            {
                Bitmap bmp = Bmp.Resize(Rect.Width, Rect.Height);
                Bmp.Dispose();
                Bmp = bmp;
            }
        }

        public void Rotate(float angle)
        {
            Debug.Assert(Bmp != null);

            if (float.IsNaN(angle))
                return;

            Bitmap bmp = Bmp.Rotate(angle);
            if (bmp != null)
            {
                Bmp.DisposeSf();
                Bmp = bmp;
            }
        }

        public void Move(int x, int y)
        {
            Rectangle r = Rect;
            r.Offset(x, y);
            Rect = r;
            CreateBoundRectangle();
        }

        public void MoveTo(int x, int y)
        {
            Rect = new Rectangle(x, y, Rect.Width, Rect.Height);
            CreateBoundRectangle();
        }

        public void Dispose()
        {
            Bmp.Dispose();
        }

        public SelectionType Click(Point point)
        {
            if (!Rect.Contains(point))
                return SelectionType.None;

            Selection = Selection + 1;
            if (Selection > SelectionType.Rotate)
                Selection = SelectionType.None;
            return Selection;
        }

        public TestType Test(Point point)
        {
            if (!Selection.ToBool())
                return TestType.None;
            else if (m_R1.Contains(point))
                return TestType.LeftTop;
            else if (m_R2.Contains(point))
                return TestType.RightTop;
            else if (m_R3.Contains(point))
                return TestType.RightBottom;
            else if (m_R4.Contains(point))
                return TestType.LeftBottom;
            return TestType.None;
        }

        public void Paint(Graphics g)
        {
            g.DrawImage(Bmp, Rect.Location);
            if (Selection.ToBool())
            {
                g.FillEllipse(Selection.ToValue<Brush>(Brushes.White, Brushes.Aqua, Brushes.Purple), m_R1);
                g.FillEllipse(Selection.ToValue<Brush>(Brushes.White, Brushes.Aqua, Brushes.Purple), m_R2);
                g.FillEllipse(Selection.ToValue<Brush>(Brushes.White, Brushes.Aqua, Brushes.Purple), m_R3);
                g.FillEllipse(Selection.ToValue<Brush>(Brushes.White, Brushes.Aqua, Brushes.Purple), m_R4);
                g.DrawRectangle(SystemPens.ActiveBorder, Rect);
            }
        }

        public static bool operator ==(ImageObj obj1, ImageObj obj2)
        {
            return obj1.Id == obj2.Id;
        }

        public static bool operator !=(ImageObj obj1, ImageObj obj2)
        {
            return obj1.Id != obj2.Id;
        }

        private void CreateBoundRectangle()
        {
            const int H = 5, W = 10;

            Rectangle r = Rect;

            m_R1 = new Rectangle(r.X - H, r.Y - H, W, W);
            m_R2 = new Rectangle(r.Right - H, r.Y - H, W, W);
            m_R3 = new Rectangle(r.Right - H, r.Bottom - H, W, W);
            m_R4 = new Rectangle(r.X - H, r.Bottom - H, W, W);
        }
    }
}
