/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Drawing;

namespace PremierTaxFree.Data
{
    /// <summary>
    /// Saves current scan operation data
    /// </summary>
    public class StateObj
    {
        public IntPtr Dib { get; set; }
        public IntPtr Main { get; set; }
        public IntPtr Scan { get; set; }
    }
}
