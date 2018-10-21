using System;
using System.Threading.Tasks;
using Trezor.Net;

public sealed class OpenTrezorWalletMenu : OpenHardwareWalletMenu<OpenTrezorWalletMenu, TrezorWallet>
{
	public event Action TrezorPINSectionOpening;

    private TrezorPINSection trezorPinSection;

    private void Awake()
    {
        trezorPinSection = GetComponentInChildren<TrezorPINSection>();
    }

    public void OpenPINSection()
    {

    }

    protected override async Task<bool> IsHardwareWalletConnected()
    {
        var trezorManager = await Task<TrezorManager>.Factory.StartNew(() => TrezorConnector.GetWindowsConnectedTrezor(null)).ConfigureAwait(false);

        return trezorManager != null;
    }
}