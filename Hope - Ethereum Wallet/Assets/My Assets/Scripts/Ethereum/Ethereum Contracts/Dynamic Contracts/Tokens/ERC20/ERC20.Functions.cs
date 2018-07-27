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
    public sealed class Functions
    {
        [Function("name", "string")]
        public sealed class Name : ConstructedFunction
        {
        }

        [Function("symbol", "string")]
        public sealed class Symbol : ConstructedFunction
        {
        }

        [Function("decimals", "string")]
        public sealed class Decimals : ConstructedFunction
        {
        }

        [Function("totalSupply", "uint256")]
        public sealed class TotalSupply : ConstructedFunction
        {
        }

        [Function("balanceOf", "uint256")]
        public sealed class BalanceOf : ConstructedFunction
        {
            [Parameter("address", "_owner", 1)]
            public string Owner => (string)input[0];

            public BalanceOf(params object[] functionInput) : base(functionInput)
            {
            }
        }

        [Function("transfer", "bool")]
        public sealed class Transfer : ConstructedFunction
        {
            [Parameter("address", "_to", 1)]
            public string To { get; set; }

            [Parameter("uint256", "_value", 2)]
            public BigInteger Value { get; set; }

            public Transfer(params object[] functionInput) : base(functionInput)
            {
            }
        }
    }
}