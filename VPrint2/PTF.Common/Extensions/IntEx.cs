/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/
using System;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Reflection;
using System.Runtime;

namespace VPrinting
{
    [Obfuscation(StripAfterObfuscation = true, ApplyToMembers = true)]
    public static class IntEx
    {
        [TargetedPatchingOptOut("na")]
        public static bool IsValueInRange(this int value, int min, int max)
        {
            return (min <= value && value <= max);
        }

        [TargetedPatchingOptOut("na")]
        public static int NextInt(this Random r, int min, int max)
        {
            return r.Next(min, max);
        }

        [TargetedPatchingOptOut("na")]
        public static int SetValueInRange(this int value, int min, int max)
        {
            if (min <= value && value <= max)
                return value;
            else if (value > max)
                return max;
            else //if ( value < min)
                return min;
        }

        /// <summary>
        /// (value1 == value2) || (value1 == value2 * 10 + checkDigit2);
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="checkDigit2"></param>
        /// <returns></returns>
        [TargetedPatchingOptOut("na")]
        public static bool EqualsWithCheckDigit(this int value1, int value2, int checkDigit2)
        {
            return (value1 == value2) || (value1 == value2 * 10 + checkDigit2);
        }

        private static readonly int[] WEIGHTS = new int[] { 2, 3, 4, 5, 6, 7, 8, 9,
										             2, 3, 4, 5, 6, 7, 8, 9,
										             2, 3, 4, 5, 6, 7, 8, 9,
										             2, 3, 4, 5, 6, 7, 8, 9};

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Value + CheckDigit</returns>
        [TargetedPatchingOptOut("na")]
        public static int CheckDigit(this int value)
        {
            char[] base_val = value.ToString().ToCharArray();

            Array.Reverse(base_val);

            int i, sum;

            for (i = 0, sum = 0; i < base_val.Length; i++)
                sum += int.Parse(base_val[i].ToString()) * WEIGHTS[i];

            // Determine check digit.
            return (value * 10) + ((sum % 11) % 10);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns>CheckDigit</returns>
        [TargetedPatchingOptOut("na")]
        public static int CheckDigit(this string value)
        {
            Debug.Assert(!value.IsNullOrEmpty());

            char[] base_val = value.ToCharArray();

            Array.Reverse(base_val);

            long i, sum;

            for (i = 0, sum = 0; i < base_val.Length; i++)
            {
                int v = 0;

                bool result = int.TryParse(base_val[i].ToString(), out v);

                Debug.Assert(result, "Cannot parse value");

                sum += v * WEIGHTS[i];
            }

            // Determine check digit.
            return (int)((sum % 11) % 10);
        }

        [TargetedPatchingOptOut("na")]
        public static bool CompareSmart(this int value1, int value1cd, int value2, int value2cd)
        {
            return value1 == value2 || value1 == value2cd || value1cd == value2 || value1cd == value2cd;
        }

        [TargetedPatchingOptOut("na")]
        public static float FromInch(this int value)
        {
            return Convert.ToSingle(PrinterUnitConvert.Convert(value, PrinterUnit.ThousandthsOfAnInch, PrinterUnit.Display) * 1000);
        }

        [TargetedPatchingOptOut("na")]
        public static float FromInch(this float value)
        {
            return Convert.ToSingle(PrinterUnitConvert.Convert(value, PrinterUnit.ThousandthsOfAnInch, PrinterUnit.Display) * 1000);
        }

        [TargetedPatchingOptOut("na")]
        public static float FromMm(this float value)
        {
            return Convert.ToSingle(PrinterUnitConvert.Convert(value, PrinterUnit.HundredthsOfAMillimeter, PrinterUnit.Display) * 100);
        }  
    }
}
