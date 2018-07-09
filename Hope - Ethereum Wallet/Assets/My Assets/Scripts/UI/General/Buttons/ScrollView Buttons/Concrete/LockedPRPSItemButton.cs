using Hope.Utils.EthereumUtils;
using System.Numerics;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which manages each locked purpose item that is displayed.
/// </summary>
public class LockedPRPSItemButton : InfoButton<LockedPRPSItemButton, HodlerItem>
{

    public Text purposeAmountText,
                dubiAmountText,
                lockPeriodText,
                timeLeftText;

    public Button releasePurposeButton;

    private EthereumNetworkManager.Settings networkSettings;
    private HodlerContract hodlerContract;
    private UserWalletManager userWalletManager;
    private FunctionGasEstimator releasePurposeHelper;

    private BigInteger id;
    private decimal lockedPurpose;
    private bool lockPeriodDone;

    private const int MIN_PERCENTAGE_TIME_RINKEBY = 3600; // For use with the rinkeby hodler where the lock period minimum is 1 hour.
    private const int MIN_PERCENTAGE_TIME_MAINNET = 7884000; // For use with the mainnet hodler where the lock period minimum is 3 months.

    /// <summary>
    /// Determines the dubi percentage given the active ethereum network.
    /// </summary>
    /// <param name="networkSettings"> The active ethereum network settings. </param>
    /// <param name="hodlerContract"> The active HodlerContract. </param>
    /// <param name="userWalletManager"> The active UserWalletManager. </param>
    [Inject]
    public void DetermineDUBIPercentage(EthereumNetworkManager.Settings networkSettings, HodlerContract hodlerContract, UserWalletManager userWalletManager)
    {
        this.networkSettings = networkSettings;
        this.hodlerContract = hodlerContract;
        this.userWalletManager = userWalletManager;
    }

    /// <summary>
    /// Adds the release purpose method to the button listener.
    /// </summary>
    private void OnEnable() => releasePurposeButton.onClick.AddListener(ReleasePurpose);

    /// <summary>
    /// Releases the purpose held in the item represented by this button.
    /// </summary>
    private void ReleasePurpose()
    {
        hodlerContract.Release(userWalletManager, releasePurposeHelper.GasLimit, releasePurposeHelper.StandardGasPrice.FunctionalGasPrice, id, lockedPurpose);
    }

    /// <summary>
    /// Updates the details of the transaction.
    /// </summary>
    /// <param name="releasePurposeHelper"> The helper which is responsible for knowing the transaction limit and price of the hodler release function. </param>
    public void UpdateTransactionDetails(FunctionGasEstimator releasePurposeHelper)
    {
        this.releasePurposeHelper = releasePurposeHelper;

        if (releasePurposeButton == null)
            return;

        releasePurposeButton.interactable = lockPeriodDone && releasePurposeHelper.CanExecuteTransaction;
    }

    /// <summary>
    /// Updates the ui elements and the transaction info whenever the HodlerItem is changed/updated.
    /// </summary>
    /// <param name="info"> The item that holds the info on the purpose locked in the contract. </param>
    protected override void OnValueUpdated(HodlerItem info)
    {
        lockedPurpose = SolidityUtils.ConvertFromUInt(info.Value, 18);
        id = info.Id;

        var minPercentageTime = networkSettings.networkType == EthereumNetworkManager.NetworkType.Mainnet ? MIN_PERCENTAGE_TIME_MAINNET : MIN_PERCENTAGE_TIME_RINKEBY;
        var releaseTimeDifference = info.ReleaseTime - info.LockedTimeStamp;
        var currentTimeDifference = info.ReleaseTime - DateTimeUtils.GetCurrentUnixTime();
        var multiplier = (decimal)releaseTimeDifference / minPercentageTime / 100;

        lockPeriodDone = currentTimeDifference < 0;
        purposeAmountText.text = lockedPurpose.ToString().LimitEnd(8, "...");
        dubiAmountText.text = (multiplier * lockedPurpose).ToString().LimitEnd(15, "...");
        lockPeriodText.text = DateTimeUtils.GetMaxTimeInterval((int)releaseTimeDifference);
        timeLeftText.text = currentTimeDifference < 0 ? "Done" : DateTimeUtils.GetMaxTimeInterval((int)currentTimeDifference);
    }

}
