/***************************************************
//  Copyright (c) Premium Tax Free 2011-2013
/***************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using VPrinting.Tools;
using Zen.Barcode;
using SSize = System.Drawing.Size;

namespace VPrinting.Documents
{
    /// <summary>
    /// Printer line. For Direct printing
    /// </summary>
    ///<see cref="http://gibsongraphics.com/pica-points.htm"/>
    [Serializable]
    public class PrintLine : IPrintLine
    {
        public string Description { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public int Size { get; set; }
        public string Text { get; set; }
        [XmlIgnore]
        public int MinX { get; set; }
        [XmlIgnore]
        public int MaxX { get; set; }
        [XmlIgnore]
        public int MinY { get; set; }
        [XmlIgnore]
        public int MaxY { get; set; }
        public PrintLine()
        {
            const int MINH = 0, MAXH = 8240, MINV = 0, MAXV = 8240;
            MinX = MINH;
            MaxX = MAXH;
            MinY = MINV;
            MaxY = MAXV;
        }
        public PrintLine(string description)
            : this()
        {
            Description = description;
        }

        public virtual void Print(StringBuilder b)
        {
            b.Append(MTPL.SetFontDensity((Cpi)this.Size));
            b.Append(this.Text);
        }

        /// <summary>
        /// X == 0 &amp;&amp; Y == 0
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return X == 0 && Y == 0;
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine(Description);
            b.AppendFormat("X={0}\r\n ", X);
            b.AppendFormat("Y={0}\r\n ", Y);
            b.AppendLine(Text);
            return b.ToString();
        }
    }

    /// <summary>
    /// Used for graphics printing
    /// </summary>
    [Serializable]
    public class GPrintLine : PrintLine
    {
        public FontWrapper Font { get; set; }
        public GPrintLine()
        {
            const int MINH = 0, MAXH = 8240, MINV = 0, MAXV = 8240;
            MinX = MINH;
            MaxX = MAXH;
            MinY = MINV;
            MaxY = MAXV;
        }
        public GPrintLine(string description)
            : this()
        {
            Description = description;
        }

        public void Print(PrintPageEventArgs e, Brush brush, Point moveAll)
        {
            if (Font == null)
                throw new ArgumentNullException("Line.Font");
            if (Font.Value == null)
                throw new ArgumentNullException("Line.Font.Value");

            var lines = Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            Size size;
            for (int i = 0, y = (int)Y; i < lines.Length; i++, y += size.Height)
            {
                e.Graphics.DrawString(lines[i], Font.Value, brush, X + moveAll.X, y + moveAll.Y);
                size = SSize.Round(e.Graphics.MeasureString(lines[i], Font.Value));
            }
        }
    }

    /// <summary>
    /// Used for graphics printing in In/mm
    /// </summary>
    [Serializable]
    public class GPrintLineUnit : PrintLine
    {
        public FontWrapper Font { get; set; }

        /// <summary>
        /// public enum PrinterUnit
        /// {
        ///     Display,
        ///     ThousandthsOfAnInch,
        ///     HundredthsOfAMillimeter,
        ///     TenthsOfAMillimeter
        /// }
        /// </summary>
        [EnumMember]
        public GraphicsUnit Units;
        public GPrintLineUnit()
        {
            Units = GraphicsUnit.Inch;
        }
        public GPrintLineUnit(string description)
            : this()
        {
            Description = description;
        }

        public void Print(PrintPageEventArgs e, Brush brush, Point moveAll)
        {
            if (Font == null)
                throw new ArgumentNullException("Line.Font");
            if (Font.Value == null)
                throw new ArgumentNullException("Line.Font.Value");

            var lines = Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            SizeF size;

            float x = (Units == GraphicsUnit.Inch) ? X.FromInch() : X;
            float y = (Units == GraphicsUnit.Inch) ? Y.FromInch() : Y;

            for (int i = 0; i < lines.Length; i++, y += size.Height)
            {
                e.Graphics.DrawString(lines[i], Font.Value, brush, x + moveAll.X, y + moveAll.Y);
                size = e.Graphics.MeasureString(lines[i], Font.Value);
            }
        }
    }

    /// <summary>
    /// Used for graphics printing of barcodes
    /// </summary>
    [Serializable]
    public class BarPrintLine : PrintLine
    {
        public int Height { get; set; }

        /// <summary>
        /// Text position
        /// </summary>
        /// <remarks>
        /// X => Barcode Middle + X
        /// Y => Barcode Bottom + Y
        /// </remarks>
        public GPrintLine BarText { get; set; }

        public BarcodeSymbology Symbology { get; set; }

        public BarPrintLine()
        {
        }

        public void Print(PrintPageEventArgs e, Brush brush, Point moveAll)
        {
            using (var bmp = BarcodeTools.BinaryWritePicture(Text, Height, Size))
            {
                e.Graphics.DrawImage(bmp, X + moveAll.X, Y + moveAll.Y);

                if (BarText == null)
                    throw new ApplicationException("BarText empty");

                SizeF s = e.Graphics.MeasureString(BarText.Text, BarText.Font.Value);

                e.Graphics.DrawString(BarText.Text, BarText.Font.Value, brush,
                    X + ((bmp.Width - s.Width) / 2 + BarText.X) + moveAll.X, // Middle
                    Y + (bmp.Height + BarText.Y) + moveAll.Y);//Bottom
            }
        }
    }

    /// <summary>
    /// Used for graphics printing of barcodes in In/mm
    /// </summary>
    [Serializable]
    public class BarPrintLineUnit : PrintLine
    {
        public int Height { get; set; }
        [EnumMember]
        public GraphicsUnit Units;
        /// <summary>
        /// Text position
        /// </summary>
        /// <remarks>
        /// X => Barcode Middle + X (in pixel)
        /// Y => Barcode Bottom + Y (in pixel)
        /// </remarks>
        public GPrintLineUnit BarText { get; set; }
        public BarPrintLineUnit()
        {
            Units = GraphicsUnit.Inch;
        }

        public void Print(PrintPageEventArgs e, Brush brush, Point moveAll)
        {
            using (var bmp = BarcodeTools.BinaryWritePicture(Text, Height, Size))
            {
                float x = (Units == GraphicsUnit.Inch) ? X.FromInch() : X;
                float y = (Units == GraphicsUnit.Inch) ? Y.FromInch() : Y;

                e.Graphics.DrawImage(bmp, X + moveAll.X, Y + moveAll.Y);

                if (BarText == null)
                    throw new ApplicationException("BarText empty");

                SizeF s = e.Graphics.MeasureString(BarText.Text, BarText.Font.Value);

                e.Graphics.DrawString(BarText.Text, BarText.Font.Value, brush,
                    (x + ((bmp.Width - s.Width) / 2 + BarText.X) + moveAll.X), // Middle
                    (y + (bmp.Height + BarText.Y)) + moveAll.Y);//Bottom
            }
        }
    }

    /// <summary>
    /// Used for direct/graphics printing for MTPL
    /// </summary>
    [Serializable]
    public class BarCodeLine : PrintLine
    {
        public string XYZ { get; set; }
        public bool Compressed { get; set; }

        public BarCodeLine()
        {
        }
        public BarCodeLine(string description)
            : base(description)
        {
        }

        public override void Print(StringBuilder b)
        {
            b.AppendLine(MTPL.PrintI2Of5Barcode(base.Text, base.Size, this.XYZ));
            b.AppendLine();
        }
    }

    /// <summary>
    /// Compares two print line instances 
    /// </summary>
    public class PrintLineComparer : IComparer<IPrintLine>
    {
        public static PrintLineComparer Default = new PrintLineComparer();
        public PrintLineComparer()
        {
        }

        public int Compare(IPrintLine a, IPrintLine b)
        {
            if (a == null)
                return 1;
            if (b == null)
                return -1;

            if (a.Y < b.Y)
            {
                return -1;
            }
            else if (a.Y > b.Y)
            {
                return 1;
            }
            else //if (a.Y == b.Y)
            {
                if (a.X < b.X)
                    return -1;
                else if (a.X > b.X)
                    return 1;
                else //if (a.X == b.X)
                    return 0;
            }
        }
    }
}
