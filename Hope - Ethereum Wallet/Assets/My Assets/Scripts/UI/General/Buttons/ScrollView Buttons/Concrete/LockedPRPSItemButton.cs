using Hope.Utils.Ethereum;
using Nethereum.Hex.HexTypes;
using System.Linq;
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

    private bool lockPeriodDone,
                 useUnlockableTime;

    private const int MIN_PERCENTAGE_TIME_RINKEBY = 3600; // For use with the rinkeby hodler where the lock period minimum is 1 hour.
    private const int MIN_PERCENTAGE_TIME_MAINNET = 7884000; // For use with the mainnet hodler where the lock period minimum is 3 months.

    /// <summary>
    /// The standard estimated gas price.
    /// </summary>
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
    /// Whether the unlockable time should be used.
    /// </summary>
    public bool UseUnlockableTime { set { useUnlockableTime = value; OnValueUpdated(ButtonInfo); } }

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

    /// <summary>
    /// Starts the updates for gas prices and the current ether balance.
    /// </summary>
    public void StartButtonUpdates()
    {
        releasePurposeButton.onClick.AddListener(ReleasePurpose);
        ObserverHelpers.SubscribeObservables(this, gasPriceObserver, etherBalanceObserver);
    }

    /// <summary>
    /// Stops all observable updates.
    /// </summary>
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
        purposeAmountText.text = lockedPurpose.ConvertDecimalToString().LimitEnd(12, "...");
        dubiAmountText.text = (multiplier * lockedPurpose).ConvertDecimalToString().LimitEnd(11, "...");
        lockPeriodText.text = DateTimeUtils.GetMaxTimeInterval((int)releaseTimeDifference);

        timeLeftText.text = currentTimeDifference < 0
            ? "Done"
            : !useUnlockableTime ? GetTimeLeftString((int)currentTimeDifference) : DateTimeUtils.TimeStampToDateTime((long)info.ReleaseTime).GetStringFormattedDate();
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

    /// <summary>
    /// Gets the formatted time left string.
    /// Formats it in up to three different time types, starting from the maximum.
    /// For example, if the Purpose will unlock in 2 hours and 59 minutes, the string will be 2h 59m xs (where x is the number of seconds).
    /// If the Purpose will unlock in 3 months, 22 days, the string will be 3m 22d xh (where x is the number of hours).
    /// </summary>
    /// <param name="unixTime"> The unix time which represents when the Purpose will be unlocked. </param>
    /// <returns> The formatted string which displays the time left until the unlock. </returns>
    private string GetTimeLeftString(long unixTime)
    {
        var times = new[]
        {
            new { MaxTime = 1, TimeText = "s" },
            new { MaxTime = DateTimeUtils.MINUTE_IN_SECONDS, TimeText = "m" },
            new { MaxTime = DateTimeUtils.HOUR_IN_SECONDS, TimeText = "h" },
            new { MaxTime = DateTimeUtils.DAY_IN_SECONDS, TimeText = "d" },
            new { MaxTime = DateTimeUtils.MONTH_IN_SECONDS, TimeText = "m" },
            new { MaxTime = DateTimeUtils.YEAR_IN_SECONDS, TimeText = "y" }
        };

        string[] resultTimeText = new string[3];

        int counter = 0;
        long remainder = unixTime;

        for (int i = times.Length - 1; i >= 0; i--)
        {
            if (unixTime / times[i].MaxTime > 0)
            {
                long time = remainder / times[i].MaxTime;

                if (time > 0)
                    resultTimeText[counter] = time + times[i].TimeText;

                remainder %= times[i].MaxTime;
                counter++;
            }

            if (counter == 3)
                break;
        }

        return string.Join(" ", resultTimeText.Where(text => !string.IsNullOrEmpty(text)));
    }
}