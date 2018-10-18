namespace Trezor.Net.Contracts.Common
{
    [ProtoBuf.ProtoContract()]
    public class PassphraseStateRequest : ProtoBuf.IExtensible
    {
        private ProtoBuf.IExtension __pbn__extensionData;
        ProtoBuf.IExtension ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [ProtoBuf.ProtoMember(1, Name = @"state")]
        public byte[] State { get; set; }
        public bool ShouldSerializeState() => State != null;
        public void ResetState() => State = null;
    }
}