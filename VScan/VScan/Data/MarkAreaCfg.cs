/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Drawing;

namespace PremierTaxFree.Data
{
    /// <summary>
    /// Mark to use to find hidden area
    /// </summary>
    [Serializable]
    public class MarkAreaCfg : IEquatable<MarkAreaCfg>, ICloneable
    {
        public Size MinAreaSize { get; set; } //new Size(20, 20)
        public Size MaxAreaSize { get; set; } //new Size(50, 50)
        public Color BackColor { get; set; } //Color.FromArgb(255, 255, 255, 255)
        public float Threshold { get; set; } //0.25f
        public int BlackWhiteThreshold { get; set; } //128

        public static MarkAreaCfg Empty = new MarkAreaCfg();

        public MarkAreaCfg()
        {
            MinAreaSize = new Size(20, 20);
            MaxAreaSize = new Size(50, 50);
            BackColor = Color.FromArgb(255, 255, 255, 255);
            Threshold = 0.25f;
            BlackWhiteThreshold = 128;
        }

        public bool Equals(MarkAreaCfg other)
        {
            return
                (MinAreaSize == other.MinAreaSize &&
                MaxAreaSize == other.MaxAreaSize &&
                BackColor == other.BackColor &&
                Threshold == other.Threshold &&
                BlackWhiteThreshold == other.BlackWhiteThreshold);
        }

        public object Clone()
        {
            return new MarkAreaCfg()
            {
                MinAreaSize = this.MinAreaSize,
                MaxAreaSize = this.MaxAreaSize,
                BackColor = this.BackColor,
                Threshold = this.Threshold,
                BlackWhiteThreshold = this.BlackWhiteThreshold,
            };
        }
    }
}
