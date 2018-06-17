using Nethereum.HdWallet;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.Web3.Accounts;
using System;

/// <summary>
/// Class which holds the data of the user's ethereum wallet and signs transactions.
/// </summary>
public class UserWallet
{

    public static event Action OnWalletLoadSuccessful;
    public static event Action OnWalletLoadUnsuccessful;

    private readonly PopupManager popupManager;
    private readonly EthereumNetwork ethereumNetwork;
    private readonly SafePasswordStorage safePassword;

    private Account account;

    private string password;

    /// <summary>
    /// The class used for signing transactions.
    /// </summary>
    public WalletTransactionSigner TransactionSigner { get; private set; }

    /// <summary>
    /// The user's public address.
    /// </summary>
    public string Address => account.Address;

    /// <summary>
    /// The password of the wallet.
    /// </summary>
    public string Password { set { password = value; } }

    /// <summary>
    /// Initializes the UserWallet with the SafePassword object.
    /// </summary>
    /// <param name="safePassword"> The SafePassword object used to encrypt the wallet. </param>
    /// <param name="popupManager"> The active PopupManager. </param>
    /// <param name="ethereumNetwork"> The active EthereumNetwork. </param>
    public UserWallet(SafePasswordStorage safePassword, PopupManager popupManager, EthereumNetwork ethereumNetwork)
    {
        this.safePassword = safePassword;
        this.popupManager = popupManager;
        this.ethereumNetwork = ethereumNetwork;

        OnWalletLoadSuccessful += safePassword.SetupPlayerPrefs;
    }

    /// <summary>
    /// Unlocks a wallet if the password is correct.
    /// </summary>
    /// <param name="userPassword"> The user's password for accessing the wallet, for extra layer of security. </param>
    public void UnlockWallet(string userPassword = null)
    {
        StartLoadingPopup("Unlocking ");
        safePassword.PopulatePrefDictionary();
        AsyncWalletEncryption.GetEncryptionPasswordAsync(safePassword, userPassword ?? password, (pass) => TryCreateAccount(pass));
    }

    /// <summary>
    /// Creates a wallet given a mnemonic phrase if it is a valid phrase.
    /// </summary>
    /// <param name="mnemonic"> The mnemonic phrase to use to derive the wallet data. </param>
    /// <param name="userPassword"> The user's password for accessing the wallet, for extra layer of security. </param>
    public void CreateWallet(string mnemonic, string userPassword = null)
    {
        StartLoadingPopup("Creating ");
        TryCreateWallet(mnemonic, wallet => AsyncWalletEncryption.GetEncryptionPasswordAsync(safePassword, userPassword ?? password, (pass) =>
        {
            AsyncWalletEncryption.EncryptWalletAsync(account.PrivateKey, wallet.Phrase, pass, walletData =>
            {
                UserWalletJsonHandler.CreateWallet(walletData);
                OnWalletLoadSuccessful?.Invoke();
            });
        }, true));
    }

    /// <summary>
    /// Checks whether a wallet currently exists or not.
    /// </summary>
    /// <param name="safePassword"> SafePassword object to use to check if the PlayerPrefs are active. </param>
    /// <returns> True if there is a readable wallet. </returns>
    public bool CanReadWallet => safePassword.HasPlayerPrefs() && UserWalletJsonHandler.JsonWalletExists;

    /// <summary>
    /// Starts the loading popup for the wallet.
    /// </summary>
    /// <param name="startingText"> The starting text to display on the loading popup. </param>
    private void StartLoadingPopup(string startingText) => popupManager.GetPopup<LoadingPopup>().SetLoadingText("wallet", startingText: startingText);

    #region Try Open/Create Methods

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
                    ExceptionManager.DisplayException(new Exception("Unable to unlock wallet, incorrect password. "), popupManager);
                    return;
                }

                account = new Account(pkey);
                OnWalletLoadSuccessful?.Invoke();
            });
        }
        catch
        {
            ExceptionManager.DisplayException(new Exception("Unable to unlock wallet, incorrect password. "), popupManager);
            OnWalletLoadUnsuccessful?.Invoke();
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
            ExceptionManager.DisplayException(new Exception("Unable to create wallet with that seed. Please try again."), popupManager);
            OnWalletLoadUnsuccessful?.Invoke();
        }
    }

    #endregion

    #region Transaction Signature Methods

    /// <summary>
    /// Signs an asset transfer request from this wallet to another wallet.
    /// </summary>
    /// <param name="gasLimit"> The gas limit to use for the transaction. </param>
    /// <param name="gasPrice"> The gas price to use for the transaction. </param>
    /// <param name="receivingAddress"> The receiving address of the transfer request. </param>
    /// <param name="assetAddress"> The address of the asset which will be transfer. </param>
    /// <param name="amount"> The amount that will be transfered. </param>
    /// <param name="onTransactionSigned"> Action to call once the transaction has been signed. </param>
    public void SignTransferRequest(HexBigInteger gasLimit, HexBigInteger gasPrice,
        string receivingAddress, string assetAddress, decimal amount, Action<TransactionSignedUnityRequest> onTransactionSigned)
    {
        popupManager.GetPopup<ConfirmSendAssetPopup>()
                    .SetSendAssetValues(receivingAddress,
                                        assetAddress,
                                        amount,
                                        gasPrice,
                                        gasLimit,
                                        () => onTransactionSigned(new TransactionSignedUnityRequest(ethereumNetwork.NetworkUrl, account.PrivateKey, account.Address)));
    }

    #endregion

}