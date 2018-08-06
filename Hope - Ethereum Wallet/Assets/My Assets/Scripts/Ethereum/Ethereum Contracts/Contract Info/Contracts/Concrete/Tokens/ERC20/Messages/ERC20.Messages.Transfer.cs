using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

/// <summary>
/// Class which mimics the ethereum ERC20 token standard and contains most functions which are active in the token standard.
/// </summary>
public sealed partial class ERC20
{
    /// <summary>
    /// Class which contains the different messages which interact/change the ERC20 token contract values on the blockchain.
    /// </summary>
    public static partial class Messages
    {
        /// <summary>
        /// Class which contains the data needed to execute the ERC20 transfer function.
        /// </summary>
        [Function("transfer", "bool")]
        public sealed class Transfer : ContractFunction
        {
            /// <summary>
            /// The address to transfer the ERC20 token to.
            /// </summary>
            [Parameter("address", "_to", 1)]
            public string To => (string)input[0];

            /// <summary>
            /// The amount of the ERC20 token to send to the destination address.
            /// </summary>
            [Parameter("uint256", "_value", 2)]
            public BigInteger Value => (BigInteger)input[1];

            /// <summary>
            /// Initializes the <see cref="Transfer"/> function with the function input parameters.
            /// </summary>
            /// <param name="functionInput"> The input parameters to pass through the function. </param>
            public Transfer(params object[] functionInput) : base(functionInput)
            {
            }
        }
    }
}