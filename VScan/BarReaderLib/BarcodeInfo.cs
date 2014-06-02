using System;
using System.Drawing;
using System.Runtime.Serialization;
using DTKBarReader;
using System.Collections;

namespace BarReaderLib
{
    [Serializable]
    public class BarcodeInfo : ISerializable
    {
        public string String { get; set; }
        public Rectangle Rectangle { get; set; }
        public byte[] Data { get; set; }

        public BarcodeInfo(Barcode barcode)
        {
            String = barcode.BarcodeString;
            Rectangle = Rectangle.FromLTRB(barcode.Left, barcode.Top, barcode.Right, barcode.Bottom);
            Data = barcode.BarcodeData;
        }

        public BarcodeInfo(SerializationInfo info, StreamingContext context)
        {
            String = info.GetString("String");
            Rectangle = (Rectangle)info.GetValue("Rectangle", typeof(Rectangle));
            Data = (byte[])info.GetValue("Data", typeof(byte[]));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("String", String, typeof(string));
            info.AddValue("Rectangle", Rectangle, typeof(Rectangle));
            info.AddValue("Data", Data, typeof(byte[]));
        }
    }
}
