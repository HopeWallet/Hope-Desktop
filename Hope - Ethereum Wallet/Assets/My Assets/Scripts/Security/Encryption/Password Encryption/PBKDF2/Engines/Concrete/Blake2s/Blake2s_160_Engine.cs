using Hope.Security.PBKDF2.Engines.Abstract;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;

namespace Hope.Security.PBKDF2.Engines.Blake2s
{
    public sealed class Blake2s_160_Engine : PBKDF2Engine
    {
        public override IDigest PBKDF2Digest => new Blake2sDigest(160);
    }
}