using System;
using System.Threading.Tasks;

public sealed class OpenTrezorWalletMenu : OpenHardwareWalletMenu<OpenTrezorWalletMenu, TrezorWallet>
{
	public event Action TrezorPINSectionOpening;

    protected override async Task<bool> IsHardwareWalletConnected()
    {
        var trezorManager = TrezorConnector.GetWindowsConnectedTrezor(null);

        return trezorManager != null;
    }
}