using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which is used as a monobehaviour component for the SendAssetPopupConfirmation.
/// </summary>
public sealed class ConfirmTransactionPopup : ConfirmTransactionPopupBase<ConfirmTransactionPopup>
{
    [SerializeField] private Image assetImage;

    [SerializeField] private TextMeshProUGUI amountText,
											 fromAddress,
											 toAddress,
											 walletName,
											 contactName,
											 feeText;

	[SerializeField] private GameObject confirmText;

    private TradableAssetManager tradableAssetManager;
    private TradableAssetImageManager tradableAssetImageManager;
    private UserWalletManager userWalletManager;
    private HopeWalletInfoManager userWalletInfoManager;
    private DynamicDataCache dynamicDataCache;
	private ContactsManager contactsManager;

	private string walletAddress;

	/// <summary>
	/// Adds the required dependencies to this popup.
	/// </summary>
	/// <param name="tradableAssetManager"> The active TradableAssetManager. </param>
	/// <param name="tradableAssetImageManager"> The active TradableAssetImageManager. </param>
	/// <param name="userWalletManager"> The active UserWalletManager. </param>
	/// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
	/// <param name="contactsManager"> The active ContactsManager. </param>
	[Inject]
	public void Construct(
		TradableAssetManager tradableAssetManager,
		TradableAssetImageManager tradableAssetImageManager,
		UserWalletManager userWalletManager,
		HopeWalletInfoManager userWalletInfoManager,
		DynamicDataCache dynamicDataCache,
		ContactsManager contactsManager)
	{
		this.tradableAssetManager = tradableAssetManager;
		this.tradableAssetImageManager = tradableAssetImageManager;
		this.userWalletManager = userWalletManager;
		this.userWalletInfoManager = userWalletInfoManager;
		this.dynamicDataCache = dynamicDataCache;
		this.contactsManager = contactsManager;

		walletAddress = userWalletManager.GetWalletAddress();

		if (userWalletManager.ActiveWalletType == UserWalletManager.WalletType.Ledger)
		{
			confirmText.SetActive(true);
			confirmText.GetComponent<TextMeshProUGUI>().text = "Confirm on your Ledger.";
		}
		else if (userWalletManager.ActiveWalletType == UserWalletManager.WalletType.Ledger)
		{
			confirmText.SetActive(true);
			confirmText.GetComponent<TextMeshProUGUI>().text = "Confirm on your Trezor.";
		}
		else
		{
			okButton.gameObject.SetActive(true);
		}
	}

    /// <summary>
    /// Displays the asset transfer request details.
    /// </summary>
    /// <param name="transactionInput"> The input of the send asset transaction request. </param>
    protected override void InternalSetConfirmationValues(object[] transactionInput)
    {
        tradableAssetImageManager.LoadImage(tradableAssetManager.GetTradableAsset(transactionInput[1].ToString()).AssetSymbol, img => assetImage.sprite = img);
        amountText.text = transactionInput[2].ToString().LimitEnd(20 - transactionInput[3].ToString().Length, "...") + " <style=Symbol>" + transactionInput[3] + "</style>";

        toAddress.text = transactionInput[0].ToString();
		CheckIfSavedContact(toAddress.text, contactName);

		fromAddress.text = userWalletManager.GetWalletAddress();
		CheckIfSavedContact(fromAddress.text, walletName);

		feeText.text = dynamicDataCache.GetData("txfee") + "<style=Symbol> ETH</style>";
	}

    /// <summary>
    /// Checks if this address is saved under a contact name
    /// </summary>
    /// <param name="address"> The potential contact address. </param>
    /// <param name="textObject"> The text object. </param>
    private void CheckIfSavedContact(string address, TMP_Text textObject)
	{
		address = address.ToLower();

		if (contactsManager.ContactList.Contains(address))
			textObject.text = "<style=Contact>" + contactsManager.ContactList[address].ContactName + "</style>";
		else if (address.EqualsIgnoreCase(walletAddress))
			textObject.text = "<style=Contact>" + walletName + "</style>";
		else
			textObject.text = string.Empty;

		contactName.gameObject.SetActive(!string.IsNullOrEmpty(contactName.text));
	}
}
