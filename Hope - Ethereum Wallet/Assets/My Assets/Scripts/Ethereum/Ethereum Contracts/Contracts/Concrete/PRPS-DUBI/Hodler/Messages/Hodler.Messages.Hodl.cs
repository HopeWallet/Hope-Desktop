using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

/// <summary>
/// Class which represents the Hodler smart contract used for locking purpose and receiving dubi.
/// </summary>
public sealed partial class Hodler : StaticSmartContract
{
    /// <summary>
    /// Class which holds messages for interacting with the Hodler smart contract.
    /// </summary>
    public static partial class Messages
    {
        /// <summary>
        /// Function used for locking some purpose into the hodler smart contract.
        /// </summary>
        [Function("hodl")]
        public sealed class Hodl : ContractFunction
        {
            /// <summary>
            /// The id to assign to the purpose that will be locked.
            /// </summary>
            [Parameter("uint256", "_id", 1)]
            public BigInteger Id => (BigInteger)input[0];

            /// <summary>
            /// The amount of purpose to lock.
            /// </summary>
            [Parameter("uint256", "_value", 2)]
            public BigInteger Value => (BigInteger)input[1];

            /// <summary>
            /// The number of months to lock the purpose for. Only 3, 6, or 12 is valid.
            /// </summary>
            [Parameter("uint256", "_months", 3)]
            public BigInteger Months => (BigInteger)input[2];

            /// <summary>
            /// Initializes the hodl function.
            /// </summary>
            /// <param name="functionInput"> The input of the function. </param>
            public Hodl(params object[] functionInput) : base(functionInput)
            {
            }
        }
    }
}