﻿using System.Security.Cryptography;

namespace Hope.Security.Encryption.Symmetric
{
    /// <summary>
    /// SymmetricEncryptor class used for encrypting data using the RijndaelManaged SymmetricAlgorithm.
    /// </summary>
    public sealed class RijndaelEncryptor : SymmetricEncryptor<RijndaelEncryptor, RijndaelManaged>
    {
        /// <summary>
        /// The key size of the <see cref="SymmetricAlgorithm"/>.
        /// </summary>
        protected override int KeySize => 256;

        /// <summary>
        /// The number of bytes to use for the salt and iv for the <see cref="SymmetricAlgorithm"/>.
        /// </summary>
        protected override int SaltIvByteSize => 32;

        /// <summary>
        /// Initializes the <see cref="RijndaelEncryptor"/> by assigning all additional encryptors to encrypt the data with.
        /// </summary>
        /// <param name="encryptors"> The encryptors to encrypt/decrypt data with. </param>
        public RijndaelEncryptor(params object[] encryptors) : base(encryptors)
        {
        }

        /// <summary>
        /// Initializes the <see cref="RijndaelEncryptor"/>.
        /// </summary>
        public RijndaelEncryptor()
        {
        }
    }
}