namespace Trezor.Net.Contracts.Debug
{
    [ProtoBuf.ProtoContract()]
    public class DebugLinkState : ProtoBuf.IExtensible
    {
        private ProtoBuf.IExtension __pbn__extensionData;
        ProtoBuf.IExtension ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [ProtoBuf.ProtoMember(1, Name = @"layout")]
        public byte[] Layout { get; set; }
        public bool ShouldSerializeLayout() => Layout != null;
        public void ResetLayout() => Layout = null;

        [ProtoBuf.ProtoMember(2, Name = @"pin")]
        [System.ComponentModel.DefaultValue("")]
        public string Pin
        {
            get { return __pbn__Pin ?? "";
            } set { __pbn__Pin = value; }
        }
        public bool ShouldSerializePin() => __pbn__Pin != null;
        public void ResetPin() => __pbn__Pin = null;
        private string __pbn__Pin;

        [ProtoBuf.ProtoMember(3, Name = @"matrix")]
        [System.ComponentModel.DefaultValue("")]
        public string Matrix
        {
            get { return __pbn__Matrix ?? "";
            } set { __pbn__Matrix = value; }
        }
        public bool ShouldSerializeMatrix() => __pbn__Matrix != null;
        public void ResetMatrix() => __pbn__Matrix = null;
        private string __pbn__Matrix;

        [ProtoBuf.ProtoMember(4, Name = @"mnemonic")]
        [System.ComponentModel.DefaultValue("")]
        public string Mnemonic
        {
            get { return __pbn__Mnemonic ?? "";
            } set { __pbn__Mnemonic = value; }
        }
        public bool ShouldSerializeMnemonic() => __pbn__Mnemonic != null;
        public void ResetMnemonic() => __pbn__Mnemonic = null;
        private string __pbn__Mnemonic;

        [ProtoBuf.ProtoMember(5, Name = @"node")]
        public Common.HDNodeType Node { get; set; }

        [ProtoBuf.ProtoMember(6, Name = @"passphrase_protection")]
        public bool PassphraseProtection
        {
            get { return __pbn__PassphraseProtection.GetValueOrDefault();
            } set { __pbn__PassphraseProtection = value; }
        }
        public bool ShouldSerializePassphraseProtection() => __pbn__PassphraseProtection != null;
        public void ResetPassphraseProtection() => __pbn__PassphraseProtection = null;
        private bool? __pbn__PassphraseProtection;

        [ProtoBuf.ProtoMember(7, Name = @"reset_word")]
        [System.ComponentModel.DefaultValue("")]
        public string ResetWord
        {
            get { return __pbn__ResetWord ?? "";
            } set { __pbn__ResetWord = value; }
        }
        public bool ShouldSerializeResetWord() => __pbn__ResetWord != null;
        public void ResetResetWord() => __pbn__ResetWord = null;
        private string __pbn__ResetWord;

        [ProtoBuf.ProtoMember(8, Name = @"reset_entropy")]
        public byte[] ResetEntropy { get; set; }
        public bool ShouldSerializeResetEntropy() => ResetEntropy != null;
        public void ResetResetEntropy() => ResetEntropy = null;

        [ProtoBuf.ProtoMember(9, Name = @"recovery_fake_word")]
        [System.ComponentModel.DefaultValue("")]
        public string RecoveryFakeWord
        {
            get { return __pbn__RecoveryFakeWord ?? "";
            } set { __pbn__RecoveryFakeWord = value; }
        }
        public bool ShouldSerializeRecoveryFakeWord() => __pbn__RecoveryFakeWord != null;
        public void ResetRecoveryFakeWord() => __pbn__RecoveryFakeWord = null;
        private string __pbn__RecoveryFakeWord;

        [ProtoBuf.ProtoMember(10, Name = @"recovery_word_pos")]
        public uint RecoveryWordPos
        {
            get { return __pbn__RecoveryWordPos.GetValueOrDefault();
            } set { __pbn__RecoveryWordPos = value; }
        }
        public bool ShouldSerializeRecoveryWordPos() => __pbn__RecoveryWordPos != null;
        public void ResetRecoveryWordPos() => __pbn__RecoveryWordPos = null;
        private uint? __pbn__RecoveryWordPos;

        [ProtoBuf.ProtoMember(11, Name = @"reset_word_pos")]
        public uint ResetWordPos
        {
            get { return __pbn__ResetWordPos.GetValueOrDefault();
            } set { __pbn__ResetWordPos = value; }
        }
        public bool ShouldSerializeResetWordPos() => __pbn__ResetWordPos != null;
        public void ResetResetWordPos() => __pbn__ResetWordPos = null;
        private uint? __pbn__ResetWordPos;

    }
}