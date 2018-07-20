using Hope.Security.ProtectedTypes.Types.Base;

namespace Hope.Security.ProtectedTypes.Types
{

    /// <summary>
    /// Class used to contain the unencrypted data of a <see langword="string"/> variable which will be disposed after use.
    /// Meant to be used within a <see langword="using"/> statement for best effect.
    /// </summary>
    public sealed class DisposableString : DisposableData<string>
    {

        /// <summary>
        /// The unencrypted <see langword="string"/> value of this <see cref="DisposableString"/>.
        /// </summary>
        public override string Value => unprotectedBytes.GetUTF8String();

        /// <summary>
        /// Initializes the <see cref="DisposableString"/> with the <see langword="byte"/>[] data.
        /// </summary>
        /// <param name="data"> The <see langword="byte"/>[] data. </param>
        public DisposableString(byte[] data) : base(data)
        {
        }
    }
}
