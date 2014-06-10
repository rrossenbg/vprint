using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CPrint2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CPrintTest.Scanning
{
    [TestClass]
    public class TiffTest
    {
        [TestMethod]
        public void test_filtering_image()
        {
            string[] files = Directory.GetFiles(@"C:\Users\Rosen.rusev\Pictures\Presenter", "*.jpg");

            foreach (var file in files)
            {
                var img = (Bitmap)Image.FromFile(file);
                if (img != null)
                {
                    var bmp3 = img.ToGrayscale4bpp();
                    bmp3.Save(Path.ChangeExtension(file, ".jpg2"), ImageFormat.Jpeg);
                    Debug.WriteLine(bmp3);
                }
            }
        }

        [TestMethod]
        public void test_find_mark_and_pixellate_image()
        {
            using (var bmp = ((Bitmap)Image.FromFile(@"C:\Users\Rosen.rusev\Pictures\Presenter\2014-06-02_0001.jpg")))
            {
                var marks = bmp.FindMarks(new Size(10, 10), new Size(100, 100)).Where(m => m.Item1 != DetectedShape.circle).ToList().ConvertAll(t => t.Item2);

                using (var g = Graphics.FromImage(bmp))
                {
                    foreach (var p in marks)
                    {
                        g.FillPolygon(Brushes.Red, p, System.Drawing.Drawing2D.FillMode.Alternate);
                    }
                }

                if (marks.Count == 2)
                {
                    var start = marks[0][0];
                    var end = marks[1][0];

                    var r = new Rectangle(
                        Math.Min(start.X, end.X),
                        Math.Min(start.Y, end.Y),
                        Math.Abs(start.X - end.X),
                        Math.Abs(start.Y - end.Y));

                    bmp.Pixellate(r);
                }
            }
        }

        [TestMethod]
        public void test_image_remove_back_border()
        {
            using (var bmp = ((Bitmap)Image.FromFile(@"C:\Users\Rosen.rusev\Pictures\Presenter\2014-06-02_0001.jpg")))
            {

            }
        }

        [TestMethod]
        public void Test_Find_Button()
        {
            PresenterCameraShooter p = new PresenterCameraShooter();
            //p.TryStartPresenter("");
            p.ClickCameraButton();
        }

        [TestMethod]
        public void Test_Click_Presenter()
        {
            var f = new Form();
            f.Load += new EventHandler(f_Load);
            Application.Run(f);
        }

        void f_Load(object sender, EventArgs e)
        {

        }

        [TestMethod]
        public void TestMSTiff()
        {
            string PATH = @"C:\Users\Rosen.rusev\Pictures\Presenter\";

            string[] files = Directory.GetFiles(PATH, "*.jpg");

            var list = new List<byte[]>();

            foreach (string path in files)
                list.Add(Image.FromFile(path).ToArray());

            File.WriteAllBytes(PATH + "result.tif", TiffConverter.WrapJpegs(list));
            //string[] filenames = Tiff.ConvertJpegToTiff(}, true);
        }

        [TestMethod]
        public void test_create_tiff1()
        {
            string[] files = Directory.GetFiles(@"C:\Users\Rosen.rusev\Pictures\Presenter", "*.jpg");

            var list = new List<Bitmap>();
            foreach (var f in files)
            {
                
                var img = ((Bitmap)Image.FromFile(f)).ToGrayscale4bpp();
                if (img != null)
                {
                    img.Save(f, ImageFormat.Jpeg);
                    list.Add(img);
                }
            }

            list.SaveMultipage(@"C:\Users\Rosen.rusev\Pictures\Presenter\test_2014_05_14.tif", "TIFF");
        }


        /// <summary>
        /// READ
        /// </summary>
        /// <see cref="http://tech.pro/tutorial/620/csharp-tutorial-image-editing-saving-cropping-and-resizing"/>
        [TestMethod]
        public void test_create_crop_rotate()
        {
            //string[] files = Directory.GetFiles(@"C:\Users\Rosen.rusev\Pictures\Presenter\New folder (7)");
            string[] files = Directory.GetFiles(@"C:\Users\Rosen.rusev\Pictures\Presenter", "*.jpg");
                //Directory.GetFiles(@"C:\Users\Rosen.rusev\Pictures\Presenter\New folder (2)\2014-05-07_0003.jpg", "*.jpg");

            int count = 0;
            foreach (var f in files)
            {
                var img = ((Bitmap)Image.FromFile(f)).CropRotateFree(new Size(1500, 500), new Size(2500, 1000));
                if (img != null)
                {
                    img.Save(@"C:\Users\Rosen.rusev\Pictures\Presenter\" + count++ + ".jpg", ImageFormat.Jpeg);
                    Debug.WriteLine(f);
                };
            }
        }


        /// <summary>
        /// READ
        /// </summary>
        /// <see cref="http://tech.pro/tutorial/620/csharp-tutorial-image-editing-saving-cropping-and-resizing"/>
        [TestMethod]
        public void test_create_crop_rotate2()
        {
            //string[] files = Directory.GetFiles(@"C:\Users\Rosen.rusev\Pictures\Presenter\New folder (7)");
            string[] files = Directory.GetFiles(@"C:\Users\Rosen.rusev\Pictures\Presenter", "*.jpg");
            //Directory.GetFiles(@"C:\Users\Rosen.rusev\Pictures\Presenter\New folder (2)\2014-05-07_0003.jpg", "*.jpg");

            int count = 0;
            foreach (var f in files)
            {
                var img = ((Bitmap)Image.FromFile(f)).CropRotateFree(new Size(500, 200), new Size(2500, 1000));
                if (img != null)
                {
                    img.Save(@"C:\Users\Rosen.rusev\Pictures\Presenter\" + count++ + ".jpg", ImageFormat.Jpeg);
                    Debug.WriteLine(f);
                };
            }
        }
    }
}
