/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace PremierTaxFree.PTFLib.Security
{
    public static class SecurityUtils
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, 
            int dwLogonType, int dwLogonProvider, out IntPtr phToken);

        [DllImport("advapi32.dll", SetLastError = true)]
        private extern static bool DuplicateToken(IntPtr ExistingTokenHandle, int SECURITY_IMPERSONATION_LEVEL, 
            out IntPtr DuplicateTokenHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hHandle);

        private const int LOGON32_LOGON_INTERACTIVE = 2;
        private const int LOGON32_LOGON_NETWORK = 3;
        private const int LOGON32_LOGON_BATCH = 4;
        private const int LOGON32_LOGON_SERVICE = 5;
        private const int LOGON32_LOGON_UNLOCK = 7;
        private const int LOGON32_LOGON_NETWORK_CLEARTEXT = 8;
        private const int LOGON32_LOGON_NEW_CREDENTIALS = 9;
        private const int LOGON32_PROVIDER_DEFAULT = 0;

        /// <summary>
        /// Tries logging in as an user. Safe. Returns true/false.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="domain"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool TryLogin(string username, string domain, string password)
        {
            IntPtr hToken = IntPtr.Zero;
            IntPtr hTokenDuplicate = IntPtr.Zero;
            try
            {
                if (LogonUser(username, domain, password,
                     LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, out hToken))
                {
                    if (DuplicateToken(hToken, 2, out hTokenDuplicate))
                    {
                        WindowsIdentity windowsIdentity = new WindowsIdentity(hTokenDuplicate);
                        WindowsImpersonationContext impersonationContext = windowsIdentity.Impersonate();
                        impersonationContext.Undo();
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (hToken != IntPtr.Zero)
                    CloseHandle(hToken);

                if (hTokenDuplicate != IntPtr.Zero)
                    CloseHandle(hTokenDuplicate);
            }
        }
    }
}
