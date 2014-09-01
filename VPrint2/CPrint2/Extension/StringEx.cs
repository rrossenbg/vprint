/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;

namespace CPrint2
{
    public static class StringEx
    {
        /// <summary>
        /// 250
        /// </summary>
        private const int MAX_FILE_LENGTH = 250;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <param name="value"></param>
        /// <param name="values"></param>
        [TargetedPatchingOptOut("na")]
        public static void AppendIfNNE(this StringBuilder b, string value, params string[] values)
        {
            if (!string.IsNullOrEmpty(value))
            {
                b.Append(value);
                b.Append(values.Length == 0 ? "" : values[0]);
            }
        }

        /// <summary>
        /// AddressMax100.AppendLineIfNNE(HeadOffice.OfficeAddress.Line1 + ",", ",");
        /// </summary>
        /// <param name="b"></param>
        /// <param name="value"></param>
        /// <param name="values"></param>
        [TargetedPatchingOptOut("na")]
        public static void AppendLineIfNNE(this StringBuilder b, string value, params string[] values)
        {
            if (!string.IsNullOrEmpty(value))
            {
                b.Append(value);
                b.AppendLine(values.Length == 0 ? "" : values[0]);
            }
        }

        /// <summary>
        /// TypeConverter conv = TypeDescriptor.GetConverter(t);
        /// return (T)conv.ConvertFromInvariantString(value);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [TargetedPatchingOptOut("na")]
        public static T Cast<T>(this string value)
        {
            if (value.IsNullOrEmpty())
                return default(T);

            Type t = typeof(T);
            TypeConverter conv = TypeDescriptor.GetConverter(t);
            return (T)conv.ConvertFromInvariantString(value);
        }

        /// <summary>
        /// (T)Convert.ChangeType(obj, typeof(T))
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        [TargetedPatchingOptOut("na")]
        public static T Cast<T>(this object obj)
        {
            if (obj == null || obj is DBNull)
                return default(T);
            return (T)Convert.ChangeType(obj, typeof(T));
        }

        /// <summary>
        /// (T)obj;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        [TargetedPatchingOptOut("na")]
        public static T cast<T>(this object obj) where T : new()
        {
            if (obj == null || obj is DBNull)
                return new T();
            return (T)obj;
        }

        [TargetedPatchingOptOut("na")]
        public static char CharOfString(this string value, int index)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (index < 0)
                throw new ArgumentNullException("index");

            index = index % value.Length;

            return value[index];
        }

        [TargetedPatchingOptOut("na")]
        public static void Clean(this StringBuilder b)
        {
            b.Replace("\n", "");
            b.Replace("\n\r", "");
            b.Replace(" ", "");
        }

        [TargetedPatchingOptOut("na")]
        public static string Clean(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            return str.Replace("\n\r", "").Replace("\n", "").Replace(" ", "");
        }

        [TargetedPatchingOptOut("na")]
        public static string concat(this string str, params object[] values)
        {
            StringBuilder b = new StringBuilder(str);
            foreach (var o in values)
                b.Append(o);
            return b.ToString();
        }

        [TargetedPatchingOptOut("na")]
        public static string join(this string str, string separ, params object[] values)
        {
            var l = new List<object>();
            l.Add(str);
            l.AddRange(values);
            return string.Join(separ, l.ToArray());
        }

        [TargetedPatchingOptOut("na")]
        public static bool EqualNoCase(this string str, string str2)
        {
            return string.Compare(str, str2, true) == 0;
        }

        [TargetedPatchingOptOut("na")]
        public static string Decrypt(this string text, string passphrase)
        {
            byte[] results;
            UTF8Encoding utf8 = new UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            using (var hashProvider = new MD5CryptoServiceProvider())
            {
                byte[] tdesKey = hashProvider.ComputeHash(utf8.GetBytes(passphrase));

                // Step 2. Create a new TripleDESCryptoServiceProvider object
                using (var tdesAlgorithm = new TripleDESCryptoServiceProvider())
                {
                    // Step 3. Setup the decoder
                    tdesAlgorithm.Key = tdesKey;
                    tdesAlgorithm.Mode = CipherMode.ECB;
                    tdesAlgorithm.Padding = PaddingMode.PKCS7;

                    // Step 4. Convert the input string to a byte[]
                    byte[] dataToDecrypt = Convert.FromBase64String(text);

                    // Step 5. Attempt to decrypt the string
                    try
                    {
                        ICryptoTransform decryptor = tdesAlgorithm.CreateDecryptor();
                        results = decryptor.TransformFinalBlock(dataToDecrypt, 0, dataToDecrypt.Length);
                    }
                    finally
                    {
                        // Clear the TripleDes and Hashprovider services of any sensitive information
                        tdesAlgorithm.Clear();
                        hashProvider.Clear();
                    }
                }
            }

            // Step 6. Return the decrypted string in UTF8 format
            return utf8.GetString(results);
        }

