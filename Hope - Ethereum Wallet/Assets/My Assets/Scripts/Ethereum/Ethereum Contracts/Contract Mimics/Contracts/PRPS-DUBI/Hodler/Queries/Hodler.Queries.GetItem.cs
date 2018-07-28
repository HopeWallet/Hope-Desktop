using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

public static partial class HodlerMimic
{

    public static partial class Queries
    {
        [Function("getItem", typeof(Output.Item))]
        public sealed class GetItem : ContractFunction
        {

            [Parameter("address", "_user", 1)]
            public string User => (string)input[0];

            [Parameter("uint256", "_id", 2)]
            public BigInteger Id => (BigInteger)input[1];

            public GetItem(params object[] functionInput) : base(functionInput)
            {
            }
        }
    }
}