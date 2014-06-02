/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System.Security;
using System.Security.Cryptography.X509Certificates;

namespace PremierTaxFree.PTFLib.Security
{
    public static class CertificatesUtils
    {
        // Strings.VScan_CertificateSerialNumber;
        /// <summary>
        /// Finds certificate by serial number
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        public static X509Certificate2 FindBySerialNumber(string serialNumber)
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            try
            {
                X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
                X509Certificate2Collection foundCertificates = (X509Certificate2Collection)
                    collection.Find(X509FindType.FindBySerialNumber, serialNumber, false);
                if (foundCertificates.Count == 0)
                    throw new SecurityException();
                return foundCertificates[0];
            }
            finally
            {
                store.Close();
            }
        }

        /// <summary>
        /// Finds certificate by issuer name
        /// </summary>
        /// <param name="issuerName"></param>
        /// <param name="validOnly"></param>
        /// <returns></returns>
        public static X509Certificate FindByIssuerName(string issuerName, bool validOnly)
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            try
            {
                X509CertificateCollection cert = store.Certificates.Find(X509FindType.FindByIssuerName, issuerName, validOnly);
                return cert[0];
            }
            finally
            {
                store.Close();
            }
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="serachType">X509FindType.FindByIssuerName</param>
        /// <param name="value">"localhost"</param>
        /// <param name="validOnly">false</param>
        /// <returns></returns>
        public static X509Certificate FindCertificateInStore(X509FindType serachType, object value, bool validOnly)
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            try
            {
                X509CertificateCollection foundCertificates = store.Certificates.Find(X509FindType.FindByIssuerName, value, validOnly);
                if (foundCertificates == null || foundCertificates.Count == 0)
                    throw new SecurityException();
                return foundCertificates[0];
            }
            finally
            {
                store.Close();
            }
        }
    }
}
