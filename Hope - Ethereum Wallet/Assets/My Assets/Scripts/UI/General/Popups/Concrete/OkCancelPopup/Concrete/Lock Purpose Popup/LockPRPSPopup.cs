using Hope.Utils.Misc;
using Nethereum.Hex.HexTypes;
using TMPro;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class used for locking purpose.
/// </summary>
public sealed partial class LockPRPSPopup : OkCancelPopupComponent<LockPRPSPopup>, IEtherBalanceObservable
{
    public Button threeMonthsButton,
                  sixMonthsButton,
                  twelveMonthsButton;

    public TMP_InputField amountInputField;
    public Slider slider;

    public TMP_Text transactionFeeText,
                    prpsBalanceText,
                    dubiBalanceText,
                    dubiRewardText;

    public Toggle maxToggle;
    public InfoMessage infoMessage;

    private LockedPRPSManager lockedPRPSManager;
    private EtherBalanceObserver etherBalanceObserver;
    private UserWalletManager userWalletManager;
    private Hodler hodlerContract;

    /// <summary>
    /// Manages the gas for the LockPRPSPopup.
    /// </summary>
    public GasManager Gas { get; private set; }

    /// <summary>
    /// Manages the amount of purpose to lock for the LockPRPSPopup.
    /// </summary>
    public AmountManager Amount { get; private set; }

    /// <summary>
    /// Manages the amount of time for locking purpose.
    /// </summary>
    public TimeManager Time { get; private set; }

    /// <summary>
    /// The current ether balance of the wallet.
    /// </summary>
    public dynamic EtherBalance { get; set; }

    /// <summary>
    /// Adds all dependencies to the LockPRPSPopup.
    /// </summary>
    /// <param name="lockPRPSManager"> The active LockPRPSManager. </param>
    /// <param name="lockedPRPSManager"> The active LockedPRPSManager. </param>
    /// <param name="gasPriceObserver"> The active GasPriceObserver. </param>
    /// <param name="etherBalanceObserver"> The active EtherBalanceObserver. </param>
    /// <param name="hodlerContract"> The active Hodler smart contract. </param>
    /// <param name="userWalletManager"> The active UserWalletManager. </param>
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

    /// <summary>
    /// Initializes the PopupManager for the info message.
    /// </summary>
    protected override void OnStart()
    {
        infoMessage.PopupManager = popupManager;
    }

    /// <summary>
    /// Closes all the managers for the LockPRPSPopup and the ether balance observer.
    /// </summary>
    private void OnDestroy()
    {
        Gas.Stop();
        Amount.Stop();
        Time.Stop();

        etherBalanceObserver.UnsubscribeObservable(this);
    }

    /// <summary>
    /// Updates the lock button interactability based on the managers.
    /// </summary>
    private void Update()
    {
        okButton.interactable = EtherBalance >= Gas.TransactionFee && Gas.IsValid && Amount.IsValid && Time.IsValid;
    }

    /// <summary>
    /// Locks purpose based on all values entered.
    /// </summary>
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