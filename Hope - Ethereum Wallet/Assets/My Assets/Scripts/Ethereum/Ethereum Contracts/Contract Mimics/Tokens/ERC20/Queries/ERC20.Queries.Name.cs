using Nethereum.ABI.FunctionEncoding.Attributes;

public static partial class ERC20
{
    public static partial class Queries
    {
        [Function("name", "string")]
        public sealed class Name : ContractFunction
        {
        }
    }
}