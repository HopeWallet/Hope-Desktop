using Hope.Security.PBKDF2.Engines.Abstract;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;

namespace Hope.Security.PBKDF2.Engines.Shake
{
    public sealed class Shake_128_Engine : PBKDF2Engine
    {
        public override IDigest PBKDF2Digest => new ShakeDigest(128);
    }
}