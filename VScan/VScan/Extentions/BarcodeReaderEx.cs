/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System.Drawing;
using DTKBarReader;
using PremierTaxFree.PTFLib.Data;
using PremierTaxFree.PTFLib;

namespace PremierTaxFree.Extensions
{
    public static class BarcodeReaderEx
    {
        /// <summary>
        /// Tries to read barcode into an image. Rotates the image untill it reads successfull 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="bmp"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Barcode[] ReadFromBitmapRotateAll(this BarcodeReader reader, ref Bitmap bmp, Voucher data)
        {
            Barcode[] results = reader.ReadFromBitmap(bmp);

            for (int count = 0; count < 4 && (results == null || results.Length == 0); count++)
            {
                bmp = bmp.RotateEx(90);
                results = reader.ReadFromBitmap(bmp);
            }

            if (results != null && results.Length > 0)
            {
                data.BarCodeArea = Rectangle.FromLTRB(results[0].Left, results[0].Top, results[0].Right, results[0].Bottom);
                data.BarCodeImage = bmp.CopyNoFree(data.BarCodeArea);
            }
            return results;
        }
    }
}
