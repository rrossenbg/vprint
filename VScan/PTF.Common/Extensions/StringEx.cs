/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace PremierTaxFree.PTFLib
{
    public static class StringEx
    {
        /// <summary>
        /// Compares two string for equality
        /// </summary>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool Equals(this string value, StringComparison options, params string[] values)
        {
            foreach (var str in values)
                if (string.Equals(value, str, options))
                    return true;
            return false;
        }

        /// <summary>
        /// Returns a string having the first N chars of current string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Top(this string value, int length)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value.Substring(0, value.Length < length ? value.Length : length);
        }

        /// <summary>
        /// Returns a string having the first N chars of current string. Concats the final string to it.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <param name="finalString"></param>
        /// <returns></returns>
        public static string Top(this string value, int length, string finalString)
        {
            if (string.IsNullOrEmpty(value) || value.Length < length)
                return value;

            return
                string.Concat(value.Substring(0, length), finalString);
        }

        /// <summary>
        /// Trims a string safely
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string TrimSafe(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            return value.Trim();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string LowerSafe(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            return value.ToLowerInvariant();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UpperSafe(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            return value.ToUpperInvariant();
        }

        /// <summary>
        /// Repleases last last few chars by new chars
        /// </summary>
        /// <param name="b"></param>
        /// <param name="oldChar"></param>
        /// <param name="newChar"></param>
        public static void ReplaceLast(this StringBuilder b, char oldChar, char newChar)
        {
            for (int i = b.Length - 1; i >= 0; i--)
            {
                if (b[i] == oldChar)
                {
                    b.Replace(oldChar, newChar, i, 1);
                    return;
                }
            }
        }

        /// <summary>
        /// Replease all chars in a string that match chars in a char array
        /// </summary>
        /// <param name="string"></param>
        /// <param name="chars"></param>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static string ReplaceAll(this string @string, char[] chars, char ch)
        {
            Debug.Assert(!string.IsNullOrEmpty(@string));

            foreach (char c in chars)
                @string = @string.Replace(c, ch);

            return @string;
        }

        /// <summary>
        /// Generates a random string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Random(this string str, int length)
        {
            System.Random rnd = new Random(DateTime.Now.Millisecond);
            byte[] buffer = new byte[length];
            rnd.NextBytes(buffer);
            return UnicodeEncoding.Default.GetString(buffer);
        }

        /// <summary>
        /// Creates an uniqueue string starting with input string.
        /// Ending with an GUID
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Uniqueue(this string value)
        {
            return string.Concat(value, Guid.NewGuid());
        }

        #region TripleDESCryptography

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="passphrase"></param>
        /// <returns></returns>
        /// <example>
        /// string Msg = "This world is round, not flat, don't believe them!";
        /// string Password = "secret";
        /// string EncryptedString = Msg.Encrypt(Password);
        /// string DecryptedString = EncryptedString.Decrypt(Password);
        /// </example>
        public static string Encrypt(this string text, string passphrase)
        {
            byte[] Results;
            UTF8Encoding UTF8 = new UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the encoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] DataToEncrypt = UTF8.GetBytes(text);

            // Step 5. Attempt to encrypt the string
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the encrypted string as a base64 encoded string
            return Convert.ToBase64String(Results);
        }

        public static string Decrypt(this string text, string passphrase)
        {
            byte[] Results;
            UTF8Encoding UTF8 = new UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the decoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] DataToDecrypt = Convert.FromBase64String(text);

            // Step 5. Attempt to decrypt the string
            try
            {
                ICryptoTransform decryptor = TDESAlgorithm.CreateDecryptor();
                Results = decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the decrypted string in UTF8 format
            return UTF8.GetString(Results);
        }

        #endregion

        #region DESCryptography

        // Fields
        private const string EMPTY = "<%@EMPTY@%>";

        public static string Encrypt(this string originalString, byte[] key)
        {
            if (string.IsNullOrEmpty(originalString))
            {
                originalString = EMPTY;
            }
            if (key.Length < 8)
            {
                throw new ArgumentException("Password key length should be nore than 8");
            }
            byte[] destinationArray = new byte[8];
            Array.Copy(key, destinationArray, 8);
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(destinationArray, destinationArray), CryptoStreamMode.Write);
            StreamWriter writer = new StreamWriter(stream2);
            writer.Write(originalString);
            writer.Flush();
            stream2.FlushFinalBlock();
            writer.Flush();
            return Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length);
        }

        // Methods
        public static string Decrypt(this string cryptedString, byte[] key)
        {
            if (string.IsNullOrEmpty(cryptedString))
            {
                throw new ArgumentNullException("The string which needs to be decrypted can not be null.");
            }
            if (key.Length < 8)
            {
                throw new ArgumentException("Password key length should be nore than 8");
            }
            byte[] destinationArray = new byte[8];
            Array.Copy(key, destinationArray, 8);
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            MemoryStream stream = new MemoryStream(Convert.FromBase64String(cryptedString));
            CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(destinationArray, destinationArray), CryptoStreamMode.Read);
            string str = new StreamReader(stream2).ReadToEnd();
            if (string.CompareOrdinal(str, EMPTY) == 0)
            {
                return string.Empty;
            }
            return str;
        }

        #endregion

        /// <summary>
        /// Generates a hash for the given plain text value and returns a
        /// base64-encoded result. Before the hash is computed, a random salt
        /// is generated and appended to the plain text. This salt is stored at
        /// the end of the hash value, so it can be used later for hash
        /// verification.
        /// </summary>
        /// <param name="text">
        /// Plaintext value to be hashed. The function does not check whether
        /// this parameter is null.
        /// </param>
        /// <param name="hashAlgorithm">
        /// Name of the hash algorithm. Allowed values are: "MD5", "SHA1",
        /// "SHA256", "SHA384", and "SHA512" (if any other value is specified
        /// MD5 hashing algorithm will be used). This value is case-insensitive.
        /// </param>
        /// <param name="saltBytes">
        /// Salt bytes. This parameter can be null, in which case a random salt
        /// value will be generated.
        /// </param>
        /// <returns>
        /// Hash value formatted as a base64-encoded string.
        /// </returns>
        /// <example>
        /// string text = "ROSSEN";
        /// string code = text.ComputeHash();
        /// bool yes = text.VerifyHash(HashType.MD5, code);
        /// </example>
        public static string ComputeHash(this string text, HashType hashAlgorithm, byte[] saltBytes)
        {
            // If salt is not specified, generate it on the fly.
            if (saltBytes == null)
            {
                // Define min and max salt sizes.
                int minSaltSize = 4;
                int maxSaltSize = 8;

                // Generate a random number for the size of the salt.
                Random random = new Random();
                int saltSize = random.Next(minSaltSize, maxSaltSize);

                // Allocate a byte array, which will hold the salt.
                saltBytes = new byte[saltSize];

                // Initialize a random number generator.
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

                // Fill the salt with cryptographically strong byte values.
                rng.GetNonZeroBytes(saltBytes);
            }

            // Convert plain text into a byte array.
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(text);

            // Allocate array, which will hold plain text and salt.
            byte[] plainTextWithSaltBytes = new byte[plainTextBytes.Length + saltBytes.Length];

            // Copy plain text bytes into resulting array.
            for (int i = 0; i < plainTextBytes.Length; i++)
                plainTextWithSaltBytes[i] = plainTextBytes[i];

            // Append salt bytes to the resulting array.
            for (int i = 0; i < saltBytes.Length; i++)
                plainTextWithSaltBytes[plainTextBytes.Length + i] = saltBytes[i];

            // Because we support multiple hashing algorithms, we must define
            // hash object as a common (abstract) base class. We will specify the
            // actual hashing algorithm class later during object creation.
            HashAlgorithm hash;

            // Initialize appropriate hashing algorithm class.
            switch (hashAlgorithm)
            {
                case HashType.SHA1:
                    hash = new SHA1Managed();
                    break;

                case HashType.SHA256:
                    hash = new SHA256Managed();
                    break;

                case HashType.SHA384:
                    hash = new SHA384Managed();
                    break;

                case HashType.SHA512:
                    hash = new SHA512Managed();
                    break;

                case HashType.MD5:
                default:
                    hash = new MD5CryptoServiceProvider();
                    break;
            }

            // Compute hash value of our plain text with appended salt.
            byte[] hashBytes = hash.ComputeHash(plainTextWithSaltBytes);

            // Create array which will hold hash and original salt bytes.
            byte[] hashWithSaltBytes = new byte[hashBytes.Length + saltBytes.Length];

            // Copy hash bytes into resulting array.
            for (int i = 0; i < hashBytes.Length; i++)
                hashWithSaltBytes[i] = hashBytes[i];

            // Append salt bytes to the result.
            for (int i = 0; i < saltBytes.Length; i++)
                hashWithSaltBytes[hashBytes.Length + i] = saltBytes[i];

            string hashValue = Convert.ToBase64String(hashWithSaltBytes);
            return hashValue;
        }

        /// <summary>
        /// Compares a hash of the specified plain text value to a given hash
        /// value. Plain text is hashed with the same salt value as the original
        /// hash.
        /// </summary>
        /// <param name="text">
        /// Plain text to be verified against the specified hash. The function
        /// does not check whether this parameter is null.
        /// </param>
        /// <param name="hashAlgorithm">
        /// Name of the hash algorithm. Allowed values are: "MD5", "SHA1", 
        /// "SHA256", "SHA384", and "SHA512" (if any other value is specified,
        /// MD5 hashing algorithm will be used). This value is case-insensitive.
        /// </param>
        /// <param name="hashValue">
        /// Base64-encoded hash value produced by ComputeHash function. This value
        /// includes the original salt appended to it.
        /// </param>
        /// <returns>
        /// If computed hash mathes the specified hash the function the return
        /// value is true; otherwise, the function returns false.
        /// </returns>
        /// <example>
        /// string text = "ROSSEN";
        /// string code = text.ComputeHash();
        /// bool yes = text.VerifyHash(HashType.MD5, code);
        /// </example>
        public static bool VerifyHash(this string text, HashType hashAlgorithm, string hashValue)
        {
            try
            {
                // Convert base64-encoded hash value into a byte array.
                byte[] hashWithSaltBytes = Convert.FromBase64String(hashValue);

                // We must know size of hash (without salt).
                int hashSizeInBits, hashSizeInBytes;

                // Size of hash is based on the specified algorithm.
                switch (hashAlgorithm)
                {
                    case HashType.SHA1:
                        hashSizeInBits = 160;
                        break;

                    case HashType.SHA256:
                        hashSizeInBits = 256;
                        break;

                    case HashType.SHA384:
                        hashSizeInBits = 384;
                        break;

                    case HashType.SHA512:
                        hashSizeInBits = 512;
                        break;

                    case HashType.MD5:
                    default:
                        hashSizeInBits = 128;
                        break;
                }

                // Convert size of hash from bits to bytes.
                hashSizeInBytes = hashSizeInBits / 8;

                // Make sure that the specified hash value is long enough.
                if (hashWithSaltBytes.Length < hashSizeInBytes)
                    return false;

                // Allocate array to hold original salt bytes retrieved from hash.
                byte[] saltBytes = new byte[hashWithSaltBytes.Length - hashSizeInBytes];

                // Copy salt from the end of the hash to the new array.
                for (int i = 0; i < saltBytes.Length; i++)
                    saltBytes[i] = hashWithSaltBytes[hashSizeInBytes + i];

                // Compute a new hash string.
                string expectedHashString = text.ComputeHash(hashAlgorithm, saltBytes);

                // If the computed hash matches the specified hash,
                // the plain text value must be correct.
                return (hashValue == expectedHashString);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Converts a string to security string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static SecureString ToSecureString(this string value)
        {
            SecureString sstring = new SecureString();
            foreach (var c in value)
                sstring.AppendChar(c);
            return sstring;
        }

        /// <summary>
        /// Converts security string to string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static unsafe string ToCommonString(this SecureString value)
        {
            Debug.Assert(value != null);
            IntPtr bstr = Marshal.SecureStringToBSTR(value);
            try
            {
                return new string((char*)bstr);
            }
            finally
            {
                Marshal.ZeroFreeBSTR(bstr);
            }
        }

        /// <summary>
        /// Protectes a string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Protect(this string value)
        {
            Debug.Assert(!string.IsNullOrEmpty(value));
            byte[] buffer = Encoding.Unicode.GetBytes(value);
            byte[] buffer2 = buffer.Protect();
            string base64value = Convert.ToBase64String(buffer2);
            return base64value;
        }

        /// <summary>
        /// Unprotects a string.
        /// </summary>
        /// <param name="base64value"></param>
        /// <returns></returns>
        public static string Unprotect(this string base64value)
        {
            Debug.Assert(!string.IsNullOrEmpty(base64value));
            byte[] buffer = Convert.FromBase64String(base64value);
            byte[] buffer2 = buffer.Unprotect();
            return Encoding.Unicode.GetString(buffer2);
        }

        /// <summary>
        /// Compares two string ignoring the case.
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool CompareNoCase(this string value1, string value2)
        {
            return string.Equals(value1, value2, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Formats a string
        /// </summary>
        /// <param name="format"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string format(this string format, params object[] values)
        {
            return string.Format(format, values);
        }

        /// <summary>
        /// Formats a string
        /// </summary>
        /// <param name="template"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string format(this string template, Hashtable table)
        {
            Debug.Assert(!string.IsNullOrEmpty(template));

            lock (table.SyncRoot)
            {
                StringBuilder builder = new StringBuilder(template);
                foreach (DictionaryEntry en in table)
                {
                    Func<string> funct = (Func<string>)en.Value;
                    Debug.Assert(funct != null);
                    builder.Replace(Convert.ToString(en.Key), funct());
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Concats values to string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string concat(this string str, params object[] values)
        {
            StringBuilder b = new StringBuilder(str);
            foreach (var o in values)
                b.Append(o);
            return b.ToString();
        }

        /// <summary>
        /// Checks whether a string is null or empty
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Checks whether a string is not null or empty
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasValue(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Reverses a string
        /// </summary>
        /// <param name="str"></param>
        public static unsafe void Reverse(this string str)
        {
            int i = 0;
            int j = str.Length - 1;

            fixed (char* fstr = str)
            {
                while (i < j)
                {
                    char temp = fstr[j];
                    fstr[j--] = fstr[i];
                    fstr[i++] = temp;
                }
            }
        }

        /// <summary>
        /// Shows whether the key is specific per settings
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsSettingsKey(string key)
        {
            return SettingsTable.SettingsKeys.Contains(key);
        }

        /// <summary>
        /// Copy string to a new string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string copy(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return new string(value.ToCharArray());
        }

        /// <summary>
        /// Multiply a string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public static string multiply(this string value, int times)
        {
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < times; i++)
                b.Append(value);
            return b.ToString();
        }

        /// <summary>
        /// Returns a substring from strign
        /// </summary>
        /// <param name="b"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string substring(this StringBuilder b, int length)
        {
            if (b.Length < length)
                return b.ToString();
            return b.ToString().Substring(0, length);
        }

        /// <summary>
        /// Returns a substring from strign
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string substring(this string str, int length)
        {
            if (string.IsNullOrEmpty(str) || str.Length < length)
                return str;
            return str.Substring(0, length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Unique(this string str)
        {
            var u = Guid.NewGuid().ToString().Replace('-', '_');
            return string.Concat(str, u);
        }
    }

    public enum HashType
    {
        SHA1,
        SHA256,
        SHA384,
        SHA512,
        MD5,
    }
}
