/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PremierTaxFree.PTFLib.Threading
{
    /// <summary>
    /// Windows event sender
    /// </summary>
    public class EventSender : IDisposable
    {
        private IntPtr mHandle;

        [DllImport("Kernel32.dll", SetLastError = true, 
            CallingConvention = CallingConvention.Winapi, 
            CharSet = CharSet.Auto)]
        public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, 
            [In, MarshalAs(UnmanagedType.Bool)] bool bManualReset, 
            [In, MarshalAs(UnmanagedType.Bool)] bool bIntialState, 
            [In, MarshalAs(UnmanagedType.BStr)] string lpName);

        [DllImport("Kernel32.dll", SetLastError = true, 
            CallingConvention = CallingConvention.Winapi, 
            CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("Kernel32.dll", SetLastError = true)]
        private static extern bool SetEvent(IntPtr hEvent);

        [DllImport("Kernel32.dll", SetLastError = true)]
        private static extern bool ResetEvent(IntPtr hEvent);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public EventSender(string name)
        {
            Debug.Assert(!string.IsNullOrEmpty(name));
            mHandle = CreateEvent(IntPtr.Zero, false, true, name);
        }

        /// <summary>
        /// Set windows event
        /// </summary>
        public void Set()
        {
            SetEvent(mHandle);
            ResetEvent(mHandle);
        }

        public void Dispose()
        {
            CloseHandle(mHandle);
        }
    }
}
