using System.Security.Cryptography;

namespace Hope.Security.Encryption.DPAPI
{
    /// <summary>
    /// Class which protects data for long term storage which can be protected and unprotected across multiple sessions.
    /// </summary>
    public static class DataProtect
    {
        /// <summary>
        /// Protects data for long term use which can be unprotected across multiple sessions. 
        /// Can be unprotected using <see cref="DataProtect.Unprotect"/>.
        /// </summary>
        /// <param name="data"> The <see langword="string"/> data to protect. </param>
        /// <returns> The encrypted data as a <see langword="string"/>. </returns>
        public static string Protect(this string data) => Protect(data, null);

        /// <summary>
        /// Unprotects data which was protected with <see cref="DataProtect.Protect"/>.
        /// </summary>
        /// <param name="data"> The encrypted <see langword="string"/> data. </param>
        /// <returns> The unprotected <see langword="string"/> data. </returns>
        public static string Unprotect(this string data) => Unprotect(data, null);

        /// <summary>
        /// Protects data for long term use which can be unprotected across multiple sessions.
        /// Can be unprotected using <see cref="Unprotect(string, string)"/>.
        /// </summary>
        /// <param name="data"> The <see langword="string"/> data to encrypt and protect. </param>
        /// <param name="optionalEntropy"> The entropy to use to encrypt the <see langword="string"/> data with. </param>
        /// <returns> The encrypted data as a <see langword="string"/>. </returns>
        public static string Protect(this string data, string optionalEntropy) => Protect(data, optionalEntropy, DataProtectionScope.CurrentUser);

        /// <summary>
        /// Unprotects data which was protected with <see cref="Protect(string, string)"/>.
        /// </summary>
        /// <param name="data"> The encrypted <see langword="string"/> data. </param>
        /// <param name="optionalEntropy"> The entropy used to encrypt the data. </param>
        /// <returns> The unprotected <see langword="string"/> data. </returns>
        public static string Unprotect(this string data, string optionalEntropy) => Unprotect(data, optionalEntropy, DataProtectionScope.CurrentUser);

        /// <summary>
        /// Protects data for long term use which can be unprotected across multiple sessions.
        /// Can be unprotected using <see cref="Unprotect(string, string, DataProtectionScope)"/>.
        /// </summary>
        /// <param name="data"> The <see langword="string"/> data to encrypt and protect. </param>
        /// <param name="optionalEntropy"> The entropy to use to encrypt the <see langword="string"/> data with. </param>
        /// <param name="scope"> The <see cref="DataProtectionScope"/> to apply during the encryption. </param>
        /// <returns> The encrypted data as a <see langword="string"/>. </returns>
        public static string Protect(this string data, string optionalEntropy, DataProtectionScope scope)
            => InternalProtect(data.DPEncrypt(optionalEntropy).GetBase64Bytes(), optionalEntropy.GetBase64Bytes(), scope).GetBase64String();


        /// <summary>
        /// Unprotects data which was protected with <see cref="Protect(string, string, DataProtectionScope)"/>.
        /// </summary>
        /// <param name="data"> The encrypted <see langword="string"/> data. </param>
        /// <param name="optionalEntropy"> The entropy used to encrypt the data. </param>
        /// <param name="scope"> The <see cref="DataProtectionScope"/> used during the encryption. </param>
        /// <returns> The unprotected <see langword="string"/> data. </returns>
        public static string Unprotect(this string data, string optionalEntropy, DataProtectionScope scope)
            => InternalUnprotect(data.GetBase64Bytes(), optionalEntropy.GetBase64Bytes(), scope).GetBase64String().DPDecrypt(optionalEntropy);

        /// <summary>
        /// Encrypts the <see langword="byte"/>[] data using <see cref="ProtectedData.Protect"/>.
        /// </summary>
        /// <param name="data"> The <see langword="byte"/>[] data to encrypt. </param>
        /// <param name="optionalEntropy"> The optional entropy <see langword="byte"/>[] data. </param>
        /// <param name="scope"> The <see cref="DataProtectionScope"/> to apply the protection with. </param>
        /// <returns> The encrypted <see langword="byte"/>[] data. </returns>
        private static byte[] InternalProtect(byte[] data, byte[] optionalEntropy, DataProtectionScope scope) => ProtectedData.Protect(data, optionalEntropy, scope);

        /// <summary>
        /// Decrypts the <see langword="byte"/>[] data using <see cref="ProtectedData.Unprotect"/>.
        /// </summary>
        /// <param name="data"> The <see langword="byte"/>[] data to decrypt. </param>
        /// <param name="optionalEntropy"> The optional entropy <see langword="byte"/>[] data to decrypt with. </param>
        /// <param name="scope"> The <see cref="DataProtectionScope"/> to apply to the unprotect method. </param>
        /// <returns> The decrypted <see langword="byte"/>[] data. </returns>
        private static byte[] InternalUnprotect(byte[] data, byte[] optionalEntropy, DataProtectionScope scope) => ProtectedData.Unprotect(data, optionalEntropy, scope);
    }
}