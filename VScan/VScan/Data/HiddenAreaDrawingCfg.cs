/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PremierTaxFree.Data
{
    /// <summary>
    /// How the drawing tools are configured to hide hidden area
    /// </summary>
    [Serializable]
    public class HiddenAreaDrawingCfg : ICloneable
    {
        public Color ForeColor { get; set; }
        public Color BackColor { get; set; }
        public HatchStyle Style { get; set; }

        public static HiddenAreaDrawingCfg Default = new HiddenAreaDrawingCfg();

        public HiddenAreaDrawingCfg()
        {
            ForeColor = Color.Red;
            BackColor = Color.Yellow;
            Style = HatchStyle.Cross;
        }

        public object Clone()
        {
            return new HiddenAreaDrawingCfg()
            {
                BackColor = this.BackColor,
                ForeColor = this.ForeColor,
                Style = this.Style
            };
        }
    }
}
