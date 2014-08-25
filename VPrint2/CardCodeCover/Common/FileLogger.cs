/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.IO;
using System.Diagnostics;

namespace CardCodeCover
{
    public class FileLogger : IDisposable
    {
        private readonly StreamWriter m_Writer;

        public FileLogger(string fileName, bool trace = false)
        {
            var info = new FileInfo(fileName);
            m_Writer = new StreamWriter(info.OpenWrite());
        }

        public void WriteLine(string message)
        {
            Trace.WriteLine(message);
            m_Writer.WriteLine(message);
        }

        public void WriteLine(string format, params object[] @params)
        {
            string message = string.Format(format, @params);
            Trace.WriteLine(message);
            m_Writer.WriteLine(message);
        }

        public void Dispose()
        {
            m_Writer.Flush();
            m_Writer.Close();
            m_Writer.Dispose();
        }
    }
}
