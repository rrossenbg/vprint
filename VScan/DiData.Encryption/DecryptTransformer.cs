using System;
using System.Security.Cryptography;

namespace DiData.Encryption
{
	/// <summary>
	/// Helper class used to decrypt encrypted data using the Rijndael encryption algorithm and the IV
	/// and the private encryption key.
	/// This class is utilised by the Decryptor class while creating a CryptoStream on which to write decrypted 
	/// data.
	/// </summary>
	public class DecryptTransformer
	{
		private EncryptionAlgorithm algorithmID;
		
		//initialisation vector
		private byte[] initVec;

		public DecryptTransformer(EncryptionAlgorithm deCryptId)
		{
			algorithmID=deCryptId;
		}

		/// <summary>
		/// Method to create an symmetric decryptor object (of type ICryptoTransform) with the specified
		/// encryption key(passed in as byte array) and the IV.
		/// </summary>
		/// <param name="bytesKey"></param>
		/// <returns></returns>
		internal ICryptoTransform GetCryptoServiceProvider(byte[] bytesKey)
		{
			// Pick the provider.
			switch (algorithmID)
			{
				case EncryptionAlgorithm.Des:
				{
					DES des = new DESCryptoServiceProvider();
					des.Mode = CipherMode.CBC;
					des.Key = bytesKey;
					des.IV = initVec;
					return des.CreateDecryptor();
				}
				case EncryptionAlgorithm.TripleDes:
				{
					TripleDES des3 = new TripleDESCryptoServiceProvider();
					des3.Mode = CipherMode.CBC;
					return des3.CreateDecryptor(bytesKey, initVec);
				}
				case EncryptionAlgorithm.Rc2:
				{
					RC2 rc2 = new RC2CryptoServiceProvider();
					rc2.Mode = CipherMode.CBC;
					return rc2.CreateDecryptor(bytesKey, initVec);
				}
				case EncryptionAlgorithm.Rijndael:
				{
					Rijndael rijndael = new RijndaelManaged();
					rijndael.Mode = CipherMode.CBC;
					return rijndael.CreateDecryptor(bytesKey, initVec);
				} 
				default:
				{
					throw new CryptographicException("Algorithm ID '" + 
						algorithmID + 
						"' not supported.");
				}
			}
		} //end GetCryptoServiceProvider

		internal byte[] IV
		{
			set{initVec = value;}
		}


	}
}
