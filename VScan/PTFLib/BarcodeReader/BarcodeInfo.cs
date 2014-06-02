using System;
using System.Drawing;
using System.Runtime.Serialization;
using DTKBarReader;

namespace PremierTaxFree.PTFLib
{
    [Serializable]
    public class BarcodeInfo : ISerializable
    {
        /// <summary>
        /// BarcodeString
        /// </summary>
        public string String { get; set; }

        /// <summary>
        /// BarcodeInfoString
        /// </summary>
        public string InfoString { get; set; }

        /// <summary>
        /// BarcodeRectangle
        /// </summary>
        public Rectangle Rectangle { get; set; }

        /// <summary>
        /// BoundingRectangle
        /// </summary>
        public Rectangle BoundingRectangle { get; set; }

        public int Length { get; set; }
        public byte[] Data { get; set; }

        public BarcodeInfo()
        {
            String = string.Empty;
            InfoString = string.Empty;
        }

        public BarcodeInfo(Barcode barcode)
        {
            String = barcode.BarcodeString;
            InfoString = barcode.BarcodeInfoString;
            Rectangle = Rectangle.FromLTRB(barcode.Left, barcode.Top, barcode.Right, barcode.Bottom);
            BoundingRectangle = (Rectangle)barcode.BoundingRectangle;
            Length = barcode.BarcodeData.Length;
            Data = barcode.BarcodeData;
        }

        public BarcodeInfo(SerializationInfo info, StreamingContext context)
        {
            String = info.GetString("String");
            InfoString = info.GetString("InfoString");
            Rectangle = (Rectangle)info.GetValue("Rectangle", typeof(Rectangle));
            BoundingRectangle = (Rectangle)info.GetValue("BoundingRectangle", typeof(Rectangle));
            Length = info.GetInt32("Length");
            Data = (byte[])info.GetValue("Data", typeof(byte[]));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("String", String, typeof(string));
            info.AddValue("InfoString", InfoString, typeof(string));
            info.AddValue("Rectangle", Rectangle, typeof(Rectangle));
            info.AddValue("BoundingRectangle", BoundingRectangle, typeof(Rectangle));
            info.AddValue("Length", Length);
            info.AddValue("Data", Data, typeof(byte[]));
        }
    }
}
