/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using PremierTaxFree.PTFLib.Native;

namespace PremierTaxFree.PTFLib.Sys
{
    public static class WinMsg
    {
        /// <summary>
        /// Sends text to recipient window
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="text"></param>
        public static void SendText(IntPtr recipient, string text)
        {
            SendText(IntPtr.Zero, recipient, text);
        }

        /// <summary>
        /// Sends text to recipient window adding sender address in it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="recipient"></param>
        /// <param name="text"></param>
        public static void SendText(IntPtr sender, IntPtr recipient, string text)
        {
            Debug.Assert(recipient != IntPtr.Zero, "Recipient hWnd my not be zero");
            Debug.Assert(!string.IsNullOrEmpty(text), "Send text may not be empty");

            user32.CopyDataStruct cds = new user32.CopyDataStruct();
            try
            {
                string newText = string.Format("{0}!{1}", sender, text);
                cds.cbData = (newText.Length + 1) * 2;
                cds.lpData = user32.LocalAlloc(0x40, cds.cbData);
                Marshal.Copy(newText.ToCharArray(), 0, cds.lpData, newText.Length);
                cds.dwData = (IntPtr)1;
                user32.SendMessage(recipient, user32.WM_COPYDATA, IntPtr.Zero, ref cds);
            }
            finally
            {
                cds.Dispose();
            }
        }


        /// <summary>
        /// Receives a text from windows messaging system
        /// </summary>
        /// <param name="lparam"></param>
        /// <returns></returns>
        /// <example>
        ////protected override void WndProc(ref Message m)
        ////{
        ////    switch (m.Msg)
        ////    {
        ////        case Win32.WM_COPYDATA:
        ////            {
        ////                string text = WinMsg.ReceiveText(m.LParam);
        ////                rtbLog.AppendText(text);
        ////            }
        ////            break;
        ////        default:
        ////            base.WndProc(ref m);
        ////            break;
        ////    }
        ////}
        /// </example>
        public static string ReceiveText(IntPtr lparam)
        {
            IntPtr sender;
            return ReceiveText(lparam, out sender);
        }

        /// <summary>
        /// Receives a text from windows messageing system. Shows sender handler if available.
        /// </summary>
        /// <param name="lparam"></param>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static string ReceiveText(IntPtr lparam, out IntPtr sender)
        {
            user32.CopyDataStruct data = (user32.CopyDataStruct)Marshal.PtrToStructure(lparam, typeof(user32.CopyDataStruct));
            string message = Marshal.PtrToStringUni(data.lpData);
            int index = message.IndexOf("!");
            sender = new IntPtr(int.Parse(message.Substring(0, index)));
            string newText = message.Substring(index + 1);
            return newText;
        }
    }
}
