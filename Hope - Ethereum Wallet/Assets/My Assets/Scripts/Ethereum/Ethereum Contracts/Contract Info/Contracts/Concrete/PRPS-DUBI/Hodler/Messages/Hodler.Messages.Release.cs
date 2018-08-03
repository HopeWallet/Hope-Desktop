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
        /// Function used for releasing some locked purpose.
        /// </summary>
        [Function("release")]
        public sealed class Release : ContractFunction
        {
            /// <summary>
            /// The id of the locked purpose.
            /// </summary>
            [Parameter("uint256", "_id", 1)]
            public BigInteger Id => (BigInteger)input[0];

            /// <summary>
            /// Initializes the Release function.
            /// </summary>
            /// <param name="functionInput"> The input of the function. </param>
            public Release(params object[] functionInput) : base(functionInput)
            {
            }
        }
    }
}