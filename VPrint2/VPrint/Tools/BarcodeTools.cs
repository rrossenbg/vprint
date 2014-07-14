/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System.Diagnostics;
using System.Drawing;
using Zen.Barcode;

namespace VPrinting.Tools
{
    public class BarcodeTools
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="voucherNumber">82620159918434718610</param>
        /// <param name="height">40</param>
        /// <param name="size">2</param>
        /// <returns></returns>
        public static Bitmap BinaryWritePicture(string voucherNumber, int height, int size, BarcodeSymbology type = BarcodeSymbology.Code25InterleavedNC)
        {
            Debug.Assert(!voucherNumber.IsNullOrEmpty());
            var barcode = BarcodeDrawFactory.GetSymbology(type);
            var image = barcode.Draw(voucherNumber, height, size);
            return (Bitmap)image;
        }
    }
}
