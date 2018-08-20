using System.Security.Cryptography;
using Hope.Security.Encryption.Symmetric;
using Hope.Security.HashGeneration;
using RandomNET.Bytes;

namespace Hope.Security.Encryption.DPAPI
{
    /// <summary>
    /// Class which implements a method of encrypting/decrypting data in memory for Windows devices.
    /// </summary>
    public sealed class WindowsMemoryEncryptor : WindowsEncryptor
    {
        private readonly AesEncryptor aes;
        private readonly byte[] randomEntropy;

        /// <summary>
        /// Initializes the <see cref="WindowsMemoryEncryptor"/> by assigning the encryptors to the <see cref="WindowsEncryptor"/> and creating our padding aes encryptor.
        /// </summary>
        /// <param name="encryptors"> The additional encryptors to use as our advanced entropy. </param>
        public WindowsMemoryEncryptor(params object[] encryptors) : base(encryptors)
        {
            aes = new AesEncryptor(encryptors);
            randomEntropy = RandomBytes.Secure.SHA3.GetBytes(32).Shake_128();
        }

        /// <summary>
        /// Encrypts <see langword="byte"/>[] data using the Windows DPAPI ProtectedMemory class.
        /// </summary>
        /// <param name="data"> The <see langword="byte"/>[] data to encrypt. </param>
        /// <param name="entropy"> The additional entropy to apply to the encryption. </param>
        /// <returns> The encrypted <see langword="byte"/>[] data. </returns>
        [SecureCaller]
        [ReflectionProtect(typeof(byte[]))]
        protected override byte[] InternalEncrypt(byte[] data, byte[] entropy)
        {
            byte[] encryptedData = data;
            if (data.Length % 16 != 0 || data.Length == 0)
            {
                // ProtectedMemory needs data in 16 byte blocks so we need to encrypt it into 16 byte blocks if it is not the case.
                encryptedData = aes.Encrypt(data, entropy?.Length > 0 ? entropy : randomEntropy);
                data.ClearBytes();
            }

            ProtectedMemory.Protect(encryptedData, MemoryProtectionScope.SameProcess);

            return encryptedData;
        }

        /// <summary>
        /// Decrypts <see langword="byte"/>[] data using the Windows DPAPI ProtectedMemory class.
        /// </summary>
        /// <param name="encryptedData"> The encrypted <see langword="byte"/>[] data to decrypt. </param>
        /// <param name="entropy"> The additional entropy to use to decrypt the data. </param>
        /// <returns> The decrypted <see langword="byte"/>[] data. </returns>
        [SecureCaller]
        [ReflectionProtect(typeof(byte[]))]
        protected override byte[] InternalDecrypt(byte[] encryptedData, byte[] entropy)
        {
            if (encryptedData.Length % 16 != 0 || encryptedData.Length == 0)
                return null;

            ProtectedMemory.Unprotect(encryptedData, MemoryProtectionScope.SameProcess);

            return aes.Decrypt(encryptedData, entropy?.Length > 0 ? entropy : randomEntropy);
        }
    }
}