/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace PTF.Reports
{
    public static class StringEx
    {
        public static string GetTextInQuotes(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var key = Regex.Matches(text, "(?!')[^']+", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
            return key.Count < 2 ? "" : key[1].Value;
        }

        public static string Repeat(this string str, int count)
        {
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < count; i++)
                b.Append(str);
            return b.ToString();
        }

        public static string DefaultIfEmpty(this string value, string @default)
        {
            if (string.IsNullOrWhiteSpace(value))
                return @default;
            return value;
        }


        /// <summary>
        /// String.Format("--{0,10}--", "test");            --      test--
        /// String.Format("--{0,-10}--", "test");           --test      --
        /// </summary>
        /// <example>
        /// var result = "".Format(new[] { "AA", "BBBB", "CCC" }, new[] { -10, 20, -12 }).Replace(" ", "&nbsp;");
        /// </example>
        /// <param name="value"></param>
        /// <param name="values"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Format(this string value, string[] values, int[] length)
        {
            Debug.Assert(values != null && length != null);
            Debug.Assert(values.Length == length.Length);

            StringBuilder b = new StringBuilder(value);

            for (int i = 0; i < values.Length; i++)
                b.AppendFormat(string.Concat("{0,-", length[i], "}"), values[i].Limit(length[i], ""));

            return b.ToString();
        }
    }
}