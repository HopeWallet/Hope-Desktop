using System;
using System.Linq;
using Ledger.Net;
using Ledger.Net.Connectivity;
using Ledger.Net.Requests;
using Ledger.Net.Responses;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.Signer;
using Nethereum.Util;

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
        var ledgerManager = LedgerConnector.GetWindowsConnectedLedger();

        if (ledgerManager == null)
        {
            MainThreadExecutor.QueueAction(WalletLoadUnsuccessful);
            return;
        }

        for (uint i = 0; i < addresses.Length; i++)
        {
            var address = await ledgerManager.GetAddressAsync(0, i).ConfigureAwait(false);
            addresses[i] = string.IsNullOrEmpty(address) ? null : address.ConvertToEthereumChecksumAddress();

            if (string.IsNullOrEmpty(address))
            {
                MainThreadExecutor.QueueAction(WalletLoadUnsuccessful);
                return;
            }
        }

        MainThreadExecutor.QueueAction(WalletLoadSuccessful);
    }

    protected override async void SignTransaction(Action<TransactionSignedUnityRequest> onTransactionSigned, Transaction transaction, uint addressIndex)
    {
        var ledgerManager = LedgerConnector.GetWindowsConnectedLedger();

        if (ledgerManager == null)
            return;

        var derivationData = Helpers.GetDerivationPathData(ledgerManager.CurrentCoin.App, ledgerManager.CurrentCoin.CoinNumber, 0, addressIndex, false, ledgerManager.CurrentCoin.IsSegwit);
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