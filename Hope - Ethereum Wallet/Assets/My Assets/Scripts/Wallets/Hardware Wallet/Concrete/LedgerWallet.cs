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

public sealed class LedgerWallet : HardwareWallet
{
    public LedgerWallet(
        EthereumNetworkManager ethereumNetworkManager,
        EthereumNetworkManager.Settings ethereumNetworkSettings,
        PopupManager popupManager) : base(ethereumNetworkManager, ethereumNetworkSettings, popupManager)
    {
    }

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

        if (!pubKeyResponse.IsSuccess)
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
            var ethSendRawTransaction = new EthSendRawTransactionUnityRequest(ethereumNetworkManager.CurrentNetwork.NetworkUrl);
            ethSendRawTransaction.SendRequest(transactionChainId.GetRLPEncoded().ToHex()).StartCoroutine();

            popupManager.CloseAllPopups();
        });
    }
}