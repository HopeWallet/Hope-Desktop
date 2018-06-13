using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which is used to confirm the hiding of an asset.
/// </summary>
public class ConfirmHideAssetPopup : OkCancelPopupComponent<ConfirmHideAssetPopup>
{

    public Image assetImage;
    public Text assetSymbol;

    private TradableAssetImageManager tradableAssetImageManager;
    private TokenContractManager tokenContractManager;

    /// <summary>
    /// The asset that will be removed.
    /// </summary>
    public TradableAsset AssetToRemove { get; set; }

    /// <summary>
    /// Adds the TradableAssetImageManager and TokenContractManager dependencies.
    /// </summary>
    /// <param name="tradableAssetImageManager"> The active TradableAssetImageManager. </param>
    /// <param name="tokenContractManager"> The active TokenContractManager. </param>
    [Inject]
    public void Construct(TradableAssetImageManager tradableAssetImageManager, TokenContractManager tokenContractManager)
    {
        this.tradableAssetImageManager = tradableAssetImageManager;
        this.tokenContractManager = tokenContractManager;
    }

    /// <summary>
    /// Initializes the popup by setting the text and image to the symbol and image of the asset to remove.
    /// </summary>
    protected override void OnStart()
    {
        assetSymbol.text = AssetToRemove.AssetSymbol;
        tradableAssetImageManager.LoadImage(AssetToRemove.AssetSymbol, image => assetImage.sprite = image);
    }

    /// <summary>
    /// Removes the tradable asset.
    /// </summary>
    protected override void OnOkClicked() => tokenContractManager.RemoveToken(AssetToRemove.AssetAddress);

}
