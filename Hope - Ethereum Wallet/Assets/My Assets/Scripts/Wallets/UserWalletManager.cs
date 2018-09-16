using Nethereum.HdWallet;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.UnityClient;
using System;
using System.Numerics;

/// <summary>
/// Class for managing the ethereum wallet of the user.
/// </summary>
public sealed class UserWalletManager
{
    public static event Action OnWalletLoadSuccessful;
    public static event Action OnWalletLoadUnsuccessful;

    private readonly LedgerWallet ledgerWallet;
    private readonly TrezorWallet trezorWallet;
    private readonly HopeWallet hopeWallet;

    private IWallet activeWallet;

    private int accountNumber;

    /// <summary>
    /// The address of the main UserWallet.
    /// </summary>
    public string WalletAddress => activeWallet.GetAddress(accountNumber);

    /// <summary>
    /// The wallet derivation path.
    /// </summary>
    public string WalletPath { get; private set; }

    /// <summary>
    /// The currently opened, currently active wallet type.
    /// </summary>
    public WalletType ActiveWalletType => activeWallet is LedgerWallet ? WalletType.Ledger : activeWallet is TrezorWallet ? WalletType.Trezor : WalletType.Hope;

    /// <summary>
    /// Initializes the UserWallet given the settings to apply.
    /// </summary>
    /// <param name="settings"> The settings to initialize the wallet with. </param>
    /// <param name="popupManager"> The PopupManager to assign to the wallet. </param>
    /// <param name="ethereumNetworkManager"> The active EthereumNetworkManager to assign to the wallet. </param>
    /// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
    /// <param name="ledgerWallet"> The active LedgerWallet. </param>
    /// <param name="trezorWallet"> The active TrezorWallet. </param>
    /// <param name="userWalletInfoManager"> The active UserWalletInfoManager. </param>
    /// <param name="walletSettings"> The player pref settings for the UserWallet. </param>
    public UserWalletManager(
        Settings settings,
        PopupManager popupManager,
        EthereumNetworkManager ethereumNetworkManager,
        DynamicDataCache dynamicDataCache,
        LedgerWallet ledgerWallet,
        TrezorWallet trezorWallet,
        HopeWalletInfoManager userWalletInfoManager,
        HopeWalletInfoManager.Settings walletSettings)
    {
        this.ledgerWallet = ledgerWallet;
        this.trezorWallet = trezorWallet;

        hopeWallet = new HopeWallet(settings.safePassword, popupManager, ethereumNetworkManager.CurrentNetwork, dynamicDataCache, userWalletInfoManager, walletSettings);
        activeWallet = hopeWallet;

        ledgerWallet.OnWalletLoadSuccessful += () => OnWalletLoadSuccessful?.Invoke();
        ledgerWallet.OnWalletLoadUnsuccessful += () => OnWalletLoadUnsuccessful?.Invoke();
        trezorWallet.OnWalletLoadSuccessful += () => OnWalletLoadSuccessful?.Invoke();
        trezorWallet.OnWalletLoadUnsuccessful += () => OnWalletLoadUnsuccessful?.Invoke();
        hopeWallet.OnWalletLoadSuccessful += () => OnWalletLoadSuccessful?.Invoke();
        hopeWallet.OnWalletLoadUnsuccessful += () => OnWalletLoadUnsuccessful?.Invoke();
    }

    /// <summary>
    /// Wrapper method for transferring a specified asset from the user's wallet to another ethereum address.
    /// </summary>
    /// <param name="tradableAsset"> The asset to transfer from the user's wallet. </param>
    /// <param name="gasLimit"> The gas limit to use for this asset transfer transaction. </param>
    /// <param name="gasPrice"> The gas price to use for this asset transfer transaction. </param>
    /// <param name="address"> The address to transfer the asset to. </param>
    /// <param name="amount"> The amount of the specified asset to send. </param>
    public void TransferAsset(TradableAsset tradableAsset, HexBigInteger gasLimit, HexBigInteger gasPrice, string address, dynamic amount)
        => tradableAsset.Transfer(this, gasLimit, gasPrice, address, amount);

