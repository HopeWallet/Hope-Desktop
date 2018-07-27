using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

public static class ERC20
{
    public static class Functions
    {
        [Function("name", "string")]
        public sealed class Name : ContractFunction
        {
        }

        [Function("symbol", "string")]
        public sealed class Symbol : ContractFunction
        {
        }

        [Function("decimals", "uint256")]
        public sealed class Decimals : ContractFunction
        {
        }

        [Function("totalSupply", "uint256")]
        public sealed class TotalSupply : ContractFunction
        {
        }

        [Function("balanceOf", "uint256")]
        public sealed class BalanceOf : ContractFunction
        {
            [Parameter("address", "_owner", 1)]
            public string Owner => (string)input[0];

            public BalanceOf(params object[] functionInput) : base(functionInput)
            {
            }
        }

        [Function("transfer", "bool")]
        public sealed class Transfer : ContractFunction
        {
            [Parameter("address", "_to", 1)]
            public string To => (string)input[0];

            [Parameter("uint256", "_value", 2)]
            public BigInteger Value => (BigInteger)input[1];

            public Transfer(params object[] functionInput) : base(functionInput)
            {
            }
        }
    }
}