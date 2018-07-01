using Hope.Security.ProtectedTypes.Types.Base;
using System;

namespace Hope.Security.ProtectedTypes.Types
{
    /// <summary>
    /// Class which represents a regular int value but has its data encrypted and hidden.
    /// </summary>
    public sealed class ProtectedInt : ProtectedType<int, DisposableInt>
    {

        /// <summary>
        /// Initializes the ProtectedInt with the int value it starts with.
        /// </summary>
        /// <param name="value"> The starting int value. </param>
        public ProtectedInt(int value) : base(value)
        {
        }

        /// <summary>
        /// Gets the byte array representation of the int value.
        /// </summary>
        /// <param name="value"> The int value to convert to bytes. </param>
        /// <returns> The byte array converted from the int. </returns>
        protected override byte[] GetBytes(int value) => BitConverter.GetBytes(value);

    }

}