    /// <summary>
    /// Signs a transaction using the main UserWallet.
    /// </summary>
    /// <typeparam name="T"> The type of the popup to display the transaction confirmation for. </typeparam>
    /// <param name="onTransactionSigned"> The action to call if the transaction is confirmed and signed. </param>
    /// <param name="gasLimit"> The gas limit to use with the transaction. </param>
    /// <param name="gasPrice"> The gas price to use with the transaction. </param>
    /// <param name="value"> The amount of Ether in wei to send to the address. </param>
    /// <param name="addressTo"> The address the transaction is being sent to. </param>
    /// <param name="data"> The data to pass along with the transaction. </param>
    /// <param name="displayInput"> The display input that goes along with the transaction request. </param>
    [SecureCallEnd]
    public void SignTransaction<T>(
        Action<TransactionSignedUnityRequest> onTransactionSigned,
        BigInteger gasLimit,
        BigInteger gasPrice,
        BigInteger value,
        string addressTo,
        string data,
        params object[] displayInput) where T : ConfirmTransactionPopupBase<T>
    {
        activeWallet.SignTransaction<T>(onTransactionSigned, gasLimit, gasPrice, value, WalletAddress, addressTo, data, WalletPath.Replace("x", accountNumber.ToString()), displayInput);
    }

    /// <summary>
    /// Switches the active WalletType.
    /// </summary>
    /// <param name="newWalletType"> The new WalletType to use to get addresses/sign transactions. </param>
    public void SwitchWalletType(WalletType newWalletType)
    {
        switch (newWalletType)
        {
            case WalletType.Ledger:
                activeWallet = ledgerWallet;
                WalletPath = Wallet.ELECTRUM_LEDGER_PATH;
                break;
            case WalletType.Trezor:
                activeWallet = trezorWallet;
                WalletPath = Wallet.DEFAULT_PATH;
                break;
            case WalletType.Hope:
                activeWallet = hopeWallet;
                WalletPath = Wallet.DEFAULT_PATH;
                break;
        }
    }

    /// <summary>
    /// Switches the active derivation path of the wallet.
    /// </summary>
    /// <param name="newPath"> The new derivation path to use. </param>
    public void SwitchWalletPath(string newPath)
    {
        if (!newPath.EqualsIgnoreCase(Wallet.DEFAULT_PATH) || !newPath.EqualsIgnoreCase(Wallet.ELECTRUM_LEDGER_PATH))
            return;

        WalletPath = newPath;
    }

    /// <summary>
    /// Switches the current account number of the wallet.
    /// </summary>
    /// <param name="newAccountNumber"> The new account number of the wallet to use. </param>
    public void SwitchWalletAccount(int newAccountNumber)
    {
        accountNumber = Math.Abs(newAccountNumber) > 49 ? 49 : Math.Abs(newAccountNumber);
    }

    /// <summary>
    /// Attempts to load a wallet given a password.
    /// Calls the action if the wallet loaded successfully.
    /// </summary>
    [SecureCallEnd]
    public void UnlockWallet() => hopeWallet.Unlock();

    /// <summary>
    /// Attempts to create a wallet given a mnemonic phrase.
    /// Calls the action with the state of successful or unsuccessful wallet creation.
    /// </summary>
    [SecureCallEnd]
    public void CreateWallet() => hopeWallet.Create();

    /// <summary>
    /// Enum representing the type of the wallet.
    /// </summary>
    public enum WalletType
    {
        Ledger,
        Trezor,
        Hope
    }

    /// <summary>
    /// Class which contains all settings related to the wallet storage/loading/unlocking.
    /// </summary>
    [Serializable]
    public class Settings
    {
        public PlayerPrefPasswordDerivation safePassword;
    }
}