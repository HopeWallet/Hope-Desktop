using Hope.Utils.Misc;
using Nethereum.Hex.HexTypes;
using TMPro;
using UnityEngine.UI;
using Zenject;

public sealed partial class LockPRPSPopup : OkCancelPopupComponent<LockPRPSPopup>, IEtherBalanceObservable
{
    public InfoMessage infoMessage;

    public TMP_InputField amountInputField;
    public Slider slider;
    public Toggle maxToggle;

    public Button lockPRPSButton,
                  threeMonthsButton,
                  sixMonthsButton,
                  twelveMonthsButton;

    public TMP_Text transactionFeeText,
                    prpsBalanceText,
                    dubiBalanceText,
                    dubiRewardText;

    private LockedPRPSManager lockedPRPSManager;
    private EtherBalanceObserver etherBalanceObserver;
    private UserWalletManager userWalletManager;
    private Hodler hodlerContract;

    public GasManager Gas { get; private set; }

    public AmountManager Amount { get; private set; }

    public TimeManager Time { get; private set; }

    public dynamic EtherBalance { get; set; }

    [Inject]
    public void Construct(
        LockPRPSManager lockPRPSManager,
        LockedPRPSManager lockedPRPSManager,
        GasPriceObserver gasPriceObserver,
        EtherBalanceObserver etherBalanceObserver,
        Hodler hodlerContract,
        UserWalletManager userWalletManager)
    {
        this.lockedPRPSManager = lockedPRPSManager;
        this.etherBalanceObserver = etherBalanceObserver;
        this.userWalletManager = userWalletManager;
        this.hodlerContract = hodlerContract;
        etherBalanceObserver.SubscribeObservable(this);

        Gas = new GasManager(lockPRPSManager, gasPriceObserver, slider, transactionFeeText);
        Amount = new AmountManager(lockPRPSManager, maxToggle, amountInputField, prpsBalanceText, dubiBalanceText, dubiRewardText);
        Time = new TimeManager(Amount, threeMonthsButton, sixMonthsButton, twelveMonthsButton, dubiRewardText);
    }

    protected override void OnStart()
    {
        infoMessage.PopupManager = popupManager;
    }

    private void OnDestroy()
    {
        Gas.Stop();
        Amount.Stop();
        Time.Stop();

        etherBalanceObserver.UnsubscribeObservable(this);
    }

    private void Update()
    {
        lockPRPSButton.interactable = EtherBalance >= Gas.TransactionFee && Gas.IsValid && Amount.IsValid && Time.IsValid;
    }

    public override void OkButton()
    {
        hodlerContract.Hodl(userWalletManager,
                            new HexBigInteger(Gas.TransactionGasLimit),
                            Gas.TransactionGasPrice.FunctionalGasPrice,
                            RandomUtils.GenerateRandomBigInteger(lockedPRPSManager.UsedIds),
                            Amount.AmountToLock,
                            Time.MonthsToLock);
    }
}