/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Xml;


namespace PremierTaxFree.PTFLib.Security
{
    public enum ProtectionOptions
    {
        None,
        Sign,
        Encript,
        SignAndEncript,
    }

    /// <summary>
    /// Protected XML class
    /// </summary>
    public sealed class ProtectedXml : XmlDocument, IDisposable
    {
        // Fields
        private ProtectionOptions m_options;
        private RSACryptoServiceProvider m_rsaKey;
        private TripleDES m_tdes;

        // Methods
        public ProtectedXml()
            : this(ProtectionOptions.Sign)
        {
        }

        public ProtectedXml(ProtectionOptions options)
        {
            this.m_options = options;

            //SETTING RSA KEY FOR SIGNING
            CspParameters parameters = new CspParameters { KeyContainerName = "XML_DSIG_RSA_KEY" };
            this.m_rsaKey = new RSACryptoServiceProvider(parameters);

            //SETTING TDES FOR ECRYPTION
            UTF8Encoding UTF8 = new UTF8Encoding();
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                const string passphrase = "ASDFGHJKLPOIUYTREWASDFGHJKLPOIUY";
                byte[] TDESKey = md5.ComputeHash(UTF8.GetBytes(passphrase));

                TripleDESCryptoServiceProvider tdesc = new TripleDESCryptoServiceProvider();
                // Step 3. Setup the encoder
                tdesc.Key = TDESKey;
                tdesc.Mode = CipherMode.ECB;
                tdesc.Padding = PaddingMode.PKCS7;
                this.m_tdes = tdesc;
            }

            //Create empty xml doc
            this.PreserveWhitespace = true;
            this.AppendChild(this.CreateXmlDeclaration("1.0", "UTF-8", null));
            XmlNode newChild = this.CreateElement("data");
            this.AppendChild(newChild);
            this.Protect();
        }

        ~ProtectedXml()
        {
            using (m_rsaKey) ;
            using (m_tdes) ;
        }

        /// <summary>
        /// Adds element to Xml document by dictionary object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public void AddElement(string name, IDictionary data)
        {
            Debug.Assert(!string.IsNullOrEmpty(name), "Name cannot be null");
            Debug.Assert(data != null, "Data cannot be null");

            this.UnProtect();

            XmlNode newChild = this.CreateElement(name);
            this.DocumentElement.AppendChild(newChild);

            foreach (string str in data.Keys)
            {
                XmlElement node = this.CreateElement(str);
                if (data[str] is IConvertible)
                {
                    node.InnerText = Convert.ToString(data[str], CultureInfo.InvariantCulture);
                }
                else if (data[str] is byte[])
                {
                    XmlCDataSection cdata = this.CreateCDataSection(Convert.ToBase64String((byte[])data[str]));
                    node.AppendChild(cdata);
                }
                else if (data[str] is Guid)
                {
                    XmlCDataSection cdata = this.CreateCDataSection(Convert.ToBase64String(((Guid)data[str]).ToByteArray()));
                    node.AppendChild(cdata);
                }
                else 
                    throw new NotImplementedException("Type is unknown");
                newChild.AppendChild(node);
            }

            this.Protect();
        }

        /// <summary>
        /// Writes xml document values to Datatable object
        /// </summary>
        /// <param name="table">Table has to be created with columns.</param>
        /// <param name="protect">True to protect xml back after reading</param>
        public void WriteTo(DataTable table, bool protect)
        {
            Debug.Assert(table != null, "Table can not be null");
            Debug.Assert(table.Columns.Count > 0, "Table should be initialized.");

            this.UnProtect();

            foreach (XmlNode node in this.DocumentElement.ChildNodes)
            {
                DataRow row = table.NewRow();

                foreach (XmlNode cnode in node.ChildNodes)
                {
                    DataColumn column = table.Columns[cnode.Name];
                    if (column == null)
                        throw new Exception("Cannot find column with name: " + cnode.Name);

                    if (column.DataType == typeof(byte[]))
                    {
                        byte[] buffer = Convert.FromBase64String(cnode.FirstChild.Value);
#if DEBUG && TEST
                        using (Image.FromStream(new MemoryStream(buffer))) ;
#endif
                        row[column] = buffer;
                    }
                    else if (column.DataType == typeof(Guid))
                    {
                        byte[] buffer = Convert.FromBase64String(cnode.FirstChild.Value);
                        row[column] = new Guid(buffer);
                    }
                    else
                    {
                        row[column] = Convert.ChangeType(cnode.InnerText, column.DataType, CultureInfo.InvariantCulture);
                    }
                }
                table.Rows.Add(row);
            }

            if (protect)
                this.Protect();
        }

        private void Protect()
        {
            switch (this.m_options)
            {
                case ProtectionOptions.None:
                    break;
                case ProtectionOptions.Encript:
                    this.EncryptXML(this.m_tdes);
                    break;
                case ProtectionOptions.Sign:
                    this.SignXml(this.m_rsaKey);
                    break;
                case ProtectionOptions.SignAndEncript:
                    this.SignXml(this.m_rsaKey);
                    this.EncryptXML(this.m_tdes);
                    break;
            }
        }

        private void UnProtect()
        {
            switch (this.m_options)
            {
                case ProtectionOptions.None:
                    break;
                case ProtectionOptions.Encript:
                    this.DecryptXML(this.m_tdes);
                    break;
                case ProtectionOptions.Sign:
                    this.VerifyXml(this.m_rsaKey);
                    this.UnSignXml();
                    break;
                case ProtectionOptions.SignAndEncript:
                    this.DecryptXML(this.m_tdes);
                    this.VerifyXml(this.m_rsaKey);
                    this.UnSignXml();
                    break;
            }
        }

        /// <summary>
        /// Free used resources
        /// </summary>
        public void Dispose()
        {
            using (m_rsaKey) { }
            using (m_tdes) { }
            GC.SuppressFinalize(this);
        }
    }
}
