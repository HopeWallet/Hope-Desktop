using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public sealed class AddableTokenButton : InfoButton<AddableTokenButton, TokenInfo>
{
    [SerializeField] private TMP_Text tokenDisplayText;
    [SerializeField] private TMP_Text tokenBalanceText;
    [SerializeField] private Image tokenIcon;

    private PopupManager popupManager;
    private TradableAssetImageManager tradableAssetImageManager;
    private UserWalletManager userWalletManager;

    private TokenInfo previousTokenInfo;

    [Inject]
    public void Construct(TradableAssetImageManager tradableAssetImageManager, PopupManager popupManager, UserWalletManager userWalletManager)
    {
        this.tradableAssetImageManager = tradableAssetImageManager;
        this.popupManager = popupManager;
        this.userWalletManager = userWalletManager;
    }

    private void Start()
    {
        Button.onClick.AddListener(() => Toggle(true));
    }

    public void Toggle(bool onClick = false)
    {
        var addTokenPopup = popupManager.GetPopup<AddTokenPopup>();

        if (addTokenPopup.ActivelySelectedButton == this && onClick)
            return;

        if (addTokenPopup.ActivelySelectedButton != null && addTokenPopup.ActivelySelectedButton != this)
            addTokenPopup.ActivelySelectedButton.Toggle();

        Button.interactable = !Button.interactable;
        addTokenPopup.ActivelySelectedButton = Button.interactable ? null : this;
    }

    protected override void OnValueUpdated(TokenInfo info)
    {
        if (info.Address.EqualsIgnoreCase(previousTokenInfo?.Address))
            return;

        previousTokenInfo = info;
        tokenBalanceText.text = "-";
        tokenDisplayText.text = info.Name.LimitEnd(55, "...") + " (" + info.Symbol + ")";
        tradableAssetImageManager.LoadImage(info.Symbol, icon => tokenIcon.sprite = icon);

        SimpleContractQueries.QueryUInt256Output<ERC20.Queries.BalanceOf>(info.Address, userWalletManager.GetWalletAddress(), userWalletManager.GetWalletAddress())
                             .OnSuccess(balance => tokenBalanceText.text = $"{balance.Value}".LimitEnd(5, "..."));
    }
}