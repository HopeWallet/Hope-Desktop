namespace Trezor.Net.Contracts.Crypto
{
    [ProtoBuf.ProtoContract()]
    public class CosiSignature : ProtoBuf.IExtensible
    {
        private ProtoBuf.IExtension __pbn__extensionData;
        ProtoBuf.IExtension ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [ProtoBuf.ProtoMember(1, Name = @"signature")]
        public byte[] Signature { get; set; }
        public bool ShouldSerializeSignature() => Signature != null;
        public void ResetSignature() => Signature = null;
    }
}