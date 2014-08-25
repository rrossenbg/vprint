using System;
using System.Diagnostics;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CardCodeCoverTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void emgu_temp_matching_test_OK()
        {
            try
            {
                var filepathSource = @"C:\IMAGES\PB\PB742001.jpg";
                var filepathTemplate = @"C:\IMAGES\PB\B\PB742007_cover2.jpg";

                Image<Bgr, byte> source = new Image<Bgr, byte>(filepathSource);
                Image<Bgr, byte> template = new Image<Bgr, byte>(filepathTemplate);
                Image<Bgr, byte> imageToShow = source.Copy();

                Stopwatch w = Stopwatch.StartNew();

                using (Image<Gray, float> result = source.MatchTemplate(template, TM_TYPE.CV_TM_CCOEFF_NORMED))
                {
                    double[] minValues, maxValues;
                    Point[] minLocations, maxLocations;
                    result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

                    // You can try different values of the threshold. I guess somewhere between 0.75 and 0.95 would be good.
                    if (maxValues[0] > 0.65)
                    {
                        // This is a match. Do something with it, for example draw a rectangle around it.
                        Rectangle match = new Rectangle(maxLocations[0], template.Size);
                        imageToShow.Draw(match, new Bgr(Color.Red), 3);
                    }
                }

                Trace.WriteLine(w.Elapsed);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }
    }
}
