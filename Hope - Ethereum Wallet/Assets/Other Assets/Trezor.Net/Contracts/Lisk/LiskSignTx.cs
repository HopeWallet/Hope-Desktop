namespace Trezor.Net.Contracts.Lisk
{
    [ProtoBuf.ProtoContract()]
    public class LiskSignTx : ProtoBuf.IExtensible
    {
        private ProtoBuf.IExtension __pbn__extensionData;
        ProtoBuf.IExtension ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [ProtoBuf.ProtoMember(1, Name = @"address_n")]
        public uint[] AddressNs { get; set; }

        [ProtoBuf.ProtoMember(2, Name = @"transaction")]
        public LiskTransactionCommon Transaction { get; set; }

        [ProtoBuf.ProtoContract()]
        public class LiskTransactionCommon : ProtoBuf.IExtensible
        {
            private ProtoBuf.IExtension __pbn__extensionData;
            ProtoBuf.IExtension ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
                => ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

            [ProtoBuf.ProtoMember(1, Name = @"type")]
            [System.ComponentModel.DefaultValue(LiskTransactionType.Transfer)]
            public LiskTransactionType Type
            {
                get { return __pbn__Type ?? LiskTransactionType.Transfer;
                } set { __pbn__Type = value; }
            }
            public bool ShouldSerializeType() => __pbn__Type != null;
            public void ResetType() => __pbn__Type = null;
            private LiskTransactionType? __pbn__Type;

            [ProtoBuf.ProtoMember(2, Name = @"amount")]
            [System.ComponentModel.DefaultValue(0)]
            public ulong Amount
            {
                get { return __pbn__Amount ?? 0;
                } set { __pbn__Amount = value; }
            }
            public bool ShouldSerializeAmount() => __pbn__Amount != null;
            public void ResetAmount() => __pbn__Amount = null;
            private ulong? __pbn__Amount;

            [ProtoBuf.ProtoMember(3, Name = @"fee")]
            public ulong Fee
            {
                get { return __pbn__Fee.GetValueOrDefault();
                } set { __pbn__Fee = value; }
            }
            public bool ShouldSerializeFee() => __pbn__Fee != null;
            public void ResetFee() => __pbn__Fee = null;
            private ulong? __pbn__Fee;

            [ProtoBuf.ProtoMember(4, Name = @"recipient_id")]
            [System.ComponentModel.DefaultValue("")]
            public string RecipientId
            {
                get { return __pbn__RecipientId ?? "";
                } set { __pbn__RecipientId = value; }
            }
            public bool ShouldSerializeRecipientId() => __pbn__RecipientId != null;
            public void ResetRecipientId() => __pbn__RecipientId = null;
            private string __pbn__RecipientId;

            [ProtoBuf.ProtoMember(5, Name = @"sender_public_key")]
            public byte[] SenderPublicKey { get; set; }
            public bool ShouldSerializeSenderPublicKey() => SenderPublicKey != null;
            public void ResetSenderPublicKey() => SenderPublicKey = null;

            [ProtoBuf.ProtoMember(6, Name = @"requester_public_key")]
            public byte[] RequesterPublicKey { get; set; }
            public bool ShouldSerializeRequesterPublicKey() => RequesterPublicKey != null;
            public void ResetRequesterPublicKey() => RequesterPublicKey = null;

            [ProtoBuf.ProtoMember(7, Name = @"signature")]
            public byte[] Signature { get; set; }
            public bool ShouldSerializeSignature() => Signature != null;
            public void ResetSignature() => Signature = null;

            [ProtoBuf.ProtoMember(8, Name = @"timestamp")]
            public uint Timestamp
            {
                get { return __pbn__Timestamp.GetValueOrDefault();
                } set { __pbn__Timestamp = value; }
            }
            public bool ShouldSerializeTimestamp() => __pbn__Timestamp != null;
            public void ResetTimestamp() => __pbn__Timestamp = null;
            private uint? __pbn__Timestamp;

            [ProtoBuf.ProtoMember(9, Name = @"asset")]
            public LiskTransactionAsset Asset { get; set; }

            [ProtoBuf.ProtoContract()]
            public class LiskTransactionAsset : ProtoBuf.IExtensible
            {
                private ProtoBuf.IExtension __pbn__extensionData;
                ProtoBuf.IExtension ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
                    => ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

                [ProtoBuf.ProtoMember(1, Name = @"signature")]
                public LiskSignatureType Signature { get; set; }

                [ProtoBuf.ProtoMember(2, Name = @"delegate")]
                public LiskDelegateType Delegate { get; set; }

                [ProtoBuf.ProtoMember(3, Name = @"votes")]
                public System.Collections.Generic.List<string> Votes { get; } = new System.Collections.Generic.List<string>();

                [ProtoBuf.ProtoMember(4, Name = @"multisignature")]
                public LiskMultisignatureType Multisignature { get; set; }

                [ProtoBuf.ProtoMember(5, Name = @"data")]
                [System.ComponentModel.DefaultValue("")]
                public string Data
                {
                    get { return __pbn__Data ?? "";
                    } set { __pbn__Data = value; }
                }
                public bool ShouldSerializeData() => __pbn__Data != null;
                public void ResetData() => __pbn__Data = null;
                private string __pbn__Data;

                [ProtoBuf.ProtoContract()]
                public class LiskSignatureType : ProtoBuf.IExtensible
                {
                    private ProtoBuf.IExtension __pbn__extensionData;
                    ProtoBuf.IExtension ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
                        => ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

                    [ProtoBuf.ProtoMember(1, Name = @"public_key")]
                    public byte[] PublicKey { get; set; }
                    public bool ShouldSerializePublicKey() => PublicKey != null;
                    public void ResetPublicKey() => PublicKey = null;
                }

                [ProtoBuf.ProtoContract()]
                public class LiskDelegateType : ProtoBuf.IExtensible
                {
                    private ProtoBuf.IExtension __pbn__extensionData;
                    ProtoBuf.IExtension ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
                        => ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

                    [ProtoBuf.ProtoMember(1, Name = @"username")]
                    [System.ComponentModel.DefaultValue("")]
                    public string Username
                    {
                        get { return __pbn__Username ?? "";
                        } set { __pbn__Username = value; }
                    }
                    public bool ShouldSerializeUsername() => __pbn__Username != null;
                    public void ResetUsername() => __pbn__Username = null;
                    private string __pbn__Username;

                }

                [ProtoBuf.ProtoContract()]
                public class LiskMultisignatureType : ProtoBuf.IExtensible
                {
                    private ProtoBuf.IExtension __pbn__extensionData;
                    ProtoBuf.IExtension ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
                        => ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

                    [ProtoBuf.ProtoMember(1, Name = @"min")]
                    public uint Min
                    {
                        get { return __pbn__Min.GetValueOrDefault();
                        } set { __pbn__Min = value; }
                    }
                    public bool ShouldSerializeMin() => __pbn__Min != null;
                    public void ResetMin() => __pbn__Min = null;
                    private uint? __pbn__Min;

                    [ProtoBuf.ProtoMember(2, Name = @"life_time")]
                    public uint LifeTime
                    {
                        get { return __pbn__LifeTime.GetValueOrDefault();
                        } set { __pbn__LifeTime = value; }
                    }
                    public bool ShouldSerializeLifeTime() => __pbn__LifeTime != null;
                    public void ResetLifeTime() => __pbn__LifeTime = null;
                    private uint? __pbn__LifeTime;

                    [ProtoBuf.ProtoMember(3, Name = @"keys_group")]
                    public System.Collections.Generic.List<string> KeysGroups { get; } = new System.Collections.Generic.List<string>();

                }

            }

            [ProtoBuf.ProtoContract()]
            public enum LiskTransactionType
            {
                Transfer = 0,
                RegisterSecondPassphrase = 1,
                RegisterDelegate = 2,
                CastVotes = 3,
                RegisterMultisignatureAccount = 4,
                CreateDapp = 5,
                TransferIntoDapp = 6,
                TransferOutOfDapp = 7,
            }

        }

    }
}