using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope.Utils.Random.Abstract
{
    public abstract class RandomStringBase<T> where T : IDigest, new()
    {
        public static string GetString() => GetString(16);

        public static string GetString(string seed) => GetString(seed.GetUTF8Bytes());

        public static string GetString(byte[] seed) => GetString(seed, 16);

        public static string GetString(int length) => GetString((byte[])null, length);

        public static string GetString(string seed, int length) => GetString(seed.GetUTF8Bytes(), length);

        public static string GetString(byte[] seed, int length) => InternalGetString(seed, length, new T());

        private static string InternalGetString(byte[] seed, int length, IDigest digest)
        {
            HopeSecureRandom secureRandom = (seed == null ? new HopeSecureRandom(digest) : new HopeSecureRandom(digest, seed));
            return secureRandom.NextBytes(length).GetBase64String().LimitEnd(length);
        }

    }
}