// This file was generated by a tool; you should avoid making direct changes.
// Consider using 'partial classes' to extend these types
// Input: messages-stellar.proto

#pragma warning disable CS1591, CS0612, CS3021, IDE1006
namespace Trezor.Net.Contracts.Stellar
{

    [ProtoBuf.ProtoContract()]
    public class StellarAssetType : ProtoBuf.IExtensible
    {
        private ProtoBuf.IExtension __pbn__extensionData;
        ProtoBuf.IExtension ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [ProtoBuf.ProtoMember(1, Name = @"type")]
        public uint Type
        {
            get { return __pbn__Type.GetValueOrDefault();
            } set { __pbn__Type = value; }
        }
        public bool ShouldSerializeType() => __pbn__Type != null;
        public void ResetType() => __pbn__Type = null;
        private uint? __pbn__Type;

        [ProtoBuf.ProtoMember(2, Name = @"code")]
        [System.ComponentModel.DefaultValue("")]
        public string Code
        {
            get { return __pbn__Code ?? "";
            } set { __pbn__Code = value; }
        }
        public bool ShouldSerializeCode() => __pbn__Code != null;
        public void ResetCode() => __pbn__Code = null;
        private string __pbn__Code;

        [ProtoBuf.ProtoMember(3, Name = @"issuer")]
        [System.ComponentModel.DefaultValue("")]
        public string Issuer
        {
            get { return __pbn__Issuer ?? "";
            } set { __pbn__Issuer = value; }
        }
        public bool ShouldSerializeIssuer() => __pbn__Issuer != null;
        public void ResetIssuer() => __pbn__Issuer = null;
        private string __pbn__Issuer;

    }
}

#pragma warning restore CS1591, CS0612, CS3021, IDE1006
