using System.Security.Cryptography;

namespace Hope.Security.Encryption.DPAPI
{
    /// <summary>
    /// Class which implements a method of encrypting/decrypting data for long term storage on Windows devices.
    /// </summary>
    public sealed class WindowsDataEncryptor : WindowsEncryptor
    {
        /// <summary>
        /// Initializes the <see cref="WindowsDataEncryptor"/> by assigning the encryptors to the <see cref="WindowsEncryptor"/>.
        /// </summary>
        /// <param name="encryptors"> The additional encryptors to use as our advanced entropy. </param>
        public WindowsDataEncryptor(params object[] encryptors) : base(encryptors)
        {
        }

        /// <summary>
        /// Encrypts <see langword="byte"/>[] data using the Windows DPAPI ProtectedData class.
        /// </summary>
        /// <param name="data"> The <see langword="byte"/>[] data to encrypt. </param>
        /// <param name="entropy"> The additional entropy to apply to the encryption. </param>
        /// <returns> The encrypted <see langword="byte"/>[] data. </returns>
        [ReflectionProtect(typeof(byte[]))]
        protected override byte[] InternalEncrypt(byte[] data, byte[] entropy)
        {
            return ProtectedData.Protect(data, entropy, DataProtectionScope.CurrentUser);
        }

        /// <summary>
        /// Decrypts <see langword="byte"/>[] data using the Windows DPAPI ProtectedData class.
        /// </summary>
        /// <param name="encryptedData"> The encrypted <see langword="byte"/>[] data to decrypt. </param>
        /// <param name="entropy"> The additional entropy to use to decrypt the data. </param>
        /// <returns> The decrypted <see langword="byte"/>[] data. </returns>
        [ReflectionProtect(typeof(byte[]))]
        protected override byte[] InternalDecrypt(byte[] encryptedData, byte[] entropy)
        {
            return ProtectedData.Unprotect(encryptedData, entropy, DataProtectionScope.CurrentUser);
        }
    }
}