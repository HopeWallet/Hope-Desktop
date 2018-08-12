using Org.BouncyCastle.Crypto;

namespace Hope.Utils.Random.Abstract
{
    public abstract class RandomIntBase<T> where T : IDigest, new()
    {
        public static int GetInt() => InternalGetInt(null, null, null);

        public static int GetInt(byte[] seed) => InternalGetInt(seed, null, null);

        public static int GetInt(string seed) => InternalGetInt(seed.GetUTF8Bytes(), null, null);

        public static int GetInt(int maxValue) => InternalGetInt(null, null, maxValue);

        public static int GetInt(byte[] seed, int maxValue) => InternalGetInt(seed, null, maxValue);

        public static int GetInt(string seed, int maxValue) => InternalGetInt(seed.GetUTF8Bytes(), null, maxValue);

        public static int GetInt(int minValue, int maxValue) => InternalGetInt(null, minValue, maxValue);

        public static int GetInt(byte[] seed, int minValue, int maxValue) => InternalGetInt(seed, minValue, maxValue);

        public static int GetInt(string seed, int minValue, int maxValue) => InternalGetInt(seed.GetUTF8Bytes(), minValue, maxValue);

        private static int InternalGetInt(byte[] seed, int? minValue, int? maxValue) => GetInt(seed, minValue, maxValue, new T());

        private static int GetInt(byte[] seed, int? minValue, int? maxValue, IDigest digest)
        {
            HopeSecureRandom secureRandom = (seed == null ? new HopeSecureRandom(digest) : new HopeSecureRandom(digest, seed));

            if (minValue.HasValue && maxValue.HasValue)
                return secureRandom.Next(minValue.Value, maxValue.Value);
            else if (maxValue.HasValue)
                return secureRandom.Next(maxValue.Value);
            else
                return secureRandom.Next();
        }
    }
}
