using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public sealed class AddableTokenButton : InfoButton<AddableTokenButton, TokenInfo>
{
    [SerializeField] private TMP_Text tokenDisplayText;
    [SerializeField] private Image tokenIcon;

    private TradableAssetImageManager tradableAssetImageManager;

    [Inject]
    public void Construct(TradableAssetImageManager tradableAssetImageManager)
    {
        this.tradableAssetImageManager = tradableAssetImageManager;
    }

    protected override void OnValueUpdated(TokenInfo info)
    {
        tokenDisplayText.text = info.Name.LimitEnd(55, "...") + " (" + info.Symbol + ")";
        tradableAssetImageManager.LoadImage(info.Symbol, icon => tokenIcon.sprite = icon);
    }
}