﻿using NBitcoin.DataEncoders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedgerWallet
{

    /// <summary>
    /// 3DES-2 private key
    /// </summary>
    public class Ledger3DESKey
    {
        byte[] _Key;
        public Ledger3DESKey(string hex)
            : this(Encoders.Hex.DecodeData(hex))
        {
        }
        public Ledger3DESKey(byte[] bytes)
        {
            if(bytes.Length != 16)
                throw new FormatException("Invalid byte count");
            _Key = bytes.ToArray();
        }

        public byte[] ToBytes()
        {
            return _Key.ToArray();
        }

        public string ToHex()
        {
            return Encoders.Hex.EncodeData(_Key, 0, 16);
        }
    }
}
