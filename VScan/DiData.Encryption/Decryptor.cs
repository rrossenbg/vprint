using System;
using System.Security.Cryptography;
using System.IO;

namespace DiData.Encryption
{
	/// <summary>
	/// Decryptor is a helper class which provides methods to decrypt an encrypted message using a private key
	/// and writed the decrypted data (as bytes of array) to a memory stream.
	/// Symmetric encryption is used in TRS which means a single key is used to encrypt and decrypt information.
	/// Rijndael algorithm, an advanced AES encryption algorithm, has been used for encryption/decryption.
	/// </summary>
	public class Decryptor
	{
		private DecryptTransformer transformer;
		private byte[] initVec;

		//~ctor
		public Decryptor(EncryptionAlgorithm algId)
		{
			transformer = new DecryptTransformer(algId);
		}
	 
		/// <summary>
		/// Decrypt the data using the symmetric private key and rijndael algorithm
		/// </summary>
		/// <param name="bytesData">data to be decrypted</param>
		/// <param name="bytesKey"></param>
		/// <returns>decrypted data as byte array</returns>
		public byte[] Decrypt(byte[] bytesData, byte[] bytesKey)
		{
			//Set up the memory stream for the decrypted data.
			MemoryStream memStreamDecryptedData = new MemoryStream();

			//Pass in the initialization vector.
			transformer.IV = initVec;

			//Create the symmetric decryptor transformer object
			ICryptoTransform transform = transformer.GetCryptoServiceProvider(bytesKey);

			//Initialise the CryptoStream class with the memory stream on which to write decrypted data, 
			//a decryptor object created by the Rijndael.CreateDecryptor() method and a mode (read/write).
			CryptoStream decStream = new CryptoStream(	memStreamDecryptedData,
														transform,
														CryptoStreamMode.Write);
			try
			{
				//Write the decrypted data to the CryptoStream and advance the position of the pointer
				//by the number of bytes written.
				decStream.Write(bytesData, 0, bytesData.Length);
			}
			catch(Exception ex)
			{
				throw new Exception("Error while writing encrypted data to the 	stream: \n" 
					+ ex.Message);
			}
			decStream.FlushFinalBlock();
			decStream.Close();
			// Send the data back.
			return memStreamDecryptedData.ToArray();
		} //end Decrypt

		public byte[] IV
		{
			set{initVec = value;}
		}


	}
}
