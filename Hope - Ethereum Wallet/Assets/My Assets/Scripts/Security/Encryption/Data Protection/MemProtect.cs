using Org.BouncyCastle.Security;
using System.Security.Cryptography;

namespace Hope.Security.Encryption.DPAPI
{

    /// <summary>
    /// Class which protects data which is stored in memory which persists only as long as the session lifetime.
    /// </summary>
    public static class MemProtect
    {

        private static readonly byte[] Entropy;

        /// <summary>
        /// Initializes the entropy to use to encrypt the data.
        /// </summary>
        static MemProtect()
        {
            SecureRandom secureRandom = new SecureRandom();
            Entropy = SecureRandom.GetNextBytes(secureRandom, 256);

            ProtectedMemory.Protect(Entropy, MemoryProtectionScope.SameProcess);
        }

        public static byte[] Protect(string data) => Protect(data.GetUTF8Bytes());

        /// <summary>
        /// Protects a piece of string text in memory.
        /// ProtectMemory should only be used with UnprotectMemory in one session of program execution.
        /// Protected data in one session cannot be unprotected in another session.
        /// </summary>
        /// <param name="data"> The string data. </param>
        /// <returns> The protected data as a byte array. </returns>
        public static byte[] Protect(byte[] data) => Protect(data, MemoryProtectionScope.SameProcess);

        /// <summary>
        /// Unprotects a byte array of data which was protected by ProtectMemory during this session history.
        /// UnprotectMemory should only be used with ProtectMemory in one session of program execution.
        /// Protected data in one session cannot be unprotected in another session.
        /// </summary>
        /// <param name="data"> The data to unprotect. </param>
        /// <returns> The unprotected string retrieved from the data. </returns>
        public static byte[] Unprotect(byte[] data) => Unprotect(data, MemoryProtectionScope.SameProcess);

        public static byte[] Protect(byte[] data, MemoryProtectionScope memoryScope) => Protect(data, memoryScope, DataProtectionScope.CurrentUser);

        public static byte[] Unprotect(byte[] data, MemoryProtectionScope memoryScope) => Unprotect(data, memoryScope, DataProtectionScope.CurrentUser);

        public static byte[] Protect(byte[] data, MemoryProtectionScope memoryScope, DataProtectionScope dataScope)
            => InternalProtect(data, memoryScope, dataScope);

        public static byte[] Unprotect(byte[] data, MemoryProtectionScope memoryScope, DataProtectionScope dataScope)
            => InternalUnprotect(data, memoryScope, dataScope);

        private static byte[] InternalProtect(this byte[] data, MemoryProtectionScope memoryScope, DataProtectionScope dataScope)
        {
            ProtectedMemory.Unprotect(Entropy, memoryScope);

            byte[] byteData = ProtectedData.Protect(data, Entropy, dataScope);

            ProtectedMemory.Protect(Entropy, memoryScope);
            ProtectedMemoryWrapper.Protect(ref byteData, memoryScope);

            return byteData;
        }

        private static byte[] InternalUnprotect(this byte[] data, MemoryProtectionScope memoryScope, DataProtectionScope dataScope)
        {
            ProtectedMemoryWrapper.Unprotect(ref data, memoryScope);
            ProtectedMemory.Unprotect(Entropy, memoryScope);

            byte[] byteData = ProtectedData.Unprotect(data, Entropy, dataScope);

            ProtectedMemory.Protect(Entropy, memoryScope);

            return byteData;
        }

    }

}