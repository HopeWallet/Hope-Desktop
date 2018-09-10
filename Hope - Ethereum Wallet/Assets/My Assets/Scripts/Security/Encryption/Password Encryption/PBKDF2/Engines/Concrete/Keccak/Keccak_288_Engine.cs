using Hope.Security.PBKDF2.Engines.Abstract;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;

namespace Hope.Security.PBKDF2.Engines.Keccak
{
    public sealed class Keccak_288_Engine : PBKDF2Engine
    {
        public override IDigest PBKDF2Digest => new KeccakDigest(288);
    }
}