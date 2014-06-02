/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;

namespace VPrinting
{
    [Serializable]
    public class AllocationDocumentLayout
    {
        public Size PaperSize { get; set; }
        public List<IImageObject> MetaObjectsList { get; set; }
        public Image DocumentImage { get; set; }

        public AllocationDocumentLayout()
        {
            PaperSize = new Size(1654, 2339);//A4
            MetaObjectsList = new List<IImageObject>();
            DocumentImage = null;
        }
    }
}
