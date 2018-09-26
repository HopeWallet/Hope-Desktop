using System;
using System.Linq;
using System.Threading.Tasks;
using Ledger.Net;
using Ledger.Net.Connectivity;
using Ledger.Net.Requests;
using Ledger.Net.Responses;
using Nethereum.HdWallet;
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

        addresses[0] = new string[50];
        addresses[1] = new string[50];

        if (!await AssignAddresses(ledgerManager, Wallet.DEFAULT_PATH) || !await AssignAddresses(ledgerManager, Wallet.ELECTRUM_LEDGER_PATH))
            return;

        MainThreadExecutor.QueueAction(WalletLoadSuccessful);
    }

    private async Task<bool> AssignAddresses(LedgerManager ledgerManager, string path)
    {
        App app = path.EqualsIgnoreCase(Wallet.DEFAULT_PATH) ? App.Bitcoin : App.Ethereum;
        int addressesIndex = path.EqualsIgnoreCase(Wallet.DEFAULT_PATH) ? 0 : 1;

        for (uint i = 0; i < addresses[addressesIndex].Length; i++)
        {
            byte[] data = GetDerivationPathData(app, i);

            var request = new EthereumAppGetPublicKeyRequest(false, false, data);
            var response = await ledgerManager.SendRequestAsync<EthereumAppGetPublicKeyResponse, EthereumAppGetPublicKeyRequest>(request).ConfigureAwait(false);

            var address = response.IsSuccess ? response.Address : null;
            addresses[addressesIndex][i] = string.IsNullOrEmpty(address) ? null : address.ConvertToEthereumChecksumAddress();

            if (string.IsNullOrEmpty(address))
            {
                MainThreadExecutor.QueueAction(WalletLoadUnsuccessful);
                return false;
            }
        }

        return true;
    }

    private byte[] GetDerivationPathData(App app, uint index)
    {
        return Helpers.GetDerivationPathData(app, 60, 0, index, false, false);
    }

    protected override async void SignTransaction(Action<TransactionSignedUnityRequest> onTransactionSigned, Transaction transaction, string path)
    {
        var ledgerManager = LedgerConnector.GetWindowsConnectedLedger();

        if (ledgerManager == null)
            return;

        var addressIndex = path.Count(c => c == '/') - 1;
        var derivationData = GetDerivationPathData(addressIndex == 3 ? App.Ethereum : App.Bitcoin, new NBitcoin.KeyPath(path).Indexes[addressIndex]);

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