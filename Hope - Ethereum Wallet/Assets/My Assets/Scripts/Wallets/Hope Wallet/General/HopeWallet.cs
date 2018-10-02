using Hope.Security.ProtectedTypes.Types;
using Nethereum.HdWallet;
using Nethereum.JsonRpc.UnityClient;
using System;
using System.Numerics;

/// <summary>
/// Class which holds the data of the user's ethereum wallet and signs transactions.
/// </summary>
public sealed class HopeWallet : SecureObject, IWallet
{
    public event Action OnWalletLoadSuccessful;
    public event Action OnWalletLoadUnsuccessful;

    private readonly MemoryEncryptor passwordEncryptor;

    private readonly PopupManager popupManager;
    private readonly DynamicDataCache dynamicDataCache;

    private readonly WalletCreator walletCreator;
    private readonly WalletUnlocker walletUnlocker;
    private readonly WalletTransactionSigner walletTransactionSigner;

    private readonly string[][] addresses = new string[2][];

    /// <summary>
    /// Initializes the UserWallet with the PlayerPrefPassword object.
    /// </summary>
    /// <param name="prefPassword"> The PlayerPrefPassword object used for managing the wallet's encryption password. </param>
    /// <param name="popupManager"> The active PopupManager. </param>
    /// <param name="ethereumNetwork"> The active EthereumNetwork. </param>
    /// <param name="dynamicDataCache"> The active ProtectedStringDataCache. </param>
    /// <param name="walletSettings"> The settings for the UserWallet. </param>
    /// <param name="userWalletInfoManager"> The active UserWalletInfoManager. </param>
    public HopeWallet(PlayerPrefPasswordDerivation prefPassword,
        PopupManager popupManager,
        EthereumNetwork ethereumNetwork,
        DynamicDataCache dynamicDataCache,
        HopeWalletInfoManager userWalletInfoManager,
        HopeWalletInfoManager.Settings walletSettings)
    {
        this.popupManager = popupManager;
        this.dynamicDataCache = dynamicDataCache;

        passwordEncryptor = new MemoryEncryptor(this);
        walletCreator = new WalletCreator(popupManager, prefPassword, dynamicDataCache, walletSettings, userWalletInfoManager);
        walletUnlocker = new WalletUnlocker(popupManager, prefPassword, dynamicDataCache, walletSettings, userWalletInfoManager);
        walletTransactionSigner = new WalletTransactionSigner(prefPassword, dynamicDataCache, ethereumNetwork, passwordEncryptor, walletSettings);
    }

    /// <summary>
    /// Gets the address of the wallet given the index of the address.
    /// </summary>
    /// <param name="addressIndex"> The index to use to retrieve the address. Valid indices range from 0-49. </param>
    /// <param name="path"> The path of the address. </param>
    /// <returns> Returns the address found at the index and path. </returns>
    [SecureCallEnd]
    public string GetAddress(int addressIndex, string path)
    {
        return path.EqualsIgnoreCase(Wallet.DEFAULT_PATH) ? addresses[0][addressIndex] : addresses[1][addressIndex];
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
        walletLoader.Load(addresses, OnWalletLoadSuccessful);
    }

    /// <summary>
    /// Gets the encrypted version of a plain text password.
    /// </summary>
    /// <param name="plainBytes"> The plain text password to encrypt. </param>
    /// <returns> The encrypted password as a byte[]. </returns>
    [SecureCallEnd]
    [ReflectionProtect(typeof(string))]
    private byte[] GetEncryptedPass(byte[] plainBytes)
    {
        return passwordEncryptor.Encrypt(plainBytes);
    }

    /// <summary>
    /// Signs a transaction using the HopeWallet.
    /// </summary>
    /// <typeparam name="T"> The type of the popup to display the transaction confirmation for. </typeparam>
    /// <param name="onTransactionSigned"> The action to call if the transaction is confirmed and signed. </param>
    /// <param name="gasLimit"> The gas limit to use with the transaction. </param>
    /// <param name="gasPrice"> The gas price to use with the transaction. </param>
    /// <param name="value"> The amount of ether in wei being sent along with the transaction. </param>
    /// <param name="addressFrom"> The address of the wallet signing the transaction. </param>
    /// <param name="addressTo"> The address the transaction is being sent to. </param>
    /// <param name="data"> The data sent along with the transaction. </param>
    /// <param name="path"> The path of the wallet to sign the transaction with. </param>
    /// <param name="displayInput"> The display input that goes along with the transaction request. </param>
    [SecureCallEnd]
    [ReflectionProtect]
    public void SignTransaction<T>(
        Action<TransactionSignedUnityRequest> onTransactionSigned,
        BigInteger gasLimit,
        BigInteger gasPrice,
        BigInteger value,
        string addressFrom,
        string addressTo,
        string data,
        string path,
        params object[] displayInput) where T : ConfirmTransactionPopupBase<T>
    {
        var promise = (dynamicDataCache.GetData("pass") as ProtectedString)?.CreateDisposableData();
        promise.OnSuccess(disposableData =>
        {
            byte[] encryptedPasswordBytes = GetEncryptedPass(disposableData.ByteValue);
            popupManager.GetPopup<T>(true)
                        .SetConfirmationValues(() => walletTransactionSigner.SignTransaction(addressFrom, path, encryptedPasswordBytes, onTransactionSigned),
                                               gasLimit,
                                               gasPrice,
                                               displayInput);
        });
    }
}