using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace VCover
{
    public static class EmguCVEx
    {
        public static void Pixellate(this Image<Bgr, byte> bmp, Rectangle rectangle, int pixelSize = 5)
        {
            // look at every pixel in the rectangle while making sure we're within the image bounds
            for (int xx = rectangle.X; xx < rectangle.X + rectangle.Width && xx < bmp.Width; xx += pixelSize)
            {
                for (int yy = rectangle.Y; yy < rectangle.Y + rectangle.Height && yy < bmp.Height; yy += pixelSize)
                {
                    int offsetX = pixelSize / 2;
                    int offsetY = pixelSize / 2;

                    // make sure that the offset is within the boundry of the image
                    while (xx + offsetX >= bmp.Width)
                        offsetX--;

                    while (yy + offsetY >= bmp.Height)
                        offsetY--;

                    // get the pixel color in the center of the soon to be pixelated area
                    var pixel = bmp[yy + offsetY, xx + offsetX];

                    // for each pixel in the pixelate size, set it to the center color
                    for (int x = xx; x < xx + pixelSize && x < bmp.Width; x++)
                        for (int y = yy; y < yy + pixelSize && y < bmp.Height; y++)
                            bmp[y, x] = pixel;
                }
            }
        }
    }
}
