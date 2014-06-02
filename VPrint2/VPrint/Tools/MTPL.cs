/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Text;

namespace VPrinting
{
    #region ENUMS

    public enum SSU { Default = 48, Millimeters = 49, Decipoints = 50, Decidots = 51, Mils = 52, BMU = 53, Micrometers = 54, Pixels = 55 }

    public enum Cpi
    {
        NA = 0,
        /// <summary>
        /// Normal
        /// </summary>
        _10 = 1,
        /// <summary>
        /// Normal high resolution
        /// </summary>
        _10HR = 2,
        /// <summary>
        /// Small
        /// </summary>
        _12 = 3,
        /// <summary>
        /// Smaller
        /// </summary>
        _15 = 4,
        /// <summary>
        /// Smallest
        /// </summary>
        _17_1 = 5,
        /// <summary>
        /// Small small
        /// </summary>
        _20 = 6,
        /// <summary>
        /// High high
        /// </summary>
        _5 = 7,
        /// <summary>
        /// Highest
        /// </summary>
        _6 = 8,
        /// <summary>
        /// Higher
        /// </summary>
        _7_5 = 9,
        /// <summary>
        /// High
        /// </summary>
        _8_6 = 10,
    }

    //public enum FontStyle
    //{
    //    Draft = 0,
    //    NLQ_Courier = 1,
    //    LQ_Courier = 2,
    //    NLQ_Sans_Serif = 3,
    //    LQ_Sans_Serif = 4,
    //    LQ_Roman = 5,
    //    LQ_Script = 6,
    //    LQ_Prestige = 7,
    //    LQ_OCR_B = 8,
    //    LQ_OCR_A = 9,
    //}

    public enum FontType
    {
        Roman = 0,//0 30 
        SansSerif = 1,//1 31 
        Courier = 2,//2 32  
        Prestige = 3,//3 33 
        Script = 4,//4 34  
        OCR_B = 5,//5 35 
        OCR_A = 6,//6 36 
        Modern = 7, //7 37
        Kaufmann = 8, //8 38 
        Gothic = 9, //9 39 
        Swiss = 10, //10 31 30 
        Quadrato = 11, //11 31 31 
        CourierIBM = 66 //66 36 36 
    }

    public enum FontDraft
    {
        MulticopyDraft = 0,
        FastDraft = 1,
        NormalDraft = 2
    }

    #endregion

    public static class MTPL
    {
        /// <summary>
        /// End of header 0x10
        /// </summary>
        public static readonly string EM = ((char)25).ToString();
        /// <summary>
        /// Start of header 0x1A
        /// </summary>
        private static readonly string SUB = ((char)26).ToString();
        public static readonly string ESC = ((char)27).ToString();        
        /// <summary>
        /// Expanded print
        /// </summary>
        public static readonly string SI2 = string.Format("{0}{1}", (char)27, (char)14);
        /// <summary>
        /// Comressed print
        /// </summary>
        public static readonly string SI = string.Format("{0}{1}", (char)27, (char)15);
        /// <summary>
        /// Normal print. Set normal print back.
        /// </summary>
        public static readonly string DC2 = string.Format("{0}{1}", (char)27, (char)18);

        public static readonly string DC4 = string.Format("{0}{1}", (char)27, (char)0x20);

        public static readonly string HQMode = CSI + "0X";

        public static readonly string MQMode = CSI + "1X";

        public static readonly string DQMode = CSI + "2X";        
        
        /// <summary>
        /// 1B 5B = > Esc[
        /// </summary>
        private static readonly string CSI = string.Format("{0}{1}", (char)27, (char)91);

        public static string PUMOn(bool activate)
        {
            if (activate)
                return CSI + "11h";
            return CSI + "11i";
        }

        public static string SetHTabStopCurrent()
        {
            return ESC + (char)72;
        }

        /// <summary>
        /// clear all horizontal and vertical tab stops
        /// </summary>
        /// <returns></returns>
        public static string ClearAllTabStop()
        {
            return string.Concat(CSI, "3;4g");
        }

        public static string SetPositionalUnitMode(SSU units)
        {
            return string.Format("{0}{1}{2}", CSI, (char)(int)units, (char)73);
        }

        public static string SetAbsoluteHorizontalPosition(int PositionalUnits)
        {
            if (PositionalUnits < 0)
                return "";
            return string.Format("{0}{1}{2}", CSI, PositionalUnits, (char)96);
        }

        public static string SetRelativeHorizontalPosition(int PositionalUnits)
        {
            if (PositionalUnits <= 0)
                return "";
            return string.Format("{0}{1}{2}", CSI, PositionalUnits, (char)97);
        }

        public static string SetHorizontalPositionBackward(int PositionalUnits)
        {
            if (PositionalUnits <= 0)
                return "";
            return string.Format("{0}{1}{2}", CSI, PositionalUnits, (char)106);
        }

        public static string SetAbsoluteVerticalPosition(int PositionalUnits)
        {
            if (PositionalUnits < 0)
                return "";
            return string.Format("{0}{1}{2}", CSI, PositionalUnits, (char)100);
        }

