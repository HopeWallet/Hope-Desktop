using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Prng;

namespace Hope.Utils.Random
{

    public static class RandomBytes
    {
        public static byte[] GetSHA512Bytes(int length)
        {
            return GetSHA512Bytes((byte[])null, length);
        }

        public static byte[] GetSHA512Bytes(string seed, int length)
        {
            return GetSHA512Bytes(seed.GetUTF8Bytes(), length);
        }

        public static byte[] GetSHA512Bytes(byte[] seed, int length)
        {
            return GetBytes(seed, length, new Sha512Digest());
        }

        public static byte[] GetSHA256Bytes(int length)
        {
            return GetSHA256Bytes((byte[])null, length);
        }

        public static byte[] GetSHA256Bytes(string seed, int length)
        {
            return GetSHA256Bytes(seed.GetUTF8Bytes(), length);
        }

        public static byte[] GetSHA256Bytes(byte[] seed, int length)
        {
            return GetBytes(seed, length, new Sha256Digest());
        }

        private static byte[] GetBytes(byte[] seed, int length, IDigest digest)
        {
            var random = new DigestRandomGenerator(digest);
            var bytes = new byte[length];

            if (seed?.Length > 0)
                random.AddSeedMaterial(seed);

            random.NextBytes(bytes);

            return bytes;
        }
    }

}