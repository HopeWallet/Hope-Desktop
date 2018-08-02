using TMPro;
using UnityEngine.UI;
using Zenject;

public sealed partial class LockPRPSPopup : ExitablePopupComponent<LockPRPSPopup>, IEtherBalanceObservable
{
	public InfoMessage infoMessage;

    public Button lockPRPSButton;
    public TMP_InputField amountInputField;
    public Slider slider;
    public Toggle maxToggle;

    public TMP_Text transactionFeeText,
                    prpsBalanceText,
                    dubiBalanceText,
                    dubiRewardText;

    private GasManager gasManager;
    private AmountManager amountManager;

    private EtherBalanceObserver etherBalanceObserver;

    public dynamic EtherBalance { get; set; }

    public GasPrice StandardGasPrice { get; set; }

    [Inject]
    public void Construct(
        LockPRPSManager lockPRPSManager,
        GasPriceObserver gasPriceObserver,
        EtherBalanceObserver etherBalanceObserver)
    {
        this.etherBalanceObserver = etherBalanceObserver;
        etherBalanceObserver.SubscribeObservable(this);

        gasManager = new GasManager(lockPRPSManager, gasPriceObserver, slider, transactionFeeText);
        amountManager = new AmountManager(lockPRPSManager, maxToggle, amountInputField, prpsBalanceText, dubiBalanceText, dubiRewardText);
    }

    protected override void OnStart()
    {
        infoMessage.PopupManager = popupManager;
    }

    private void OnDestroy()
    {
        gasManager.Stop();
        amountManager.Stop();
        etherBalanceObserver.UnsubscribeObservable(this);
    }

    private void Update()
    {
        lockPRPSButton.interactable = EtherBalance >= gasManager.TransactionFee && gasManager.IsValid;
    }

}