using Hope.Utils.Random.Abstract;
using Org.BouncyCastle.Crypto.Digests;

namespace Hope.Utils.Random
{
    public static class RandomInt
    {
        public sealed class SHA256 : RandomIntBase<Sha256Digest> { }

        public sealed class SHA512 : RandomIntBase<Sha512Digest> { }

        public sealed class Keccak : RandomIntBase<KeccakDigest> { }

        public sealed class Blake2b : RandomIntBase<Blake2bDigest> { }
    }
}
