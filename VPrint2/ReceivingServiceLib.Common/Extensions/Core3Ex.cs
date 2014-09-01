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
        public static bool IsNullOrWhiteSpace(this string value)
        {
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
#if DEBUGGER
                Trace.WriteLine(ex, "ISRV");
#endif
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
#if DEBUGGER
                Trace.WriteLine(ex, "ISRV");
#endif
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
            return (T)Convert.ChangeType(value, typeof(T));
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string concat(this string str, params object[] values)
        {
            var b = new StringBuilder(str);
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

            var files = from.GetFiles();

            foreach (var file in files)
                file.CopyTo(to.CombineFileName(file.Name).FullName, @overwrite);

            return files.Length;
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static bool EqualsNoCase(this string value1, string value2)
        {
            return string.Equals(value1, value2, StringComparison.InvariantCultureIgnoreCase);
        }

        //[TargetedPatchingOptOut("na")]
        //[Obfuscation]
        //public static string Unique(this string str)
        //{
        //    var u = Guid.NewGuid().ToString().Replace('-', '_');
        //    return string.Concat(str, u);
        //}

        //[TargetedPatchingOptOut("na")]
        //[Obfuscation]
        //public static string Limit(this string str, int length)
        //{
        //    if (str == null)
        //        return null;
        //    return str.Substring(0, Math.Min(str.Length, length));
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Any name</param>
        /// <returns></returns>
        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string ToUniqueFileName(this string name)
        {
            return Path.GetFileNameWithoutExtension(name).Unique().Limit(MAX_FILE_LENGTH).concat(Path.GetExtension(name));
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string FromObject<T>(this XmlSerializer serializer, T value)
        {
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
            using (var str = XmlReader.Create(new StringReader(text)))
            {
                Debug.Assert(str != null);
                return (T)serializer.Deserialize(str);
            }
        }

        #region SECURITY
        [Obfuscation(StripAfterObfuscation = true)]
        private static readonly byte[] IVString = Encoding.ASCII.GetBytes("ryojvlzmdalyglrj");
        [Obfuscation(StripAfterObfuscation = true)]
        private static readonly byte[] keyString = Encoding.ASCII.GetBytes("hcxilkqbbhczfeultgbskdmaunivmfuo");

        [Obfuscation(StripAfterObfuscation = true)]
        private static readonly byte[] IVFile = Encoding.ASCII.GetBytes("RYO777ZMDALYGLRJ");
        [Obfuscation(StripAfterObfuscation = true)]
        private static readonly byte[] keyFile = Encoding.ASCII.GetBytes("HCXILKQBBHCZF666TGBSKDMAUNIVMFUO");
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

        private const int BUFSIZE = 16384;

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static void EncriptFile(this FileInfo fromFile, FileInfo toFile)
        {
            Debug.Assert(fromFile != null);
            Debug.Assert(toFile != null);

            using (SymmetricAlgorithm algorithm = Rijndael.Create())
            using (ICryptoTransform encryptor = algorithm.CreateEncryptor(keyFile, IVFile))
            using (Stream from = fromFile.Open(FileMode.Open, FileAccess.Read))
            using (Stream to = toFile.Open(FileMode.OpenOrCreate, FileAccess.Write))
            using (Stream c = new CryptoStream(to, encryptor, CryptoStreamMode.Write))
                from.CopyTo(c, BUFSIZE);
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

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static void DecriptFile(this FileInfo fromFile, FileInfo toFile)
        {
            Debug.Assert(fromFile != null);
            Debug.Assert(toFile != null);

            using (SymmetricAlgorithm algorithm = Rijndael.Create())
            using (ICryptoTransform decryptor = algorithm.CreateDecryptor(keyFile, IVFile))
            using (Stream from = fromFile.Open(FileMode.Open, FileAccess.Read))
            using (Stream to = toFile.Open(FileMode.OpenOrCreate, FileAccess.Write))
            using (Stream c = new CryptoStream(from, decryptor, CryptoStreamMode.Read))
                c.CopyTo(to, BUFSIZE);     
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string Reverse(this string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

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
            return DateTime.Now.Millisecond % 2 == 0;
        }

        [TargetedPatchingOptOut("na")]
        public static void RunSafe(this Action handle)
        {
            try
            {
                handle();
            }
            catch
            {
            }
        }

        [TargetedPatchingOptOut("na")]
        public static void RunSafe<T>(this Action<T> handle, T t)
        {
            try
            {
                handle(t);
            }
            catch
            {
            }
        }
    }
}
