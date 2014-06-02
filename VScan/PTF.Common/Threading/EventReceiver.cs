/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace PremierTaxFree.PTFLib.Threading
{
    /// <summary>
    /// Windows event receiver class
    /// </summary>
    public class EventReceiver : CycleWorkerBase
    {
        private readonly string mName;

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr OpenEvent(UInt32
            dwDesiredAccess, Boolean bInheritHandle, String lpName);

        /// <summary>
        /// New event arrived event
        /// </summary>
        public event EventHandler NewEvent;

        public EventReceiver(string name)
        {
            Debug.Assert(!string.IsNullOrEmpty(name));
            mName = name;
        }

        protected override void ThreadFunction()
        {
            // Same as EVENT_ALL_ACCESS value
            const uint unEventPermissions = 2031619; 

            IntPtr hEvent = OpenEvent(unEventPermissions, false, mName);
            if (hEvent == IntPtr.Zero)
                throw new Exception("Cannot open event.");

            AutoResetEvent arEvent = new AutoResetEvent(false);
            try
            {
                arEvent.Handle = hEvent;
                WaitHandle[] waitHandles = { arEvent };

                while (Running)
                {
                    int waitResult = WaitHandle.WaitAny(waitHandles, 2000, false);
                    if (waitResult == WaitHandle.WaitTimeout)
                    {
                        Debug.WriteLine("Timeout.");
                    }
                    else if (0 == waitResult)
                    {
                        Debug.WriteLine("0!");
                        if (NewEvent != null)
                            NewEvent(this, EventArgs.Empty);
                    }
                    else if (1 == waitResult)
                    {
                        Debug.WriteLine("1!");
                        // Do Something Else
                    }
                    else
                    {
                        Debug.WriteLine("Error!");
                    }
                }
            }
            finally
            {
                arEvent.Close();
            }
        }
    }
}
