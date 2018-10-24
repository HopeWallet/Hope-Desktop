using System;
using Zenject;

public sealed class EnterTrezorPINPopup : ExitablePopupComponent<EnterTrezorPINPopup>
{
    public event Action ReloadPINSection;

    public event Action CheckingPIN;

    private TrezorWallet trezorWallet;

    private bool checkingPin;

    public TrezorPINSection TrezorPINSection { get; private set; }

    [Inject]
    public void Construct(TrezorWallet trezorWallet)
    {
        this.trezorWallet = trezorWallet;
    }

    public void ReEnterPIN()
    {
        if (!checkingPin)
            return;

        MainThreadExecutor.QueueAction(() => ReloadPINSection?.Invoke());

        checkingPin = false;
    }

    public void CheckPIN()
    {
        MainThreadExecutor.QueueAction(() => CheckingPIN?.Invoke());

        checkingPin = true;
    }

    protected override void OnStart()
    {
        TrezorPINSection = GetComponentInChildren<TrezorPINSection>();
    }
}