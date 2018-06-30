using System.Security.Cryptography;

namespace Hope.Security.Encryption.DPAPI
{

    /// <summary>
    /// Class which protects data for long term storage which can be protected and unprotected across multiple sessions.
    /// </summary>
    public static class StorageProtect
    {

        public static string Protect(this string data) => Protect(data, null);

        public static string Unprotect(this string data) => Unprotect(data, null);

        public static string Protect(this string data, string optionalEntropy) => Protect(data, optionalEntropy, DataProtectionScope.CurrentUser);

        public static string Unprotect(this string data, string optionalEntropy) => Unprotect(data, optionalEntropy, DataProtectionScope.CurrentUser);

        public static string Protect(this string data, string optionalEntropy, DataProtectionScope scope)
            => InternalProtect(data.DPEncrypt(optionalEntropy).GetBase64Bytes(), optionalEntropy.GetBase64Bytes(), scope).GetBase64String();

        public static string Unprotect(this string data, string optionalEntropy, DataProtectionScope scope)
            => InternalUnprotect(data.GetBase64Bytes(), optionalEntropy.GetBase64Bytes(), scope).GetBase64String().DPDecrypt(optionalEntropy);

        private static byte[] InternalProtect(byte[] data, byte[] optionalEntropy, DataProtectionScope scope) => ProtectedData.Protect(data, optionalEntropy, scope);

        private static byte[] InternalUnprotect(byte[] data, byte[] optionalEntropy, DataProtectionScope scope) => ProtectedData.Unprotect(data, optionalEntropy, scope);


    }

}