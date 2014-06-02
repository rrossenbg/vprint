using System;
using System.Security.Cryptography;
using System.IO;

namespace DiData.Encryption
{
	/// <summary>
	/// Encryptor is a helper class which provides methods to encrypt a message using a private key
	/// into a memory stream.
	/// Symmetric encryption is used in TRS which means a single key is used to encrypt and decrypt information.
	/// Rijndael algorithm, an advanced AES encryption algorithm, has been used for encryption/decryption.
	/// </summary>
	public class Encryptor
	{
		private EncryptTransformer transformer;
		private byte[] initVec;
		private byte[] encKey;

		public Encryptor(EncryptionAlgorithm algId)
		{
			transformer=new EncryptTransformer(algId);	
		}
			
		/// <summary>
		/// Method to encrypt a message using the Rjindael algorithm and the private encryption key
		/// </summary>
		/// <param name="bytesData">Data to encrypt (in byte array format)</param>
		/// <param name="bytesKey">byte array containing the encryption key</param>
		/// <returns>Encrypted data</returns>
		public byte[] Encrypt(byte[] bytesData, byte[] bytesKey)
		{
			//Set up the stream that will hold the encrypted data.
			MemoryStream memStreamEncryptedData = new MemoryStream();

			transformer.IV = initVec;
			//Create a ICryptoTransform object defining the basic operations of encryption
			//i.e. using the private Key and the IV
			ICryptoTransform transform = transformer.GetCryptoServiceProvider(bytesKey);

			//Create a CryptoStream to which encrypted data will be written using the above created
			//ICryptoTransform object
			CryptoStream encStream = new CryptoStream(	memStreamEncryptedData, 
														transform,
														CryptoStreamMode.Write);
			try
			{
				//Encrypt the data, write it to the memory stream.
				encStream.Write(bytesData, 0, bytesData.Length);
			}
			catch(Exception ex)
			{
				throw new Exception("Error while writing encrypted data to the stream: \n" 
					+ ex.Message);
			}
			//Set the IV and key for the client to retrieve
			encKey = transformer.Key;
			initVec = transformer.IV;
			encStream.FlushFinalBlock();
			encStream.Close();

			//Send the data back.
			return memStreamEncryptedData.ToArray();
		}//end Encrypt

		public byte[] IV
		{
			get{return initVec;}
			set{initVec = value;}
		}

		public byte[] Key
		{
			get{return encKey;}
		}

	}
}
