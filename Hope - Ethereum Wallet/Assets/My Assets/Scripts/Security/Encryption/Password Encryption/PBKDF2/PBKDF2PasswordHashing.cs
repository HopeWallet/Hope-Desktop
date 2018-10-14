using Hope.Random;
using Hope.Security.PBKDF2.Engines.Abstract;
using Hope.Security.PBKDF2.Engines.SHA3;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using System.Linq;

namespace Hope.Security.PBKDF2
{
    /// <summary>
    /// Class used to create and verify password hashes using an implementation of PBKDF2 with a specified core hashing algorithm
    /// </summary>
    public sealed class PBKDF2PasswordHashing
    {
        private readonly PBKDF2Engine engine;

        private const int ITERATIONS = 50000;
        private const int SALT_SIZE = 64;
        private const int HASH_SIZE = 128;

        private const int MIN_ITERATIONS = 100;
        private const int MIN_SALT_SIZE = 8;
        private const int MIN_HASH_SIZE = 16;

        /// <summary>
        /// Initializes the PBKDF2 instance with a default SHA3-512 engine for the password hashing.
        /// </summary>
        public PBKDF2PasswordHashing() : this(new SHA3_512_Engine())
        {
        }

        /// <summary>
        /// Initializes the PBKDF2 instance with a custom <see cref="PBKDF2Engine"/> instance.
        /// </summary>
        /// <param name="engine"> The <see cref="PBKDF2Engine"/> to use to hash the passwords. </param>
        public PBKDF2PasswordHashing(PBKDF2Engine engine)
        {
            this.engine = engine;
        }

        /// <summary>
        /// Gets the salted password hash of a password.
        /// </summary>
        /// <param name="password"> The password to get the salted hash for. </param>
        /// <returns> The salted hash as a <see langword="string"/>. </returns>
        public string GetSaltedPasswordHash(string password) => GetSaltedPasswordHash(password, ITERATIONS);

        /// <summary>
        /// Gets the salted password hash of a password.
        /// </summary>
        /// <param name="password"> The password to get the salted hash for. </param>
        /// <param name="iterations"> The number of iterations to apply to the encryption. </param>
        /// <returns> The salted hash as a <see langword="string"/>. </returns>
        public string GetSaltedPasswordHash(string password, int iterations) => GetSaltedPasswordHash(password, iterations, SALT_SIZE);

        /// <summary>
        /// Gets the salted password hash of a password.
        /// </summary>
        /// <param name="password"> The password to get the salted hash for. </param>
        /// <param name="iterations"> The number of iterations to apply to the encryption. </param>
        /// <param name="saltSize"> The size of the salt. </param>
        /// <returns> The salted hash as a <see langword="string"/>. </returns>
        public string GetSaltedPasswordHash(string password, int iterations, int saltSize) => GetSaltedPasswordHash(password, iterations, saltSize, HASH_SIZE);

        /// <summary>
        /// Gets the salted password hash of a password.
        /// </summary>
        /// <param name="password"> The password to get the salted hash for. </param>
        /// <param name="iterations"> The number of iterations to apply to the encryption. </param>
        /// <param name="saltSize"> The size of the salt. </param>
        /// <param name="hashSize"> The size of the hash. </param>
        /// <returns> The salted hash as a <see langword="string"/>. </returns>
        public string GetSaltedPasswordHash(string password, int iterations, int saltSize, int hashSize) => GetSaltedPasswordHash(password.GetUTF8Bytes(), iterations, saltSize, hashSize).GetBase64String();

        /// <summary>
        /// Gets the salted password hash of a password.
        /// </summary>
        /// <param name="password"> The password to get the salted hash for. </param>
        /// <returns> The salted hash as a <see langword="byte"/>[]. </returns>
        public byte[] GetSaltedPasswordHash(byte[] password) => GetSaltedPasswordHash(password, ITERATIONS);

        /// <summary>
        /// Gets the salted password hash of a password.
        /// </summary>
        /// <param name="password"> The password to get the salted hash for. </param>
        /// <param name="iterations"> The number of iterations to apply to the encryption. </param>
        /// <returns> The salted hash as a <see langword="byte"/>[]. </returns>
        public byte[] GetSaltedPasswordHash(byte[] password, int iterations) => GetSaltedPasswordHash(password, iterations, SALT_SIZE);

