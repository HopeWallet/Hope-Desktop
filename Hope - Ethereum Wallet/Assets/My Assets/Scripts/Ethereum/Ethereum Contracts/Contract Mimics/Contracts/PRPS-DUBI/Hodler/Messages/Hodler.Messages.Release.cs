﻿using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

public static partial class HodlerMimic
{

    public static partial class Messages
    {
        [Function("release")]
        public sealed class Release : ContractFunction
        {

            [Parameter("uint256", "_id", 1)]
            public BigInteger Id => (BigInteger)input[0];

            public Release(params object[] functionInput) : base(functionInput)
            {
            }
        }
    }
}