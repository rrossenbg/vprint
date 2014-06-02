/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Drawing;
using System.Security.Cryptography;

namespace PremierTaxFree.PTFLib
{
    public static class ByteArrayEx
    {
        /// <summary>
        /// Compresses byte array
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] Compress(this byte[] input)
        {
            Debug.Assert(input != null);
            Debug.Assert(input.Length > 0);

            using (MemoryStream memory = new MemoryStream())
            using (Stream gzip = new GZipStream(memory, CompressionMode.Compress))
            {
                gzip.Write(input, 0, input.Length);
                gzip.Close();
                return memory.ToArray();
            }
        }

        /// <summary>
        /// Decomress byte array
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] Decompress(this byte[] input)
        {
            Debug.Assert(input != null);
            Debug.Assert(input.Length > 0);

            StringBuilder str = new StringBuilder();

            byte[] buffer = new byte[4096];

            using (MemoryStream memory = new MemoryStream(input))
            using (Stream gzip = new GZipStream(memory, CompressionMode.Decompress))
            {
                while (true)
                {
                    int size = gzip.Read(buffer, 0, buffer.Length);
                    if (size > 0)
                        str.Append(Encoding.Unicode.GetString(buffer, 0, size));
                    else
                        break;
                }
                gzip.Close();
            }
            return Encoding.Unicode.GetBytes(str.ToString());
        }

        /// <summary>
        /// byte[] buffer = UTF8Encoding.Default.GetBytes("ROSSEN RUSEV");
        /// byte[] compressed = buffer.Compress2();
        /// byte[] decompressed = compressed.Decompress2();
        /// string str = UTF8Encoding.Default.GetString(decompressed).Trim('\0');
        /// Console.WriteLine(str);
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Compress2(this byte[] data)
        {
            // We can expect a good compression
            // ratio from an empty array!
            MemoryStream ms = new MemoryStream();
            using (Stream ds = new DeflateStream(ms, CompressionMode.Compress))
                ds.Write(data, 0, data.Length);

            byte[] compressed = ms.ToArray();
            return compressed;
        }

        /// <summary>
        /// byte[] buffer = UTF8Encoding.Default.GetBytes("ROSSEN RUSEV");
        /// byte[] compressed = buffer.Compress2();
        /// byte[] decompressed = compressed.Decompress2();
        /// string str = UTF8Encoding.Default.GetString(decompressed).Trim('\0');
        /// Console.WriteLine(str);
        /// </summary>
        /// <param name="compressed"></param>
        /// <returns></returns>
        public static byte[] Decompress2(this byte[] compressed)
        {
            byte[] data = new byte[1024];
            // Decompress back to the data array:
            MemoryStream ms = new MemoryStream(compressed);
            using (Stream ds = new DeflateStream(ms, CompressionMode.Decompress))
                ds.Read(data, 0, data.Length);
            return data;
        }


        /// <summary>
        /// Creates xml document from byte array
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static XmlDocument CreateXmlDoc(this byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
                return null;

            XmlDocument doc = new XmlDocument();
            using (var memory = new MemoryStream(buffer))
            using (var reader = XmlReader.Create(memory))
                doc.Load(reader);
            return doc;
        }

        /// <summary>
        /// Creates xdocument from byte array
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static XDocument CreateXDoc(this byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
                return null;

            XDocument doc = null;
            using (var memory = new MemoryStream(buffer))
            using (var reader = XmlReader.Create(memory))
                doc = XDocument.Load(reader, LoadOptions.PreserveWhitespace);
            return doc;
        }

        /// <summary>
        /// Creates image object from byte array
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Image ToImage(this byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
                return null;

            using (MemoryStream memory = new MemoryStream(buffer))
                return Image.FromStream(memory);
        }

        /// <summary>
        /// Protects byte array
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static byte[] Protect(this byte[] buffer)
        {
            return ProtectedData.Protect(buffer, null, DataProtectionScope.CurrentUser);
        }

        /// <summary>
        /// Unprotects byte array
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static byte[] Unprotect(this byte[] buffer)
        {
            return ProtectedData.Unprotect(buffer, null, DataProtectionScope.CurrentUser);
        }
    }
}
