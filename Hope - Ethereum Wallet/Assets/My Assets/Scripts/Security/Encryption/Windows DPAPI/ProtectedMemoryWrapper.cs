using Hope.Security.Encryption.Symmetric;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;

namespace Hope.Security.Encryption.DPAPI
{

    /// <summary>
    /// Wrapper class which adds padding to <see langword="byte"/>[] data which wants to be protected but is not a multiple of 16.
    /// </summary>
    public static class ProtectedMemoryWrapper
    {
        private static readonly byte[] ENTROPY;

        /// <summary>
        /// Initializes the <see cref="ProtectedMemoryWrapper"/> with a key and iv for the padding encryption.
        /// </summary>
        static ProtectedMemoryWrapper()
        {
            SecureRandom secureRandom = new SecureRandom();
            ENTROPY = SecureRandom.GetNextBytes(secureRandom, 16);
        }

        /// <summary>
        /// Protects data in memory using <see cref="ProtectedMemory.Protect"/> and also adds padding to the <see langword="byte"/>[] data if it is not a multiple of 16.
        /// Does not work with <see langword="readonly"/> variables.
        /// </summary>
        /// <param name="data"> The <see langword="byte"/>[] data to protect. </param>
        /// <param name="scope"> The <see cref="MemoryProtectionScope"/> to protect the data with. </param>
        public static void Protect(ref byte[] data, MemoryProtectionScope scope)
        {
            if (data.Length % 16 != 0 || data.Length == 0)
                data = AesEncryptor.Encrypt(data, ENTROPY.GetBase64String());

            ProtectedMemory.Protect(data, scope);
        }

        /// <summary>
        /// Unprotects <see langword="byte"/>[] data in memory using <see cref="ProtectedMemory.Unprotect"/>.
        /// </summary>
        /// <param name="data"> The <see langword="byte"/>[] data to unprotect. </param>
        /// <param name="scope"> The <see cref="MemoryProtectionScope"/> used to protect the data, now being used to unprotect the data. </param>
        public static void Unprotect(ref byte[] data, MemoryProtectionScope scope)
        {
            if (data.Length % 16 != 0 || data.Length == 0)
                return;

            ProtectedMemory.Unprotect(data, scope);

            data = AesEncryptor.Decrypt(data, ENTROPY.GetBase64String());
        }
    }
}