        public static string SetRelativeVerticalPosition(int PositionalUnits)
        {
            if (PositionalUnits <= 0)
                return "";
            return string.Format("{0}{1}{2}", CSI, PositionalUnits, (char)101);
        }

        public static string SetVerticalPositionBackward(int PositionalUnits)
        {
            if (PositionalUnits <= 0)
                return "";
            return string.Format("{0}{1}{2}", CSI, PositionalUnits, (char)107);
        }

        public static string SetAbsolutePosition(int x, int y)
        {
            if (x < 0 || y < 0)
                return "";
            return string.Format("{0}{1};{2}f", CSI, y, x);
        }

        public static string SetFormLength(int PositionalUnits)
        {
            if (PositionalUnits < 0)
                return "";
            return string.Format("{0}{1}{2}", CSI, PositionalUnits, (char)116);
        }

        public static string SetFontDensity(Cpi size)
        {
            switch (size)
            {
                case Cpi.NA:
                    return string.Empty;
                case Cpi._10:
                    return CSI + "4w";
                case Cpi._10HR:
                    return CSI + "12w";
                case Cpi._12:
                    return CSI + "5w";
                case Cpi._15:
                    return CSI + "6w";
                case Cpi._17_1:
                    return CSI + "7w";
                case Cpi._20:
                    return CSI + "11w";
                case Cpi._5:
                    return CSI + "0w";
                case Cpi._6:
                    return CSI + "1w";
                case Cpi._7_5:
                    return CSI + "2w";
                case Cpi._8_6:
                    return CSI + "3w";
                default:
                    throw new NotImplementedException();
            }
        }

        public static string SetFontStyle()
        {
            //Set font 513 in register 7
            //Select register 7 for use
            return CSI + "7;513 D" + CSI + "17m";
        }

        public static string SetFontType(FontType type)
        {
            int value = (int)type;
            return NumToString(value.ToString());
        }

        public static string SetCompressed(bool on)
        {
            return on ? SI : DC2;
        }

        public static string SetExpanded(bool on)
        {
            return on ? SI2 : DC2;
        }

        //Format: SUB [F] a [n] [;xyz] [;p] EM
        //SUB (hex.1A, dec.26) Start header
        //F Print feature (see section "Barcode Print Feature F"
        //to select the F codes, page 10)
        //a ASCII a = "A"..."S" Barcode Types (see section "Barcode Types")
        //n ASCII n = "0"..."90" Barcode height in n/6 inch.
        //At n="0" the Barcode height equals to 1/12 inch.
        //; ASCII Separation character
        //x ASCII x = "0"..."3" Width of the narrow bar (see section "Barcode width")
        //y ASCII y = "0"..."3" Width of the narrow space (see section "Barcode width")
        //z ASCII z = "0"..."3" Ratio of wide to narrow (see section "Barcode width")
        //p ASCII p = "0"…"9" Barcode orientation
        //EM (hex.19, dec.25) End of header
        private static readonly string I2Of5BARCODETEMPLATE = 
                CSI + "?11~" + //SUB
                NumToString("1A21") + //[F]
                "C{1}" + // a [n] =>Barcode C - Interleved , Hight - 1
                ";{2}" + // [;xyz] => 000, XYZ - 001
                NumToString("1914") + ":{0};" + //Barcode value
                NumToString("14") + // [;p]
                CSI + "?10~"; //EM

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barNumber"></param>
        /// <param name="size"></param>
        /// <param name="xyz">000, 001, 333</param>
        /// <returns></returns>
        public static string PrintI2Of5Barcode(string barNumber, int size, string xyz)
        {
            //return Esc + "?11~" + HexToString("1A") + " A3;111" + ((char)25).ToString() + ((char)20).ToString() + ":" + barText + ":" + ((char)20).ToString() + ((char)27).ToString() + "[?10~";
            //return Esc + "?11~" + HexToString("1A20") + "A2" + ";" + "000" + HexToString("19") + HexToString("14") + ":" + barText + ":" + HexToString("14") + Esc + "?10~";
            return string.Format(I2Of5BARCODETEMPLATE, barNumber, size, xyz);
        }

        public static string PrintUSPSBarcode(string barText)
        {
            //return Esc + ((char)66).ToString() + ((char)32).ToString() + ((char)112).ToString() + ((char)49).ToString() + ((char)50).ToString() + ((char)51).ToString() + ((char)52).ToString() + ((char)13).ToString();
            return CSI + NumToString("31207031323334350D");
        }

        /// <summary>
        /// MTPL.HexToString("0A313233FF");
        /// MTPL.HexToString("0A 31 32 33 FF");
        /// MTPL.HexToString("10 11 12 13 14", 10);
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static string NumToString(string value, int fromBase = 16)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("value");

            var value2 = value.Replace(" ", "");

            if (value2.Length % 2 != 0)
                throw new ArgumentOutOfRangeException("value");

            StringBuilder b = new StringBuilder();

            for (int i = 0; i < value2.Length; i += 2)
            {
                var s = value2.Substring(i, 2);
                var c = (char)Convert.ToByte(s, fromBase);
                b.Append(c.ToString());
            }

            return b.ToString();
        }
    }
}
