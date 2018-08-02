using Hope.Utils.EthereumUtils;
using Nethereum.Hex.HexTypes;
using TMPro;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which manages each locked purpose item that is displayed.
/// </summary>
public sealed class LockedPRPSItemButton : InfoButton<LockedPRPSItemButton, Hodler.Output.Item>, IStandardGasPriceObservable, IEtherBalanceObservable
{
    public TMP_Text purposeAmountText,
                    dubiAmountText,
                    lockPeriodText,
                    timeLeftText;

    public Button releasePurposeButton;

    private EthereumNetworkManager.Settings networkSettings;
    private UserWalletManager userWalletManager;
    private GasPriceObserver gasPriceObserver;
    private EtherBalanceObserver etherBalanceObserver;
    private Hodler hodlerContract;
    private Hodler.Output.Item item;

    private dynamic etherBalance;
    private decimal lockedPurpose;
    private bool lockPeriodDone;

    private const int MIN_PERCENTAGE_TIME_RINKEBY = 3600; // For use with the rinkeby hodler where the lock period minimum is 1 hour.
    private const int MIN_PERCENTAGE_TIME_MAINNET = 7884000; // For use with the mainnet hodler where the lock period minimum is 3 months.

    public GasPrice StandardGasPrice { get; set; }

    /// <summary>
    /// The current ether balance of the wallet.
    /// </summary>
    public dynamic EtherBalance
    {
        get { return etherBalance; }
        set
        {
            etherBalance = value;
            RecheckIfFunctionCanBeSent();
        }
    }

    /// <summary>
    /// Adds the required dependencies to this class.
    /// </summary>
    /// <param name="networkSettings"> The active ethereum network settings. </param>
    /// <param name="hodlerContract"> The active HodlerContract. </param>
    /// <param name="userWalletManager"> The active UserWalletManager. </param>
    /// <param name="gasPriceObserver"> The active GasPriceObserver. </param>
    /// <param name="etherBalanceObserver"> The active EtherBalanceObserver. </param>
    [Inject]
    public void Construct(
        EthereumNetworkManager.Settings networkSettings,
        Hodler hodlerContract,
        UserWalletManager userWalletManager,
        GasPriceObserver gasPriceObserver,
        EtherBalanceObserver etherBalanceObserver)
    {
        this.networkSettings = networkSettings;
        this.hodlerContract = hodlerContract;
        this.userWalletManager = userWalletManager;
        this.gasPriceObserver = gasPriceObserver;
        this.etherBalanceObserver = etherBalanceObserver;
    }

    public void StartButtonUpdates()
    {
        releasePurposeButton.onClick.AddListener(ReleasePurpose);
        ObserverHelpers.SubscribeObservables(this, gasPriceObserver, etherBalanceObserver);
    }

    public void EndButtonUpdates()
    {
        ObserverHelpers.UnsubscribeObservables(this, gasPriceObserver, etherBalanceObserver);
    }

    /// <summary>
    /// Releases the purpose held in the item represented by this button.
    /// </summary>
    private void ReleasePurpose()
    {
        hodlerContract.Release(userWalletManager, new HexBigInteger(item.UnlockableGasLimit.Value), StandardGasPrice.FunctionalGasPrice, item.Id, lockedPurpose);
    }

    /// <summary>
    /// Updates the ui elements and the transaction info whenever the HodlerItem is changed/updated.
    /// </summary>
    /// <param name="info"> The item that holds the info on the purpose locked in the contract. </param>
    protected override void OnValueUpdated(Hodler.Output.Item info)
    {
        lockedPurpose = SolidityUtils.ConvertFromUInt(info.Value, 18);
        item = info;

        var minPercentageTime = networkSettings.networkType == EthereumNetworkManager.NetworkType.Mainnet ? MIN_PERCENTAGE_TIME_MAINNET : MIN_PERCENTAGE_TIME_RINKEBY;
        var releaseTimeDifference = info.ReleaseTime - info.LockedTimeStamp;
        var currentTimeDifference = info.ReleaseTime - DateTimeUtils.GetCurrentUnixTime();
        var multiplier = (decimal)releaseTimeDifference / minPercentageTime / 100;

        lockPeriodDone = currentTimeDifference < 0;
        purposeAmountText.text = lockedPurpose.ConvertDecimalToString().LimitEnd(15, "...");
        dubiAmountText.text = (multiplier * lockedPurpose).ConvertDecimalToString().LimitEnd(15, "...");
        lockPeriodText.text = DateTimeUtils.GetMaxTimeInterval((int)releaseTimeDifference);
        timeLeftText.text = currentTimeDifference < 0 ? "Done" : DateTimeUtils.GetMaxTimeInterval((int)currentTimeDifference);
    }

    /// <summary>
    /// Called when the gas prices have changed.
    /// </summary>
    public void OnGasPricesUpdated() => RecheckIfFunctionCanBeSent();

    /// <summary>
    /// Rechecks if the function can be executed once the ether balance changes, the gas price changes, or the gas limit is received.
    /// </summary>
    private void RecheckIfFunctionCanBeSent()
    {
        OnValueUpdated(item);

        if (!item.UnlockableGasLimit.HasValue)
            return;

        releasePurposeButton.interactable = item.UnlockableGasLimit.HasValue
            && etherBalance > GasUtils.CalculateMaximumGasCost(StandardGasPrice.FunctionalGasPrice.Value, item.UnlockableGasLimit.Value);
    }
}