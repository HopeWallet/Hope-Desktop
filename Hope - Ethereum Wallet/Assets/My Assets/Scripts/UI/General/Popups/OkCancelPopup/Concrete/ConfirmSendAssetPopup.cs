using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which is used as a monobehaviour component for the SendAssetPopupConfirmation.
/// </summary>
public class ConfirmSendAssetPopup : ConfirmTransactionRequestPopup<ConfirmSendAssetPopup>
{

    public Image assetImage;

    public Text amountText,
                addressText;

    private TradableAssetManager tradableAssetManager;
    private TradableAssetImageManager tradableAssetImageManager;

    /// <summary>
    /// Adds the required dependencies to this popup.
    /// </summary>
    /// <param name="tradableAssetManager"> The active TradableAssetManager. </param>
    /// <param name="tradableAssetImageManager"> The active TradableAssetImageManager. </param>
    [Inject]
    public void Construct(TradableAssetManager tradableAssetManager, TradableAssetImageManager tradableAssetImageManager)
    {
        this.tradableAssetManager = tradableAssetManager;
        this.tradableAssetImageManager = tradableAssetImageManager;
    }

    protected override void InternalSetConfirmationValues(object[] transactionInput)
    {
        tradableAssetImageManager.LoadImage(tradableAssetManager.GetTradableAsset(transactionInput[1].ToString()).AssetSymbol, img => assetImage.sprite = img);
        amountText.text = transactionInput[2].ToString();
        addressText.text = transactionInput[0].ToString();
    }
}
