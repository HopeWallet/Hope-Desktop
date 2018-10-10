using Nethereum.Hex.HexTypes;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which displays the popup for sending a TradableAsset.
/// </summary>
public sealed partial class SendAssetPopup : OkCancelPopupComponent<SendAssetPopup>, ITabButtonObservable, IEnterButtonObservable
{
	public event Action<bool> AnimateAdvancedMode;
	public Action contactsClosed;

	[SerializeField]
	private HopeInputField addressField,
											amountField,
											gasLimitField,
											gasPriceField;

	[SerializeField]
	private TMP_Text assetBalance,
									  assetSymbol,
									  transactionFee,
									  contactName,
									  currencyText,
									  oppositeCurrencyAmountText;

	[SerializeField] private GameObject maxText;

	[SerializeField] private Toggle advancedModeToggle, maxToggle;

	[SerializeField] private Image assetImage;
	[SerializeField] private Slider transactionSpeedSlider;
	[SerializeField] private Button contactsButton, currencyButton;

	[SerializeField] private TooltipItem[] tooltipItems;

	private readonly List<Selectable> simpleModeSelectableFields = new List<Selectable>();
	private readonly List<Selectable> advancedModeSelectableFields = new List<Selectable>();

	private EthereumTransactionButtonManager ethereumTransactionButtonManager;
	private UserWalletManager userWalletManager;
	private DynamicDataCache dynamicDataCache;
	private ButtonClickObserver buttonClickObserver;

	private bool advancedMode;

	/// <summary>
	/// The <see cref="AssetManager"/> of this <see cref="SendAssetPopup"/>.
	/// </summary>
	public AssetManager Asset { get; private set; }

	/// <summary>
	/// The <see cref="AmountManager"/> of this <see cref="SendAssetPopup"/>.
	/// </summary>
	public AmountManager Amount { get; private set; }

	/// <summary>
	/// The <see cref="AddressManager"/> of this <see cref="SendAssetPopup"/>.
	/// </summary>
	public AddressManager Address { get; private set; }

	/// <summary>
	/// The <see cref="GasManager"/> of this <see cref="SendAssetPopup"/>.
	/// </summary>
	public GasManager Gas { get; private set; }

	/// <summary>
	/// The last selectable field of the SendAssetPopup based on whether the popup is in advanced mode or not.
	/// </summary>
	private Selectable LastSelectableField => advancedModeToggle.IsToggledOn ? gasPriceField.InputFieldBase : amountField.InputFieldBase;

	/// <summary>
	/// Adds the required dependencies to the SendAssetPopup.
	/// </summary>
	/// <param name="currencyManager"> The active CurrencyManager. </param>
	/// <param name="userWalletManager"> The active UserWalletManager. </param>
	/// <param name="tradableAssetManager"> The active TradableAssetManager. </param>
	/// <param name="tradableAssetPriceManager"> The active TradableAssetPriceManager. </param>
	/// <param name="ethereumTransactionButtonManager"> The active EthereumTransactionButtonManager. </param>
	/// <param name="etherBalanceObserver"> The active EtherBalanceObserver. </param>
	/// <param name="gasPriceObserver"> The active GasPriceObserver. </param>
	/// <param name="updateManager"> The active UpdateManager. </param>
	/// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
	/// <param name="periodicUpdateManager"> The active PeriodicUpdateManager. </param>
	/// <param name="contactsManager"> The active ContactsManager. </param>
	/// <param name="buttonClickObserver"> The active ButtonClickObserver. </param>
	/// <param name="restrictedAddressManager"> The active RestrictedAddressManager </param>
	[Inject]
	public void Construct(
		CurrencyManager currencyManager,
		UserWalletManager userWalletManager,
		TradableAssetManager tradableAssetManager,
		TradableAssetPriceManager tradableAssetPriceManager,
		EthereumTransactionButtonManager ethereumTransactionButtonManager,
		EtherBalanceObserver etherBalanceObserver,
		GasPriceObserver gasPriceObserver,
		UpdateManager updateManager,
		DynamicDataCache dynamicDataCache,
		PeriodicUpdateManager periodicUpdateManager,
		ContactsManager contactsManager,
		ButtonClickObserver buttonClickObserver,
		RestrictedAddressManager restrictedAddressManager)
	{
		this.ethereumTransactionButtonManager = ethereumTransactionButtonManager;
		this.userWalletManager = userWalletManager;
		this.dynamicDataCache = dynamicDataCache;
		this.buttonClickObserver = buttonClickObserver;

		Asset = new AssetManager(tradableAssetManager, etherBalanceObserver, updateManager, assetSymbol, assetBalance, assetImage);
		Gas = new GasManager(tradableAssetManager, tradableAssetPriceManager, currencyManager, gasPriceObserver, periodicUpdateManager, advancedModeToggle, transactionSpeedSlider, gasLimitField, gasPriceField, transactionFee);
		Address = new AddressManager(addressField, contactName, contactsManager, restrictedAddressManager);
		Amount = new AmountManager(currencyManager, tradableAssetPriceManager, maxToggle, maxText, amountField, currencyText, oppositeCurrencyAmountText, currencyButton, assetSymbol.text);

		Gas.SetupDependencies(Amount);
		Amount.SetupDependencies(Gas, Asset);

		simpleModeSelectableFields.Add(addressField.InputFieldBase);
		simpleModeSelectableFields.Add(amountField.InputFieldBase);

		advancedModeSelectableFields.AddRange(simpleModeSelectableFields);
		advancedModeSelectableFields.Add(gasLimitField.InputFieldBase);
		advancedModeSelectableFields.Add(gasPriceField.InputFieldBase);

		contactsClosed = () => contactsButton.interactable = true;
	}

