/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace RemoteTracerLib
{
    /// <summary>
    /// The Tracer traces any class or library.
    /// It's expecialy designed to trace
    /// Windows Sevices and Web Services.
    /// </summary>
    /// <example>
    ///
    /// //Server - an exe that refers this library.
    /// static void Main(string[] args)
    /// {
    ///     var cert = CertificatesUtils.FindByIssuerName("rrossenbg", false);
    ///     Trace.Listeners.Add(new WebTracerServer("127.0.0.1", 8080, "localhost", cert));
    ///     while(true)
    ///     for (int i = 0; i < int.MaxValue; i++)
    ///     {
    ///         Trace.WriteLine(i.ToString());
    ///         Thread.Sleep(0);
    ///     }
    ///     Console.WriteLine("Done");
    ///     Console.Read();
    /// }
    /// ------------------------------------
    /// </example>
    public class WebTracerServer : TraceListener
    {
        private static string ms_Url;
        private static int ms_Port;
        private static string ms_ServerName;
        private static X509Certificate ms_Certificate;

        public WebTracerServer(string url, int port, string serverName, X509Certificate cert)
        {
            ms_Url = url;
            ms_Port = port;
            ms_ServerName = serverName;
            ms_Certificate = cert;
        }

        public override void Write(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;
            SendTextAsync(message);
        }

        public override void WriteLine(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;
            string newmessage = string.Concat(message, Environment.NewLine);
            SendTextAsync(newmessage);
        }

        private static void SendTextAsync(string text)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(OnThreadStarted), text);
        }

        private static void OnThreadStarted(object data)
        {
            try
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP))
                {
                    try
                    {
                        socket.ReceiveTimeout = 100;
                        socket.SendTimeout = 100;
                        socket.Connect(ms_Url, ms_Port);

                        using (Stream stream = new NetworkStream(socket))
                        {
                            SslStream ssl = new SslStream(stream, false,
                                new RemoteCertificateValidationCallback(OnCertificateValidationCallback),
                                new LocalCertificateSelectionCallback(OnLocalCertificateSelectionCallback));

                            X509CertificateCollection coll = new X509CertificateCollection();
                            coll.Add(ms_Certificate);

                            ssl.AuthenticateAsClient(ms_ServerName, coll, SslProtocols.Tls, false);

                            using (BinaryWriter writer = new BinaryWriter(ssl, Encoding.UTF8))
                            {
                                writer.Write(Convert.ToString(data));
                                writer.Flush();
                            }
                            stream.Close();
                        }

                        socket.Shutdown(SocketShutdown.Both);
                        socket.Disconnect(true);
                    }
                    finally
                    {
                        socket.Close();
                    }
                }
            }
            catch
            {
                //No errors
            }
        }

        static bool OnCertificateValidationCallback(object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        static X509Certificate OnLocalCertificateSelectionCallback(object sender, string targetHost,
            X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            return ms_Certificate;
        }
    }
}
