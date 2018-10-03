using System;
using System.Linq;
using Ledger.Net;
using Ledger.Net.Connectivity;
using Ledger.Net.Requests;
using Ledger.Net.Responses;
using NBitcoin;
using Nethereum.HdWallet;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.Signer;
using Nethereum.Util;
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
    /// Initializes all of the addresses for this Ledger wallet.
    /// </summary>
    public override async void InitializeAddresses()
    {
        addresses[0] = new string[50];
        addresses[1] = new string[50];

        var ledgerManager = LedgerConnector.GetWindowsConnectedLedger();

        if (ledgerManager == null)
        {
            MainThreadExecutor.QueueAction(WalletLoadUnsuccessful);
            return;
        }

        var pubKeyResponse = await ledgerManager.GetPublicKeyResponse(Wallet.ELECTRUM_LEDGER_PATH.TrimEnd('x', '/'), true, false).ConfigureAwait(false);

        if (pubKeyResponse?.IsSuccess != true)
        {
            MainThreadExecutor.QueueAction(WalletLoadUnsuccessful);
            return;
        }

        var pubKeyData = pubKeyResponse.PublicKeyData;
        var chainCodeData = pubKeyResponse.ExtraData.Take(32).ToArray();

        var electrumLedgerXPub = new ExtPubKey(new PubKey(pubKeyData).Compress(), chainCodeData);
        var defaultXPub = electrumLedgerXPub.Derive(0);

        for (uint i = 0; i < addresses[0].Length; i++)
        {
            addresses[0][i] = new EthECKey(defaultXPub.Derive(i).PubKey.ToBytes(), false).GetPublicAddress().ConvertToEthereumChecksumAddress();
            addresses[1][i] = new EthECKey(electrumLedgerXPub.Derive(i).PubKey.ToBytes(), false).GetPublicAddress().ConvertToEthereumChecksumAddress();
        }

        MainThreadExecutor.QueueAction(WalletLoadSuccessful);
    }

    /// <summary>
    /// Signs a transaction using the Ledger wallet.
    /// </summary>
    /// <param name="onTransactionSigned"> Action to call once the transaction has been signed. </param>
    /// <param name="transaction"> The Transaction object containing all the data to sign. </param>
    /// <param name="path"> The path of the address signing the transaction. </param>
    protected override async void SignTransaction(Action<TransactionSignedUnityRequest> onTransactionSigned, Transaction transaction, string path)
    {
        var ledgerManager = LedgerConnector.GetWindowsConnectedLedger();

        if (ledgerManager == null)
            return;

        var addressIndex = path.Count(c => c == '/') - 1;
        var derivationData = Helpers.GetDerivationPathData(path);

        var request = new EthereumAppSignTransactionRequest(derivationData.Concat(transaction.GetRLPEncoded()).ToArray());
        var response = await ledgerManager.SendRequestAsync<EthereumAppSignTransactionResponse, EthereumAppSignTransactionRequest>(request).ConfigureAwait(false);

        if (!response.IsSuccess)
            return;

        var transactionChainId = new TransactionChainId(
            transaction.Nonce,
            transaction.GasPrice,
            transaction.GasLimit,
            transaction.ReceiveAddress,
            transaction.Value,
            transaction.Data,
            new byte[] { (byte)ethereumNetworkSettings.networkType },
            response.SignatureR,
            response.SignatureS,
            new byte[] { (byte)response.SignatureV });

        MainThreadExecutor.QueueAction(() =>
        {
            onTransactionSigned?.Invoke(new TransactionSignedUnityRequest(transactionChainId.GetRLPEncoded().ToHex(), ethereumNetworkManager.CurrentNetwork.NetworkUrl));
            popupManager.CloseAllPopups();
        });
    }
}