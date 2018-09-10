using Org.BouncyCastle.Crypto;

namespace Hope.Security.PBKDF2.Engines.Abstract
{
    public abstract class PBKDF2Engine
    {
        public abstract IDigest PBKDF2Digest { get; }
    }
}
