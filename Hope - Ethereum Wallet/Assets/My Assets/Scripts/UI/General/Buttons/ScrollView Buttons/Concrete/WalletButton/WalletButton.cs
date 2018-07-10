using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Zenject;

public sealed class WalletButton : InfoButton<WalletButton, WalletInfo>
{

    public TMP_Text walletNameText;

    private PopupManager popupManager;
    private DynamicDataCache dynamicDataCache;

    [Inject]
    public void Construct(PopupManager popupManager, DynamicDataCache dynamicDataCache)
    {
        this.popupManager = popupManager;
        this.dynamicDataCache = dynamicDataCache;
    }

    protected override void OnAwake()
    {
        Button.onClick.AddListener(WalletButtonClicked);
    }

    protected override void OnValueUpdated(WalletInfo info)
    {
        walletNameText.text = info.WalletName?.LimitEnd(14, "...");
    }

    private void WalletButtonClicked()
    {
        dynamicDataCache.SetData("walletnum", ButtonInfo.WalletNum);
        popupManager.GetPopup<UnlockWalletPopup>();
    }
}