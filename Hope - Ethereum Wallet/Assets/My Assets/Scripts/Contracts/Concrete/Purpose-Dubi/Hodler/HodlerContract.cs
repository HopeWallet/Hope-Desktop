using System;
using System.Numerics;

/// <summary>
/// Contract which is used for locking purpose and receiving dubi.
/// </summary>
public class HodlerContract : ContractBase
{

    private const string FUNC_HODL = "hodl";
    private const string FUNC_RELEASE = "release";
    private const string FUNC_GETITEM = "getItem";

    /// <summary>
    /// The function names associated with the Hodler contract.
    /// </summary>
    protected override string[] FunctionNames => new string[] { FUNC_HODL, FUNC_RELEASE, FUNC_GETITEM };

    /// <summary>
    /// Initializes the contract with the address and abi.
    /// </summary>
    /// <param name="contractAddress"> The contract address. </param>
    /// <param name="abi"> The contract abi. </param>
    public HodlerContract(string contractAddress, string abi) : base(contractAddress, abi)
    {
    }

    /// <summary>
    /// Gets an item that is locked given the id.
    /// </summary>
    /// <param name="address"> The address to check for the item. </param>
    /// <param name="id"> The id which holds an item in the mapping. </param>
    /// <param name="onItemReceived"> Action to call once the item has been received. </param>
    public void GetItem(string address, BigInteger id, Action<HodlerItem> onItemReceived) => this.ComplexContractViewCall(this[FUNC_GETITEM], onItemReceived, address, id);

}
