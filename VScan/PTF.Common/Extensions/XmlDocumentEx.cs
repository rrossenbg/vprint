/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace PremierTaxFree.PTFLib
{
    /// <summary>
    /// Encrypt / Decrypt XML
    /// </summary>
    /// <see cref="http://msdn.microsoft.com/en-us/library/ms229746.aspx"/>
    public static class XmlDocumentEx
    {
        // Part - I
        /// <summary>
        /// Decripts an XmlDocument
        /// </summary>
        /// <param name="doc"></param>
        public static void Decrypt(this XmlDocument doc)
        {
            if (doc == null)
            {
                throw new ArgumentNullException("Doc");
            }
            new EncryptedXml(doc).DecryptDocument();
        }

        /// <summary>
        /// Decripts an XmlDocument by triple desc key
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="tdes"></param>
        public static void DecryptXML(this XmlDocument doc, TripleDES tdes)
        {
            XmlElement element = (XmlElement)doc.GetElementsByTagName("EncryptedData")[0];
            EncryptedData encryptedData = new EncryptedData();
            encryptedData.LoadXml(element);
            EncryptedXml xml = new EncryptedXml();
            byte[] decryptedData = xml.DecryptData(encryptedData, tdes);
            xml.ReplaceData(element, decryptedData);
        }

        /// <summary>
        /// Encripts an XmlDocument by a certificate and element name
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="elementToEncrypt"></param>
        /// <param name="cert"></param>
        public static void Encrypt(this XmlDocument doc, string elementToEncrypt, X509Certificate2 cert)
        {
            if (doc == null)
            {
                throw new ArgumentNullException("Doc");
            }
            if (elementToEncrypt == null)
            {
                throw new ArgumentNullException("ElementToEncrypt");
            }
            if (cert == null)
            {
                throw new ArgumentNullException("Cert");
            }
            XmlElement inputElement = doc.GetElementsByTagName(elementToEncrypt)[0] as XmlElement;
            if (inputElement == null)
            {
                throw new XmlException("The specified element was not found");
            }
            EncryptedData encryptedData = new EncryptedXml().Encrypt(inputElement, cert);
            EncryptedXml.ReplaceElement(inputElement, encryptedData, false);
        }

        /// <summary>
        /// Encripts an XmlDocument by tripl desc key
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="tdes"></param>
        public static void EncryptXML(this XmlDocument doc, TripleDES tdes)
        {
            byte[] buffer = new EncryptedXml(doc).EncryptData(doc.DocumentElement, tdes, false);
            EncryptedData encryptedData = new EncryptedData
            {
                EncryptionMethod = new EncryptionMethod("http://www.w3.org/2001/04/xmlenc#tripledes-cbc"),
                Type = "http://www.w3.org/2001/04/xmlenc#Element",
                CipherData = new CipherData()
            };
            encryptedData.CipherData.CipherValue = buffer;
            EncryptedXml.ReplaceElement(doc.DocumentElement, encryptedData, false);
        }

        /// <summary>
        /// Signs an XmlDocument by RSA key
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="key"></param>
        public static void SignXml(this XmlDocument doc, RSA key)
        {
            if (doc == null)
            {
                throw new ArgumentException("Doc");
            }
            if (key == null)
            {
                throw new ArgumentException("Key");
            }
            SignedXml xml = new SignedXml(doc)
            {
                SigningKey = key
            };
            Reference reference = new Reference
            {
                Uri = ""
            };
            XmlDsigEnvelopedSignatureTransform transform = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(transform);
            xml.AddReference(reference);
            xml.ComputeSignature();
            XmlElement node = xml.GetXml();
            doc.DocumentElement.AppendChild(doc.ImportNode(node, true));
        }

        /// <summary>
        /// Unsigns an XmlDocument
        /// </summary>
        /// <param name="doc"></param>
        public static void UnSignXml(this XmlDocument doc)
        {
            if (doc == null)
            {
                throw new ArgumentException("Doc");
            }
            for (XmlNodeList list = doc.GetElementsByTagName("Signature"); (list != null) && (list.Count > 0); list = doc.GetElementsByTagName("Signature"))
            {
                XmlNode oldChild = list[0];
                oldChild.ParentNode.RemoveChild(oldChild);
            }
        }

        /// <summary>
        /// Verifies the XmlDocument signature
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="key"></param>
        public static void VerifyXml(this XmlDocument doc, RSA key)
        {
            if (doc == null)
            {
                throw new ArgumentException("Doc");
            }
            if (key == null)
            {
                throw new ArgumentException("Key");
            }
            SignedXml xml = new SignedXml(doc);
            XmlNodeList elementsByTagName = doc.GetElementsByTagName("Signature");
            if (elementsByTagName.Count <= 0)
            {
                throw new CryptographicException("Verification failed: No Signature was found in the document.");
            }
            if (elementsByTagName.Count >= 2)
            {
                throw new CryptographicException("Verification failed: More that one signature was found for the document.");
            }
            xml.LoadXml((XmlElement)elementsByTagName[0]);
            if (!xml.CheckSignature(key))
            {
                throw new CryptographicException("Verification failed: The XML signature is not valid.");
            }
        }

        //Part - II

        /// <summary>
        /// Encripts an XmlDocument by RSA algorithm
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="elementToEnc"></param>
        /// <param name="encryptionElementID"></param>
        /// <param name="alg"></param>
        /// <param name="keyName"></param>
        public static void Encrypt(this XmlDocument doc, string elementToEnc, string encryptionElementID, RSA alg, string keyName)
        {
            // Check the arguments.
            if (doc == null)
                throw new ArgumentNullException("Doc");
            if (elementToEnc == null)
                throw new ArgumentNullException("ElementToEncrypt");
            if (encryptionElementID == null)
                throw new ArgumentNullException("EncryptionElementID");
            if (alg == null)
                throw new ArgumentNullException("Alg");
            if (keyName == null)
                throw new ArgumentNullException("KeyName");

            ////////////////////////////////////////////////
            // Find the specified element in the XmlDocument
            // object and create a new XmlElemnt object.
            ////////////////////////////////////////////////
            XmlElement elementToEncrypt = doc.GetElementsByTagName(elementToEnc)[0] as XmlElement;

            // Throw an XmlException if the element was not found.
            if (elementToEncrypt == null)
            {
                throw new XmlException("The specified element was not found");
            }

            RijndaelManaged sessionKey = null;

            try
            {
                //////////////////////////////////////////////////
                // Create a new instance of the EncryptedXml class
                // and use it to encrypt the XmlElement with the
                // a new random symmetric key.
                //////////////////////////////////////////////////

                // Create a 256 bit Rijndael key.
                sessionKey = new RijndaelManaged();
                sessionKey.KeySize = 256;

                EncryptedXml eXml = new EncryptedXml();

                byte[] encryptedElement = eXml.EncryptData(elementToEncrypt, sessionKey, false);
                ////////////////////////////////////////////////
                // Construct an EncryptedData object and populate
                // it with the desired encryption information.
                ////////////////////////////////////////////////

                EncryptedData edElement = new EncryptedData();
                edElement.Type = EncryptedXml.XmlEncElementUrl;
                edElement.Id = encryptionElementID;
                // Create an EncryptionMethod element so that the
                // receiver knows which algorithm to use for decryption.

                edElement.EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncAES256Url);
                // Encrypt the session key and add it to an EncryptedKey element.
                EncryptedKey ek = new EncryptedKey();

                byte[] encryptedKey = EncryptedXml.EncryptKey(sessionKey.Key, alg, false);

                ek.CipherData = new CipherData(encryptedKey);

                ek.EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncRSA15Url);

                // Create a new DataReference element
                // for the KeyInfo element.  This optional
                // element specifies which EncryptedData
                // uses this key.  An XML document can have
                // multiple EncryptedData elements that use
                // different keys.
                DataReference dRef = new DataReference();

                // Specify the EncryptedData URI.
                dRef.Uri = "#" + encryptionElementID;

                // Add the DataReference to the EncryptedKey.
                ek.AddReference(dRef);
                // Add the encrypted key to the
                // EncryptedData object.

                edElement.KeyInfo.AddClause(new KeyInfoEncryptedKey(ek));
                // Set the KeyInfo element to specify the
                // name of the RSA key.


                // Create a new KeyInfoName element.
                KeyInfoName kin = new KeyInfoName();

                // Specify a name for the key.
                kin.Value = keyName;

                // Add the KeyInfoName element to the
                // EncryptedKey object.
                ek.KeyInfo.AddClause(kin);
                // Add the encrypted element data to the
                // EncryptedData object.
                edElement.CipherData.CipherValue = encryptedElement;
                ////////////////////////////////////////////////////
                // Replace the element from the original XmlDocument
                // object with the EncryptedData element.
                ////////////////////////////////////////////////////
                EncryptedXml.ReplaceElement(elementToEncrypt, edElement, false);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (sessionKey != null)
                {
                    sessionKey.Clear();
                }
            }
        }

        /// <summary>
        /// Decripts and XmlDocument by RSA algorithm
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="alg"></param>
        /// <param name="keyName"></param>
        public static void Decrypt(this XmlDocument doc, RSA alg, string keyName)
        {
            // Check the arguments.  
            if (doc == null)
                throw new ArgumentNullException("Doc");
            if (alg == null)
                throw new ArgumentNullException("Alg");
            if (keyName == null)
                throw new ArgumentNullException("KeyName");

            // Create a new EncryptedXml object.
            EncryptedXml exml = new EncryptedXml(doc);

            // Add a key-name mapping.
            // This method can only decrypt documents
            // that present the specified key name.
            exml.AddKeyNameMapping(keyName, alg);

            // Decrypt the element.
            exml.DecryptDocument();
        }
    }
}
