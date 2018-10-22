using System;
using System.Threading.Tasks;
using NBitcoin;
using Trezor.Net;
using Trezor.Net.Contracts.Bitcoin;
using Trezor.Net.Contracts.Common;
using UnityEngine.Events;
using Transaction = Nethereum.Signer.Transaction;

public sealed class TrezorWallet : HardwareWallet
{
    public event Action PINIncorrect;

    private readonly UIManager uiManager;

    private bool advance;

    public TrezorWallet(
        EthereumNetworkManager ethereumNetworkManager,
        EthereumNetworkManager.Settings ethereumNetworkSettings,
        PopupManager popupManager,
        UIManager uiManager) : base(ethereumNetworkManager, ethereumNetworkSettings, popupManager)
    {
        this.uiManager = uiManager;
    }

    protected override async Task<ExtendedPublicKeyDataHolder> GetExtendedPublicKeyData()
    {
        var trezorManager = TrezorConnector.GetWindowsConnectedTrezor(GetExtendedPublicKeyDataEnterPin);
        if (trezorManager == null)
            return null;

        var publicKeyRequest = new GetPublicKey { AddressNs = KeyPath.Parse(EXTENDED_PUBLIC_KEY_PATH).Indexes, ShowDisplay = false };
        var publicKeyResponse = (PublicKey)null;

        while (publicKeyResponse == null)
        {
            try
            {
                publicKeyResponse = await trezorManager.SendMessageAsync<PublicKey, GetPublicKey>(publicKeyRequest).ConfigureAwait(false);
            }
            catch (FailureException<Failure>)
            {
                MainThreadExecutor.QueueAction(() => PINIncorrect?.Invoke());

                publicKeyResponse = null;
                advance = false;
            }
        }

        if (publicKeyResponse == null)
            return null;

        return new ExtendedPublicKeyDataHolder { publicKeyData = publicKeyResponse.Node.PublicKey, chainCodeData = publicKeyResponse.Node.ChainCode };
    }

    protected override Task<SignedTransactionDataHolder> GetSignedTransactionData(Transaction transaction, string path, Action onSignatureRequestSent)
    {
        throw new NotImplementedException();
    }

    private async Task<string> GetSignedTransactionDataEnterPin()
    {
        return null;
    }

    private async Task<string> GetExtendedPublicKeyDataEnterPin()
    {
        var advanceAction = new UnityAction(() => advance = true);

        var trezorMenu = uiManager.ActiveMenu as OpenTrezorWalletMenu;
        trezorMenu.UpdatePINSection();
        trezorMenu.TrezorPINSection.NextButton.onClick.AddListener(advanceAction);

        while (!advance)
        {
            await Task.Delay(100);
        }

        advance = false;

        trezorMenu.CheckPIN();
        trezorMenu.TrezorPINSection.NextButton.onClick.RemoveListener(advanceAction);

        return trezorMenu.TrezorPINSection.PinText;
    }
}