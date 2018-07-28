using Nethereum.ABI.FunctionEncoding.Attributes;

/// <summary>
/// Class which mimics the ethereum ERC20 token standard and contains most functions which are active in the token standard.
/// </summary>
public static partial class ERC20
{
    /// <summary>
    /// Class which contains the different queries for receiving data from the ERC20 token contract.
    /// </summary>
    public static partial class Queries
    {
        /// <summary>
        /// Class which contains the data needed to read the balance of a certain address of the ERC20 token contract.
        /// </summary>
        [Function("balanceOf", "uint256")]
        public sealed class BalanceOf : ContractFunction
        {
            /// <summary>
            /// The owner to check the ERC20 token balance of.
            /// </summary>
            [Parameter("address", "_owner", 1)]
            public string Owner => (string)input[0];

            /// <summary>
            /// Initializes the <see cref="BalanceOf"/> function with the function input parameters.
            /// </summary>
            /// <param name="functionInput"> The input parameters to pass through the function. </param>
            public BalanceOf(params object[] functionInput) : base(functionInput)
            {
            }
        }
    }
}