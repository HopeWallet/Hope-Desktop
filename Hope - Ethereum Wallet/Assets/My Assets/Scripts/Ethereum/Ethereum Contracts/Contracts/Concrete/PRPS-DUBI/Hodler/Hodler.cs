using Hope.Utils.Ethereum;
using Nethereum.Hex.HexTypes;
using System;
using System.Numerics;

/// <summary>
/// Class which represents the Hodler smart contract used for locking purpose and receiving dubi.
/// </summary>
public sealed partial class Hodler : StaticSmartContract
{
    /// <summary>
    /// Initializes the <see cref="Hodler"/> contract given the ethereum network settings and settings of this contract.
    /// </summary>
    /// <param name="ethereumNetworkSettings"> The active <see cref="EthereumNetworkManager.Settings"/>. </param>
    /// <param name="settings"> The active <see cref="Hodler.Settings"/>. </param>
    public Hodler(EthereumNetworkManager.Settings ethereumNetworkSettings, Settings settings) : base(ethereumNetworkSettings, settings)
    {
    }

    /// <summary>
    /// Gets an item that is locked given the id.
    /// </summary>
    /// <param name="address"> The address to check for the item. </param>
    /// <param name="id"> The id which holds an item in the mapping. </param>
    /// <param name="onItemReceived"> Action to call once the item has been received. </param>
    public void GetItem(string address, BigInteger id, Action<Output.Item> onItemReceived)
    {
        ContractUtils.QueryContract<Queries.GetItem, Output.Item>(ContractAddress, address, onItemReceived, address, id);
    }

    /// <summary>
    /// Locks a certain amount of purpose into the Hodler smart contract.
    /// </summary>
    /// <param name="userWalletManager"> The class managing the wallet. </param>
    /// <param name="gasLimit"> The gas limit to send with the transaction. </param>
    /// <param name="gasPrice"> The gas price to send with the transaction. </param>
    /// <param name="id"> The id of the lock function call. </param>
    /// <param name="value"> The amount of purpose to lock. </param>
    /// <param name="monthsToLock"> How many months the purpose should be locked for. </param>
    public void Hodl(UserWalletManager userWalletManager, HexBigInteger gasLimit, HexBigInteger gasPrice, BigInteger id, decimal value, int monthsToLock)
    {
        userWalletManager.SignTransaction<ConfirmLockPopup>(request =>
        {
            ContractUtils.SendContractMessage<Messages.Hodl>(ContractAddress,
                                                             request,
                                                             gasPrice,
                                                             gasLimit,
                                                             () => UnityEngine.Debug.Log("Successfully locked " + value + " PRPS"),
                                                             id,
                                                             SolidityUtils.ConvertToUInt(value, 18),
                                                             new BigInteger(monthsToLock));
        }, gasLimit, gasPrice, monthsToLock, value);
    }

    /// <summary>
    /// Releases some purpose from the Hodler smart contract.
    /// </summary>
    /// <param name="userWalletManager"> The class managing the wallet. </param>
    /// <param name="gasLimit"> The gas limit to send with the transaction. </param>
    /// <param name="gasPrice"> The gas price to send with the transaction. </param>
    /// <param name="id"> The id of the locked purpose item. </param>
    /// <param name="amountToRelease"> The amount of purpose that will be released. </param>
    public void Release(UserWalletManager userWalletManager, HexBigInteger gasLimit, HexBigInteger gasPrice, BigInteger id, decimal amountToRelease)
    {
        userWalletManager.SignTransaction<GeneralTransactionConfirmationPopup>(request =>
        {
            ContractUtils.SendContractMessage<Messages.Release>(ContractAddress,
                                                                request,
                                                                gasPrice,
                                                                gasLimit,
                                                                () => UnityEngine.Debug.Log("Successfully released " + amountToRelease + " Purpose"),
                                                                id);
        }, gasLimit, gasPrice, "Release Purpose Confirmation");
    }

    /// <summary>
    /// Class which holds the contract addresses for this smart contract.
    /// </summary>
    [Serializable]
    public sealed class Settings : SettingsBase
    {
    }
}