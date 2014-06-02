/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System.Diagnostics;

namespace VPrinting.Tools
{
    public static class ESC_P2
    {
        public static readonly string HT = ((char)9).ToString();
        public static readonly string LF = ((char)10).ToString();
        public static readonly string VT = ((char)11).ToString();
        public static readonly string FF = ((char)12).ToString();
        public static readonly string ESC = ((char)27).ToString();

        public static string SetFormLength(int length)
        {
            return string.Format("{0}(C{1}", ESC, (char)length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lines">1-127</param>
        /// <returns></returns>
        public static string SetPageLines(int lines)
        {
            Debug.Assert(lines > 0 && lines < 128);
            return string.Format("{0}C{1}", ESC, (char)lines);
        }

        /// <summary>
        /// ESC 1 n
        /// </summary>
        /// <param name="x"></param>
        /// <returns>C-24</returns>
        public static string SetLeftMargin(int x)
        {
            return string.Format("{0}1{2}", ESC, x);
        }

        /// <summary>
        /// ESC $ nl nh
        /// </summary>
        /// <param name="x"></param>
        /// <returns>C-31</returns>
        public static string SetAbsoluteHorizontalPosition(int x)
        {
            return "";
        }
    }
}
