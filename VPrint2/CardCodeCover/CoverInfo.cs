/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;

namespace CardCodeCover
{
    [Serializable]
    public class Cover
    {
        public double Distance { get; set; }
        public Size Offset { get; set; }
        public Rectangle Rectangle { get; set; }
    }

    [Serializable]
    public class CoverInfo
    {
        public Rectangle Match { get; set; }
        public List<Cover> HiddenAreas { get; set; }

        public bool HasMatch
        {
            get
            {
                return this.Match != Rectangle.Empty;
            }
        }

        public CoverInfo()
        {
            HiddenAreas = new List<Cover>();
        }

        public void Clear()
        {
            Match = Rectangle.Empty;
            HiddenAreas.Clear();
        }
    }
}
