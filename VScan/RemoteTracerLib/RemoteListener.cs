/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace RemoteTracerLib
{
    public class RemoteListener : MarshalByRefObject
    {
        private TextWriter m_TextWriter;

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public RemoteListener(TextWriter writer)
        {
            m_TextWriter = writer;
        }

        public void Write(string message)
        {
            m_TextWriter.Write(message);
        }

        public void WriteLine(string message)
        {
            m_TextWriter.WriteLine(message);
        }
    }
}
