using System;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.Signer;

public sealed class TrezorWallet : HardwareWallet
{
    public TrezorWallet(
        EthereumNetworkManager ethereumNetworkManager,
        EthereumNetworkManager.Settings ethereumNetworkSettings,
        PopupManager popupManager) : base(ethereumNetworkManager, ethereumNetworkSettings, popupManager)
    {
    }

    public override async void InitializeAddresses()
    {
    }

    protected override async void SignTransaction(Action<TransactionSignedUnityRequest> onTransactionSigned, Transaction transaction, uint addressIndex)
    {
    }
}