using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

public static class ERC20
{
    public static class Functions
    {
        [Function("name", "string")]
        public sealed class Name : QueryFunction
        {
        }

        [Function("symbol", "string")]
        public sealed class Symbol : QueryFunction
        {
        }

        [Function("decimals", "uint256")]
        public sealed class Decimals : QueryFunction
        {
        }

        [Function("totalSupply", "uint256")]
        public sealed class TotalSupply : QueryFunction
        {
        }

        [Function("balanceOf", "uint256")]
        public sealed class BalanceOf : QueryFunction
        {
            [Parameter("address", "_owner", 1)]
            public string Owner => (string)input[0];

            public BalanceOf(params object[] functionInput) : base(functionInput)
            {
            }
        }

        [Function("transfer", "bool")]
        public sealed class Transfer : QueryFunction
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