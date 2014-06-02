/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace PremierTaxFree.PTFLib.Security
{
    /// <summary>
    /// Protected scope class
    /// </summary>
    public class ProtectedScope : IDisposable
    {
        // Fields
        private const int SPI_SETSCREENSAVERRUNNING = 0x61;
        private const int SPIF_SENDCHANGE = 2;
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;

        // Methods
        /// <summary>
        /// Frees protected scope
        /// </summary>
        public void Dispose()
        {
            ShowStartMenu();
            EnableCTRLALTDEL();
        }

        /// <summary>
        /// Enables Ctrl-Alt-Del
        /// </summary>
        public static void EnableCTRLALTDEL()
        {
            string name = @"Software\Microsoft\Windows\CurrentVersion\Policies\System";
            RegistryKey currentUser = Registry.CurrentUser;
            if (currentUser.OpenSubKey(name) != null)
            {
                currentUser.DeleteSubKeyTree(name);
            }
        }

        [DllImport("user32.dll")]
        private static extern int FindWindow(string className, string windowText);

        /// <summary>
        /// Disables Ctrl-Alt-Del
        /// </summary>
        public static void KillCtrlAltDelete()
        {
            string str = "1";
            string subkey = @"Software\Microsoft\Windows\CurrentVersion\Policies\System";
            RegistryKey key = Registry.CurrentUser.CreateSubKey(subkey);
            key.SetValue("DisableTaskMgr", str);
            key.Close();
        }

        /// <summary>
        /// Hides start menu
        /// </summary>
        public static void KillStartMenu()
        {
            ShowWindow(FindWindow("Shell_TrayWnd", ""), 0);
        }

        [DllImport("user32.dll")]
        public static extern bool LockWorkStation();

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// Shows start menu
        /// </summary>
        public static void ShowStartMenu()
        {
            ShowWindow(FindWindow("Shell_TrayWnd", ""), 1);
        }

        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int command);

        /// <summary>
        /// Starts protected scope
        /// </summary>
        public void Start()
        {
            KillStartMenu();
            KillCtrlAltDelete();
        }

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfoA", SetLastError = true)]
        private static extern bool SystemParametersInfo(uint action, uint param, uint vparam, uint init);
    }
}