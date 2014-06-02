/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

namespace VPrinting
{
    public static class ASCII
    {
        public static readonly string NUL = ((char)0).ToString();
        public static readonly string BS = ((char)8).ToString();
        public static readonly string HT = ((char)9).ToString();
        public static readonly string LF = ((char)10).ToString();
        public static readonly string VT = ((char)11).ToString();
        public static readonly string FF = ((char)12).ToString();
        public static readonly string CR = ((char)13).ToString();        
        public static readonly string ESC = ((char)27).ToString();

        public static bool TryParse(string value, out string result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(value))
                return false;

            switch (value.ToUpperInvariant())
            {
                case "NUL":
                    result = NUL;
                    return true;
                case "BS":
                    result = BS;
                    return true;
                case "HT":
                    result = HT;
                    return true;
                case "LF":
                    result = LF;
                    return true;
                case "VT":
                    result = VT;
                    return true;
                case "FF":
                    result = FF;
                    return true;
                case "CR":
                    result = CR;
                    return true;
                case "ESC":
                    result = ESC;
                    return true;
                default:
                    return false;
            }
        }
    }
}
