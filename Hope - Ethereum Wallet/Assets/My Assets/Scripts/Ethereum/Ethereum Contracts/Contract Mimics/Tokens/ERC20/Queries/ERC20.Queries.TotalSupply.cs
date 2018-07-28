using Nethereum.ABI.FunctionEncoding.Attributes;

public static partial class ERC20
{
    public static partial class Queries
    {
        [Function("totalSupply", "uint256")]
        public sealed class TotalSupply : ContractFunction
        {
        }
    }
}