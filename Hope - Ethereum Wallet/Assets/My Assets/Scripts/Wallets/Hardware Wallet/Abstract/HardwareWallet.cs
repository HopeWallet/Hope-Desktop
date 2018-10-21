using System;
using System.Numerics;
using System.Threading.Tasks;
using Hope.Utils.Ethereum;
using NBitcoin;
using Nethereum.HdWallet;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.RLP;
using Nethereum.Signer;
using Nethereum.Util;
using Transaction = Nethereum.Signer.Transaction;

/// <summary>
/// Base class used for any concrete hardware wallet implementations.
/// </summary>
public abstract class HardwareWallet : IWallet
{
    public event Action OnWalletLoadSuccessful;
    public event Action OnWalletLoadUnsuccessful;

    protected readonly string[][] addresses = new string[2][];

    protected readonly EthereumNetworkManager ethereumNetworkManager;
    protected readonly EthereumNetworkManager.Settings ethereumNetworkSettings;
    protected readonly PopupManager popupManager;

    protected const string EXTENDED_PUBLIC_KEY_PATH = "m/44'/60'/0'";

    private bool signingTransaction;

    /// <summary>
    /// Initializes the HardwareWallet instance.
    /// </summary>
    /// <param name="ethereumNetworkManager"> The active EthereumNetworkManager. </param>
    /// <param name="ethereumNetworkSettings"> The settings for the EthereumNetworkManager. </param>
    /// <param name="popupManager"> The active PopupManager. </param>
    protected HardwareWallet(
        EthereumNetworkManager ethereumNetworkManager,
        EthereumNetworkManager.Settings ethereumNetworkSettings,
        PopupManager popupManager)
    {
        this.ethereumNetworkManager = ethereumNetworkManager;
        this.ethereumNetworkSettings = ethereumNetworkSettings;
        this.popupManager = popupManager;
    }

    /// <summary>
    /// Gets the address given the index of the address and the path.
    /// </summary>
    /// <param name="addressIndex"> The index of the address to retrieve. </param>
    /// <param name="path"> The path of the address to retrieve. </param>
    /// <returns> The address located at that index and derivation path. </returns>
    public string GetAddress(int addressIndex, string path)
    {
        return path.EqualsIgnoreCase(Wallet.DEFAULT_PATH) ? addresses[0][addressIndex] : addresses[1][addressIndex];
    }

    /// <summary>
    /// Method used for initializing all the addresses for this hardware wallet.
    /// </summary>
    public async void InitializeAddresses()
    {
        addresses[0] = new string[50];
        addresses[1] = new string[50];

        var data = await GetExtendedPublicKeyData();

        if (data == null)
        {
            MainThreadExecutor.QueueAction(WalletLoadUnsuccessful);
            return;
        }

        var electrumLedgerXPub = new ExtPubKey(new PubKey(data.publicKeyData).Compress(), data.chainCodeData);
        var defaultXPub = electrumLedgerXPub.Derive(0);

        for (uint i = 0; i < addresses[0].Length; i++)
        {
            addresses[0][i] = new EthECKey(defaultXPub.Derive(i).PubKey.ToBytes(), false).GetPublicAddress().ConvertToEthereumChecksumAddress();
            addresses[1][i] = new EthECKey(electrumLedgerXPub.Derive(i).PubKey.ToBytes(), false).GetPublicAddress().ConvertToEthereumChecksumAddress();
        }

        MainThreadExecutor.QueueAction(WalletLoadSuccessful);
    }

    /// <summary>
    /// Signs a transaction using this IWallet.
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
        if (signingTransaction)
            return;

        signingTransaction = true;

        TransactionUtils.GetAddressTransactionCount(addressFrom).OnSuccess(async txCount =>
        {
            var transaction = new Transaction(
                txCount.ToBytesForRLPEncoding(),
                gasPrice.ToBytesForRLPEncoding(),
                gasLimit.ToBytesForRLPEncoding(),
                addressTo.HexToByteArray(),
                value.ToBytesForRLPEncoding(),
                data.HexToByteArray(),
                new byte[] { 0 },
                new byte[] { 0 },
                (byte)ethereumNetworkSettings.networkType);

            var signedTransactionData = await GetSignedTransactionData(
                transaction,
                path,
                () => MainThreadExecutor.QueueAction(() => popupManager.GetPopup<T>(true)?.SetConfirmationValues(null, gasLimit, gasPrice, displayInput))).ConfigureAwait(false);

            signingTransaction = false;

            if (signedTransactionData == null)
            {
                return;
            }

            else if (!signedTransactionData.signed)
            {
                MainThreadExecutor.QueueAction(() => popupManager.CloseActivePopup());
                return;
            }

            var transactionChainId = new TransactionChainId(
                transaction.Nonce,
                transaction.GasPrice,
                transaction.GasLimit,
                transaction.ReceiveAddress,
                transaction.Value,
                transaction.Data,
                new byte[] { (byte)ethereumNetworkSettings.networkType },
                signedTransactionData.r,
                signedTransactionData.s,
                new byte[] { (byte)signedTransactionData.v });

            MainThreadExecutor.QueueAction(() =>
            {
                onTransactionSigned?.Invoke(new TransactionSignedUnityRequest(transactionChainId.GetRLPEncoded().ToHex(), ethereumNetworkManager.CurrentNetwork.NetworkUrl));
                popupManager.CloseAllPopups();
            });
        });
    }

    /// <summary>
    /// Invokes the OnWalletLoadSuccessful event if the wallet successfully loaded.
    /// </summary>
    protected void WalletLoadSuccessful() => OnWalletLoadSuccessful?.Invoke();

    /// <summary>
    /// Invokes the OnWalletLoadUnsuccessful event if the wallet did not successfully load.
    /// </summary>
    protected void WalletLoadUnsuccessful() => OnWalletLoadUnsuccessful?.Invoke();

    /// <summary>
    /// Abstract method used for retrieving the public key data from the hardware wallet.
    /// </summary>
    /// <returns> Task returning the ExtendedPublicKeyDataHolder instance. </returns>
    protected abstract Task<ExtendedPublicKeyDataHolder> GetExtendedPublicKeyData();

    /// <summary>
    /// Abstract method used for getting the signed transaction data from the hardware wallet.
    /// </summary>
    /// <param name="transaction"> The Transaction object containing all the data to sign. </param>
    /// <param name="path"> The path of the address signing the transaction. </param>
    /// <param name="onSignatureRequestSent"> Action to call once the request for a transaction signature has been sent. </param>
    /// <returns> Task returning the SignedTransactionDataHolder instance. </returns>
    protected abstract Task<SignedTransactionDataHolder> GetSignedTransactionData(Transaction transaction, string path, Action onSignatureRequestSent);

    /// <summary>
    /// Class holding the data needed to create the extended public key (xpub).
    /// </summary>
    protected sealed class ExtendedPublicKeyDataHolder
    {
        public byte[] publicKeyData;
        public byte[] chainCodeData;
    }

    /// <summary>
    /// Class holding the data of a signed transaction.
    /// </summary>
    protected sealed class SignedTransactionDataHolder
    {
        public bool signed;
        public uint v;
        public byte[] r;
        public byte[] s;
    }
}