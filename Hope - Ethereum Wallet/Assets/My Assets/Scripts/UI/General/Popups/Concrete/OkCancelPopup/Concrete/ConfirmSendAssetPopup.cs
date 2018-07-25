using TMPro;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which is used as a monobehaviour component for the SendAssetPopupConfirmation.
/// </summary>
public sealed class ConfirmSendAssetPopup : ConfirmTransactionRequestPopup<ConfirmSendAssetPopup>
{
    public Image assetImage;

    public TMP_Text amountText,
                    fromAddress,
                    toAddress,
                    walletName,
                    contactName,
                    feeText;

    private TradableAssetManager tradableAssetManager;
    private TradableAssetImageManager tradableAssetImageManager;
    private UserWalletManager userWalletManager;

    /// <summary>
    /// Adds the required dependencies to this popup.
    /// </summary>
    /// <param name="tradableAssetManager"> The active TradableAssetManager. </param>
    /// <param name="tradableAssetImageManager"> The active TradableAssetImageManager. </param>
    /// <param name="userWalletManager"> The active UserWalletManager. </param>
    [Inject]
    public void Construct(
        TradableAssetManager tradableAssetManager,
        TradableAssetImageManager tradableAssetImageManager,
        UserWalletManager userWalletManager)
    {
        this.tradableAssetManager = tradableAssetManager;
        this.tradableAssetImageManager = tradableAssetImageManager;
        this.userWalletManager = userWalletManager;
    }

    /// <summary>
    /// Displays the asset transfer request details.
    /// </summary>
    /// <param name="transactionInput"> The input of the send asset transaction request. </param>
    protected override void InternalSetConfirmationValues(object[] transactionInput)
    {
        tradableAssetImageManager.LoadImage(tradableAssetManager.GetTradableAsset(transactionInput[1].ToString()).AssetSymbol, img => assetImage.sprite = img);
        amountText.text = transactionInput[2].ToString();
        toAddress.text = transactionInput[0].ToString();
        fromAddress.text = userWalletManager.WalletAddress;
        // walletName
        // contactName (empty string if nothing)
        // feeText
    }
}
