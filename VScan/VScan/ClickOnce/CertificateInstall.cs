using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;

namespace PremierTaxFree
{
    public class CertificateInstall
    {
        public static void TryAddInStore(X509Certificate2 cert, StoreName storeName, StoreLocation storeLocation, string user)
        {
            X509Store store = new X509Store(storeName, storeLocation);

            try
            {
                store.Open(OpenFlags.ReadWrite);

                if (!store.Certificates.Contains(cert))
                    store.Add(cert);

                int indexOfCert = store.Certificates.IndexOf(cert);
                X509Certificate2 certInStore = store.Certificates[indexOfCert];

                if (!string.IsNullOrEmpty(user))
                    AddAccessToCertificate(certInStore, user);
            }
            finally
            {
                store.Close();
            }
        }

        public static void TryRemoveFromStore(X509Certificate2 cert, StoreName storeName, StoreLocation storeLocation)
        {
            X509Store store = new X509Store(storeName, storeLocation);

            try
            {
                store.Open(OpenFlags.ReadWrite);

                if (store.Certificates.Contains(cert))
                    store.Remove(cert);
            }
            finally
            {
                store.Close();
            }
        }

        public static void RemoveFromStore(string name, StoreName storeName, StoreLocation storeLocation)
        {
            X509Store store = new X509Store(storeName, storeLocation);

            try
            {
                store.Open(OpenFlags.ReadWrite);

                foreach (X509Certificate2 cert in store.Certificates)
                {
                    if (cert.SubjectName.Name.StartsWith("CN=WSE2QuickStartServer"))
                    {
                        store.Remove(cert);
                        break;
                    }
                }
            }
            finally
            {
                store.Close();
            }
        }

        public static void AddAccessToCertificate(X509Certificate2 cert, string user)
        {
            RSACryptoServiceProvider rsa = cert.PrivateKey as RSACryptoServiceProvider;

            if (rsa != null)
            {
                string keyfilepath =
                    FindKeyLocation(rsa.CspKeyContainerInfo.UniqueKeyContainerName);

                FileInfo file = new FileInfo(keyfilepath + "\\" +
                    rsa.CspKeyContainerInfo.UniqueKeyContainerName);

                FileSecurity fs = file.GetAccessControl();

                NTAccount account = new NTAccount(user);

                fs.AddAccessRule(new FileSystemAccessRule(account,
                    FileSystemRights.FullControl, AccessControlType.Allow));

                file.SetAccessControl(fs);
            }
        }

        private static string FindKeyLocation(string keyFileName)
        {
            string text1 = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string text2 = text1 + @"\Microsoft\Crypto\RSA\MachineKeys";
            string[] textArray1 = Directory.GetFiles(text2, keyFileName);
            if (textArray1.Length > 0)
            {
                return text2;
            }
            string text3 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string text4 = text3 + @"\Microsoft\Crypto\RSA\";
            textArray1 = Directory.GetDirectories(text4);
            if (textArray1.Length > 0)
            {
                foreach (string text5 in textArray1)
                {
                    textArray1 = Directory.GetFiles(text5, keyFileName);
                    if (textArray1.Length != 0)
                    {
                        return text5;
                    }
                }
            }
            return
                "Private key exists but is not accessible";
        }
    }
}