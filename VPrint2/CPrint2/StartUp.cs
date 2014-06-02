/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System.Windows.Forms;
using Microsoft.Win32;

namespace CPrint2
{
    public class StartUp 
    {
        const string PATH = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        public static void TryToAddApp()
        {
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(PATH, true);

            if (!IsStartupItem())
                rkApp.SetValue(Application.ProductName, Application.ExecutablePath);
        }

        public static void TryToRemoveApp()
        {
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(PATH, true);

            if (IsStartupItem())
                rkApp.DeleteValue(Application.ProductName, false);
        }

        private static bool IsStartupItem()
        {
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(PATH, true);
            return (rkApp.GetValue(Application.ProductName) != null);
        }
    }
}
