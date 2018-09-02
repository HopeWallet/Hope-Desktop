﻿using Nethereum.Hex.HexTypes;
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

	[SerializeField] private HopeInputField addressField,
											amountField,
											gasLimitField,
											gasPriceField;

	[SerializeField] private TMP_Text assetBalance,
									  assetSymbol,
									  transactionFee,
									  contactName,
									  currencyText,
									  oppositeCurrencyAmountText;

	[SerializeField] private Toggle advancedModeToggle, maxToggle;

	[SerializeField] private Image assetImage;
	[SerializeField] private Slider transactionSpeedSlider;
	[SerializeField] private Button contactsButton, currencyButton;

	private bool advancedMode;

	private UserWalletManager userWalletManager;
	private DynamicDataCache dynamicDataCache;
	private ButtonClickObserver buttonClickObserver;

	private readonly List<Selectable> selectableFields = new List<Selectable>();
	private InputField lastSelectableField;

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
	/// Adds the required dependencies to the SendAssetPopup.
	/// </summary>
	/// <param name="userWalletManager"> The active UserWalletManager. </param>
	/// <param name="tradableAssetManager"> The active TradableAssetManager. </param>
	/// <param name="tradableAssetImageManager"> The active TradableAssetImageManager. </param>
	/// <param name="etherBalanceObserver"> The active EtherBalanceObserver. </param>
	/// <param name="gasPriceObserver"> The active GasPriceObserver. </param>
	/// <param name="updateManager"> The active UpdateManager. </param>
	/// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
	/// <param name="periodicUpdateManager"> The active PeriodicUpdateManager. </param>
	/// <param name="contactsManager"> The active ContactsManager. </param>
	/// <param name="buttonClickObserver"> The active ButtonClickObserver. </param>
	[Inject]
	public void Construct(
		UserWalletManager userWalletManager,
		TradableAssetManager tradableAssetManager,
		TradableAssetImageManager tradableAssetImageManager,
		EtherBalanceObserver etherBalanceObserver,
		GasPriceObserver gasPriceObserver,
		UpdateManager updateManager,
		DynamicDataCache dynamicDataCache,
		PeriodicUpdateManager periodicUpdateManager,
		ContactsManager contactsManager,
		ButtonClickObserver buttonClickObserver)
	{
		this.userWalletManager = userWalletManager;
		this.dynamicDataCache = dynamicDataCache;
		this.buttonClickObserver = buttonClickObserver;

		Asset = new AssetManager(tradableAssetManager, tradableAssetImageManager, etherBalanceObserver, updateManager, assetSymbol, assetBalance, assetImage);
		Gas = new GasManager(tradableAssetManager, gasPriceObserver, periodicUpdateManager, advancedModeToggle, transactionSpeedSlider, gasLimitField, gasPriceField, transactionFee);
		Address = new AddressManager(addressField, contactName, contactsManager);
		Amount = new AmountManager(maxToggle, amountField, currencyText, oppositeCurrencyAmountText, currencyButton, assetSymbol.text);

        Gas.SetupDependencies(Amount);
        Amount.SetupDependencies(Gas, Asset);

		selectableFields.Add(addressField.InputFieldBase);
		selectableFields.Add(amountField.InputFieldBase);
		selectableFields.Add(gasLimitField.InputFieldBase);
		selectableFields.Add(gasPriceField.InputFieldBase);

		lastSelectableField = amountField.InputFieldBase;

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
	}

	/// <summary>
	/// Updates the send button interactability based on the GasManager, AddressManager, AmountManager IsValid properties.
	/// </summary>
	private void Update() => okButton.interactable = !Gas.Error && !addressField.Error && !amountField.Error;

	/// <summary>
	/// Starts the asset transfer.
	/// </summary>
	public override void OkButton()
    {
        dynamicDataCache.SetData("txfee", Gas.TransactionFee.ToString());
        userWalletManager.TransferAsset(Asset.ActiveAsset,
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
    }

	private void AdvancedModeClicked()
	{
		advancedMode = !advancedMode;

		lastSelectableField = advancedMode ? gasPriceField.InputFieldBase : amountField.InputFieldBase;
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

		selectableFields.MoveToNextSelectable();
	}

	/// <summary>
	/// Clicks the send button if on the last input field 
	/// </summary>
	/// <param name="clickType"> The enter button ClickType </param>
	public void EnterButtonPressed(ClickType clickType)
	{
		if (clickType != ClickType.Down)
			return;

		if (InputFieldUtils.GetActiveInputField() == lastSelectableField && okButton.interactable)
			okButton.Press();
		else
			selectableFields.MoveToNextSelectable();
	}
}