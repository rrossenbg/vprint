using System;
using System.Drawing;
using System.Runtime;

namespace VCover
{
    public static class Class1
    {
        [TargetedPatchingOptOut("na")]
        public static Rectangle ScrollOffsetY(this Rectangle rect, float offsetY)
        {
            var r = rect;
            r.Offset(0, -(int)offsetY);
            return r;
        }

        [TargetedPatchingOptOut("na")]
        public static Size Offset(this Rectangle rect1, Rectangle rect2)
        {
            return new Size((rect1.X - rect2.X), (rect1.Y - rect2.Y));
        }

        [TargetedPatchingOptOut("na")]
        public static double Distance(this Rectangle rect1, Rectangle rect2)
        {
            var c1 = rect1.Center();
            var c2 = rect2.Center();
            return Math.Sqrt((c2.X - c1.X) * (c2.X - c1.X) + (c2.Y - c1.Y) * (c2.Y - c1.Y));
        }

        [TargetedPatchingOptOut("na")]
        public static double Distance(this Point p1, Point p2)
        {
            return Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));
        }

        [TargetedPatchingOptOut("na")]
        public static Point Center(this Rectangle rect)
        {
            return new Point((rect.Left + rect.Width) / 2, (rect.Top + rect.Height) / 2);
        }
    }
}
