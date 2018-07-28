using Nethereum.ABI.FunctionEncoding.Attributes;

public static partial class ERC20
{
    public static partial class Queries
    {
        [Function("symbol", "string")]
        public sealed class Symbol : ContractFunction
        {
        }
    }
}