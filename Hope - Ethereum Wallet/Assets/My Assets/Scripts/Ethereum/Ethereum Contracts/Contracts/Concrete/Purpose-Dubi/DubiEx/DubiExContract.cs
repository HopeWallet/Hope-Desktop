using Nethereum.Contracts;
using System;

/// <summary>
/// Class used for managing the DubiEx contract.
/// </summary>
public class DubiExContract : ContractBase
{

    private const string FUNC_TAKEORDER = "takeOrder";
    private const string FUNC_MAKEORDER = "makeOrder";
    private const string FUNC_CANCELORDER = "cancelOrder";
    private const string FUNC_TAKEORDERS = "takeOrders";
    private const string FUNC_MAKEORDERS = "makeOrders";
    private const string FUNC_CANCELORDERS = "cancelOrders";

    /// <summary>
    /// The function names of the DubiEx contract.
    /// </summary>
    protected override string[] FunctionNames => new string[] { FUNC_TAKEORDER, FUNC_MAKEORDER, FUNC_CANCELORDER, FUNC_TAKEORDERS, FUNC_MAKEORDERS, FUNC_CANCELORDERS };

    /// <summary>
    /// Initializes the DubiEx contract.
    /// </summary>
    /// <param name="contractAddress"> The address of this contract. </param>
    /// <param name="abi"> The abi of the contract. </param>
    /// <param name="onContractInitialized"> Action to call when the contract has been intialized. </param>
    public DubiExContract(string contractAddress, string abi, Action<ContractBase, string> onContractInitialized = null) : base(contractAddress, abi, onContractInitialized) { }

}