        [TargetedPatchingOptOut("na")]
        public static string Encrypt(this string text, string passphrase)
        {
            byte[] results;
            UTF8Encoding utf8 = new UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            using (var hashProvider = new MD5CryptoServiceProvider())
            {
                byte[] tdesKey = hashProvider.ComputeHash(utf8.GetBytes(passphrase));

                // Step 2. Create a new TripleDESCryptoServiceProvider object
                using (var tdesAlgorithm = new TripleDESCryptoServiceProvider())
                {
                    // Step 3. Setup the encoder
                    tdesAlgorithm.Key = tdesKey;
                    tdesAlgorithm.Mode = CipherMode.ECB;
                    tdesAlgorithm.Padding = PaddingMode.PKCS7;

                    // Step 4. Convert the input string to a byte[]
                    byte[] dataToEncrypt = utf8.GetBytes(text);

                    // Step 5. Attempt to encrypt the string
                    try
                    {
                        ICryptoTransform encryptor = tdesAlgorithm.CreateEncryptor();
                        results = encryptor.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
                    }
                    finally
                    {
                        // Clear the TripleDes and Hashprovider services of any sensitive information
                        tdesAlgorithm.Clear();
                        hashProvider.Clear();
                    }
                }
            }

            // Step 6. Return the encrypted string as a base64 encoded string
            return Convert.ToBase64String(results);
        }

        [TargetedPatchingOptOut("na")]
        public static string EscapeXml(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;
            return SecurityElement.Escape(value);
        }

        [TargetedPatchingOptOut("na")]
        public static char Last(this StringBuilder b)
        {
            if (b.Length == 0)
                throw new IndexOutOfRangeException();
            return b[b.Length - 1];
        }

        [TargetedPatchingOptOut("na")]
        public static string format(this string format, params object[] values)
        {
            return string.Format(format, values);
        }

        /// <summary>
        /// Formats string by replaicing all keys with their values
        /// </summary>
        /// <param name="template">[Key1]BlaBla[Key2]BlaBla</param>
        /// <param name="values">string KeyValue collection</param>
        /// <param name="ignoreEmptyValues">Ignores null or empty values</param>
        /// <returns></returns>
        [TargetedPatchingOptOut("na")]
        public static string format(this string template, Dictionary<string, string> values, bool ignoreEmptyValues = true)
        {
            Debug.Assert(!string.IsNullOrEmpty(template));
            Debug.Assert(values != null);

            StringBuilder b = new StringBuilder(template);

            foreach (var item in values)
            {
                if (ignoreEmptyValues && item.Value.IsNullOrEmpty())
                    continue;
                b.Replace(string.Concat("[", item.Key, "]"), item.Value);
            }

            return b.ToString();
        }

        [TargetedPatchingOptOut("na")]
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        [TargetedPatchingOptOut("na")]
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        [TargetedPatchingOptOut("na")]
        public static bool IsEmpty(this StringBuilder b)
        {
            return b.Length == 0;
        }

        [TargetedPatchingOptOut("na")]
        public static T Index<T>(this IList<T> items, int index)
        {
            Debug.Assert(index >= 0);

            if (items.Count > index)
                return items[index];
            return default(T);
        }

        [TargetedPatchingOptOut("na")]
        public static string Limit(this string str, int length)
        {
            if (str == null)
                return null;
            return str.Substring(0, Math.Min(str.Length, length));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxChars">Max chars</param>
        /// <param name="addition">String or null</param>
        /// <returns></returns>
        [TargetedPatchingOptOut("na")]
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

        ///// <summary>
        ///// Syntax error. Please use it in this way.
        ///// </summary>
        ///// <param name="value"></param>
        ///// <param name="count"></param>
        ///// <returns></returns>
        //[TargetedPatchingOptOut("na")]
        //public static string Miltiply(this string value, int count)
        //{
        //    StringBuilder b = new StringBuilder();
        //    for (int i = 0; i < count; i++)
        //        b.Append(value);
        //    return b.toString();
        //}

        //[TargetedPatchingOptOut("na")]
        //public static string Multiply(this string value, int count)
        //{
        //    StringBuilder b = new StringBuilder();
        //    for (int i = 0; i < count; i++)
        //        b.Append(value);
        //    return b.toString();
        //}

        [TargetedPatchingOptOut("na")]
        public static string PhoneFormat(this string value)
        {
            StringBuilder b = new StringBuilder();
            foreach (char c in value)
            {
                b.Append(c);
                b.Append(" ");
            }
            return b.ToString();
        }

        [TargetedPatchingOptOut("na")]
        public static string[] SplitSafe(this string value, params string[] values)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            return value.Split(values, StringSplitOptions.None);
        }

