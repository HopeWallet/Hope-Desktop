using Hope.Utils.EthereumUtils;
using UnityEngine.UI;

public class LockedPRPSItemButton : InfoButton<LockedPRPSItemButton, HodlerItem>
{

    public Text purposeAmountText,
                dubiAmountText,
                lockPeriodText,
                timeLeftText;

    public Button releasePurposeButton;

    private const int MIN_PERCENTAGE_TIME_RINKEBY = 3600; // For use with the rinkeby hodler where the lock period minimum is 1 hour.
    private const int MIN_PERCENTAGE_TIME_MAINNET = 7884000; // For use with the mainnet hodler where the lock period minimum is 3 months.

    protected override void OnValueUpdated(HodlerItem value)
    {
        var correctedPrpsAmount = SolidityUtils.ConvertFromUInt(value.Value, 18);


        purposeAmountText.text = correctedPrpsAmount.ToString();
        dubiAmountText.text = ((correctedPrpsAmount / 300)).ToString();
        lockPeriodText.text = DateTimeUtils.GetMaxTimeInterval((int)(value.ReleaseTime - value.LockedTimeStamp));
    }

    

}
