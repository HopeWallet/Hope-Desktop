namespace Hope.Security.Encryption.DPAPI
{
    /// <summary>
    /// Base class used for encrypting/decrypting data meant for long/short term storage on Windows devices.
    /// </summary>
    public abstract class WindowsEncryptor : AdvancedEntropyEncryptor
    {
        /// <summary>
        /// Initializes the <see cref="WindowsEncryptor"/> by assigning the encryptors to the <see cref="AdvancedEntropyEncryptor"/>.
        /// </summary>
        /// <param name="encryptors"> The additional encryptors to use as our advanced entropy. </param>
        protected WindowsEncryptor(params object[] encryptors) : base(encryptors)
        {
        }

        /// <summary>
        /// Encrypts <see langword="byte"/>[] data with an additional entropy parameter.
        /// Only compatible on Windows related devices.
        /// </summary>
        /// <param name="data"> The <see langword="byte"/>[] data to encrypt. </param>
        /// <param name="entropy"> The additional entropy to apply to the encryption. </param>
        /// <returns> The encrypted <see langword="byte"/>[] data. </returns>
        [SecureCaller]
        public override byte[] Encrypt(byte[] data, byte[] entropy) => InternalEncrypt(data, GetAdvancedEntropyHash(entropy));

        /// <summary>
        /// Decrypts <see langword="byte"/>[] data using the additional entropy parameter.
        /// Only compatible on Windows related devices.
        /// </summary>
        /// <param name="encryptedData"> The encrypted <see langword="byte"/>[] data to decrypt. </param>
        /// <param name="entropy"> The additional entropy to use to decrypt the data. </param>
        /// <returns> The decrypted <see langword="byte"/>[] data. </returns>
        [SecureCaller]
        public override byte[] Decrypt(byte[] encryptedData, byte[] entropy) => InternalDecrypt(encryptedData, GetAdvancedEntropyHash(entropy));

        /// <summary>
        /// Encrypts <see langword="byte"/>[] data using the chosen Windows DPAPI encryption method.
        /// </summary>
        /// <param name="data"> The <see langword="byte"/>[] data to encrypt. </param>
        /// <param name="entropy"> The additional entropy to apply to the encryption. </param>
        /// <returns> The encrypted <see langword="byte"/>[] data. </returns>
        [SecureCaller]
        protected abstract byte[] InternalEncrypt(byte[] data, byte[] entropy);

        /// <summary>
        /// Decrypts <see langword="byte"/>[] data using the chosen Windows DPAPI encryption method.
        /// </summary>
        /// <param name="encryptedData"> The encrypted <see langword="byte"/>[] data to decrypt. </param>
        /// <param name="entropy"> The additional entropy to use to decrypt the data. </param>
        /// <returns> The decrypted <see langword="byte"/>[] data. </returns>
        [SecureCaller]
        protected abstract byte[] InternalDecrypt(byte[] encryptedData, byte[] entropy);
    }
}