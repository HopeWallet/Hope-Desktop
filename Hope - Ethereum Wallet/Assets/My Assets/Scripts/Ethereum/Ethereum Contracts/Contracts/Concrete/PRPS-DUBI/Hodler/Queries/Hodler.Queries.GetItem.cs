using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

/// <summary>
/// Class which represents the Hodler smart contract used for locking purpose and receiving dubi.
/// </summary>
public sealed partial class Hodler : StaticSmartContract
{
    /// <summary>
    /// Class which holds any queries in the Hodler smart contract.
    /// </summary>
    public static partial class Queries
    {
        /// <summary>
        /// Class which represents the query for receiving the status of locked purpose.
        /// </summary>
        [Function("getItem", typeof(Output.Item))]
        public sealed class GetItem : ContractFunction
        {
            /// <summary>
            /// The user who locked the purpose.
            /// </summary>
            [Parameter("address", "_user", 1)]
            public string User => (string)input[0];

            /// <summary>
            /// The id of the locked purpose.
            /// </summary>
            [Parameter("uint256", "_id", 2)]
            public BigInteger Id => (BigInteger)input[1];

            /// <summary>
            /// Initializes the query by assigning the function input.
            /// </summary>
            /// <param name="functionInput"> The input of the query function. </param>
            public GetItem(params object[] functionInput) : base(functionInput)
            {
            }
        }
    }
}