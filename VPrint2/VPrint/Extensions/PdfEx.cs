using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;

namespace VPrinting
{
    public static class PdfEx
    {
        private class oLock { }

        private static string sm_DLLPath;

        static PdfEx()
        {
            sm_DLLPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "gsdll32.dll");
            Debug.Assert(File.Exists(sm_DLLPath));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pdfFile"></param>
        /// <param name="pngDir"></param>
        /// <returns></returns>
        /// <example>
        /// @"C:\IMAGES\PORTUGAL\New folder\OA_24993.pdf".DrawToImage("C:\\TEST");
        /// </example>
        public static List<FileInfo> DrawToImage(this FileInfo pdfFile)
        {
            // @"C:\PROJECTS\VPrint2\Others\gsdll32.dll";
            lock (typeof(oLock))
            {
                GhostscriptVersionInfo gvi = new GhostscriptVersionInfo(sm_DLLPath);
                using (GhostscriptRasterizer rasterizer = new GhostscriptRasterizer())
                {
                    rasterizer.Open(pdfFile.FullName, gvi, false);

                    int dpi_x = 96;
                    int dpi_y = 96;

                    var files = new List<FileInfo>();

                    var pngDir = pdfFile.Directory;

                    for (int i = 1; i <= rasterizer.PageCount; i++)
                    {
                        var path = pngDir.CombineFileName(string.Concat(pdfFile.GetFileNameWithoutExtension(), '.', i, ".jpg"));
                        files.Add(path);
                        Global.IgnoreList.Add(path.FullName);

                        using (Image img = rasterizer.GetPage(dpi_x, dpi_y, i))
                            img.Save(path.FullName, ImageFormat.Jpeg);
                    }

                    return files;
                }
            }
        }
    }
}
