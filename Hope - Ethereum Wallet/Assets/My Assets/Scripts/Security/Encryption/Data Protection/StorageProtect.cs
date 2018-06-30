using System.Security.Cryptography;

namespace Hope.Security.Encryption.DPAPI
{

    /// <summary>
    /// Class which protects data for long term storage which can be protected and unprotected across multiple sessions.
    /// </summary>
    public static class StorageProtect
    {

        /// <summary>
        /// Protects data for long term use which can be unprotected across multiple sessions.
        /// </summary>
        /// <param name="data"> The string data to protect. </param>
        /// <returns> The encrypted data as a string. </returns>
        public static string Protect(this string data) => Protect(data, null);

        /// <summary>
        /// Unprotects data which was protected with StorageProtect.Protect.
        /// </summary>
        /// <param name="data"> The encrypted string data. </param>
        /// <returns> The unprotected original data. </returns>
        public static string Unprotect(this string data) => Unprotect(data, null);

        /// <summary>
        /// Protects data for long term use which can be unprotected across multiple sessions.
        /// Accepts an optional entropy as an additional encryption parameter.
        /// </summary>
        /// <param name="data"> The string data to encrypt and protect. </param>
        /// <param name="optionalEntropy"> The entropy to use to encrypt the string data with. </param>
        /// <returns> The encrypted data as a string. </returns>
        public static string Protect(this string data, string optionalEntropy) => Protect(data, optionalEntropy, DataProtectionScope.CurrentUser);

        /// <summary>
        /// Unprotects data which was protected with StorageProtect.Protect.
        /// Accepts an optional entropy which was used along with StorageProtect.Protect to encrypt the data.
        /// </summary>
        /// <param name="data"> The encrypted string data. </param>
        /// <param name="optionalEntropy"> The entropy used to encrypt the data. </param>
        /// <returns> The unprotected original data. </returns>
        public static string Unprotect(this string data, string optionalEntropy) => Unprotect(data, optionalEntropy, DataProtectionScope.CurrentUser);

        /// <summary>
        /// Protects data for long term use which can be unprotected across multiple sessions.
        /// Accepts an optional entropy as an additional encryption parameter.
        /// Accepts an optional DataProtectionScope which defines how the data should be protected.
        /// </summary>
        /// <param name="data"> The string data to encrypt and protect. </param>
        /// <param name="optionalEntropy"> The entropy to use to encrypt the string data with. </param>
        /// <param name="scope"> The protection scope to apply during the encryption. </param>
        /// <returns> The encrypted data as a string. </returns>
        public static string Protect(this string data, string optionalEntropy, DataProtectionScope scope)
            => InternalProtect(data.DPEncrypt(optionalEntropy).GetBase64Bytes(), optionalEntropy.GetBase64Bytes(), scope).GetBase64String();


        /// <summary>
        /// Unprotects data which was protected with StorageProtect.Protect.
        /// Accepts an optional entropy which was used along with StorageProtect.Protect to encrypt the data.
        /// Accepts an optional DataProtectionScope which defines how the data was protected and how it should be unprotected.
        /// </summary>
        /// <param name="data"> The encrypted string data. </param>
        /// <param name="optionalEntropy"> The entropy used to encrypt the data. </param>
        /// <param name="scope"> The protection scope used during the encryption. </param>
        /// <returns> The unprotected original data. </returns>
        public static string Unprotect(this string data, string optionalEntropy, DataProtectionScope scope)
            => InternalUnprotect(data.GetBase64Bytes(), optionalEntropy.GetBase64Bytes(), scope).GetBase64String().DPDecrypt(optionalEntropy);

        /// <summary>
        /// Encrypts the byte data using ProtectedData.Protect.
        /// </summary>
        /// <param name="data"> The byte data to encrypt. </param>
        /// <param name="optionalEntropy"> The optional entropy byte data. </param>
        /// <param name="scope"> The scope to apply the protection with. </param>
        /// <returns> The encrypted byte data. </returns>
        private static byte[] InternalProtect(byte[] data, byte[] optionalEntropy, DataProtectionScope scope) => ProtectedData.Protect(data, optionalEntropy, scope);

        /// <summary>
        /// Decrypts the byte data using ProtectedData.Unprotect.
        /// </summary>
        /// <param name="data"> The data to decrypt. </param>
        /// <param name="optionalEntropy"> The optional entropy byte data to decrypt with. </param>
        /// <param name="scope"> The scope to apply to the unprotect method. </param>
        /// <returns> The decrypted byte data. </returns>
        private static byte[] InternalUnprotect(byte[] data, byte[] optionalEntropy, DataProtectionScope scope) => ProtectedData.Unprotect(data, optionalEntropy, scope);


    }

}