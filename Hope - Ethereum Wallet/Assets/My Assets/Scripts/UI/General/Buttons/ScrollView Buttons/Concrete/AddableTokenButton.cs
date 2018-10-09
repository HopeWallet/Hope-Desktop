using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public sealed class AddableTokenButton : InfoButton<AddableTokenButton, AddableTokenInfo>
{
    [SerializeField] private TMP_Text tokenDisplayText;
    [SerializeField] private Image tokenIcon;
    [SerializeField] private CheckBox checkBox;

    private TokenListManager tokenListManager;
    private TradableAssetImageManager tradableAssetImageManager;

    [Inject]
    public void Construct(TokenListManager tokenListManager, TradableAssetImageManager tradableAssetImageManager)
    {
        this.tokenListManager = tokenListManager;
        this.tradableAssetImageManager = tradableAssetImageManager;

        checkBox.OnCheckboxClicked += OnCheckboxChanged;
    }

    protected override void OnValueUpdated(AddableTokenInfo info)
    {
        tokenDisplayText.text = info.TokenInfo.Name.LimitEnd(55, "...") + " (" + info.TokenInfo.Symbol + ")";
        tradableAssetImageManager.LoadImage(info.TokenInfo.Symbol, icon => tokenIcon.sprite = icon);
        checkBox.SetValue(info.Enabled);
    }

    private void OnCheckboxChanged(bool enabled)
    {
        tokenListManager.UpdateToken(ButtonInfo.TokenInfo.Address, enabled, true);
        ButtonInfo.Enabled = enabled;
    }
}