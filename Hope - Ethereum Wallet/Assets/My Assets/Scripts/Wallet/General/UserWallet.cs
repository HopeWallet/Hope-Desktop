using Hope.Security.ProtectedTypes.Types;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.UnityClient;
using System;

/// <summary>
/// Class which holds the data of the user's ethereum wallet and signs transactions.
/// </summary>
public sealed class UserWallet : SecureObject
{
    public static event Action OnWalletLoadSuccessful;

    private readonly EphemeralEncryption passwordEncryptor;

    private readonly PopupManager popupManager;
    private readonly DynamicDataCache dynamicDataCache;

    private readonly WalletCreator walletCreator;
    private readonly WalletUnlocker walletUnlocker;
    private readonly WalletTransactionSigner walletTransactionSigner;

    private string[] addresses;

    /// <summary>
    /// Initializes the UserWallet with the PlayerPrefPassword object.
    /// </summary>
    /// <param name="prefPassword"> The PlayerPrefPassword object used for managing the wallet's encryption password. </param>
    /// <param name="popupManager"> The active PopupManager. </param>
    /// <param name="ethereumNetwork"> The active EthereumNetwork. </param>
    /// <param name="dynamicDataCache"> The active ProtectedStringDataCache. </param>
    /// <param name="walletSettings"> The settings for the UserWallet. </param>
    /// <param name="userWalletInfoManager"> The active UserWalletInfoManager. </param>
    public UserWallet(PlayerPrefPassword prefPassword,
        PopupManager popupManager,
        EthereumNetwork ethereumNetwork,
        DynamicDataCache dynamicDataCache,
        UserWalletInfoManager userWalletInfoManager,
        UserWalletInfoManager.Settings walletSettings)
    {
        this.popupManager = popupManager;
        this.dynamicDataCache = dynamicDataCache;

        passwordEncryptor = new EphemeralEncryption(this);
        walletCreator = new WalletCreator(popupManager, prefPassword, dynamicDataCache, walletSettings, userWalletInfoManager);
        walletUnlocker = new WalletUnlocker(popupManager, prefPassword, dynamicDataCache, walletSettings, userWalletInfoManager);
        walletTransactionSigner = new WalletTransactionSigner(prefPassword, dynamicDataCache, ethereumNetwork, passwordEncryptor, walletSettings);
    }

    /// <summary>
    /// Gets the address of the wallet given the index of the address.
    /// </summary>
    /// <param name="addressIndex"> The index to use to retrieve the address. Valid indices range from 0-49. </param>
    /// <returns> Returns the address found at the index. </returns>
    [SecureCallEnd]
    public string GetAddress(int addressIndex)
    {
        return addresses[addressIndex];
    }

    /// <summary>
    /// Unlocks a wallet if the password is correct.
    /// </summary>
    [SecureCaller]
    [ReflectionProtect]
    public void Unlock()
    {
        Load(walletUnlocker);
    }

    /// <summary>
    /// Creates a new wallet.
    /// </summary>
    [SecureCaller]
    [ReflectionProtect]
    public void Create()
    {
        Load(walletCreator);
    }

    /// <summary>
    /// Loads a wallet given the type of wallet loader to use.
    /// </summary>
    /// <param name="walletLoader"> The wallet loader to use, whether it is the <see cref="WalletCreator"/> or <see cref="WalletUnlocker"/>. </param>
    [SecureCaller]
    [ReflectionProtect]
    private void Load(WalletLoaderBase walletLoader)
    {
        walletLoader.Load(out addresses, OnWalletLoadSuccessful);
    }

    /// <summary>
    /// Signs a transaction using this UserWallet.
    /// </summary>
    /// <typeparam name="T"> The type of the popup to display the transaction confirmation for. </typeparam>
    /// <param name="onTransactionSigned"> The action to call if the transaction is confirmed and signed. </param>
    /// <param name="gasLimit"> The gas limit to use with the transaction. </param>
    /// <param name="gasPrice"> The gas price to use with the transaction. </param>
    /// <param name="signerAddress"> The address of the wallet signing the transaction. </param>
    /// <param name="transactionInput"> The input that goes along with the transaction request. </param>
    [SecureCaller]
    [ReflectionProtect]
    public void SignTransaction<T>(
        Action<TransactionSignedUnityRequest> onTransactionSigned,
        HexBigInteger gasLimit,
        HexBigInteger gasPrice,
        string signerAddress,
        params object[] transactionInput) where T : ConfirmTransactionPopupBase<T>
    {
        using (var pass = (dynamicDataCache.GetData("pass") as ProtectedString)?.CreateDisposableData())
        {
            string encryptedPassword = passwordEncryptor.Encrypt(pass.Value);
            popupManager.GetPopup<T>(true)
                        .SetConfirmationValues(() => walletTransactionSigner.SignTransaction(signerAddress, encryptedPassword, onTransactionSigned),
                                               gasLimit,
                                               gasPrice,
                                               transactionInput);
        }
    }
}