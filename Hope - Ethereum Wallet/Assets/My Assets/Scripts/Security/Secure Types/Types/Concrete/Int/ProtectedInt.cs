using Hope.Security.ProtectedTypes.Types.Base;
using System;

namespace Hope.Security.ProtectedTypes.Types
{
    /// <summary>
    /// Class which represents a regular <see langword="int"/> value but has its data encrypted and hidden.
    /// </summary>
    public sealed class ProtectedInt : ProtectedType<int, DisposableInt>
    {
        /// <summary>
        /// Initializes the <see cref="ProtectedInt"/> with the <see langword="int"/> value it starts with.
        /// </summary>
        /// <param name="value"> The starting <see langword="int"/> value. </param>
        public ProtectedInt(int value) : base(value)
        {
        }

        /// <summary>
        /// Gets the <see langword="byte"/>[] data representation of the <see langword="int"/> value.
        /// </summary>
        /// <param name="value"> The <see langword="int"/> value to convert to <see langword="byte"/>[] data. </param>
        /// <returns> The <see langword="byte"/>[] data converted from the <see langword="int"/>. </returns>
        protected override byte[] GetBytes(int value) => BitConverter.GetBytes(value);
    }
}