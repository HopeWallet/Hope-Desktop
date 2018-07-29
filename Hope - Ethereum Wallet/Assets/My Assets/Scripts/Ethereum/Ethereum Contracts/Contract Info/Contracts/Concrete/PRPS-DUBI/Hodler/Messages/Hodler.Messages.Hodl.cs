using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

public sealed partial class HodlerMimic : StaticSmartContract
{

    public static partial class Messages
    {
        [Function("hodl")]
        public sealed class Hodl : ContractFunction
        {

            [Parameter("uint256", "_id", 1)]
            public BigInteger Id => (BigInteger)input[0];

            [Parameter("uint256", "_value", 2)]
            public BigInteger Value => (BigInteger)input[1];

            [Parameter("uint256", "_months", 3)]
            public BigInteger Months => (BigInteger)input[2];

            public Hodl(params object[] functionInput) : base(functionInput)
            {
            }
        }
    }
}