using Zenject;

public sealed class LockPRPSPopup : ExitablePopupComponent<LockPRPSPopup>, IEtherBalanceObservable
{
	public InfoMessage infoMessage;

    private LockPRPSManager lockPRPSManager;

    public dynamic EtherBalance { get; set; }

    public GasPrice StandardGasPrice { get; set; }

    [Inject]
    public void Construct(LockPRPSManager lockPRPSManager)
    {
        this.lockPRPSManager = lockPRPSManager;
    }

    protected override void OnStart()
    {
        infoMessage.PopupManager = popupManager;
    }

    private void OnDestroy()
    {

    }
}