/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PTF.Reports.Common
{
    public class RijndaelCryptography
    {
        // Fields
        private byte[] _IV = Encoding.ASCII.GetBytes("@:LDkfif()_!xd23");
        private byte[] _key = Encoding.ASCII.GetBytes("982*723!EFjhdu;<");
        private readonly RijndaelManaged myRijndael = new RijndaelManaged();
        private UTF8Encoding textConverter;

        // Methods
        public RijndaelCryptography()
        {
            this.myRijndael.Mode = CipherMode.CBC;
            this.textConverter = new UTF8Encoding();
        }

        public string Decrypt(byte[] encrypted)
        {
            ICryptoTransform transform = this.myRijndael.CreateDecryptor(this._key, this._IV);
            MemoryStream stream = new MemoryStream(encrypted);
            CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read);
            byte[] fromEncrypt = new byte[encrypted.Length];
            stream2.Read(fromEncrypt, 0, fromEncrypt.Length);
            stream2.Close();
            return this.textConverter.GetString(fromEncrypt);
        }

        public byte[] Encrypt(string txtToEncrypt)
        {
            ICryptoTransform transform = this.myRijndael.CreateEncryptor(this._key, this._IV);
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write);
            byte[] toEncrypt = this.textConverter.GetBytes(txtToEncrypt);
            stream2.Write(toEncrypt, 0, toEncrypt.Length);
            stream2.FlushFinalBlock();
            stream2.Close();
            return stream.ToArray();
        }

        public virtual void GenKey()
        {
            this.myRijndael.GenerateKey();
            this.myRijndael.GenerateIV();
            this._key = this.myRijndael.Key;
            this._IV = this.myRijndael.IV;
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