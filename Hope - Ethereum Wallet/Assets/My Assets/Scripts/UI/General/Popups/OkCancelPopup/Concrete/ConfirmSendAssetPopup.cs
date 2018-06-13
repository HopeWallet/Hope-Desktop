using Nethereum.Hex.HexTypes;
using System;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which is used as a monobehaviour component for the SendAssetPopupConfirmation.
/// </summary>
public class ConfirmSendAssetPopup : OkCancelPopupComponent<ConfirmSendAssetPopup>
{

    public Image assetImage;

    public Text amountText,
                addressText,
                timerText;

    private Action confirmPressed;

    private HexBigInteger gasLimit,
                          gasPrice;

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

    /// <summary>
    /// Sets the values of the current send asset transaction.
    /// </summary>
    /// <param name="receivingAddress"> The address receiving the asset. </param>
    /// <param name="assetAddress"> The address of the asset being sent. </param>
    /// <param name="transferAmount"> The amount of the asset to send. </param>
    /// <param name="price"> The gas price of the transaction. </param>
    /// <param name="limit"> The gas limit of the transaction. </param>
    /// <param name="onConfirmPressed"> Action called when the confirm button is pressed. </param>
    public void SetSendAssetValues(string receivingAddress, string assetAddress, decimal transferAmount, 
        HexBigInteger price, HexBigInteger limit, Action onConfirmPressed)
    {
        tradableAssetImageManager.LoadImage(tradableAssetManager.GetTradableAsset(assetAddress).AssetSymbol, img => assetImage.sprite = img);
        amountText.text = transferAmount + "";
        addressText.text = receivingAddress;
        gasLimit = limit;
        gasPrice = price;
        confirmPressed = onConfirmPressed;
    }

    /// <summary>
    /// Initializes the component by getting all required components used to display the transaction info.
    /// </summary>
    protected override void OnStart()
    {
        new CountdownTimer(time => timerText.text = time + "", () => { okButton.interactable = true; timerText.text = ""; }, 5f, 1f).StartCountdown();
    }

    /// <summary>
    /// Called when the confirm button is clicked, which executes the transfer of the asset.
    /// </summary>
    protected override void OnOkClicked() => confirmPressed?.Invoke();

}
