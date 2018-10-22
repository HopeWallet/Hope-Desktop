using Zenject;

public sealed class EnterTrezorPINPopup : ExitablePopupComponent<EnterTrezorPINPopup>
{
    private TrezorWallet trezorWallet;

    private bool pinSectionOpen;

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