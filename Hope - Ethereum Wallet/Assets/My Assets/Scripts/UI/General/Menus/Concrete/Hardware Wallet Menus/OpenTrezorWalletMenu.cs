using System;
using System.Threading.Tasks;
using Trezor.Net;

public sealed class OpenTrezorWalletMenu : OpenHardwareWalletMenu<OpenTrezorWalletMenu, TrezorWallet>
{
	public event Action TrezorPINSectionOpening;

    public event Action IncorrectPIN;
    public event Action CheckingPIN;

    private bool pinSectionOpen;

    public TrezorPINSection TrezorPINSection { get; private set; }

    protected override void OnAwake()
    {
        TrezorPINSection = GetComponentInChildren<TrezorPINSection>();
    }

    public void OpenPINSection()
    {
        if (pinSectionOpen)
        {
            pinSectionOpen = true;
            TrezorPINSectionOpening?.Invoke();
        }
        else
        {
            IncorrectPIN?.Invoke();
        }
    }

    public void CheckPIN()
    {
        CheckingPIN?.Invoke();
    }

    protected override async Task<bool> IsHardwareWalletConnected()
    {
        var trezorManager = await Task<TrezorManager>.Factory.StartNew(() => TrezorConnector.GetWindowsConnectedTrezor(null)).ConfigureAwait(false);

        return trezorManager != null;
    }
}