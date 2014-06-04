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
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ReceivingServiceLib
{
    [Obfuscation(StripAfterObfuscation = true)]
    public static class CoreEx
    {
        /// <summary>
        /// 250
        /// </summary>
        private const int MAX_FILE_LENGTH = 250;

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string join(this string format, params object[] data)
        {
            Security.Check();
            return string.Join("", data);
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static bool IsNullOrWhiteSpace(this string value)
        {
            Security.Check();
            return string.IsNullOrWhiteSpace(value);
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string IfNullOrEmptyThrow<T>(this string value) where T : Exception, new()
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new T();
            return value;
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static void DeleteSafe(this FileInfo info)
        {
            try
            {
                if (info == null)
                    return;

                info.Refresh();
                if (info.Exists)
                    info.Delete();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex, "ISRV");
            }
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static void DeleteSafe(this DirectoryInfo info, bool recursive)
        {
            try
            {
                if (info == null)
                    return;

                info.Refresh();
                if (info.Exists)
                    info.Delete(recursive);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex, "ISRV");
            }
        }

        /// <summary>
        /// return (T)value;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static T cast<T>(this object value)
        {
            return (T)value;
        }

        /// <summary>
        /// return (T)Convert.ChangeType(value, typeof(T));
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static T Cast<T>(this object value) where T : IConvertible
        {
            Security.Check();
            return (T)Convert.ChangeType(value, typeof(T));
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string concat(this string str, params object[] values)
        {
            Security.Check();
            StringBuilder b = new StringBuilder(str);
            foreach (var o in values)
                b.Append(o);
            return b.ToString();
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
        public static U ConvertTo<T, U>(this T value, string name = null) where T : IConvertible
        {
            if (name != null && value == null)
                throw new Exception(name + " can not be null");
            try
            {
                return (U)Convert.ChangeType(value, typeof(U), CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new Exception(name.join(" can not be converted to ", typeof(U)), ex);
            }
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static XElement ElementThrow(this XContainer container, string name)
        {
            Security.Check();
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
        public static int CopyFiles(this DirectoryInfo from, DirectoryInfo to, bool @overwrite = true)
        {
            Debug.Assert(from != null);

            Security.Check();

            var files = from.GetFiles();

            foreach (var file in files)
                file.CopyTo(to.CombineFileName(file.Name).FullName, @overwrite);

            return files.Length;
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static bool EqualsNoCase(this string value1, string value2)
        {
            Security.Check();
            return string.Equals(value1, value2, StringComparison.InvariantCultureIgnoreCase);
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string Unique(this string str)
        {
            Security.Check();
            var u = Guid.NewGuid().ToString().Replace('-', '_');
            return string.Concat(str, u);
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string Limit(this string str, int length)
        {
            if (str == null)
                return null;
            return str.Substring(0, Math.Min(str.Length, length));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Any name</param>
        /// <returns></returns>
        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string ToUniqueFileName(this string name)
        {
            Security.Check();
            return Path.GetFileNameWithoutExtension(name).Unique().Limit(MAX_FILE_LENGTH).concat(Path.GetExtension(name));
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string FromObject<T>(this XmlSerializer serializer, T value)
        {
            Security.Check();
            var builder = new StringBuilder();
            using (var str = XmlWriter.Create(new StringWriter(builder)))
            {
                Debug.Assert(str != null);
                serializer.Serialize(str, value);
                return builder.ToString();
            }
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static T ToObject<T>(this string text)
        {
            Debug.Assert(!string.IsNullOrEmpty(text));
            Security.Check();
            XmlSerializer formatter = new XmlSerializer(typeof(T));
            using (var str = XmlReader.Create(new StringReader(text)))
            {
                Debug.Assert(str != null);
                return (T)formatter.Deserialize(str);
            }
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static T ToObject<T>(this XmlSerializer serializer, string text)
        {
            Security.Check();
            using (var str = XmlReader.Create(new StringReader(text)))
            {
                Debug.Assert(str != null);
                return (T)serializer.Deserialize(str);
            }
        }

        #region SECURITY
        [Obfuscation(StripAfterObfuscation = true)]
        private static readonly byte[] rgbIV = Encoding.ASCII.GetBytes("ryojvlzmdalyglrj");
        [Obfuscation(StripAfterObfuscation = true)]
        private static readonly byte[] key = Encoding.ASCII.GetBytes("hcxilkqbbhczfeultgbskdmaunivmfuo");
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
                using (var cs = new CryptoStream(memory, rijn.CreateEncryptor(key, rgbIV), CryptoStreamMode.Write))
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
                using (var cs = new CryptoStream(memory, rijn.CreateDecryptor(key, rgbIV), CryptoStreamMode.Write))
                {
                    cs.Write(encryptedTextBytes, 0, encryptedTextBytes.Length);
                    cs.Close();
                }

                return Encoding.UTF8.GetString(memory.ToArray());
            }
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string Reverse(this string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            Security.Check();

            char[] charArray = value.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
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

            Security.Check();

            StringBuilder b = new StringBuilder();

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
            return DateTime.Now.Millisecond % 2 == 0;
        }
    }
}
