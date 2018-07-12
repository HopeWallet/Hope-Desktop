using Hope.Security.ProtectedTypes.Types;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.UnityClient;
using System;

/// <summary>
/// Class for managing the ethereum wallet of the user.
/// </summary>
public class UserWalletManager
{
    private readonly UserWallet userWallet;

    private readonly ProtectedInt walletNumber = new ProtectedInt(0);
    private readonly ProtectedInt accountNumber = new ProtectedInt(0);

    /// <summary>
    /// The address of the main UserWallet.
    /// </summary>
    public string WalletAddress => userWallet.Address;

    /// <summary>
    /// Initializes the UserWallet given the settings to apply.
    /// </summary>
    /// <param name="settings"> The settings to initialize the wallet with. </param>
    /// <param name="popupManager"> The PopupManager to assign to the wallet. </param>
    /// <param name="ethereumNetworkManager"> The active EthereumNetworkManager to assign to the wallet. </param>
    /// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
    public UserWalletManager(
        Settings settings,
        PopupManager popupManager,
        EthereumNetworkManager ethereumNetworkManager,
        DynamicDataCache dynamicDataCache)
    {
        settings.safePassword.AddCharLookups(settings.safePasswordCharLookups);
        userWallet = new UserWallet(settings.safePassword, popupManager, ethereumNetworkManager.CurrentNetwork, dynamicDataCache);
    }

    /// <summary>
    /// Wrapper method for getting a specific asset balance of the user's ethereum wallet.
    /// </summary>
    /// <param name="tradableAsset"> The asset to check the balance of. </param>
    /// <param name="onBalanceReceived"> Called when the balnace has been received. </param>
    public void GetAssetBalance(TradableAsset tradableAsset, Action<dynamic> onBalanceReceived)
        => tradableAsset.GetBalance(userWallet, onBalanceReceived);

    /// <summary>
    /// Wrapper method for transferring a specified asset from the user's wallet to another ethereum address.
    /// </summary>
    /// <param name="tradableAsset"> The asset to transfer from the user's wallet. </param>
    /// <param name="gasLimit"> The gas limit to use for this asset transfer transaction. </param>
    /// <param name="gasPrice"> The gas price to use for this asset transfer transaction. </param>
    /// <param name="address"> The address to transfer the asset to. </param>
    /// <param name="amount"> The amount of the specified asset to send. </param>
    public void TransferAsset(TradableAsset tradableAsset, HexBigInteger gasLimit, HexBigInteger gasPrice, string address, dynamic amount)
        => tradableAsset.Transfer(userWallet, gasLimit, gasPrice, address, amount);

    /// <summary>
    /// Signs a transaction using the main UserWallet.
    /// </summary>
    /// <typeparam name="T"> The type of the popup to display the transaction confirmation for. </typeparam>
    /// <param name="onTransactionSigned"> The action to call if the transaction is confirmed and signed. </param>
    /// <param name="gasLimit"> The gas limit to use with the transaction. </param>
    /// <param name="gasPrice"> The gas price to use with the transaction. </param>
    /// <param name="transactionInput"> The input that goes along with the transaction request. </param>
    public void SignTransaction<T>(
        Action<TransactionSignedUnityRequest> onTransactionSigned,
        HexBigInteger gasLimit,
        HexBigInteger gasPrice,
        params object[] transactionInput) where T : ConfirmTransactionRequestPopup<T>
    {
        userWallet.SignTransaction<T>(onTransactionSigned, gasLimit, gasPrice, transactionInput);
    }

    public void SwitchWallet(int walletNum, int accountNum)
    {
        walletNumber.SetValue(walletNum);
        accountNumber.SetValue(accountNum);
    }

    /// <summary>
    /// Attempts to load a wallet given a password.
    /// Calls the action if the wallet loaded successfully.
    /// </summary>
    public void UnlockWallet() => userWallet.Unlock();

    /// <summary>
    /// Attempts to create a wallet given a mnemonic phrase.
    /// Calls the action with the state of successful or unsuccessful wallet creation.
    /// </summary>
    public void CreateWallet() => userWallet.Create();

    /// <summary>
    /// Class which contains the settings for safely storing the password to the wallet.
    /// </summary>
    [Serializable]
    public class Settings
    {
        public PlayerPrefPassword safePassword;
        public string[] safePasswordCharLookups;
    }

}
