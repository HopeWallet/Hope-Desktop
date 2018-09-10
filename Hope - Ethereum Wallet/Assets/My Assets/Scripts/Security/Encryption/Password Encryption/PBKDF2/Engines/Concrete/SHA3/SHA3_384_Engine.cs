using Hope.Security.PBKDF2.Engines.Abstract;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;

namespace Hope.Security.PBKDF2.Engines.SHA3
{
    public sealed class SHA3_384_Engine : PBKDF2Engine
    {
        public override IDigest PBKDF2Digest => new Sha3Digest(384);
    }
}