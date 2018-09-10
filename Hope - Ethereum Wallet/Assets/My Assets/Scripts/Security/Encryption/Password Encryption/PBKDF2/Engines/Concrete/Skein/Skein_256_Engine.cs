using Hope.Security.PBKDF2.Engines.Abstract;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;

namespace Hope.Security.PBKDF2.Engines.Skein
{
    public sealed class Skein_256_Engine : PBKDF2Engine
    {
        public override IDigest PBKDF2Digest => new SkeinDigest(256, 256);
    }
}