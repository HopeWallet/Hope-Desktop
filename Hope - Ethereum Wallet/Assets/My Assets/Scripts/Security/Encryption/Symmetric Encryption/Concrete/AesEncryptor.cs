using System.Security.Cryptography;

namespace Hope.Security.Encryption.Symmetric
{
    /// <summary>
    /// SymmetricEncryptor class used for encrypting data using the AesManaged SymmetricAlgorithm.
    /// </summary>
    public sealed class AesEncryptor : SymmetricEncryptor<AesEncryptor, AesManaged>
    {
        /// <summary>
        /// The key size of the <see cref="SymmetricAlgorithm"/>.
        /// </summary>
        protected override int KeySize => 128;

        /// <summary>
        /// The number of bytes to use for the salt and iv for the <see cref="SymmetricAlgorithm"/>.
        /// </summary>
        protected override int SaltIvByteSize => 16;

        /// <summary>
        /// Initializes the <see cref="AesEncryptor"/> by assigning all additional encryptors to encrypt the data with.
        /// </summary>
        /// <param name="encryptors"> The encryptors to encrypt/decrypt data with. </param>
        public AesEncryptor(params object[] encryptors) : base(encryptors)
        {
        }

        /// <summary>
        /// Initializes the <see cref="AesEncryptor"/>.
        /// </summary>
        public AesEncryptor()
        {
        }
    }
}