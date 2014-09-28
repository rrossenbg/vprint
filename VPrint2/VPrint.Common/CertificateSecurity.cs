/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;
using VPrinting.Common;
using VPrinting;

namespace VPrint.Common
{
    /// <summary>
    /// 
    /// </summary>
    /// <see cref="http://ianreddy.wordpress.com/2011/02/14/sign-data-using-certificates-in-c/"/>
    /// <example>
    ///  var sec = new CertificateSecurity(X509FindType.FindBySerialNumber, Strings.CERTNUMBER, StoreLocation.LocalMachine);
    ///  if (sec.Loaded)
    ///  {
    ///     var signFilePath = new FileInfo(Path.ChangeExtension(fullFilePath, ".sgn"));
    ///     item.Signature = sec.SignData(bmp.ToArray());
    ///     File.WriteAllBytes(signFilePath.FullName, item.Signature);
    ///     item.FileInfoList.Add(signFilePath);
    ///   }
    /// </example>
    [Obfuscation(ApplyToMembers = true)]
    public class CertificateSecurity
    {
        private static X509Certificate2 ms_certificate;

        public bool Loaded
        {
            get
            {
                return ms_certificate != null;
            }
        }

        /// <summary>
        /// Finds a certificate using the X509FindType and Value in all stores
        /// </summary>
        /// <param name="findType"></param>
        /// <param name="findValue"></param>
        /// <returns></returns>
        public static X509Certificate2Collection FindCertificateInAllStores(X509FindType findType, string findValue)
        {
            //Get Certificates from Local Machine Store
            X509Certificate2Collection certificateCollectionLocalMachine = FindCertificateInStore(findType, findValue, StoreLocation.LocalMachine);

            //Get Certificates from Current User Store
            X509Certificate2Collection certificateCollectionCurrentUser = FindCertificateInStore(findType, findValue, StoreLocation.CurrentUser);

            //Merge Certificate/s collection
            certificateCollectionLocalMachine.AddRange(certificateCollectionCurrentUser);

            return certificateCollectionLocalMachine;
        }

        /// <summary>
        /// Gets certificate, using  the specified X509FindType and Value, from a store location
        /// </summary>
        /// <param name="findType"></param>
        /// <param name="findValue"></param>
        /// <param name="storeLocation"></param>
        /// <returns></returns>
        public static X509Certificate2Collection FindCertificateInStore(X509FindType findType, string findValue, StoreLocation storeLocation)
        {
            //Check in Local Machine Store
            X509Store store = new X509Store(StoreName.My, storeLocation);

            //Open Store
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            //Get the cert collection
            X509Certificate2Collection certificates = store.Certificates.Find(findType, findValue, false);

            //Close Store
            store.Close();

            return certificates;
        }

        public CertificateSecurity(X509FindType findType, string findValue, StoreLocation storeLocation)
        {
            if (ms_certificate == null)
            {
                lock (typeof(CertificateSecurity))
                {
                    var certificates = FindCertificateInStore(findType, findValue, storeLocation);
                    ms_certificate = (certificates != null && certificates.Count != 0) ? ms_certificate = certificates[0] : null;
                }
            }
        }

        public CertificateSecurity(byte[] buffer, string pass)
        {
            if (ms_certificate == null)
            {
                ms_certificate = new X509Certificate2(buffer, pass);
            }
        }

        /// <summary>
        /// Sign Data using Certificate
        /// </summary>
        /// <param name="certificate"></param>
        /// <param name="dataToBeSigned"></param>
        /// <returns></returns>
        public byte[] SignData(byte[] dataToBeSigned)
        {
            if (ms_certificate == null || !ms_certificate.HasPrivateKey)
                return null;

            lock (ms_certificate)
            {
                RSACryptoServiceProvider csp = (RSACryptoServiceProvider)ms_certificate.PrivateKey;
                SHA1Managed sha1 = new SHA1Managed();
                byte[] hash = sha1.ComputeHash(dataToBeSigned);
                var buffer = csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
                return buffer;
            }
        }

        public bool? Verify(byte[] dataToBeVerified, byte[] signatureData)
        {
            if (ms_certificate == null || !ms_certificate.HasPrivateKey)
                return null;

            lock (ms_certificate)
            {
                try
                {
                    RSACryptoServiceProvider csp = (RSACryptoServiceProvider)ms_certificate.PrivateKey;
                    SHA1Managed sha1 = new SHA1Managed();
                    byte[] hash = sha1.ComputeHash(dataToBeVerified);
                    bool result = csp.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA1"), signatureData);
                    return result;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
