using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using VPrinting.Common;

namespace VPrinting
{
    static class Test
    {
        [STAThread]
        static void Main()
        {
            StateSaver.Default.Path = Path.ChangeExtension(Application.ExecutablePath, "dat");
            StateSaver.Default.Load();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var form = new SetupForm();
            form.Selection = StateSaver.Default.Get("Selection", Rectangle.Empty);
            form.Img = Image.FromFile(@"C:\Users\Rosen.rusev\Desktop\Images\img-130628084640-0001.jpg");
            StateSaver.Default.Set("Selection", form.Selection);
            Application.Run(form);
            StateSaver.Default.Save();
        }
    }

    static class Test2
    {
        [STAThread]
        static void Main()
        {
            double[,] matrix = new double[,]
            { 
                {1, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 1, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 1, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 1, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 1, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 1, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 1, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 1, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 1},
            };

            double factor = 1.0 / 9.0;

            //Edge detect

            double[,] matrix2 = new double[,]
            {
                    {1,  1,  1},
                    {1, -7,  1},
                    {1,  1,  1}
            };

            double factor2 = 1.0;

            //Emboss

            double[,] matrix3 = new double[,]
            {
                {-1, -1,  0},
                {-1,  0,  1},
                {0,  1,  1}
            };

            double factor3 = 1.0;
            int bias3 = 128;

            //

            double[,] matrix4 = new double[,]
            {
                {1, 2, 	1},
                {2, 4, 	2},
                {1, 2, 	1}
            };

            double factor4 = 1.0 / 9.0;
            int bias4 = 0;

            Bitmap bmp = new Bitmap(@"C:\Users\Rosen.rusev\Desktop\Images\filtertest.jpg");
            bmp.ConvolutionFilter(matrix4, factor4, bias4);
            bmp.Save(@"C:\Users\Rosen.rusev\Desktop\Images\result.jpg", ImageFormat.Jpeg);
        }
    }

    static class Test3
    {
        [STAThread]
        static void Main()
        {
            Bitmap bmp = new Bitmap(@"C:\Users\Rosen.rusev\Desktop\Images\img-130628084640-0001.jpg");
            bmp.Pixellate(new Rectangle(300, 300, 300, 300), 8);
            bmp.Save(@"C:\Users\Rosen.rusev\Desktop\Images\result.jpg", ImageFormat.Jpeg);
        }
    }

    static class Test4
    {
        [STAThread]
        static void Main()
        {
            var form = new Form();
            form.Load += new EventHandler(form_Load);
            Application.Run(form);
        }

        static void form_Load(object sender, EventArgs e)
        {
            FileMsgForm.show(sender as Form, "Test", "Test");
        }
    }
}