        /// <summary>
        /// Gets the salted password hash of a password.
        /// </summary>
        /// <param name="password"> The password to get the salted hash for. </param>
        /// <param name="iterations"> The number of iterations to apply to the encryption. </param>
        /// <param name="saltSize"> The size of the salt. </param>
        /// <returns> The salted hash as a <see langword="byte"/>[]. </returns>
        public byte[] GetSaltedPasswordHash(byte[] password, int iterations, int saltSize) => GetSaltedPasswordHash(password, iterations, saltSize, HASH_SIZE);

        /// <summary>
        /// Gets the salted password hash of a password.
        /// </summary>
        /// <param name="password"> The password to get the salted hash for. </param>
        /// <param name="iterations"> The number of iterations to apply to the encryption. </param>
        /// <param name="saltSize"> The size of the salt. </param>
        /// <param name="hashSize"> The size of the hash. </param>
        /// <returns> The salted hash as a <see langword="byte"/>[]. </returns>
        public byte[] GetSaltedPasswordHash(byte[] password, int iterations, int saltSize, int hashSize) => InternalGetSaltedPasswordHash(password, iterations, saltSize, hashSize);

        /// <summary>
        /// Verifies a password using the specified algorithm.
        /// </summary>
        /// <param name="password"> The password to verify. </param>
        /// <param name="saltedHash"> The salted hash of the password to compare the password with. </param>
        /// <returns> Whether the password is correct or not. </returns>
        public bool VerifyPassword(string password, string saltedHash) => VerifyPassword(password, saltedHash, ITERATIONS);

        /// <summary>
        /// Verifies a password using the specified algorithm.
        /// </summary>
        /// <param name="password"> The password to verify. </param>
        /// <param name="saltedHash"> The salted hash of the password to compare the password with. </param>
        /// <param name="iterations"> The number of iterations to apply to the hash generation. </param>
        /// <returns> Whether the password is correct or not. </returns>
        public bool VerifyPassword(string password, string saltedHash, int iterations) => VerifyPassword(password, saltedHash, iterations, SALT_SIZE);

        /// <summary>
        /// Verifies a password using the specified algorithm.
        /// </summary>
        /// <param name="password"> The password to verify. </param>
        /// <param name="saltedHash"> The salted hash of the password to compare the password with. </param>
        /// <param name="iterations"> The number of iterations to apply to the hash generation. </param>
        /// <param name="saltSize"> The size of the salt. </param>
        /// <returns> Whether the password is correct or not. </returns>
        public bool VerifyPassword(string password, string saltedHash, int iterations, int saltSize) => VerifyPassword(password, saltedHash, iterations, saltSize, HASH_SIZE);

        /// <summary>
        /// Verifies a password using the specified algorithm.
        /// </summary>
        /// <param name="password"> The password to verify. </param>
        /// <param name="saltedHash"> The salted hash of the password to compare the password with. </param>
        /// <param name="iterations"> The number of iterations to apply to the hash generation. </param>
        /// <param name="saltSize"> The size of the salt. </param>
        /// <param name="hashSize"> The size of the hash. </param>
        /// <returns> Whether the password is correct or not. </returns>
        public bool VerifyPassword(string password, string saltedHash, int iterations, int saltSize, int hashSize) => VerifyPassword(password.GetUTF8Bytes(), saltedHash, iterations, saltSize, hashSize);

        /// <summary>
        /// Verifies a password using the specified algorithm.
        /// </summary>
        /// <param name="password"> The password to verify. </param>
        /// <param name="saltedHash"> The salted hash of the password to compare the password with. </param>
        /// <returns> Whether the password is correct or not. </returns>
        public bool VerifyPassword(byte[] password, string saltedHash) => VerifyPassword(password, saltedHash, ITERATIONS);

        /// <summary>
        /// Verifies a password using the specified algorithm.
        /// </summary>
        /// <param name="password"> The password to verify. </param>
        /// <param name="saltedHash"> The salted hash of the password to compare the password with. </param>
        /// <param name="iterations"> The number of iterations to apply to the hash generation. </param>
        /// <returns> Whether the password is correct or not. </returns>
        public bool VerifyPassword(byte[] password, string saltedHash, int iterations) => VerifyPassword(password, saltedHash, iterations, SALT_SIZE);

        /// <summary>
        /// Verifies a password using the specified algorithm.
        /// </summary>
        /// <param name="password"> The password to verify. </param>
        /// <param name="saltedHash"> The salted hash of the password to compare the password with. </param>
        /// <param name="iterations"> The number of iterations to apply to the hash generation. </param>
        /// <param name="saltSize"> The size of the salt. </param>
        /// <returns> Whether the password is correct or not. </returns>
        public bool VerifyPassword(byte[] password, string saltedHash, int iterations, int saltSize) => VerifyPassword(password, saltedHash, iterations, saltSize, HASH_SIZE);

