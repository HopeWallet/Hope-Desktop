using Hope.Utils.Misc;
using Nethereum.Hex.HexTypes;
using System.Numerics;
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

    private GasManager gasManager;
    private AmountManager amountManager;
    private TimeManager timeManager;

    private LockedPRPSManager lockedPRPSManager;
    private EtherBalanceObserver etherBalanceObserver;
    private UserWalletManager userWalletManager;
    private Hodler hodlerContract;

    public dynamic EtherBalance { get; set; }

    public GasPrice StandardGasPrice { get; set; }

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

        gasManager = new GasManager(lockPRPSManager, gasPriceObserver, slider, transactionFeeText);
        amountManager = new AmountManager(lockPRPSManager, maxToggle, amountInputField, prpsBalanceText, dubiBalanceText, dubiRewardText);
        timeManager = new TimeManager(amountManager, threeMonthsButton, sixMonthsButton, twelveMonthsButton, dubiRewardText);
    }

    protected override void OnStart()
    {
        infoMessage.PopupManager = popupManager;
    }

    private void OnDestroy()
    {
        gasManager.Stop();
        amountManager.Stop();
        timeManager.Stop();

        etherBalanceObserver.UnsubscribeObservable(this);
    }

    private void Update()
    {
        lockPRPSButton.interactable = EtherBalance >= gasManager.TransactionFee && gasManager.IsValid && amountManager.IsValid && timeManager.IsValid;
    }

    public override void OkButton()
    {
        hodlerContract.Hodl(userWalletManager,
                            new HexBigInteger(gasManager.TransactionGasLimit),
                            gasManager.TransactionGasPrice.FunctionalGasPrice,
                            RandomUtils.GenerateRandomBigInteger(lockedPRPSManager.UsedIds),
                            amountManager.AmountToLock,
                            timeManager.MonthsToLock);
    }
}