using System;
using System.Linq;
using System.Threading.Tasks;
using Ledger.Net;
using Ledger.Net.Connectivity;
using Ledger.Net.Requests;
using Ledger.Net.Responses;
using Nethereum.HdWallet;
using Transaction = Nethereum.Signer.Transaction;

/// <summary>
/// Class which manages the Ledger hardware wallet.
/// </summary>
public sealed class LedgerWallet : HardwareWallet
{
    /// <summary>
    /// Initializes the LedgerWallet by passing all info to the base HardwareWallet class.
    /// </summary>
    /// <param name="ethereumNetworkManager"> The active EthereumNetworkManager. </param>
    /// <param name="ethereumNetworkSettings"> The settings for the EthereumNetworkManager. </param>
    /// <param name="popupManager"> The active PopupManager. </param>
    public LedgerWallet(
        EthereumNetworkManager ethereumNetworkManager,
        EthereumNetworkManager.Settings ethereumNetworkSettings,
        PopupManager popupManager) : base(ethereumNetworkManager, ethereumNetworkSettings, popupManager)
    {
    }

    /// <summary>
    /// Gets the public key data from the Ledger wallet.
    /// </summary>
    /// <returns> Task returning the ExtendedPublicKeyDataHolder instance. </returns>
    protected override async Task<ExtendedPublicKeyDataHolder> GetExtendedPublicKeyData()
    {
        var ledgerManager = LedgerConnector.GetWindowsConnectedLedger();
        if (ledgerManager == null)
            return null;

        var pubKeyResponse = await ledgerManager.GetPublicKeyResponse(EXTENDED_PUBLIC_KEY_PATH, true, false).ConfigureAwait(false);
        if (pubKeyResponse?.IsSuccess != true)
            return null;

        return new ExtendedPublicKeyDataHolder { publicKeyData = pubKeyResponse.PublicKeyData, chainCodeData = pubKeyResponse.ExtraData.Take(32).ToArray() };
    }

    /// <summary>
    /// Gets the signed transaction data from the Ledger wallet.
    /// </summary>
    /// <param name="transaction"> The transaction to sign. </param>
    /// <param name="path"> The path of the address to sign the transaction with. </param>
    /// <param name="onSignatureRequestSent"> Action to call once the signature request has been sent. </param>
    /// <returns> Task returning the SignedTransactionDataHolder instance. </returns>
    protected override async Task<SignedTransactionDataHolder> GetSignedTransactionData(Transaction transaction, string path, Action onSignatureRequestSent)
    {
        var ledgerManager = LedgerConnector.GetWindowsConnectedLedger();
        var address = ledgerManager == null
            ? null
            : (await ledgerManager.GetPublicKeyResponse(Wallet.ELECTRUM_LEDGER_PATH.Replace("x", "0"), false, false).ConfigureAwait(false))?.Address;

        // Don't sign transaction if app is not Ethereum, or if the first address doesn't match the first address of the opened Ledger wallet.
        if (string.IsNullOrEmpty(address) || !address.EqualsIgnoreCase(addresses[1][0]))
            return null;

        var addressIndex = path.Count(c => c == '/') - 1;
        var derivationData = Helpers.GetDerivationPathData(path);

        var request = new EthereumAppSignTransactionRequest(derivationData.Concat(transaction.GetRLPEncoded()).ToArray());

        onSignatureRequestSent?.Invoke();

        var response = await ledgerManager.SendRequestAsync<EthereumAppSignTransactionResponse, EthereumAppSignTransactionRequest>(request).ConfigureAwait(false);
        if (!response.IsSuccess)
            return new SignedTransactionDataHolder();

        return new SignedTransactionDataHolder { signed = true, v = response.SignatureV, r = response.SignatureR, s = response.SignatureS };
    }
}