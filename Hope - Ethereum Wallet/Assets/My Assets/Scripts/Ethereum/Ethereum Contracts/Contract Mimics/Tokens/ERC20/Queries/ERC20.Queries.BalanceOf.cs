using Nethereum.ABI.FunctionEncoding.Attributes;

public static partial class ERC20
{
    public static partial class Queries
    {
        [Function("balanceOf", "uint256")]
        public sealed class BalanceOf : ContractFunction
        {
            [Parameter("address", "_owner", 1)]
            public string Owner => (string)input[0];

            public BalanceOf(params object[] functionInput) : base(functionInput)
            {
            }
        }
    }
}