        /// <summary>
        /// Verifies a password using the specified algorithm.
        /// </summary>
        /// <param name="password"> The password to verify. </param>
        /// <param name="saltedHash"> The salted hash of the password to compare the password with. </param>
        /// <param name="iterations"> The number of iterations to apply to the hash generation. </param>
        /// <param name="saltSize"> The size of the salt. </param>
        /// <param name="hashSize"> The size of the hash. </param>
        /// <returns> Whether the password is correct or not. </returns>
        public bool VerifyPassword(byte[] password, string saltedHash, int iterations, int saltSize, int hashSize) => InternalVerifyPassword(password, saltedHash.GetBase64Bytes(), iterations, saltSize, hashSize);

        /// <summary>
        /// Verifies a password using a specified algorithm.
        /// </summary>
        /// <param name="password"> The password to verify. </param>
        /// <param name="saltedHash"> The salted hash of the password to compare the password with. </param>
        /// <param name="iterations"> The number of iterations to apply to the hash generation. </param>
        /// <param name="saltSize"> The size of the salt. </param>
        /// <param name="hashSize"> The size of the hash. </param>
        /// <returns> Whether the password is correct or not. </returns>
        private bool InternalVerifyPassword(byte[] password, byte[] saltedHash, int iterations, int saltSize, int hashSize)
        {
            saltSize = saltSize <= MIN_SALT_SIZE ? MIN_SALT_SIZE : saltSize;
            return CheckEquals(password, saltedHash.Skip(saltSize).ToArray(), saltedHash.Take(saltSize).ToArray(), iterations, hashSize);
        }

        /// <summary>
        /// Gets the salted password hash of a password.
        /// </summary>
        /// <param name="password"> The password to get the salted hash for. </param>
        /// <param name="iterations"> The number of iterations to apply to the encryption. </param>
        /// <param name="saltSize"> The size of the salt. </param>
        /// <param name="hashSize"> The size of the hash. </param>
        /// <returns> The salted hash as a <see langword="byte"/>[]. </returns>
        private byte[] InternalGetSaltedPasswordHash(byte[] password, int iterations, int saltSize, int hashSize)
        {
            byte[] salt = new AdvancedSecureRandom(engine.PBKDF2Digest).NextBytes(saltSize <= MIN_SALT_SIZE ? MIN_SALT_SIZE : saltSize);
            return salt.Concat(GeneratePasswordHash(password, salt, iterations, hashSize)).ToArray();
        }

        /// <summary>
        /// Generates a password hash using the <see cref="IDigest"/> and a salt.
        /// </summary>
        /// <param name="password"> The password to generate the hash for. </param>
        /// <param name="salt"> The salt to use to generate our password hash. </param>
        /// <param name="iterations"> The number of iterations to apply to the encryption. </param>
        /// <param name="hashSize"> The size of the hash. </param>
        /// <returns> The generated password hash as a <see langword="byte"/>[]. </returns>
        private byte[] GeneratePasswordHash(byte[] password, byte[] salt, int iterations, int hashSize)
        {
            var generator = new Pkcs5S2ParametersGenerator(engine.PBKDF2Digest);
            generator.Init(password, salt, iterations <= MIN_ITERATIONS ? MIN_ITERATIONS : iterations);

            return ((KeyParameter)generator.GenerateDerivedMacParameters((hashSize <= MIN_HASH_SIZE ? MIN_HASH_SIZE : hashSize) * 8)).GetKey();
        }

        /// <summary>
        /// Checks if our password is equal to the correct password contained in the hash.
        /// </summary>
        /// <param name="password"> The password to check. </param>
        /// <param name="hash"> The hash of our correct password. </param>
        /// <param name="salt"> The salt pulled out from our salted hash. </param>
        /// <param name="iterations"> The number of iterations applied to the password encryption. </param>
        /// <param name="hashSize"> The size of the password hash. </param>
        /// <returns> Whether the password is equal to the correct password in the password hash. </returns>
        private bool CheckEquals(byte[] password, byte[] hash, byte[] salt, int iterations, int hashSize)
        {
            byte[] correctHash = hash;
            byte[] hashToCheck = GeneratePasswordHash(password, salt, iterations, hashSize);

            uint diff = (uint)correctHash.Length ^ (uint)hashToCheck.Length;

            for (int i = 0; i < correctHash.Length && i < hashToCheck.Length; i++)
                diff |= (uint)(correctHash[i] ^ hashToCheck[i]);

            hash.ClearBytes();
            salt.ClearBytes();

            return diff == 0;
        }
    }
}
