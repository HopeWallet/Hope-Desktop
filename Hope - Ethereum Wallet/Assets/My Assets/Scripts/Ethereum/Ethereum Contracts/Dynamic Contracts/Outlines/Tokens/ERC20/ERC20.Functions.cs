using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public partial class ERC20
{
    public static class Functions
    {
        [Function("name", "string")]
        public sealed class Name : FunctionMessage
        {
        }

        [Function("symbol", "string")]
        public sealed class Symbol : FunctionMessage
        {
        }

        [Function("decimals", "string")]
        public sealed class Decimals : FunctionMessage
        {
        }

        [Function("totalSupply", "uint256")]
        public sealed class TotalSupply : FunctionMessage
        {
        }

        [Function("balanceOf", "uint256")]
        public sealed class BalanceOf : FunctionMessage
        {
            [Parameter("address", "_owner", 1)]
            public string Owner { get; set; }
        }

        [Function("transfer", "bool")]
        public sealed class Transfer : FunctionMessage
        {
            [Parameter("address", "_to", 1)]
            public string To { get; set; }

            [Parameter("uint256", "_value", 2)]
            public BigInteger Value { get; set; }
        }
    }
}