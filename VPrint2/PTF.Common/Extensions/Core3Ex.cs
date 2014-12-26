/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace VPrinting
{
    [Obfuscation(StripAfterObfuscation = true)]
    public static class Core3Ex
    {
        /// <summary>
        /// 250
        /// </summary>
        private const int MAX_FILE_LENGTH = 250;

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string join(this string format, params object[] data)
        {
            return string.Join("", data);
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string IfNullOrEmptyThrow<T>(this string value) where T : Exception, new()
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new T();
            return value;
        }

        ///// <summary>
        ///// return (T)value;
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //[TargetedPatchingOptOut("na")]
        //[Obfuscation]
        //public static T cast<T>(this object value)
        //{
        //    return (T)value;
        //}

        ///// <summary>
        ///// return (T)Convert.ChangeType(value, typeof(T));
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //[TargetedPatchingOptOut("na")]
        //[Obfuscation]
        //public static T Cast<T>(this object value) where T : IConvertible
        //{
        //    return (T)Convert.ChangeType(value, typeof(T));
        //}

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string concat(this string str, params object[] values)
        {
            var b = new StringBuilder(str);
            foreach (var o in values)
                b.Append(o);
            return b.ToString();
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static T2 ConvertTo<T1, T2>(this T1 value)
            where T1 : IConvertible
            where T2 : IConvertible
        {
            return (T2)Convert.ChangeType(value, typeof(T2), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="Exception">If name != null && value == null</exception>
        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static T2 ConvertToThrow<T1, T2>(this T1 value, string name = "")
            where T1 : IConvertible
            where T2 : IConvertible
        {
            try
            {
                return (T2)Convert.ChangeType(value, typeof(T2), CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new Exception(name.join(" can not be converted to ", typeof(T2)), ex);
            }
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static XElement ElementThrow(this XContainer container, string name)
        {
            var node = container.Element(name);
            if (node == null)
                throw new Exception("Can not find " + name);
            return node;
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string ElementValueOrDefault(this XContainer container, string name, string @default)
        {
            var node = container.Element(name);
            if (node == null)
                return @default;
            return node.Value;
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static bool EqualsNoCase(this string value1, string value2)
        {
            return string.Equals(value1, value2, StringComparison.InvariantCultureIgnoreCase);
        }

        #region SECURITY
        [Obfuscation(StripAfterObfuscation = true)]
        private static readonly byte[] IVString = Encoding.ASCII.GetBytes("ryojvlzmdalyglrj");
        [Obfuscation(StripAfterObfuscation = true)]
        private static readonly byte[] keyString = Encoding.ASCII.GetBytes("hcxilkqbbhczfeultgbskdmaunivmfuo");
        #endregion

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string EncryptString(this string clearText)
        {
            if (string.IsNullOrWhiteSpace(clearText))
                throw new ArgumentNullException("clearText");

            byte[] clearTextBytes = Encoding.UTF8.GetBytes(clearText);

            using (var rijn = SymmetricAlgorithm.Create())
            using (var memory = new MemoryStream())
            {
                using (var cs = new CryptoStream(memory, rijn.CreateEncryptor(keyString, IVString), CryptoStreamMode.Write))
                {
                    cs.Write(clearTextBytes, 0, clearTextBytes.Length);
                    cs.Close();
                }
                return Convert.ToBase64String(memory.ToArray());
            }
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string DecryptString(this string encryptedText)
        {
            if (string.IsNullOrWhiteSpace(encryptedText))
                throw new ArgumentNullException("encryptedText");

            byte[] encryptedTextBytes = Convert.FromBase64String(encryptedText);

            using (var rijn = SymmetricAlgorithm.Create())
            using (var memory = new MemoryStream())
            {
                using (var cs = new CryptoStream(memory, rijn.CreateDecryptor(keyString, IVString), CryptoStreamMode.Write))
                {
                    cs.Write(encryptedTextBytes, 0, encryptedTextBytes.Length);
                    cs.Close();
                }

                return Encoding.UTF8.GetString(memory.ToArray());
            }
        }


        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int memcmp(byte[] b1, byte[] b2, long count);

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static bool Compare(this byte[] b1, byte[] b2)
        {
            if (b1 == null)
                throw new ArgumentNullException("b1");
            if (b2 == null)
                throw new ArgumentNullException("b2");

            return b1.Length == b2.Length && memcmp(b1, b2, b1.Length) == 0;
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static bool Contains(this string value, Predicate<char> funct)
        {
            if (value == null)
                return false;

            foreach (char c in value)
                if (funct(c))
                    return true;

            return false;
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string ToString<T>(this List<T> list, Func<T, string> funct, char splitter)
        {
            Debug.Assert(list != null);
            Debug.Assert(funct != null);

            var b = new StringBuilder();

            for (int i = 0; i < list.Count; i++)
            {
                b.Append(funct(list[i]));
                if (i < list.Count - 1)
                    b.Append(splitter);
            }

            return b.ToString();
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static bool Random(this bool value)
        {
            return DateTime.Now.Second % 2 == 0;
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static T ToYesNo<T>(this bool value, T yes, T no)
        {
            return value ? yes : no;
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static int Random(this int value, int min = 0, int max = int.MaxValue)
        {
            System.Random rnd = new Random(DateTime.Now.Millisecond);
            return rnd.Next(min, max);
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static DateTime At(this DateTime date, TimeSpan time)
        {
            var todayAt = DateTime.Now.Date.Add(time);
            if (todayAt > DateTime.Now)
                return todayAt;
            var tomorrowAt = DateTime.Now.Date.AddDays(1).Add(time);
            return tomorrowAt;
        }

        //[TargetedPatchingOptOut("na")]
        //public static void RunSafe(this Action handle)
        //{
        //    try
        //    {
        //        handle();
        //    }
        //    catch
        //    {
        //    }
        //}

        //[TargetedPatchingOptOut("na")]
        //public static void RunSafe<T>(this Action<T> handle, T t)
        //{
        //    try
        //    {
        //        handle(t);
        //    }
        //    catch
        //    {
        //    }
        //}
    }
}
