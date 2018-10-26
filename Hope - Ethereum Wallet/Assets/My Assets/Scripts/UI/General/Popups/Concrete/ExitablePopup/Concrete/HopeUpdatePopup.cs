using TMPro;
using UnityEngine;
using Zenject;

public sealed class HopeUpdatePopup : OkCancelPopupComponent<HopeUpdatePopup>
{
    [SerializeField] private TMP_Text updateAvailableText;

    private WalletVersionManager walletVersionManager;

    [Inject]
    public void Construct(WalletVersionManager walletVersionManager)
    {
        this.walletVersionManager = walletVersionManager;
    }

    protected override void OnStart()
    {
        updateAvailableText.text = $"New Hope update available! (v{walletVersionManager.LatestWalletVersion})";
    }

    protected override void OnCancelClicked()
    {
        if (popupManager.ActivePopupType == typeof(HopeUpdatePopup))
            popupManager.CloseActivePopup();
    }

    protected override void OnOkClicked()
    {
        Application.OpenURL(walletVersionManager.LatestWalletVersionUrl);
    }
}
