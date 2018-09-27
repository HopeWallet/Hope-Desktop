using System.IO;

namespace Ledger.Net.Responses
{
    public abstract class GetPublicKeyResponseBase : ResponseBase
    {
        public byte[] PublicKeyData;
        public byte[] ExtraData;

        public string Address { get; }

        public abstract string PublicKey { get; }

        protected GetPublicKeyResponseBase(byte[] data) : base(data)
        {
            if (!IsSuccess)
            {
                return;
            }

            using (var memoryStream = new MemoryStream(data))
            {
                var publicKeyLength = memoryStream.ReadByte();
                PublicKeyData = memoryStream.ReadAllBytes(publicKeyLength);
                var addressLength = memoryStream.ReadByte();
                Address = GetAddressFromStream(memoryStream, addressLength);
                ExtraData = memoryStream.ReadAllBytes(data.Length - (publicKeyLength + addressLength));
            }
        }

        protected abstract string GetAddressFromStream(Stream memoryStream, int addressLength);
    }
}
