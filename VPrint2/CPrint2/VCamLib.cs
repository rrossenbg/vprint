using System.Drawing;
using System.Runtime.InteropServices;
using Emgu.CV.Structure;

namespace CPrint2
{
    public static class VCamLib
    {
        [DllImport(@"\x86\VCamLib.dll",
            CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall,
            SetLastError = true)]
        public static extern void ReadSettings(ref int highS, ref int minWidth, ref int minHeight);

        [DllImport(@"\x86\VCamLib.dll",
            CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall,
            SetLastError = true)]
        public static extern void SaveSettings(int highS, int minWidth, int minHeight);

        [DllImport(@"\x86\VCamLib.dll",
           CharSet = CharSet.Unicode,
           CallingConvention = CallingConvention.StdCall,
           SetLastError = true)]
        public static unsafe extern void ProcessImage(MIplImage image, MIplImage thresholded, MIplImage hsv, ref Rectangle rect);
    }
}