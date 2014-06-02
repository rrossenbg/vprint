/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

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
    ///     Console.WriteLine("Server started.");
    ///     Trace.Listeners.Add(new RemoteTracerServer());
    ///     for (int i = 0; i < 1000; i++)
    ///     {
    ///         Trace.WriteLine("Now is " + DateTime.Now);
    ///         Thread.Sleep(1000);
    ///     }
    ///     Trace.WriteLine("Super..");
    /// }
    /// ------------------------------------
    /// 
    /// //Client - anther exe that refers the same library.       
    /// static void Main(string[] args)
    /// {
    ///     Console.WriteLine("Client started.");
    ///     RemoteTracerClient client = new RemoteTracerClient(Console.Out);
    ///     Console.Read();
    /// }
    /// 
    /// </example>
    public class RemoteTracerServer : TraceListener
    {
        private RemoteListener m_ResumeListener;

        public string ListenerUrl { get; set; }

        public RemoteTracerServer()
        {
            IChannel clientChannel = ChannelServices.GetChannel("tcp");
            if (clientChannel == null)
                ChannelServices.RegisterChannel(new TcpClientChannel(),false);
            ListenerUrl = "tcp://localhost:15456/RemoteListener";
        }

        public override void Write(string message)
        {
            if (TryGetObject())
            {
                RunSafe(new Action<string>(m_ResumeListener.Write), message);
            }
        }

        public override void WriteLine(string message)
        {
            if (TryGetObject())
            {
                RunSafe(new Action<string>(m_ResumeListener.WriteLine), message);
            }
        }

        private bool TryGetObject()
        {
            m_ResumeListener = (RemoteListener)Activator.GetObject(typeof(RemoteListener), ListenerUrl);
            return m_ResumeListener != null;
        }

        private void RunSafe(Action<string> act, string message)
        {
            try
            {
                act(message);
            }
            catch
            {
                //Do not do anything
            }
        }
    }
}