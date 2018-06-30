using Org.BouncyCastle.Security;
using System.Security.Cryptography;

namespace Hope.Security.Encryption.DPAPI
{

    /// <summary>
    /// Class which protects data which is stored in memory which persists only as long as the session lifetime.
    /// </summary>
    public static class MemoryProtect
    {

        private static readonly byte[] Entropy;

        /// <summary>
        /// Initializes the entropy to use to encrypt the data.
        /// </summary>
        static MemoryProtect()
        {
            SecureRandom secureRandom = new SecureRandom();
            Entropy = SecureRandom.GetNextBytes(secureRandom, 256);

            ProtectedMemory.Protect(Entropy, MemoryProtectionScope.SameProcess);
        }

        /// <summary>
        /// Protects a piece of string text in memory.
        /// Protect should only be used with MemoryProtect.Unprotect in one session of program execution.
        /// Protected data in one session cannot be unprotected in another session.
        /// </summary>
        /// <param name="data"> The data to protect. </param>
        /// <returns> The protected data as a byte array. </returns>
        public static byte[] Protect(byte[] data) => Protect(data, MemoryProtectionScope.SameProcess);

        /// <summary>
        /// Unprotects a byte array of data which was protected by MemoryProtect.Protect during this session history.
        /// Unprotect should only be used with MemoryProtect.Protect in one session of program execution.
        /// Protected data in one session cannot be unprotected in another session.
        /// </summary>
        /// <param name="data"> The data to unprotect. </param>
        /// <returns> The unprotected data. </returns>
        public static byte[] Unprotect(byte[] data) => Unprotect(data, MemoryProtectionScope.SameProcess);

        public static byte[] Protect(byte[] data, MemoryProtectionScope memoryScope) => Protect(data, memoryScope, DataProtectionScope.CurrentUser);

        public static byte[] Unprotect(byte[] data, MemoryProtectionScope memoryScope) => Unprotect(data, memoryScope, DataProtectionScope.CurrentUser);

        public static byte[] Protect(byte[] data, MemoryProtectionScope memoryScope, DataProtectionScope dataScope)
            => InternalProtect(data, memoryScope, dataScope);

        public static byte[] Unprotect(byte[] data, MemoryProtectionScope memoryScope, DataProtectionScope dataScope)
            => InternalUnprotect(data, memoryScope, dataScope);

        private static byte[] InternalProtect(this byte[] data, MemoryProtectionScope memoryScope, DataProtectionScope dataScope)
        {
            if (data == null || data.Length == 0)
                return null;

            ProtectedMemory.Unprotect(Entropy, memoryScope);

            byte[] byteData = ProtectedData.Protect(data, Entropy, dataScope);

            ProtectedMemory.Protect(Entropy, memoryScope);
            ProtectedMemoryWrapper.Protect(ref byteData, memoryScope);

            return byteData;
        }

        private static byte[] InternalUnprotect(this byte[] data, MemoryProtectionScope memoryScope, DataProtectionScope dataScope)
        {
            if (data == null || data.Length == 0)
                return null;

            ProtectedMemoryWrapper.Unprotect(ref data, memoryScope);
            ProtectedMemory.Unprotect(Entropy, memoryScope);

            byte[] byteData = ProtectedData.Unprotect(data, Entropy, dataScope);

            ProtectedMemory.Protect(Entropy, memoryScope);

            return byteData;
        }

    }

}