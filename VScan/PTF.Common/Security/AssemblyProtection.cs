/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Diagnostics;
using System.Security;
using System.Security.Cryptography.X509Certificates;

namespace PremierTaxFree.PTFLib.Security
{
    public static class AssemblyProtection
    {
        /// <summary>
        /// Verifies assembly there is a certain certificate into machine cerficate store
        /// </summary>
        public static void Verify()
        {
            X509Store store = new X509Store("MY", StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            try
            {
                X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
                X509Certificate2Collection foundCertificates = (X509Certificate2Collection)
                    collection.Find(X509FindType.FindBySerialNumber, Strings.VScan_CertificateSerialNumber, false);
                if (foundCertificates.Count == 0)
                    throw new SecurityException();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex, "ERR: AssemblyProtection::Verify");
                throw;
            }
            finally
            {
                store.Close();
            }
        }
    }
}
