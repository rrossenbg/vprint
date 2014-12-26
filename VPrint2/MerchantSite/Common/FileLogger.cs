/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Security.Principal;
using System.Web;

namespace MerchantSite.Common
{

    /// <summary>
    /// Logs to file asynchronously, error safe
    /// </summary>
    public class FileLogger
    {
        /// <summary>
        /// Buffers length 1024 * 16
        /// </summary>
        private const int LENGTH = 1024 * 16; //16KB
        private readonly byte[] m_Buffer1 = new byte[LENGTH];
        private readonly byte[] m_Buffer2 = new byte[LENGTH];
        private static readonly string ms_Line = new string('=', 40);

        public string FileName { get; set; }
        public int MaxWriteLoops { get; set; }

        private static FileLogger ms_instance = new FileLogger();
        public static FileLogger Instance
        {
            get { return ms_instance; }
        }

        private FileLogger()
        {
            var asm = Assembly.GetEntryAssembly();
            FileName = (asm != null) ? Path.ChangeExtension(asm.Location, "log") : HttpContext.Current.Server.MapPath("~/App_Data/app.log");
            MaxWriteLoops = 1000;//40MB
        }

        /// <summary>
        /// Write string to file.
        /// Synchronously. Not error safe.
        /// </summary>
        /// <param name="message"></param>
        public void Write(string message)
        {
            Debug.Assert(!string.IsNullOrEmpty(FileName));
            Debug.Assert(message.Length < LENGTH);

            lock (this)
            {
                int count = 0;
                byte[] b1 = m_Buffer1;
                byte[] b2 = m_Buffer2;

                int bytesCount = Encoding.UTF8.GetByteCount(message);
                Encoding.UTF8.GetBytes(message, 0, message.Length, b2, 0);

                using (var str1 = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                using (var writer = new BinaryWriter(str1))
                using (var str2 = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new BinaryReader(str2))
                {
                    int readCount = reader.Read(b1, 0, bytesCount);
                    writer.Write(b2, 0, bytesCount);

                    while (readCount > 0)
                    {
                        readCount = reader.Read(b2, 0, bytesCount);

                        writer.Write(b1, 0, bytesCount);

                        Array.Clear(b1, 0, bytesCount);

                        b1 = (b1 == m_Buffer1) ? m_Buffer2 : m_Buffer1;//1
                        b2 = (b1 == m_Buffer1) ? m_Buffer2 : m_Buffer1;//2

                        if (count++ > MaxWriteLoops)
                            return;
                    }
                }
            }
        }

        /// <summary>
        /// Logs info (safe, asynch)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="source"></param>
        public static void LogInfo(string message, string source)
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine(ms_Line);
            b.AppendLine("INFO");
            b.AppendFormat("Source: {0}\r\n", source);
            b.AppendFormat("{0:s}\r\n", DateTime.Now);
            b.AppendLine(message);
            new Action<string>((s) => Instance.Write(s)).FireAndForgetSafe(b.ToString());
        }

        /// <summary>
        /// Logs warning (safe, asynch)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="source"></param>
        public static void LogWarning(string message, string source)
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine(ms_Line);
            b.AppendLine("WARNING");
            b.AppendFormat("Source: {0}\r\n", source);
            b.AppendFormat("{0:s}\r\n", DateTime.Now);
            b.AppendLine(message);
            new Action<string>((s) => Instance.Write(s)).FireAndForgetSafe(b.ToString());
        }

        /// <summary>
        /// Logs error (safe, asynch)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="source"></param>
        public static void LogError(string message, string source)
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine(ms_Line);
            b.AppendLine("ERROR");
            b.AppendFormat("Source: {0}\r\n", source);
            b.AppendFormat("{0:s}\r\n", DateTime.Now);
            b.AppendLine(message);
            new Action<string>((s) => Instance.Write(s)).FireAndForgetSafe(b.ToString());
        }
    }
}
