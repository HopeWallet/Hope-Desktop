using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;

/// <summary>
/// Class which displays the information for a ReceiveAssetPopup.
/// </summary>
public class ReceiveAssetPopup : ExitablePopupComponent<ReceiveAssetPopup>
{

	[SerializeField] private Button copyAddressButton;

	[SerializeField] private Image qrImage;

	[SerializeField] private TextMeshProUGUI addressText;

	[SerializeField] private InfoMessage infoMessage;

	private TradableAssetManager tradableAssetManager;
    private TradableAssetImageManager tradableAssetImageManager;
    private UserWalletManager userWalletManager;

    /// <summary>
    /// Injects the dependencies for the ReceiveAssetPopup.
    /// </summary>
    /// <param name="tradableAssetManager"> The active TradableAssetManager. </param>
    /// <param name="tradableAssetImageManager"> The active TradableAssetImageManager. </param>
    [Inject]
    public void Construct(TradableAssetManager tradableAssetManager, 
        TradableAssetImageManager tradableAssetImageManager, 
        UserWalletManager userWalletManager)
    {
        this.tradableAssetManager = tradableAssetManager;
        this.tradableAssetImageManager = tradableAssetImageManager;
        this.userWalletManager = userWalletManager;
    }

    /// <summary>
    /// Initializes all components and assigns them with the values of the current TradableAsset.
    /// </summary>
    protected override void OnStart()
    {
        var walletAddress = userWalletManager.WalletAddress;
        var tradableAsset = tradableAssetManager.ActiveTradableAsset;
        var symbol = tradableAsset.AssetSymbol;

		addressText.text = walletAddress;
        qrImage.sprite = QRUtils.GenerateQRCode(walletAddress);

		infoMessage.PopupManager = popupManager;

		copyAddressButton.onClick.AddListener(CopyAddressClicked);
    }

    /// <summary>
    /// Copies the user's main wallet address to the clipboard.
    /// </summary>
    private void CopyAddressClicked() => ClipboardUtils.CopyToClipboard(userWalletManager.WalletAddress);

}
