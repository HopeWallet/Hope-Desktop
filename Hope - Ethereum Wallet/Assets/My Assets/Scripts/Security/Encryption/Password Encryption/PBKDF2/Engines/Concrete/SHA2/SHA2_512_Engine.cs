using Hope.Security.PBKDF2.Engines.Abstract;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;

namespace Hope.Security.PBKDF2.Engines.SHA2
{
    public sealed class SHA2_512_Engine : PBKDF2Engine
    {
        public override IDigest PBKDF2Digest => new Sha512Digest();
    }
}