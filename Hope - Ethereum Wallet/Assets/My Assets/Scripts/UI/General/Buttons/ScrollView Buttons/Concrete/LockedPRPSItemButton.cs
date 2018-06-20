using Hope.Utils.EthereumUtils;
using UnityEngine.UI;
using Zenject;

public class LockedPRPSItemButton : InfoButton<LockedPRPSItemButton, HodlerItem>
{

    public Text purposeAmountText,
                dubiAmountText,
                lockPeriodText,
                timeLeftText;

    public Button releasePurposeButton;

    private EthereumNetworkManager.Settings networkSettings;

    private const int MIN_PERCENTAGE_TIME_RINKEBY = 3600; // For use with the rinkeby hodler where the lock period minimum is 1 hour.
    private const int MIN_PERCENTAGE_TIME_MAINNET = 7884000; // For use with the mainnet hodler where the lock period minimum is 3 months.

    /// <summary>
    /// Determines the dubi percentage given the active ethereum network.
    /// </summary>
    /// <param name="ethereumNetworkManager"> The active ethereum network. </param>
    [Inject]
    public void DetermineDUBIPercentage(EthereumNetworkManager.Settings networkSettings) => this.networkSettings = networkSettings;

    protected override void OnValueUpdated(HodlerItem value)
    {
        var minPercentageTime = networkSettings.networkType == EthereumNetworkManager.NetworkType.Mainnet ? MIN_PERCENTAGE_TIME_MAINNET : MIN_PERCENTAGE_TIME_RINKEBY;
        var releaseTimeDifference = value.ReleaseTime - value.LockedTimeStamp;
        var currentTimeDifference = value.ReleaseTime - DateTimeUtils.GetCurrentUnixTime();
        var correctedPrpsAmount = SolidityUtils.ConvertFromUInt(value.Value, 18);
        var multiplier = (decimal)releaseTimeDifference / minPercentageTime / 100;
        
        purposeAmountText.text = correctedPrpsAmount.ToString().LimitEnd(8, "...");
        dubiAmountText.text = (multiplier * correctedPrpsAmount).ToString().LimitEnd(15, "...");
        lockPeriodText.text = DateTimeUtils.GetMaxTimeInterval((int)releaseTimeDifference);
        timeLeftText.text = currentTimeDifference < 0 ? "Done" : DateTimeUtils.GetMaxTimeInterval((int)currentTimeDifference);
        releasePurposeButton.interactable = currentTimeDifference < 0;
    }

}
