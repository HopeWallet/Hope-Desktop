﻿using System;
using System.Threading.Tasks;
using NBitcoin;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.Signer;
using Trezor.Net.Contracts.Bitcoin;
using Transaction = Nethereum.Signer.Transaction;

public sealed class TrezorWallet : HardwareWallet
{
    public TrezorWallet(
        EthereumNetworkManager ethereumNetworkManager,
        EthereumNetworkManager.Settings ethereumNetworkSettings,
        PopupManager popupManager) : base(ethereumNetworkManager, ethereumNetworkSettings, popupManager)
    {
    }

    protected override async Task<ExtendedPublicKeyDataHolder> GetExtendedPublicKeyData()
    {
        var trezorManager = TrezorConnector.GetWindowsConnectedTrezor(EnterPin);
        if (trezorManager == null)
            return null;

        var publicKeyRequest = new GetPublicKey{ AddressNs = KeyPath.Parse(EXTENDED_PUBLIC_KEY_PATH).Indexes, ShowDisplay = false };
        var publicKeyResponse = await trezorManager.SendMessageAsync<PublicKey, GetPublicKey>(publicKeyRequest).ConfigureAwait(false);

        if (publicKeyResponse == null)
            return null;

        return new ExtendedPublicKeyDataHolder { publicKeyData = publicKeyResponse.Node.PublicKey, chainCodeData = publicKeyResponse.Node.ChainCode };
    }

    protected override Task<SignedTransactionDataHolder> GetSignedTransactionData(Transaction transaction, string path, Action onSignatureRequestSent)
    {
        throw new NotImplementedException();
    }

    private async Task<string> EnterPin()
    {
        while (HopeTesting.Instance.pin.Length < 4)
        {
            await Task.Delay(100);
        }

        return HopeTesting.Instance.pin;
    }
}