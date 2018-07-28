using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

public static partial class ERC20
{
    public static partial class Messages
    {
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