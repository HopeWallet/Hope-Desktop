using System;
using System.Threading.Tasks;
using Trezor.Net;

public sealed class OpenTrezorWalletMenu : OpenHardwareWalletMenu<OpenTrezorWalletMenu, TrezorWallet>
{
	public event Action TrezorPINSectionOpening;

    public TrezorPINSection TrezorPINSection { get; private set; }

    protected override void OnAwake()
    {
        TrezorPINSection = GetComponentInChildren<TrezorPINSection>();
    }

    public void OpenPINSection()
    {
        TrezorPINSectionOpening?.Invoke();
    }

    protected override async Task<bool> IsHardwareWalletConnected()
    {
        var trezorManager = await Task<TrezorManager>.Factory.StartNew(() => TrezorConnector.GetWindowsConnectedTrezor(null)).ConfigureAwait(false);

        return trezorManager != null;
    }
}