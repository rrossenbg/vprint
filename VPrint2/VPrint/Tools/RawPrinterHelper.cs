/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using VPrinting.Native;

namespace VPrinting
{
    public class DirectHelper
    {
        public static bool SendStringToPrinter(string szPrinterName, string docName, string text)
        {
            IntPtr pText = Marshal.StringToCoTaskMemAnsi(text);
            try
            {
                DOCINFO di = new DOCINFO();
                di.pDocName = docName;
                di.pDataType = "RAW";
                IntPtr hPrinter = IntPtr.Zero;
                if (!PrintDirect.OpenPrinter(szPrinterName.Normalize(), ref hPrinter, IntPtr.Zero))
                    BombWin32();
                if (!PrintDirect.StartDocPrinter(hPrinter, 1, ref di))
                    BombWin32();
                if (!PrintDirect.StartPagePrinter(hPrinter))
                    BombWin32();
                int dwWritten = 0;
                if (!PrintDirect.WritePrinter(hPrinter, pText, text.Length, ref dwWritten))
                    BombWin32();
                PrintDirect.EndPagePrinter(hPrinter);
                PrintDirect.EndDocPrinter(hPrinter);
                PrintDirect.ClosePrinter(hPrinter);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
            finally
            {
                Marshal.FreeCoTaskMem(pText);
            }
        }

        private static void BombWin32()
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }

