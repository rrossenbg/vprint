/*
*	Copyright © 2011 Premier Tax Free
*	WebRemoteTracerServer
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;

namespace RemoteTracerLib
{
    public class WebRemoteTracerServer : TraceListener
    {
        public string ListenerUrl { get; set; }

        public WebRemoteTracerServer()
        {
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

        private void A()
        {
            WebRequest req = HttpWebRequest.Create(ListenerUrl);
            using (var res = req.GetResponse())
            {
                res.
            }
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
