using System;
using System.Threading.Tasks;
using NBitcoin;
using Trezor.Net;
using Trezor.Net.Contracts.Bitcoin;
using Trezor.Net.Contracts.Common;
using Trezor.Net.Contracts.Ethereum;
using UnityEngine.Events;
using Transaction = Nethereum.Signer.Transaction;

public sealed class TrezorWallet : HardwareWallet
{
    public event Action PINIncorrect;

    private readonly UIManager uiManager;

    private bool advance,
                 forceCancel;

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

    protected override async Task<SignedTransactionDataHolder> GetSignedTransactionData(Transaction transaction, string path, Action onSignatureRequestSent)
    {
        var trezorManager = TrezorConnector.GetWindowsConnectedTrezor(GetSignedTransactionDataEnterPin);
        if (trezorManager == null)
            return null;

        var signedTransactionResponse = (EthereumTxRequest)null;
        var signedTransactionRequest = new EthereumSignTx
        {
            Nonce = transaction.Nonce,
            AddressNs = KeyPath.Parse(path).Indexes,
            GasPrice = transaction.GasPrice,
            GasLimit = transaction.GasLimit,
            To = transaction.ReceiveAddress,
            Value = transaction.Value,
            DataInitialChunk = transaction.Data,
            DataLength = (uint)transaction.Data.Length,
            ChainId = (uint)ethereumNetworkSettings.networkType
        };

        while (true)
        {
            try
            {
                signedTransactionResponse = await trezorManager.SendMessageAsync<EthereumTxRequest, EthereumSignTx>(signedTransactionRequest, onSignatureRequestSent)
                                                               .ConfigureAwait(false);
                break;
            }
            catch (FailureException<Failure>)
            {
                if (forceCancel)
                {
                    forceCancel = false;
                    break;
                }

                if (trezorManager.PinRequest.HasValue && trezorManager.PinRequest == false)
                {
                    break;
                }
            }
        }

        if (signedTransactionResponse == null)
            return new SignedTransactionDataHolder();

        return new SignedTransactionDataHolder
        {
            signed = true,
            v = signedTransactionResponse.SignatureV,
            r = signedTransactionResponse.SignatureR,
            s = signedTransactionResponse.SignatureS
        };
    }

    private async Task<string> GetSignedTransactionDataEnterPin()
    {
        var popup = (EnterTrezorPINPopup)null;
        MainThreadExecutor.QueueAction(() => popup = popupManager.GetPopup<EnterTrezorPINPopup>(true));

        while (popup == null)
        {
            await Task.Delay(100);
        }

        var advanceAction = new UnityAction(() => advance = true);

        popup.ReEnterPIN();
        popup.TrezorPINSection.NextButton.onClick.AddListener(advanceAction);

        while (!advance)
        {
            if (popupManager.ActivePopupType != typeof(EnterTrezorPINPopup))
            {
                forceCancel = true;
                advance = false;

                return string.Empty;
            }

            await Task.Delay(100);
        }

        advance = false;

        popup.CheckPIN();
        popup.TrezorPINSection.NextButton.onClick.RemoveListener(advanceAction);

        return popup.TrezorPINSection.PinText;
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