	/// <summary>
	/// Sets up the contacts button and info message.
	/// </summary>
	protected override void OnStart()
	{
		advancedModeToggle.transform.GetComponent<Toggle>().AddToggleListener(AdvancedModeClicked);
		contactsButton.onClick.AddListener(() => { popupManager.GetPopup<ContactsPopup>(true).SetSendAssetPopup(this); contactsButton.interactable = false; });
		buttonClickObserver.SubscribeObservable(this);

		bool showTooltips = SecurePlayerPrefs.GetBool("show tooltips");

		foreach (TooltipItem tooltip in tooltipItems)
		{
			if (showTooltips)
				tooltip.PopupManager = popupManager;
			else if (tooltip.infoIcon)
				tooltip.gameObject.SetActive(false);
			else
				tooltip.enabled = false;
		}
	}

	/// <summary>
	/// Updates the send button interactability based on the GasManager, AddressManager, AmountManager IsValid properties.
	/// </summary>
	private void Update()
	{
		okButton.interactable = !Gas.Error && !addressField.Error && !amountField.Error;
	}

	/// <summary>
	/// Starts the asset transfer.
	/// </summary>
	public override void OkButton()
	{
		dynamicDataCache.SetData("txfee", Gas.TransactionFee.ToString());

		Asset.ActiveAsset.Transfer(
			userWalletManager,
			new HexBigInteger(Gas.TransactionGasLimit),
			Gas.TransactionGasPrice.FunctionalGasPrice,
			Address.SendAddress,
			Amount.SendableAmount);
	}

	/// <summary>
	/// Destroys the AssetManager and GasManager.
	/// </summary>
	private void OnDestroy()
	{
		Asset.Destroy();
		Gas.Destroy();

		buttonClickObserver.UnsubscribeObservable(this);
		TopBarButtons.popupClosed?.Invoke();

		ethereumTransactionButtonManager.Refresh();
	}

	/// <summary>
	/// Called when the advanced mode toggle is pressed.
	/// </summary>
	private void AdvancedModeClicked()
	{
		advancedMode = !advancedMode;

		AnimateAdvancedMode?.Invoke(advancedMode);
	}

	/// <summary>
	/// Moves to the next input field
	/// </summary>
	/// <param name="clickType"> The tab button ClickType </param>
	public void TabButtonPressed(ClickType clickType)
	{
		if (clickType != ClickType.Down)
			return;

		if (popupManager.ActivePopupType != typeof(SendAssetPopup))
			return;

		if (!advancedModeToggle.IsToggledOn)
			simpleModeSelectableFields.MoveToNextSelectable();
		else
			advancedModeSelectableFields.MoveToNextSelectable();
	}

	/// <summary>
	/// Clicks the send button if on the last input field 
	/// </summary>
	/// <param name="clickType"> The enter button ClickType </param>
	public void EnterButtonPressed(ClickType clickType)
	{
		if (clickType != ClickType.Down)
			return;

		if (popupManager.ActivePopupType != typeof(SendAssetPopup))
			return;

		if (InputFieldUtils.GetActiveInputField() == LastSelectableField && okButton.interactable)
			okButton.Press();
		else if (!advancedModeToggle.IsToggledOn)
			simpleModeSelectableFields.MoveToNextSelectable();
		else
			advancedModeSelectableFields.MoveToNextSelectable();
	}
}