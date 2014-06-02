/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PremierTaxFree.PTFLib.Security
{
    public class RijndaelCryptography
    {
        // Fields
        private byte[] _IV;
        private byte[] _key;
        private byte[] encrypted;
        private byte[] fromEncrypt;
        private RijndaelManaged myRijndael = new RijndaelManaged();
        private ASCIIEncoding textConverter;
        private byte[] toEncrypt;

        // Methods
        public RijndaelCryptography()
        {
            this.myRijndael.Mode = CipherMode.CBC;
            this.textConverter = new ASCIIEncoding();
        }

        public string Decrypt(byte[] crypted)
        {
            ICryptoTransform transform = this.myRijndael.CreateDecryptor(this._key, this._IV);
            MemoryStream stream = new MemoryStream(crypted);
            CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read);
            this.fromEncrypt = new byte[crypted.Length];
            stream2.Read(this.fromEncrypt, 0, this.fromEncrypt.Length);
            stream2.Close();
            return this.textConverter.GetString(this.fromEncrypt);
        }

        public void Encrypt(string TxtToEncrypt)
        {
            ICryptoTransform transform = this.myRijndael.CreateEncryptor(this._key, this._IV);
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write);
            this.toEncrypt = this.textConverter.GetBytes(TxtToEncrypt);
            stream2.Write(this.toEncrypt, 0, this.toEncrypt.Length);
            stream2.FlushFinalBlock();
            stream2.Close();
            this.encrypted = stream.ToArray();
        }

        public virtual void GenKey()
        {
            this.myRijndael.GenerateKey();
            this.myRijndael.GenerateIV();
            this._key = this.myRijndael.Key;
            this._IV = this.myRijndael.IV;
        }

        // Properties
        public byte[] Encrypted
        {
            get
            {
                return this.encrypted;
            }
        }

        public byte[] IV
        {
            get
            {
                return this._IV;
            }
            set
            {
                this._IV = value;
            }
        }

        public byte[] Key
        {
            get
            {
                return this._key;
            }
            set
            {
                this._key = value;
            }
        }
    }
}
 
