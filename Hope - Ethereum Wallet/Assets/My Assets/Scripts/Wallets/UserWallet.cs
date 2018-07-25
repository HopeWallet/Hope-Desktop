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
    private readonly EthereumNetwork ethereumNetwork;
    private readonly PlayerPrefPassword prefPassword;
    private readonly DynamicDataCache dynamicDataCache;

    private readonly WalletCreator walletCreator;
    private readonly WalletUnlocker walletUnlocker;
    private readonly WalletTransactionSigner walletTransactionSigner;

    private ProtectedString[] addresses;

    public string Address { get; private set; }

    /// <summary>
    /// Initializes the UserWallet with the PlayerPrefPassword object.
    /// </summary>
    /// <param name="prefPassword"> The PlayerPrefPassword object used for managing the wallet's encryption password. </param>
    /// <param name="popupManager"> The active PopupManager. </param>
    /// <param name="ethereumNetwork"> The active EthereumNetwork. </param>
    /// <param name="dynamicDataCache"> The active ProtectedStringDataCache. </param>
    public UserWallet(PlayerPrefPassword prefPassword,
        PopupManager popupManager,
        EthereumNetwork ethereumNetwork,
        DynamicDataCache dynamicDataCache)
    {
        this.prefPassword = prefPassword;
        this.popupManager = popupManager;
        this.ethereumNetwork = ethereumNetwork;
        this.dynamicDataCache = dynamicDataCache;

        passwordEncryptor = new EphemeralEncryption(this);
        walletCreator = new WalletCreator(popupManager, prefPassword, dynamicDataCache);
        walletUnlocker = new WalletUnlocker(popupManager, prefPassword, dynamicDataCache);
        walletTransactionSigner = new WalletTransactionSigner(prefPassword, dynamicDataCache, ethereumNetwork, passwordEncryptor);
    }

    /// <summary>
    /// Unlocks a wallet if the password is correct.
    /// </summary>
    /// <param name="walletNum"> The number of the wallet to unlock. </param>
    [SecureCaller]
    [ReflectionProtect]
    public void Unlock()
    {
        Load(walletUnlocker);
    }

    [SecureCaller]
    [ReflectionProtect]
    public void Create()
    {
        Load(walletCreator);
    }

    [SecureCaller]
    [ReflectionProtect]
    private void Load(WalletLoaderBase walletLoader)
    {
        walletLoader.Load(out addresses, OnWalletLoadSuccessful, () => Address = GetAddress(0));
    }

    [SecureCallEnd]
    [ReflectionProtect(typeof(string))]
    public string GetAddress(int addressIndex)
    {
        if (addresses?.Length == 0)
        {
            ExceptionManager.DisplayException(new Exception("Wallet not created/unlocked yet!"));
            return null;
        }

        using (var address = addresses[addressIndex].CreateDisposableData())
            return address.Value;
    }

    /// <summary>
    /// Signs a transaction using this UserWallet.
    /// </summary>
    /// <typeparam name="T"> The type of the popup to display the transaction confirmation for. </typeparam>
    /// <param name="onTransactionSigned"> The action to call if the transaction is confirmed and signed. </param>
    /// <param name="gasLimit"> The gas limit to use with the transaction. </param>
    /// <param name="gasPrice"> The gas price to use with the transaction. </param>
    /// <param name="transactionInput"> The input that goes along with the transaction request. </param>
    [SecureCallEnd]
    [ReflectionProtect]
    public void SignTransaction<T>(
        Action<TransactionSignedUnityRequest> onTransactionSigned,
        HexBigInteger gasLimit,
        HexBigInteger gasPrice,
        params object[] transactionInput) where T : ConfirmTransactionRequestPopup<T>
    {
        using (var pass = (dynamicDataCache.GetData("pass") as ProtectedString)?.CreateDisposableData())
        {
            string encryptedPassword = passwordEncryptor.Encrypt(pass.Value);
            popupManager.GetPopup<T>(true)
                    .SetConfirmationValues(() => walletTransactionSigner.SignTransaction(Address, encryptedPassword, onTransactionSigned),
                                           gasLimit,
                                           gasPrice,
                                           transactionInput);

            GC.Collect();
        }
    }
}