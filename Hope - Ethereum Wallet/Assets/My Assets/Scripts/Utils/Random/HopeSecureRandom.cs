using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Security;
using System;

namespace Hope.Utils.Random
{
    public sealed class HopeSecureRandom : System.Random
    {
        private readonly SecureRandom secureRandom;

        public HopeSecureRandom() : this((object[])null)
        {
        }

        public HopeSecureRandom(params object[] seedData) : this(new Sha256Digest(), seedData)
        {
        }

        public HopeSecureRandom(IDigest randomDigest) : this(randomDigest, null)
        {
        }

        public HopeSecureRandom(IDigest randomDigest, params object[] seedData)
        {
            secureRandom = GetSecureRandom(randomDigest, seedData);
        }

        public override int Next()
        {
            return secureRandom.Next();
        }

        public override int Next(int maxValue)
        {
            return secureRandom.Next(maxValue);
        }

        public override int Next(int minValue, int maxValue)
        {
            return secureRandom.Next(minValue, maxValue);
        }

        public override double NextDouble()
        {
            return secureRandom.NextDouble();
        }

        public long NextLong()
        {
            return secureRandom.NextLong();
        }

        public byte NextByte()
        {
            return (byte)Next(Byte.MinValue, Byte.MaxValue + 1);
        }

        public byte[] NextBytes(int length)
        {
            byte[] buffer;
            NextBytes(buffer = new byte[length]);

            return buffer;
        }

        public override void NextBytes(byte[] buffer)
        {
            secureRandom.NextBytes(buffer);
        }

        private SecureRandom GetSecureRandom(IDigest randomDigest, object[] seedData)
        {
            IRandomGenerator randomGenerator = new DigestRandomGenerator(randomDigest);
            seedData?.ForEach(seed => randomGenerator.AddSeedMaterial(seed.GetType() == typeof(byte[]) ? (byte[])seed : seed.ToString().GetUTF8Bytes()));

            if (seedData == null || seedData.Length == 0)
                randomGenerator.AddSeedMaterial(SecureRandom.GetNextBytes(new SecureRandom(), 16));

            return new SecureRandom(randomGenerator);
        }
    }
}