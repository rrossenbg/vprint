/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using PTF.Reports.Common;

namespace PTF.Reports
{
    public static class StringEx
    {
        public static string Format(this string template, params object[] values)
        {
            return string.Format(template, values);
        }

        public static T Index<T>(this IList<T> items, int index)
        {
            Debug.Assert(index >= 0);

            if (items.Count > index)
                return items[index];
            return default(T);
        }

        public static T Cast<T>(this string value)
        {
            Type t = typeof(T);
            TypeConverter conv = TypeDescriptor.GetConverter(t);
            return (T)conv.ConvertFromInvariantString(value);
        }

        public static string TrimSafe(this string value, params char[] chars)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            return value.Trim(chars);
        }

        public static string Concat2(this string str, params object[] values)
        {
            StringBuilder b = new StringBuilder(str);
            foreach (var o in values)
                b.Append(o);
            return b.ToString();
        }

        public static bool CompareNoCase(this string value1, string value2)
        {
            return string.Equals(value1, value2, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string SubString(this String str, int length)
        {
            if (!string.IsNullOrEmpty(str) && str.Length < length)
                return str;
            return str.Substring(0, length);
        }

        public static string SubString(this StringBuilder b, int length)
        {
            if (b.Length < length)
                return b.ToString();
            return b.ToString().Substring(0, length);
        }

        public static char Last(this StringBuilder b)
        {
            if (b.Length == 0)
                throw new IndexOutOfRangeException();
            return b[b.Length - 1];
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="b"></param>
        /// <param name="value"></param>
        /// <param name="values"></param>
        public static void AppendIfNNE(this StringBuilder b, string value, params string[] values)
        {
            if (!string.IsNullOrEmpty(value))
            {
                b.Append(value);
                b.Append(values.Length == 0 ? "" : values[1]);
            }
        }

        /// <summary>
        /// AddressMax100.AppendLineIfNNE(HeadOffice.OfficeAddress.Line1 + ",", ",");
        /// </summary>
        /// <param name="b"></param>
        /// <param name="value"></param>
        /// <param name="values"></param>
        public static void AppendLineIfNNE(this StringBuilder b, string value, params string[] values)
        {
            if (!string.IsNullOrEmpty(value))
            {
                b.Append(value);
                b.AppendLine(values.Length == 0 ? "" : values[1]);
            }
        }

        /// <summary>
        /// Modifies the input string depending of the content string.
        /// 'The quick brown fox jumps over the lazy dog.' + 'This is demo!' = 'This is demo!this is demo!this is demo!This'
        /// </summary>
        /// <param name="basestr"></param>
        /// <param name="inherstr"></param>
        /// <returns></returns>
        public static string ToContentString(this string basestr, string contentstr)
        {
            Debug.Assert(!string.IsNullOrEmpty(contentstr));

            if (string.IsNullOrEmpty(basestr))
                return basestr;

            StringBuilder b = new StringBuilder();

            for (int i = 0, j = 0; i < basestr.Length; i++, j++)
            {
                if (j == contentstr.Length)
                    j = 0;

                if (Char.IsLower(basestr, i))
                {
                    b.Append(contentstr[j].ToString().ToLower());
                }
                else if (Char.IsUpper(basestr, i))
                {
                    b.Append(contentstr[j].ToString().ToUpper());
                }
                else
                {
                    b.Append(contentstr[j]);
                }
            }

            return b.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxChars">Max chars</param>
        /// <param name="addition">String or null</param>
        /// <returns></returns>
        public static string Limit(this string text, int maxChars, string addition)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            if (text.Length < maxChars)
                return text;

            string[] words = text.Split(' ');
            StringBuilder b = new StringBuilder();

            b.Append(words[0]);

            for (int i = 1; i < words.Length && b.Length < maxChars; i++)
            {
                b.Append(' ');
                b.Append(words[i]);
            }

            b.Append(addition);

            return b.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        /// <example>
        /// "Range: bytes=143642-";
        /// </example>
        public static bool TryParse(this string value, out int result)
        {
            int i = 0, from = -1, to = value.Length;

            for (; i < value.Length; i++)
            {
                if (from == -1)
                {
                    if (Char.IsDigit(value[i]))
                        from = i;
                }
                else
                {
                    if (!Char.IsDigit(value[i]))
                    {
                        to = i;
                        break;
                    }
                }
            }

            string str = (from == -1) ? null : value.Substring(from, to - from);
            return int.TryParse(str, out result);
        }

        public static MatchCollection GetLinks(this string page)
        {
            const string pattern = @"((https?|ftp|gopher|telnet|file|notes|ms-help):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)";

            Regex re = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            var matches = re.Matches(page);
            return matches;
        }

        public static MatchCollection GetHrefLinks(this string htmlPage)
        {
            const string pattern = "(?<=href=(\"|\'))(?<href>[^\"\']+)";
            Regex re = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
            MatchCollection mas = re.Matches(htmlPage);
            return mas;
        }

        public static MatchCollection GetSrcLinks(this string htmlPage)
        {
            const string pattern = "(?<=src=(\"|\'))(?<img>[^\"\']+)";
            Regex re = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
            MatchCollection mas = re.Matches(htmlPage);
            return mas;
        }

        public static string PathCombine(this string path, string relative)
        {
            try
            {
                Uri uri = new Uri(new Uri(path), relative);
                return uri.AbsoluteUri;
            }
            catch
            {
                return path;
            }
        }

        public static bool IsNullEmptyOrWhite(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static string Encript(this string value, string key = null, string vector = null)
        {
            var cripto = new RijndaelCryptography();
            if (key != null && vector != null)
            {
                cripto.Key = ASCIIEncoding.Default.GetBytes(key);
                cripto.IV = ASCIIEncoding.Default.GetBytes(vector);
            }
            var buffer = cripto.Encrypt(value);
            var result = Convert.ToBase64String(buffer);
            return result.TrimSafe('\0');
        }

        public static string Decrypt(this string value, string key = null, string vector = null)
        {
            var buffer = Convert.FromBase64String(value);
            var cripto = new RijndaelCryptography();
            if (key != null && vector != null)
            {
                cripto.Key = ASCIIEncoding.Default.GetBytes(key);
                cripto.IV = ASCIIEncoding.Default.GetBytes(vector);
            }
            var result = cripto.Decrypt(buffer);
            return result.TrimSafe('\0');
        }

        public static List<T> Split<T>(this string value, char splitter)
        {
            List<T> list = new List<T>();
            var values = value.Split(new char[] { splitter }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string str in values)
            {
                T t = (T)Convert.ChangeType(str, typeof(T));
                list.Add(t);
            }
            return list;
        }

        public static List<T> Split<T, U>(this string value, char splitter, Func<string, T> tFunct, Func<string, U> uFunct, Func<U, T> uToTconvFunct)
        {
            List<T> list = new List<T>();
            var values = value.Split(new char[] { splitter }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string str in values)
            {
                string s = str.TrimSafe();
                try
                {
                    T t = tFunct(s);
                    list.Add(t);
                }
                catch
                {
                    U u = uFunct(s);
                    T t = uToTconvFunct(u);
                    list.Add(t);
                }
            }
            return list;
        }

        public static T Parse<T>(this string value, T @default)
        {
            if (string.IsNullOrWhiteSpace(value))
                return @default;
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static T ToEnum<T>(this string value, T @default)
        {
            Debug.Assert(typeof(T).IsEnum);

            if (string.IsNullOrWhiteSpace(value))
                return @default;
            return (T) Enum.Parse(typeof(T), value);
        }
    }
}
