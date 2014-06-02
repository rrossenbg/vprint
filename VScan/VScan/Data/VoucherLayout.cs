/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Drawing;

namespace PremierTaxFree.Data
{
    /// <summary>
    /// Layout of the voucher
    /// </summary>
    [Serializable]
    public class VoucherLayout : ICloneable
    {
        public Image Background { get; set; }
        public Rectangle BarcodeArea { get; set; }
        public Rectangle CardcodeArea { get; set; }
        public Rectangle PrintArea { get; set; }

        public static VoucherLayout Default = new VoucherLayout();

        public object Clone()
        {
            return new VoucherLayout()
            {
                Background = new Bitmap(this.Background),
                BarcodeArea = this.BarcodeArea,
                CardcodeArea = this.CardcodeArea,
                PrintArea = this.PrintArea
            };
        }
    }
}
