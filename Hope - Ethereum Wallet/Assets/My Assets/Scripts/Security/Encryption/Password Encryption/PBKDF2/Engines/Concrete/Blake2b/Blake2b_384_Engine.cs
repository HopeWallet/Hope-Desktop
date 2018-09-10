using Hope.Security.PBKDF2.Engines.Abstract;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;

namespace Hope.Security.PBKDF2.Engines.Blake2b
{
    public sealed class Blake2b_384_Engine : PBKDF2Engine
    {
        public override IDigest PBKDF2Digest => new Blake2bDigest(384);
    }
}