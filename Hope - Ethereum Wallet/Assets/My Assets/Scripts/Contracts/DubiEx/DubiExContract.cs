using Nethereum.Contracts;
using System;

/// <summary>
/// Class used for managing the DubiEx contract.
/// </summary>
public class DubiExContract : ContractBase
{

    private Function takeOrder,
                     makeOrder,
                     cancelOrder,
                     takeOrders,
                     makeOrders,
                     cancelOrders;

    /// <summary>
    /// Initializes the DubiEx contract.
    /// </summary>
    /// <param name="contractAddress"> The address of this contract. </param>
    /// <param name="abi"> The abi of the contract. </param>
    /// <param name="onContractInitialized"> Action to call when the contract has been intialized. </param>
    public DubiExContract(string contractAddress, string abi, Action<ContractBase, string> onContractInitialized = null) : base(contractAddress, abi, onContractInitialized) { }

    /// <summary>
    /// Initializes all DubiEx functions.
    /// </summary>
    /// <param name="onContractInitialized"> Action to call when the contract has been initialized. </param>
    protected override void InitializeContract(Action<ContractBase, string> onContractInitialized)
    {
        takeOrder = contract.GetFunction("takeOrder");
        makeOrder = contract.GetFunction("makeOrder");
        cancelOrder = contract.GetFunction("cancelOrder");
        takeOrders = contract.GetFunction("takeOrders");
        makeOrders = contract.GetFunction("makeOrders");
        cancelOrders = contract.GetFunction("cancelOrders");
    }
}