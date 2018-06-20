using Hope.Utils.EthereumUtils;
using System.Numerics;
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
    private HodlerContract hodlerContract;
    private UserWalletManager userWalletManager;
    private TransactionHelper releasePurposeHelper;

    private BigInteger id;
    private decimal lockedPurpose;

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

    private void OnEnable()
    {
        releasePurposeButton.onClick.AddListener(ReleasePurpose);
    }

    private void OnDisable()
    {
        releasePurposeButton.onClick.RemoveAllListeners();
    }

    private void ReleasePurpose()
    {
        hodlerContract.Release(userWalletManager, releasePurposeHelper.GasLimit, releasePurposeHelper.GasPrice, id, lockedPurpose);
    }

    public void UpdateTransactionGas(TransactionHelper releasePurposeHelper)
    {
        this.releasePurposeHelper = releasePurposeHelper;

        releasePurposeButton.interactable = releasePurposeButton.interactable && releasePurposeHelper.CanExecuteTransaction;
    }

    protected override void OnValueUpdated(HodlerItem value)
    {
        lockedPurpose = SolidityUtils.ConvertFromUInt(value.Value, 18);
        id = value.Id;

        var minPercentageTime = networkSettings.networkType == EthereumNetworkManager.NetworkType.Mainnet ? MIN_PERCENTAGE_TIME_MAINNET : MIN_PERCENTAGE_TIME_RINKEBY;
        var releaseTimeDifference = value.ReleaseTime - value.LockedTimeStamp;
        var currentTimeDifference = value.ReleaseTime - DateTimeUtils.GetCurrentUnixTime();
        var multiplier = (decimal)releaseTimeDifference / minPercentageTime / 100;
        
        purposeAmountText.text = lockedPurpose.ToString().LimitEnd(8, "...");
        dubiAmountText.text = (multiplier * lockedPurpose).ToString().LimitEnd(15, "...");
        lockPeriodText.text = DateTimeUtils.GetMaxTimeInterval((int)releaseTimeDifference);
        timeLeftText.text = currentTimeDifference < 0 ? "Done" : DateTimeUtils.GetMaxTimeInterval((int)currentTimeDifference);
        releasePurposeButton.interactable = currentTimeDifference < 0;
    }

}
