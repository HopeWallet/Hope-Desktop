using Hope.Utils.Random.Abstract;
using Org.BouncyCastle.Crypto.Digests;

namespace Hope.Utils.Random
{
    /// <summary>
    /// Utility class for generating random byte data.
    /// </summary>
    public static class RandomBytes
    {
        public sealed class SHA1 : RandomBytesBase<Sha1Digest> { }

        public sealed class SHA3 : RandomBytesBase<Sha3Digest> { }

        public sealed class SHA256 : RandomBytesBase<Sha256Digest> { }

        public sealed class SHA384 : RandomBytesBase<Sha384Digest> { }

        public sealed class SHA512 : RandomBytesBase<Sha512Digest> { }

        public sealed class Keccak : RandomBytesBase<KeccakDigest> { }

        public sealed class Blake2b : RandomBytesBase<Blake2bDigest> { }

        public sealed class Whirlpool : RandomBytesBase<WhirlpoolDigest> { }
    }
}