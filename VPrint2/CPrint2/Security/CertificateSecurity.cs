using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CPrint2
{
    /// <summary>
    /// 
    /// </summary>
    /// <see cref="http://ianreddy.wordpress.com/2011/02/14/sign-data-using-certificates-in-c/"/>
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
                //Create a RSA Provider, using the private key
                RSACryptoServiceProvider rsaCryptoServiceProvider = (RSACryptoServiceProvider)ms_certificate.PrivateKey;
                //Sign the data using a desired hashing algorithm
                return rsaCryptoServiceProvider.SignData(dataToBeSigned, new SHA1CryptoServiceProvider());
            }
        }
    }
}
