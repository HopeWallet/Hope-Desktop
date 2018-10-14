using Hope.Security.ProtectedTypes.Types.Base;

namespace Hope.Security.ProtectedTypes.Types
{
    /// <summary>
    /// Class which represents a regular <see langword="string"/> value but has its data encrypted and hidden.
    /// </summary>
    public sealed class ProtectedString : ProtectedType<string, DisposableString>
    {
        /// <summary>
        /// Initializes the <see cref="ProtectedString"/> with the <see langword="string"/> value it starts with.
        /// </summary>
        /// <param name="value"> The starting <see langword="string"/> value. </param>
        public ProtectedString(string value) : base(value)
        {
        }

        /// <summary>
        /// Initializes the <see cref="ProtectedString"/> with the <see langword="byte"/>[] value it starts with.
        /// </summary>
        /// <param name="value"> The starting <see langword="byte"/>[] value. </param>
        public ProtectedString(byte[] value) : base(value)
        {
        }

        /// <summary>
        /// Initializes the <see cref="ProtectedString"/> with the <see langword="string"/> value and additional <see cref="SecureObject"/> instances to encrypt the data with.
        /// </summary>
        /// <param name="value"> The starting <see langword="string"/> value. </param>
        /// <param name="encryptionObjects"> The additional <see cref="SecureObject"/> instances to encrypt the <see langword="string"/> data with. </param>
        public ProtectedString(string value, params SecureObject[] encryptionObjects) : base(value, encryptionObjects)
        {
        }

        /// <summary>
        /// Initializes the <see cref="ProtectedString"/> with the <see langword="byte"/>[] value and additional <see cref="SecureObject"/> instances to encrypt the data with.
        /// </summary>
        /// <param name="value"> The starting <see langword="byte"/>[] value. </param>
        /// <param name="encryptionObjects"> The additional <see cref="SecureObject"/> instances to encrypt the <see langword="byte"/>[] data with. </param>
        public ProtectedString(byte[] value, params SecureObject[] encryptionObjects) : base(value, encryptionObjects)
        {
        }

        /// <summary>
        /// Gets the <see langword="byte"/>[] data representation of the <see langword="string"/>.
        /// </summary>
        /// <param name="value"> The <see langword="string"/> value to convert to <see langword="byte"/>[] data. </param>
        /// <returns> The <see langword="byte"/>[] data of the string value. </returns>
        protected override byte[] GetBytes(string value) => value.GetUTF8Bytes();
    }
}