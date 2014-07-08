using System;
using System.Diagnostics;
using System.IO;
using HobexCommonLib;

namespace HobexServer
{
    public class HobexTraceWriter : TraceListener, IDisposable
    {
        public string FolderName { get; set; }
        public string FullFileName { get; private set; }

        public HobexTraceWriter()
        {
            Trace.Listeners.Add(this);
        }

        public new void Dispose()
        {
            Trace.Listeners.Remove(this);
            base.Dispose();
        }

        public override void Write(string message)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(FolderName));

            lock (typeof(HobexTraceWriter))
            {
                FullFileName = Path.Combine(FolderName, "ProgramOutput{0:_dd_MM_yy}.log".format(DateTime.Now));
                File.AppendAllLines(FullFileName, new string[] { message });
            }
        }

        public override void WriteLine(string message)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(FolderName));

            lock (typeof(HobexTraceWriter))
            {
                FullFileName = Path.Combine(FolderName, "ProgramOutput{0:_dd_MM_yy}.log".format(DateTime.Now));
                File.AppendAllLines(FullFileName, new string[] { message + Environment.NewLine });
            }
        }
    }
}
