using Nethereum.ABI.FunctionEncoding.Attributes;

public static partial class ERC20
{
    public static partial class Queries
    {
        [Function("decimals", "uint256")]
        public sealed class Decimals : ContractFunction
        {
        }
    }
}