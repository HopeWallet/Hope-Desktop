using TMPro;
using UnityEngine;
using Zenject;

/// <summary>
/// Class used for displaying the PRPS release amounts on the ConfirmReleasePopup
/// </summary>
public sealed class ConfirmReleasePopup : ConfirmTransactionPopupBase<ConfirmReleasePopup>
{
    [SerializeField] private TextMeshProUGUI questionText,
                                             currentPurposeBalanceText,
                                             releaseAmountText;

    private TradableAssetManager tradableAssetManager;

    /// <summary>
    /// Addres the TradableAssetManager dependency.
    /// </summary>
    /// <param name="tradableAssetManager"> The active TradableAssetManager. </param>
    [Inject]
    public void Construct(TradableAssetManager tradableAssetManager) => this.tradableAssetManager = tradableAssetManager;

    /// <summary>
    /// Displays the asset transfer request details.
    /// </summary>
    /// <param name="transactionInput"> The input of the send asset transaction request. </param>
    protected override void InternalSetConfirmationValues(object[] transactionInput)
    {
        questionText.text = $"Are you sure you would like to release {transactionInput[0]} Purpose?";
        releaseAmountText.text = $"+{transactionInput[0]}";
        currentPurposeBalanceText.text = $"{tradableAssetManager.ActiveTradableAsset.AssetBalance}";
    }
}
