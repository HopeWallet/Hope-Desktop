using TMPro;
using UnityEngine.UI;

public sealed class SendAssetPopupAssetManager
{

    private readonly TradableAssetManager tradableAssetManager;
    private readonly TradableAssetImageManager tradableAssetImageManager;

    private readonly TMP_Text assetBalance,
                              assetSymbol;

    private readonly Image assetImage;

    public SendAssetPopupAssetManager(
        TradableAssetManager tradableAssetManager,
        TradableAssetImageManager tradableAssetImageManager,
        TMP_Text assetSymbol,
        TMP_Text assetBalance,
        Image assetImage)
    {
        this.tradableAssetManager = tradableAssetManager;
        this.tradableAssetImageManager = tradableAssetImageManager;
        this.assetSymbol = assetSymbol;
        this.assetBalance = assetBalance;
        this.assetImage = assetImage;

        SetValues();
    }

    private void SetValues()
    {
        var activeAsset = tradableAssetManager.ActiveTradableAsset;

        assetBalance.text = StringUtils.LimitEnd(activeAsset.AssetBalance.ToString(), 14, "...");
        assetSymbol.text = activeAsset.AssetSymbol;
        tradableAssetImageManager.LoadImage(activeAsset.AssetSymbol, img => assetImage.sprite = img);
    }

}