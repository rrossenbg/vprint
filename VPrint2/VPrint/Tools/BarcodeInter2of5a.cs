/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using VPrinting.Extentions;

namespace VPrinting
{
    [Serializable]
    public class BarcodeInter2of5 : IImageObject
    {
        private const int MARGIN = 10, PADDINGDOWN = 30;

        private static readonly string gLeftGuard = "1010";
        private static readonly string gRightGuard = "01101";
        private static readonly string[] gOdd = { "1011001", "1101011", "1001011", "1100101", "1011011", "1101101", "1001101", "1010011", "1101001", "1001001" };
        private static readonly string[] gEven = { "0100110", "0010100", "0110100", "0011010", "0100100", "0010010", "0110010", "0101100", "0010110", "0110110" };

        public Point Location { get; set; }
        public Size Size { get; set; }
        private string m_text;
        public string Text
        {
            get { return m_text; }
            set
            {
                Validate(value);
                m_text = value;
                EncodedMessage = EncodeBarcode(value);
            }
        }
        public Font Font { get; set; }
        public string Format { get; set; }
        public string EncodedMessage { get; set; }
        public bool Selected { get; set; }
        public string BoundColumn { get; set; }

        public BarcodeInter2of5()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <example>
        /// Bitmap barcodeImage = new Bitmap(250, 100);
        /// using (Graphics g = Graphics.FromImage(barcodeImage))
        /// {
        ///     PrintBarcode(g, 280, 70);
        /// }
        /// </example>
        public void Draw(Graphics g, Point offset, DrawingSurface surface)
        {
            Debug.Assert(!string.IsNullOrEmpty(Text));
            Debug.Assert(!string.IsNullOrEmpty(EncodedMessage));

            Point p = Location;
            p.Offset(offset);

            using (SolidBrush white = new SolidBrush(Color.White), black = new SolidBrush(Color.Black))
            using (Font font = new Font(FontFamily.GenericMonospace, 20, FontStyle.Regular))
            {
                Rectangle r = new Rectangle(p, Size);
                
                g.FillRectangle(white, r);

                float xPos = p.X + MARGIN;
                int yTop = p.Y + MARGIN;

                for (int i = 0; i < EncodedMessage.Length; i++)
                {
                    if (EncodedMessage[i] == '1')
                        g.FillRectangle(black, xPos, yTop, Font.Size, Size.Height - PADDINGDOWN);
                    xPos += Font.Size;
                }

                xPos = p.X + MARGIN;
                yTop += Size.Height - PADDINGDOWN - 2;

                for (int i = 0; i < Text.Length; i++)
                {
                    g.DrawString(Text[i].ToString().Trim(), font, black, xPos, yTop);
                    xPos += 14;
                }

                if (Selected && surface == DrawingSurface.Screen)
                    g.DrawRectangles(Pens.Black, r.ToRectArray(new Size(5,5)).ToArray());
            }
        }

        public bool Contains(Point p)
        {
            return new Rectangle(Location, Size).Contains(p);
        }

        public void Measure(Graphics g)
        {
            int width = EncodedMessage.Length * Convert.ToInt32(Font.Size) + 2 * MARGIN;
            Size = new Size(width, 110);
        }

        private static void Validate(string message)
        {
            Regex reNum = new Regex(@"^\d+$");
            if (reNum.Match(message).Success == false)
                throw new Exception("Encode string must be numeric");
        }

        private static string EncodeBarcode(string message)
        {
            StringBuilder b = new StringBuilder(gLeftGuard);

            for (int i = 0; i < message.Length; i++)
            {
                if ((i % 2) == 0)
                {
                    b.Append(gOdd[Convert.ToInt32(message[i].ToString())]);
                }
                else
                {
                    b.Append(gEven[Convert.ToInt32(message[i].ToString())]);
                }
            }

            b.Append(gRightGuard);

            return b.ToString();
        }
    }
}
