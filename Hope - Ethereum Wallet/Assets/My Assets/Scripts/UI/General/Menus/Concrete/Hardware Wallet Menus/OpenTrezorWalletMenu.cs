using System;
using System.Threading.Tasks;
using Trezor.Net;

public sealed class OpenTrezorWalletMenu : OpenHardwareWalletMenu<OpenTrezorWalletMenu, TrezorWallet>
{
    public event Action TrezorPINSectionOpening;
    public event Action ReloadPINSection;

    public event Action CheckingPIN;

    private bool pinSectionOpen;

    public TrezorPINSection TrezorPINSection { get; private set; }

    protected override void OnAwake()
    {
        TrezorPINSection = GetComponentInChildren<TrezorPINSection>();
    }

    public void UpdatePINSection()
    {
        if (!pinSectionOpen)
        {
            pinSectionOpen = true;

            MainThreadExecutor.QueueAction(() => TrezorPINSectionOpening?.Invoke());
        }
        else
        {
            MainThreadExecutor.QueueAction(() => ReloadPINSection?.Invoke());
        }
    }

    public void CheckPIN()
    {
        MainThreadExecutor.QueueAction(() => CheckingPIN?.Invoke());
    }

    protected override async Task<bool> IsHardwareWalletConnected()
    {
        var trezorManager = await Task<TrezorManager>.Factory.StartNew(() => TrezorConnector.GetWindowsConnectedTrezor(null)).ConfigureAwait(false);

        return trezorManager != null;
    }
}