        [TargetedPatchingOptOut("na")]
        public static string SplitJoinSafe(this string value, string separator, params string[] values)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            var result = value.Split(values, StringSplitOptions.None);
            return string.Join(separator, result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <param name="separator"></param>
        /// <example>
        /// var result1 = "123456789".SplitJoinSafe(2, ".");  //12.34.56.78.9
        /// var result2 = "12345678".SplitJoinSafe(3, "-");   //123-456-78
        /// var result3 = "1234567800".SplitJoinSafe(3, "x"); //123x456x780x0
        /// </example>
        /// <returns></returns>
        [TargetedPatchingOptOut("na")]
        public static string SplitJoinSafe(this string value, int length, string separator)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            var b = new StringBuilder();

            for (int index = 0; index < value.Length; index += length)
            {
                if (index + length < value.Length)
                {
                    b.Append(value.Substring(index, length));
                    b.Append(separator);
                }
                else
                {
                    b.Append(value.Substring(index, value.Length - index));
                    break;
                }
            }
            return b.ToString();
        }

        [TargetedPatchingOptOut("na")]
        public static string Substring(this StringBuilder b, int length)
        {
            if (b.Length < length)
                return b.ToString();
            return b.ToString().Substring(0, length);
        }

        /// <summary>
        /// Modifies the input string depending of the content string.
        /// 'The quick brown fox jumps over the lazy dog.' + 'This is demo!' = 'This is demo!this is demo!this is demo!This'
        /// </summary>
        /// <param name="basestr"></param>
        /// <param name="inherstr"></param>
        /// <returns></returns>
        [TargetedPatchingOptOut("na")]
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

        [TargetedPatchingOptOut("na")]
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
        public static T ToObject<T>(this XmlSerializer serializer, string text)
        {
            using (var str = XmlReader.Create(new StringReader(text)))
            {
                Debug.Assert(str != null);
                return (T)serializer.Deserialize(str);
            }
        }

        [TargetedPatchingOptOut("na")]
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
        public static string ToUniqueFileName(this string name)
        {
            return Path.GetFileNameWithoutExtension(name).Unique().Limit(MAX_FILE_LENGTH).concat(Path.GetExtension(name));
        }

        [TargetedPatchingOptOut("na")]
        public static string TrimSafe(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            return value.Trim();
        }

        [TargetedPatchingOptOut("na")]
        public static string ChangeFilePath(this string fullFilePath, Func<string, string> changeFunct)
        {
            Debug.Assert(changeFunct != null);

            var file = new FileInfo(fullFilePath);
            return Path.Combine(file.Directory.FullName, changeFunct(file.Name));
        }

        [TargetedPatchingOptOut("na")]
        public static string Unique(this string str)
        {
            var u = Guid.NewGuid().ToString().Replace('-', '_');
            return string.Concat(str, u);
        }

        [TargetedPatchingOptOut("na")]
        public static string Unique(this string str, string ext)
        {
            var u = Guid.NewGuid().ToString().Replace('-', '_');
            return string.Concat(str, u, ext);
        }

        #region SECURITY
        private static readonly byte[] rgbIV = Encoding.ASCII.GetBytes("ryojvlzmdalyglrj");
        private static readonly byte[] key = Encoding.ASCII.GetBytes("hcxilkqbbhczfeultgbskdmaunivmfuo");
        #endregion

        [TargetedPatchingOptOut("na")]
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
        public static string Reverse(this string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            char[] charArray = value.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        [TargetedPatchingOptOut("na")]
        public static void StartProcessSafe(this string fullFileName)
        {
            try
            {
                Process.Start(new ProcessStartInfo(fullFileName));
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        /// <summary>
        /// Expected format A;B or A,B or A:B
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [TargetedPatchingOptOut("na")]
        public static Size ParseSize(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                throw new ArgumentException("s");

            string[] ss = s.Split(',', ';', ':');
            if (ss.Length != 2)
                throw new ArgumentOutOfRangeException();

            int width = int.Parse(ss[0].Trim());
            int height = int.Parse(ss[1].Trim());
            return new Size(width, height);
        }

        /// <summary>
        /// Expected format A;B or A,B or A:B
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [TargetedPatchingOptOut("na")]
        public static Point ParsePoint(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                throw new ArgumentException("s");

            string[] ss = s.Split(',', ';', ':');
            if (ss.Length != 2)
                throw new ArgumentOutOfRangeException();

            int x = int.Parse(ss[0].Trim());
            int y = int.Parse(ss[1].Trim());
            return new Point(x, y);
        }
    }
}
