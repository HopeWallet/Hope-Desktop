using Hope.Security.ProtectedTypes.Types.Base;
using System;

namespace Hope.Security.ProtectedTypes.Types
{

    /// <summary>
    /// Class used to contain the unencrypted data of an int variable which will be disposed after use.
    /// Meant to be used within a <see langword="using"/> statement for best effect.
    /// </summary>
    public sealed class DisposableInt : DisposableData<int>
    {

        /// <summary>
        /// The <see langword="int"/> value of this <see cref="DisposableInt"/>.
        /// </summary>
        public override int Value => BitConverter.ToInt32(unprotectedBytes, 0);

        /// <summary>
        /// Initializes the <see cref="DisposableInt"/> with the <see langword="byte"/>[] data of the int.
        /// </summary>
        /// <param name="data"> The <see langword="byte"/>[] data of the int. </param>
        public DisposableInt(byte[] data) : base(data)
        {
        }
    }
}
