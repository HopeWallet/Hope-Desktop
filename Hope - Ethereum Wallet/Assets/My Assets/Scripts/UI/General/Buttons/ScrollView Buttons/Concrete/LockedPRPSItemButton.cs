using Hope.Utils.EthereumUtils;
using Nethereum.Hex.HexTypes;
using System.Numerics;
using TMPro;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which manages each locked purpose item that is displayed.
/// </summary>
public sealed class LockedPRPSItemButton : InfoButton<LockedPRPSItemButton, HodlerMimic.Output.Item>
{
    public TMP_Text purposeAmountText,
                    dubiAmountText,
                    lockPeriodText,
                    timeLeftText;

    public Button releasePurposeButton;

    private EthereumNetworkManager.Settings networkSettings;
    private HodlerContract hodlerContract;
    private UserWalletManager userWalletManager;
    private HodlerMimic.Output.Item item;

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
    public void DetermineDUBIPercentage(
        EthereumNetworkManager.Settings networkSettings,
        HodlerContract hodlerContract,
        UserWalletManager userWalletManager)
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
        hodlerContract.Release(userWalletManager, new HexBigInteger(item.UnlockableGasLimit.Value), new HexBigInteger(0), id, lockedPurpose);
    }

    /// <summary>
    /// Updates the ui elements and the transaction info whenever the HodlerItem is changed/updated.
    /// </summary>
    /// <param name="info"> The item that holds the info on the purpose locked in the contract. </param>
    protected override void OnValueUpdated(HodlerMimic.Output.Item info)
    {
        lockedPurpose = SolidityUtils.ConvertFromUInt(info.Value, 18);
        item = info;
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