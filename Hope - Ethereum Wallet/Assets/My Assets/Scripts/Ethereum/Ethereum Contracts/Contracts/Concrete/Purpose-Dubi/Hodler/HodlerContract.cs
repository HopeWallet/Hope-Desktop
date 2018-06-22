using Hope.Utils.EthereumUtils;
using Nethereum.Hex.HexTypes;
using System;
using System.Numerics;

/// <summary>
/// Contract which is used for locking purpose and receiving dubi.
/// </summary>
public class HodlerContract : ContractBase
{

    public const string FUNC_HODL = "hodl";
    public const string FUNC_RELEASE = "release";
    public const string FUNC_GETITEM = "getItem";

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

    public void Hodl(UserWalletManager userWalletManager, HexBigInteger gasLimit, HexBigInteger gasPrice, BigInteger id, BigInteger value, int monthsToLock)
    {
        userWalletManager.SignTransaction<ConfirmPRPSLockPopup>(request =>
        {
            this.ExecuteContractFunction(this[FUNC_HODL],
                                         request,
                                         userWalletManager.WalletAddress,
                                         gasLimit,
                                         gasPrice,
                                         () => UnityEngine.Debug.Log("Successfully locked " + value + " PRPS"),
                                         id,
                                         value,
                                         monthsToLock);
        }, gasLimit, gasPrice, monthsToLock, value);
    }

    public void Release(UserWalletManager userWalletManager, HexBigInteger gasLimit, HexBigInteger gasPrice, BigInteger id, decimal amountToRelease)
    {
        userWalletManager.SignTransaction<GeneralTransactionConfirmationPopup>(request =>
        {
            this.ExecuteContractFunction(this[FUNC_RELEASE],
                                         request,
                                         userWalletManager.WalletAddress,
                                         gasLimit,
                                         gasPrice,
                                         () => UnityEngine.Debug.Log("Successfully released " + amountToRelease + " Purpose."),
                                         id);
        }, gasLimit, gasPrice, "Release Purpose Confirmation");
    }

}
