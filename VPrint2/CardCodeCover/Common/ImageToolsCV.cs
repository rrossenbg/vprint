using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace CardCodeCover.Common
{
    public static class ImageToolsCV
    {
        public static bool MatchTemplate(Image<Bgr, byte> source, Image<Bgr, byte> template, ref Rectangle match, float threshold = 0.65f)
        {
//#if DEBUG
//            Image<Bgr, byte> imageToShow = source.Copy();
//#endif

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
                    match = new Rectangle(maxLocations[0], template.Size);
//#if DEBUG
//                    imageToShow.Draw(match, new Bgr(Color.Red), 3);
//#endif
                    return true;
                }
                return false;
            }
        }
    }
}
