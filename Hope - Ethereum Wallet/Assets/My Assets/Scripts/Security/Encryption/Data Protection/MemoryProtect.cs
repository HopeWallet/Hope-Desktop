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
        /// Initializes the entropy to use to encrypt and decrypt the data.
        /// </summary>
        static MemoryProtect()
        {
            SecureRandom secureRandom = new SecureRandom();
            Entropy = SecureRandom.GetNextBytes(secureRandom, 256);

            ProtectedMemory.Protect(Entropy, MemoryProtectionScope.SameProcess);
        }

        /// <summary>
        /// Protects some byte data in memory.
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

        /// <summary>
        /// Protects some byte data in memory.
        /// Protect should only be used with MemoryProtect.Unprotect in one session of program execution.
        /// Protected data in one session cannot be unprotected in another session.
        /// Accepts optional MemoryProtectionScope parameter which defines which processes can unprotect the data.
        /// </summary>
        /// <param name="data"> The data to protect. </param>
        /// <param name="memoryScope"> The memory scope to apply to the protection. </param>
        /// <returns> The protected data as a byte array. </returns>
        public static byte[] Protect(byte[] data, MemoryProtectionScope memoryScope) => Protect(data, memoryScope, DataProtectionScope.CurrentUser);

        /// <summary>
        /// Unprotects a byte array of data which was protected by MemoryProtect.Protect during this session history.
        /// Unprotect should only be used with MemoryProtect.Protect in one session of program execution.
        /// Protected data in one session cannot be unprotected in another session.
        /// Accepts optional MemoryProtectionScope parameter which should be the same as how the data was protected.
        /// </summary>
        /// <param name="data"> The data to unprotect. </param>
        /// <param name="memoryScope"> The memory scope applied to the data protection. </param>
        /// <returns> The unprotected data. </returns>
        public static byte[] Unprotect(byte[] data, MemoryProtectionScope memoryScope) => Unprotect(data, memoryScope, DataProtectionScope.CurrentUser);

        /// <summary>
        /// Protects some byte data in memory.
        /// Protect should only be used with MemoryProtect.Unprotect in one session of program execution.
        /// Protected data in one session cannot be unprotected in another session.
        /// Accepts optional MemoryProtectionScope parameter which defines which processes can unprotect the data.
        /// Accepts optional DataProtectionScope which defines which users can unprotect the data.
        /// </summary>
        /// <param name="data"> The data to protect. </param>
        /// <param name="memoryScope"> The memory scope to apply to the protection. </param>
        /// <param name="dataScope"> The data protection scope to apply to the protection. </param>
        /// <returns> The protected data as a byte array. </returns>
        public static byte[] Protect(byte[] data, MemoryProtectionScope memoryScope, DataProtectionScope dataScope)
            => InternalProtect(data, memoryScope, dataScope);

        /// <summary>
        /// Unprotects a byte array of data which was protected by MemoryProtect.Protect during this session history.
        /// Unprotect should only be used with MemoryProtect.Protect in one session of program execution.
        /// Protected data in one session cannot be unprotected in another session.
        /// Accepts optional MemoryProtectionScope parameter which should be the same as how the data was protected.
        /// Accepts optional DataProtectionScope which should be the same as when the data was protected.
        /// </summary>
        /// <param name="data"> The data to unprotect. </param>
        /// <param name="memoryScope"> The memory scope applied to the data protection. </param>
        /// <param name="dataScope"> The data protection scope which was applied during the protection. </param>
        /// <returns> The unprotected data. </returns>
        public static byte[] Unprotect(byte[] data, MemoryProtectionScope memoryScope, DataProtectionScope dataScope)
            => InternalUnprotect(data, memoryScope, dataScope);

        /// <summary>
        /// Encrypts the data using ProtectedMemory.Protect and ProtectedData.Protect.
        /// Uses the randomized entropy which was generated on session launch to encrypt the data.
        /// </summary>
        /// <param name="data"> The data to encrypt. </param>
        /// <param name="memoryScope"> The memory scope to apply to the encryption. </param>
        /// <param name="dataScope"> The data scope to apply to the encryption. </param>
        /// <returns> The encrypted byte data. </returns>
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

        /// <summary>
        /// Decrypts the data using ProtectedMemory.Unprotect and ProtectedData.Unprotect.
        /// Uses the randomized entropy which was generated on session launch to decrypt the data.
        /// </summary>
        /// <param name="data"> The data to decrypt. </param>
        /// <param name="memoryScope"> The memory scope applied to the encryption. </param>
        /// <param name="dataScope"> The data scope applied to the encryption. </param>
        /// <returns> The decrypted byte data. </returns>
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