using Org.BouncyCastle.Security;
using System;
using System.IO;
using System.Security.Cryptography;

namespace Hope.Security.Encryption.DPAPI
{

    /// <summary>
    /// Wrapper class which adds padding to byte data which wants to be protected but is not a multiple of 16.
    /// </summary>
    public static class ProtectedMemoryWrapper
    {

        private static readonly byte[] PAD_KEY;
        private static readonly byte[] PAD_IV;

        /// <summary>
        /// Initializes the ProtectedMemoryWrapper with a key and iv for the padding encryption.
        /// </summary>
        static ProtectedMemoryWrapper()
        {
            SecureRandom secureRandom = new SecureRandom();
            PAD_KEY = SecureRandom.GetNextBytes(secureRandom, 16);
            PAD_IV = SecureRandom.GetNextBytes(secureRandom, 16);

            ProtectPaddingSeed();
        }

        /// <summary>
        /// Protects data in memory using ProtectedMemory.Protect and also adds padding to the byte array if it is not a multiple of 16.
        /// Does not work with readonly variables.
        /// </summary>
        /// <param name="data"> The data to protect. </param>
        /// <param name="scope"> The scope to protect the data with. </param>
        public static void Protect(byte[] data, MemoryProtectionScope scope)
        {
            if (data.Length % 16 != 0 || data.Length == 0)
                data = AddPadding(data.ToBase64String());

            ProtectedMemory.Protect(data, scope);
        }

        /// <summary>
        /// Unprotects data in memory using ProtectedMemory.Unprotect.
        /// </summary>
        /// <param name="data"> The data to unprotect. </param>
        /// <param name="scope"> The scope used to protect the data, now being used to unprotect the data. </param>
        public static void Unprotect(byte[] data, MemoryProtectionScope scope)
        {
            if (data.Length % 16 != 0 || data.Length == 0)
                return;

            ProtectedMemory.Unprotect(data, scope);

            data = RemovePadding(data.ToBase64String());
        }

        /// <summary>
        /// Pads the data by encrypting it using the AES encryption algorithm which encrypts data to a multiple of 16.
        /// </summary>
        /// <param name="data"> The data to encrypt. </param>
        /// <returns> The encrypted and padded byte data. </returns>
        private static byte[] AddPadding(string data)
        {
            UnprotectPaddingSeed();

            byte[] encryptedData;
            using (Aes aes = Aes.Create())
            {
                ICryptoTransform encryptor = aes.CreateEncryptor(PAD_KEY, PAD_IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter sw = new StreamWriter(cs))
                        sw.Write(data);

                    encryptedData = ms.ToArray();
                }
            }

            ProtectPaddingSeed();

            return encryptedData;
        }

        /// <summary>
        /// Unpads the data by decrypting it using the AES encryption algorithm.
        /// </summary>
        /// <param name="data"> The data to decrypt. </param>
        /// <returns> The decrypted and unpadded byte data. </returns>
        private static byte[] RemovePadding(string data)
        {
            UnprotectPaddingSeed();

            byte[] decryptedData;
            using (Aes aes = Aes.Create())
            {
                ICryptoTransform decryptor = aes.CreateDecryptor(PAD_KEY, PAD_IV);

                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(data)))
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (StreamReader sw = new StreamReader(cs))
                    decryptedData = sw.ReadToEnd().FromBase64String();
            }

            ProtectPaddingSeed();

            return decryptedData;
        }

        /// <summary>
        /// Calls ProtectedMemory.Protect on the key and iv of the AES algorithm.
        /// </summary>
        private static void ProtectPaddingSeed()
        {
            ProtectedMemory.Protect(PAD_KEY, MemoryProtectionScope.SameProcess);
            ProtectedMemory.Protect(PAD_IV, MemoryProtectionScope.SameProcess);
        }

        /// <summary>
        /// Calls ProtectedMemory.Unprotect on the key and iv of the AES algorithm.
        /// </summary>
        private static void UnprotectPaddingSeed()
        {
            ProtectedMemory.Unprotect(PAD_KEY, MemoryProtectionScope.SameProcess);
            ProtectedMemory.Unprotect(PAD_IV, MemoryProtectionScope.SameProcess);
        }
    }

}