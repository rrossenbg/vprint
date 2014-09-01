/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using CardCodeCover;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using VPrinting;

namespace ReceivingServiceLib.Drawing
{
    public class ImageToolsCV
    {
        public bool MatchTemplate(byte[] sourceArr, Bitmap templateBmp, string hiddenAreas, ref string areasStr, float threshold = 0.65f)
        {
            using (var mem1 = new MemoryStream(sourceArr))
            using (var bmp1 = new Bitmap(mem1))
            using (Image<Bgr, byte> source = new Image<Bgr, byte>(bmp1))
            using (Image<Bgr, byte> template = new Image<Bgr, byte>(templateBmp))
            using (Image<Gray, float> result = source.MatchTemplate(template, TM_TYPE.CV_TM_CCOEFF_NORMED))
            {
                double[] minValues, maxValues;
                Point[] minLocations, maxLocations;

                result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

                // You can try different values of the threshold. 
                // I guess somewhere between 0.75 and 0.95 would be good.
                if (maxValues[0] > threshold)
                {
                    // This is a match. Do something with it, for example draw a rectangle around it.
                    var match = new Rectangle(maxLocations[0], template.Size);

                    var areas = new List<Rectangle>();
                    var cinfo = hiddenAreas.ToObject<CoverInfo>();
                    foreach (var area in cinfo.HiddenAreas)
                    {
                        var location = match.Location;
                        location.Offset(area.Offset.Width, area.Offset.Height);
                        var rect = new Rectangle(location, area.Rectangle.Size);
                        areas.Add(rect);
                    }
                    areasStr = areas.FromObjectXml();
                    return true;
                }
                return false;
            }
        }
    }
}
