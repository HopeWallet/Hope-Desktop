using Hope.Security.Encryption;
using Hope.Utils.EthereumUtils;
using Nethereum.HdWallet;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.Web3.Accounts;
using System;

/// <summary>
/// Class which holds the data of the user's ethereum wallet and signs transactions.
/// </summary>
public sealed class UserWallet
{

    // TODO:
    // Only open wallet once the password has been verified
    // Only open the wallet once it has been created and fully encrypted
    // Only locate the wallet seed and the private key when signing a transaction

    public static event Action OnWalletLoadSuccessful;

    private readonly PopupManager popupManager;
    private readonly EthereumNetwork ethereumNetwork;
    private readonly PlayerPrefPassword prefPassword;
    private readonly ProtectedStringDataCache protectedStringDataCache;

    private Account account;

    /// <summary>
    /// The user's public address.
    /// </summary>
    public string Address => account.Address;

    /// <summary>
    /// Checks whether a wallet currently exists or not.
    /// </summary>
    public bool CanReadWallet => prefPassword.HasPlayerPrefs() && UserWalletJsonHandler.JsonWalletExists;

    /// <summary>
    /// Initializes the UserWallet with the PlayerPrefPassword object.
    /// </summary>
    /// <param name="prefPassword"> The PlayerPrefPassword object used for managing the wallet's encryption password. </param>
    /// <param name="popupManager"> The active PopupManager. </param>
    /// <param name="ethereumNetwork"> The active EthereumNetwork. </param>
    /// <param name="protectedStringDataCache"> The active ProtectedStringDataCache. </param>
    public UserWallet(PlayerPrefPassword prefPassword,
        PopupManager popupManager,
        EthereumNetwork ethereumNetwork,
        ProtectedStringDataCache protectedStringDataCache)
    {
        this.prefPassword = prefPassword;
        this.popupManager = popupManager;
        this.ethereumNetwork = ethereumNetwork;
        this.protectedStringDataCache = protectedStringDataCache;
    }

    /// <summary>
    /// Unlocks a wallet if the password is correct.
    /// </summary>
    public void UnlockWallet()
    {
        StartLoadingPopup("Unlocking ");
        prefPassword.PopulatePrefDictionary();

        using (var str = protectedStringDataCache.GetData(0).CreateDisposableData())
            AsyncWalletEncryption.GetEncryptionPasswordAsync(prefPassword, str.Value, (pass) => TryCreateAccount(pass));
    }

    /// <summary>
    /// Creates a wallet given a mnemonic phrase if it is a valid phrase.
    /// </summary>
    /// <param name="mnemonic"> The mnemonic phrase to use to derive the wallet data. </param>
    public void CreateWallet(string mnemonic)
    {
        StartLoadingPopup("Creating ");
        using (var str = protectedStringDataCache.GetData(0).CreateDisposableData())
        {
            TryCreateWallet(mnemonic, wallet => AsyncWalletEncryption.GetEncryptionPasswordAsync(prefPassword, str.Value, (pass) =>
            {
                AsyncWalletEncryption.EncryptWalletAsync(account.PrivateKey, wallet.Phrase, pass, walletData =>
                {
                    UserWalletJsonHandler.CreateWallet(walletData);
                    prefPassword.SetupPlayerPrefs(0);
                    OnWalletLoadSuccessful?.Invoke();
                });
            }, true));
        }
    }

    /// <summary>
    /// Signs a transaction using this UserWallet.
    /// </summary>
    /// <typeparam name="T"> The type of the popup to display the transaction confirmation for. </typeparam>
    /// <param name="onTransactionSigned"> The action to call if the transaction is confirmed and signed. </param>
    /// <param name="gasLimit"> The gas limit to use with the transaction. </param>
    /// <param name="gasPrice"> The gas price to use with the transaction. </param>
    /// <param name="transactionInput"> The input that goes along with the transaction request. </param>
    public void SignTransaction<T>(Action<TransactionSignedUnityRequest> onTransactionSigned,
        HexBigInteger gasLimit, HexBigInteger gasPrice, params object[] transactionInput) where T : ConfirmTransactionRequestPopup<T>
    {
        popupManager.GetPopup<T>(true)
                    .SetConfirmationValues(() => onTransactionSigned(new TransactionSignedUnityRequest(protectedStringDataCache, ethereumNetwork.NetworkUrl, account.PrivateKey, account.Address)),
                                           gasLimit,
                                           gasPrice,
                                           transactionInput);
    }

    /// <summary>
    /// Starts the loading popup for the wallet.
    /// </summary>
    /// <param name="startingText"> The starting text to display on the loading popup. </param>
    private void StartLoadingPopup(string startingText) => popupManager.GetPopup<LoadingPopup>().SetLoadingText("wallet", startingText: startingText);

    /// <summary>
    /// Attempts to unlock the wallet with the given password.
    /// </summary>
    /// <param name="password"> The password to attempt to unlock the wallet with. </param>
    private void TryCreateAccount(string password)
    {
        try
        {
            AsyncWalletEncryption.DecryptWalletAsync(UserWalletJsonHandler.GetWallet(), password, (pkey, seed) =>
            {
                if (string.IsNullOrEmpty(pkey))
                {
                    ExceptionManager.DisplayException(new Exception("Unable to unlock wallet, incorrect password. "));
                    return;
                }

                account = new Account(pkey);
                OnWalletLoadSuccessful?.Invoke();
            });
        }
        catch
        {
            ExceptionManager.DisplayException(new Exception("Unable to unlock wallet, incorrect password. "));
        }
    }

    /// <summary>
    /// Attempts to create a wallet given a mnemonic phrase.
    /// </summary>
    /// <param name="mnemonic"> The phrase to attempt to create a wallet with. </param>
    /// <param name="onWalletCreatedSuccessfully"> Action to call if the wallet was created successfully. </param>
    private void TryCreateWallet(string mnemonic, Action<Wallet> onWalletCreatedSuccessfully)
    {
        try
        {
            var wallet = new Wallet(mnemonic, null, WalletUtils.DetermineCorrectPath(mnemonic));
            account = wallet.GetAccount(0);
            onWalletCreatedSuccessfully?.Invoke(wallet);
        }
        catch
        {
            ExceptionManager.DisplayException(new Exception("Unable to create wallet with that seed. Please try again."));
        }
    }

}