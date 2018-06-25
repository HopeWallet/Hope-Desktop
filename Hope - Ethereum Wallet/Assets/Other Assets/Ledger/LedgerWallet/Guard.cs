﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedgerWallet
{
    class Guard
    {
        internal static void AssertKeyPath(NBitcoin.KeyPath keyPath)
        {
            if(keyPath.Indexes.Length > 10)
                throw new ArgumentOutOfRangeException("keypath", "The key path should have a maximum size of 10 derivations");
        }
    }
}
