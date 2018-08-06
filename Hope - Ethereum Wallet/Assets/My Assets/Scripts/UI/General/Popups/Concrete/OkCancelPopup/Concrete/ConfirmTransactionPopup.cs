using System;
using TMPro;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which is used as a monobehaviour component for the SendAssetPopupConfirmation.
/// </summary>
public sealed class ConfirmTransactionPopup : ConfirmTransactionPopupBase<ConfirmTransactionPopup>
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
    private DynamicDataCache dynamicDataCache;

    /// <summary>
    /// Adds the required dependencies to this popup.
    /// </summary>
    /// <param name="tradableAssetManager"> The active TradableAssetManager. </param>
    /// <param name="tradableAssetImageManager"> The active TradableAssetImageManager. </param>
    /// <param name="userWalletManager"> The active UserWalletManager. </param>
    /// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
    [Inject]
    public void Construct(
        TradableAssetManager tradableAssetManager,
        TradableAssetImageManager tradableAssetImageManager,
        UserWalletManager userWalletManager,
        DynamicDataCache dynamicDataCache)
    {
        this.tradableAssetManager = tradableAssetManager;
        this.tradableAssetImageManager = tradableAssetImageManager;
        this.userWalletManager = userWalletManager;
        this.dynamicDataCache = dynamicDataCache;
    }

    /// <summary>
    /// Displays the asset transfer request details.
    /// </summary>
    /// <param name="transactionInput"> The input of the send asset transaction request. </param>
    protected override void InternalSetConfirmationValues(object[] transactionInput)
    {
        tradableAssetImageManager.LoadImage(tradableAssetManager.GetTradableAsset(transactionInput[1].ToString()).AssetSymbol, img => assetImage.sprite = img);
        amountText.text = transactionInput[2].ToString().LimitEnd(20 - transactionInput[3].ToString().Length, "...") + " " + transactionInput[3];
        toAddress.text = transactionInput[0].ToString();
        fromAddress.text = userWalletManager.WalletAddress;
        feeText.text = dynamicDataCache.GetData("txfee") + " ETH";
		CheckIfSendingToContact();
		//Wallet name
    }

	/// <summary>
	/// Checks if sending a transaction to a saved contact
	/// </summary>
	private void CheckIfSendingToContact()
	{
		if (SecurePlayerPrefs.HasKey(toAddress.text))
			contactName.text = "[ " + SecurePlayerPrefs.GetString(toAddress.text) + " ]";

		else
			contactName.text = "";

		contactName.gameObject.SetActive(string.IsNullOrEmpty(contactName.text) ? false : true);
	}
}