    public class RawPrinterHelper
    {
        // Structure and API declarions:
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }
        [DllImport("winspool.drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        // SendBytesToPrinter()
        // When the function is given a printer name and an unmanaged array
        // of bytes, the function sends those bytes to the print queue.
        // Returns true on success, false on failure.
        public static bool SendBytesToPrinter(string szPrinterName, string docName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false; // Assume failure unless you specifically succeed.

            di.pDocName = docName;
            di.pDataType = "RAW";

            // Open the printer.
            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                // Start a document.
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    // Start a page.
                    if (StartPagePrinter(hPrinter))
                    {
                        // Write your bytes.
                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
            // If you did not succeed, GetLastError may give more information
            // about why not.
            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }

        public static bool SendFileToPrinter(string szPrinterName, string docName, string szFileName)
        {
            // Open the file.
            FileStream fs = new FileStream(szFileName, FileMode.Open);
            // Create a BinaryReader on the file.
            BinaryReader br = new BinaryReader(fs);
            // Dim an array of bytes big enough to hold the file's contents.
            Byte[] bytes = new Byte[fs.Length];
            bool bSuccess = false;
            // Your unmanaged pointer.
            IntPtr pUnmanagedBytes = new IntPtr(0);
            int nLength;

            nLength = Convert.ToInt32(fs.Length);
            // Read the contents of the file into the array.
            bytes = br.ReadBytes(nLength);
            // Allocate some unmanaged memory for those bytes.
            pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
            // Copy the managed byte array into the unmanaged array.
            Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
            // Send the unmanaged bytes to the printer.
            bSuccess = SendBytesToPrinter(szPrinterName, docName, pUnmanagedBytes, nLength);
            // Free the unmanaged memory that you allocated earlier.
            Marshal.FreeCoTaskMem(pUnmanagedBytes);
            return bSuccess;
        }

        public static bool SendStringToPrinter(string szPrinterName, string docName, string szString)
        {
            IntPtr pBytes;
            Int32 dwCount;
            // How many characters are in the string?
            dwCount = szString.Length;
            // Assume that the printer is expecting ANSI text, and then convert
            // the string to ANSI text.
            pBytes = Marshal.StringToCoTaskMemAnsi(szString);
            // Send the converted ANSI string to the printer.
            SendBytesToPrinter(szPrinterName, docName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
            return true;
        }

        /// <summary>
        /// Internal worker to send the file over LPR. If the sending succeeds 
        /// and the del flag is TRUE the file <para>fname</para> will be deleted.
        /// If any error occurs the file will not be deleted.  
        /// </summary>
        /// <param name="fullFilePath">fullFilePath</param>
        /// <param name="host">192.168.44.158</param>
        /// <param name="queueName">test</param>
        /// <param name="userName">rosen</param>
        /// <param name="delete">flag delete after print</param>
        /// <example>RawPrinterHelper.SendFileTcp(fileName, "192.168.44.158", "test", "rosen", false);</example>
        public static void SendFileTcp(string fullFilePath, string host, string queueName, string userName, bool delete, int port = 515)
        {
            Random rnd = new Random();
            try
            {
                ////////////////////////////////////////////////////////
                /// PREPARE TCPCLIENT
                ///
                using (var client = new TcpClient())
                {
                    client.Connect(host, port);
                    NetworkStream nws = client.GetStream();
                    if (!nws.CanWrite)
                    {
                        nws.Close();
                        client.Close();
                        throw new Exception("-20: cannot write to network stream");
                    }

                    ////////////////////////////////////////////////////////
                    /// SOME LOCAL VARIABLES
                    ///
                    string localhost = Dns.GetHostName();
                    int jobID = rnd.Next();
                    string dname = string.Format("dfA{0}{1}", jobID, localhost);
                    string cname = string.Format("cfA{0}{1}", jobID, localhost);
                    string controlfile = string.Format("H{0}\nP{1}\nf{2}\nU{3}\nN{4}\n",
                                                localhost, userName, dname, dname, Path.GetFileName(fullFilePath));

                    const int BUFSIZE = 4 * 1024;			// 4KB buffer
                    byte[] buffer = new byte[BUFSIZE];		// 
                    byte[] ack = new byte[4];				// for the acknowledges
                    int cnt;								// for read acknowledge

                    ////////////////////////////////////////////////////////
                    /// COMMAND: RECEIVE A PRINTJOB
                    ///      +----+-------+----+
                    ///      | 02 | Queue | LF |
                    ///      +----+-------+----+
                    ///
                    int pos = 0;
                    buffer[pos++] = 2;
                    for (int i = 0; i < queueName.Length; i++)
                        buffer[pos++] = (byte)queueName[i];

                    buffer[pos++] = (byte)'\n';

                    nws.Write(buffer, 0, pos);
                    nws.Flush();

                    /////////////////////////////////////////////////////////
                    /// READ ACK
                    cnt = nws.Read(ack, 0, 4);
                    if (ack[0] != 0)
                    {
                        nws.Close();
                        client.Close();
                        throw new Exception("-21: no ACK on COMMAND 02.");
                    }

                    /////////////////////////////////////////////////////////
                    /// SUBCMD: RECEIVE CONTROL FILE
                    ///
                    ///      +----+-------+----+------+----+
                    ///      | 02 | Count | SP | Name | LF |
                    ///      +----+-------+----+------+----+
                    ///      Command code - 2
                    ///      Operand 1 - Number of bytes in control file
                    ///      Operand 2 - Name of control file
                    ///
                    pos = 0;
                    buffer[pos++] = 2;
                    string len = controlfile.Length.ToString();
                    for (int i = 0; i < len.Length; i++)
                        buffer[pos++] = (byte)len[i];

                    buffer[pos++] = (byte)' ';

                    for (int i = 0; i < cname.Length; i++)
                        buffer[pos++] = (byte)cname[i];

                    buffer[pos++] = (byte)'\n';

                    nws.Write(buffer, 0, pos);
                    nws.Flush();

                    /////////////////////////////////////////////////////////
                    /// READ ACK
                    cnt = nws.Read(ack, 0, 4);
                    if (ack[0] != 0)
                    {
                        nws.Close();
                        client.Close();
                        throw new Exception("-22: no ACK on SUBCMD 2");
                    }

                    /////////////////////////////////////////////////////////
                    /// ADD CONTENT OF CONTROLFILE
                    pos = 0;
                    for (int i = 0; i < controlfile.Length; i++)
                        buffer[pos++] = (byte)controlfile[i];

                    buffer[pos++] = 0;

                    nws.Write(buffer, 0, pos);
                    nws.Flush();

                    /////////////////////////////////////////////////////////
                    /// READ ACK
                    cnt = nws.Read(ack, 0, 4);
                    if (ack[0] != 0)
                    {
                        nws.Close();
                        client.Close();
                        throw new Exception("-23: no ACK on CONTROLFILE");
                    }

                    /////////////////////////////////////////////////////////
                    /// SUBCMD: RECEIVE DATA FILE
                    ///
                    ///      +----+-------+----+------+----+
                    ///      | 03 | Count | SP | Name | LF |
                    ///      +----+-------+----+------+----+
                    ///      Command code - 3
                    ///      Operand 1 - Number of bytes in data file
                    ///      Operand 2 - Name of data file
                    ///
                    pos = 0;
                    buffer[pos++] = 3;

                    FileInfo DataFileInfo = new FileInfo(fullFilePath);
                    len = DataFileInfo.Length.ToString();

                    for (int i = 0; i < len.Length; i++)
                        buffer[pos++] = (byte)len[i];

                    buffer[pos++] = (byte)' ';

                    for (int i = 0; i < dname.Length; i++)
                        buffer[pos++] = (byte)dname[i];

                    buffer[pos++] = (byte)'\n';

                    nws.Write(buffer, 0, pos);
                    nws.Flush();

                    /////////////////////////////////////////////////////////
                    /// READ ACK
                    cnt = nws.Read(ack, 0, 4);
                    if (ack[0] != 0)
                    {
                        nws.Close();
                        client.Close();
                        throw new Exception("-24: no ACK on SUBCMD 3");
                    }

                    /////////////////////////////////////////////////////////
                    /// ADD CONTENT OF DATAFILE

                    // use BinaryReader as print files may contain non ASCII characters.
                    //			FileStream fs = new FileStream(fname, FileMode.Open, FileAccess.Read);
                    //        	BinaryReader br = new BinaryReader(fs);
                    //        	long totalbytes = 0;
                    //            while (br.PeekChar() > -1)
                    //            {
                    //				int n = br.Read(buffer, 0, BUFSIZE);
                    //				totalbytes += n;
                    //	            nws.Write(buffer, 0, n);
                    //            	nws.Flush();
                    //            }
                    //			br.Close();
                    //			fs.Close();

                    // Code Patched
                    // thanx to Karl Fleishmann

                    long totalbytes = 0;
                    int bytesRead = 0;
                    FileStream fstream = new FileStream(fullFilePath, FileMode.Open);
                    while ((bytesRead = fstream.Read(buffer, 0, BUFSIZE)) > 0)
                    {
                        totalbytes += bytesRead;
                        nws.Write(buffer, 0, bytesRead);
                        nws.Flush();
                    }
                    fstream.Close();

                    if (DataFileInfo.Length != totalbytes)
                    {
                        string msg = fullFilePath + ": file length error";
                        // just proceed for now
                    }

                    // close data file with a 0 ..
                    pos = 0;
                    buffer[pos++] = 0;
                    nws.Write(buffer, 0, pos);
                    nws.Flush();

                    /////////////////////////////////////////////////////////
                    /// READ ACK
                    cnt = nws.Read(ack, 0, 4);
                    if (ack[0] != 0)
                    {
                        nws.Close();
                        client.Close();
                        throw new Exception("-25: no ACK on DATAFILE");
                    }

                    nws.Close();
                    client.Close();
                }
            }
            finally
            {
                // all printed well
                // should we delete the file?
                if (delete)
                    File.Delete(fullFilePath);
            }
        }

        public enum PrinterStatus
        {
            Other = 1,
            Unknown,
            Idle,
            Printing,
            Warmup,
            Stopped,
            printing,
            Offline
        }

        public static PrinterStatus GetPrinterStat(string printerDevice)
        {
            string path = string.Format("win32_printer.DeviceId='{0}'", printerDevice);
            using (var printer = new ManagementObject(path))
            {
                printer.Get();
                PropertyDataCollection printerProperties = printer.Properties;
                PrinterStatus status = (PrinterStatus)Convert.ToInt32(printer.Properties["PrinterStatus"].Value);
                return status;
            }
        }
    }
}
