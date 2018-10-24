using System;
using Zenject;

public sealed class EnterTrezorPINPopup : ExitablePopupComponent<EnterTrezorPINPopup>
{
    public event Action ReloadPINSection;

    public event Action CheckingPIN;

    private TrezorWallet trezorWallet;

    public TrezorPINSection TrezorPINSection { get; private set; }

    [Inject]
    public void Construct(TrezorWallet trezorWallet)
    {
        this.trezorWallet = trezorWallet;
    }

    protected override void OnStart()
    {
        TrezorPINSection = GetComponentInChildren<TrezorPINSection>();
    }
}