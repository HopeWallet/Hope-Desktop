using Hope.Security.ProtectedTypes.Types.Base;

namespace Hope.Security.ProtectedTypes.Types
{

    /// <summary>
    /// Class which represents a regular string value but has its data encrypted and hidden.
    /// </summary>
    public sealed class ProtectedString : ProtectedType<string, DisposableString>
    {

        /// <summary>
        /// Initializes the ProtectedString with the string value it starts with.
        /// </summary>
        /// <param name="value"> The starting string value. </param>
        public ProtectedString(string value) : base(value)
        {
        }

        /// <summary>
        /// Gets the byte array representation of the string.
        /// </summary>
        /// <param name="value"> The string value to convert to bytes. </param>
        /// <returns> The byte array. </returns>
        protected override byte[] GetBytes(string value) => value.GetUTF8Bytes();
    }

}