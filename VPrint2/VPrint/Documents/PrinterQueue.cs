/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Text;
using System.Threading;
using Thr = System.Threading;

namespace VPrinting.Documents
{
    /// <summary>
    /// Queues and prints vouchers directly on the printer
    /// Use Printer's -> BidirPrint On 
    /// Driver's -> PrintDirectly to the Printer (No queue)
    /// </summary>
    public class PrinterQueue
    {
        private const string PASS = "@KEY123@";
        public static event ThreadExceptionEventHandler Error;
        private static PrinterQueue ms_instance = new PrinterQueue();
        public static string CacheDirectory { get; set; }

        /// <summary>
        /// 2 sec.
        /// </summary>
        private readonly TimeSpan TIMEOUT = TimeSpan.FromSeconds(2);
        /// <summary>
        /// 15 sec.
        /// </summary>
        private readonly TimeSpan ERRTIMEOUT = TimeSpan.FromSeconds(15);

        private Thread m_PrintThread = null;
        private readonly ArrayList m_Queue = ArrayList.Synchronized(new ArrayList());

        /// <summary>
        /// Adds a document to printer queue
        /// </summary>
        /// <param name="docName"></param>
        /// <param name="docText"></param>
        public static void AddJob(string printerName, string docName, string docText)
        {
            ms_instance.AddInternal(printerName, docName, docText);
        }

        /// <summary>
        /// Starts/stops printer queue
        /// </summary>
        /// <returns>True- working, False- not working, Null- not started</returns>
        public static bool? StartStop()
        {
            if (ms_instance.m_PrintThread == null)
                return null;

            if ((ms_instance.m_PrintThread.ThreadState & Thr.ThreadState.Suspended) == Thr.ThreadState.Suspended)
            {
                ms_instance.m_PrintThread.Resume();
                return true;
            }
            else
            {
                ms_instance.m_PrintThread.Suspend();
                return false;
            }
        }

        /// <summary>
        /// Reads file from disk. Adds it to the queue
        /// </summary>
        /// <param name="fileName"></param>
        public static void AddFromFile(string fileName)
        {
            string docName = Path.GetFileNameWithoutExtension(fileName);
            string docText = null;
            KeyValue value = null;

            lock (typeof(PrinterQueue))
            {
                docText = File.ReadAllText(fileName);
                string text = docText.Decrypt(PASS);
                value = KeyValue.Parse(text);
            }

            ms_instance.AddInternal(value.Key, docName, value.Value2);
        }

        /// <summary>
        /// Empties disk cache
        /// </summary>
        public static void EmptyCache()
        {
            lock (typeof(PrinterQueue))
            {
                if (!Directory.Exists(PrinterQueue.CacheDirectory))
                    Directory.CreateDirectory(PrinterQueue.CacheDirectory);

                foreach (var file in Directory.GetFiles(PrinterQueue.CacheDirectory))
                    File.Delete(file);

                Thread.Sleep(2000);
            }
        }

        private void AddInternal(string printerName, string docName, string docText)
        {
            TryStart();

            lock (this)
            {
                m_Queue.Add(new KeyValue(printerName, docName, docText));
            }
        }

        private void TryStart()
        {
            if (m_PrintThread == null)
            {
                m_PrintThread = new Thread(PrintThreadFunction);
                m_PrintThread.IsBackground = true;
                m_PrintThread.Priority = ThreadPriority.Lowest;
                m_PrintThread.Name = "Print Job Thread";
                m_PrintThread.SetApartmentState(ApartmentState.MTA);
                m_PrintThread.Start();
            }
        }

        private void PrintThreadFunction()
        {
            while (true)
            {
                try
                {
                    KeyValue pair;
                    lock (this)
                    {
                        while (m_Queue.Count == 0)
                            Monitor.Wait(this, TIMEOUT);

                        pair = (KeyValue)m_Queue[0];

                        m_Queue.RemoveAt(0);

                        Debug.WriteLine(m_Queue.Count, "PrintJobs");
                    }

                    string printerName = pair.Key;
                    string docName = pair.Value1;
                    string docText = pair.Value2;
#if ENCODE
                    string encText = pair.ToString().Encrypt(PASS);

                    lock (typeof(PrinterQueue))
                    {
                        if (!string.IsNullOrEmpty(CacheDirectory))
                            File.WriteAllText(Path.Combine(CacheDirectory, string.Concat(docName, ".vol")), encText);
                    }
#endif

                    if (!DirectHelper.SendStringToPrinter(printerName, docName, docText))
                    {
                        m_Queue.Insert(0, new KeyValue(printerName, docName, docText));
                        Thread.Sleep(ERRTIMEOUT);
                    }
                }
                catch (Exception ex)
                {
                    if (Error != null)
                        Error(this, new ThreadExceptionEventArgs(ex));
                }
                finally
                {
                    Thread.Sleep(0);
                }
            }
        }
    }

    public class KeyValue
    {
        public string Key { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }

        public KeyValue(string key, string value1, string value2)
        {
            Key = key;
            Value1 = value1;
            Value2 = value2;
        }

        public static KeyValue Parse(string value)
        {
            StringReader reader = new StringReader(value);
            var key = reader.ReadLine();
            var value1 = reader.ReadLine();
            var value2 = reader.ReadLine();
            return new KeyValue(key, value1, value2);
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine(Key);
            b.AppendLine(Value1);
            b.AppendLine(Value2);
            return b.ToString();
        }
    }
}
