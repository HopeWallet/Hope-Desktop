using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Hope.Utils.Ethereum;
using Ledger.Net.Connectivity;
using Ledger.Net.Requests;
using Ledger.Net.Responses;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.Signer;

public sealed class LedgerWallet : HardwareWallet
{

    private LedgerWallet(
        EthereumNetworkManager ethereumNetworkManager,
        EthereumNetworkManager.Settings ethereumNetworkSettings) : base(ethereumNetworkManager, ethereumNetworkSettings)
    {
    }

    public override async void InitializeAddresses()
    {
        var ledgerManager = LedgerConnector.GetWindowsConnectedLedger();

        if (ledgerManager == null)
        {
            WalletLoadUnsuccessful();
            return;
        }

        for (uint i = 0; i < addresses.Length; i++)
        {
            addresses[i] = await ledgerManager.GetAddressAsync(0, i).ConfigureAwait(false);

            if (string.IsNullOrEmpty(addresses[i]))
            {
                WalletLoadUnsuccessful();
                return;
            }
        }

        WalletLoadSuccessful();
    }

    protected override async void SignTransaction(Action<TransactionSignedUnityRequest> onTransactionSigned, Transaction transaction, uint addressIndex)
    {
        var ledgerManager = LedgerConnector.GetWindowsConnectedLedger();

        if (ledgerManager == null)
            return;

        var derivationData = Ledger.Net.Helpers.GetDerivationPathData(ledgerManager.CurrentCoin.App, ledgerManager.CurrentCoin.CoinNumber, 0, addressIndex, false, ledgerManager.CurrentCoin.IsSegwit);
        var request = new EthereumAppSignTransactionRequest(derivationData.Concat(transaction.GetRLPEncoded()).ToArray());
        var response = await ledgerManager.SendRequestAsync<EthereumAppSignTransactionResponse, EthereumAppSignTransactionRequest>(request).ConfigureAwait(false);

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

        var ethSendRawTransaction = new EthSendRawTransactionUnityRequest(ethereumNetworkManager.CurrentNetwork.NetworkUrl);
        MainThreadExecutor.QueueAction(() => ethSendRawTransaction.SendRequest(transactionChainId.GetRLPEncoded().ToHex()).StartCoroutine());
    }
}