using System;
using System.Threading.Tasks;

public sealed class OpenTrezorWalletMenu : OpenHardwareWalletMenu<OpenTrezorWalletMenu, TrezorWallet>
{
	public event Action TrezorPINSectionOpening;

    protected override Task<bool> IsHardwareWalletConnected()
    {
        throw new NotImplementedException();
    }
}