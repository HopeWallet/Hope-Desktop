using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>, ITabButtonObservable, IEnterButtonObservable
{
	[SerializeField] private GameObject defaultCurrencyDropdown;
	[SerializeField] private CheckBox idleTimeoutTimeCheckbox, countdownTimerCheckbox, transactionNotificationCheckbox, updateNotificationCheckbox;
	[SerializeField] private HopeInputField idleTimeoutTimeInputField;

	[SerializeField] private Button walletCategoryButton;
	[SerializeField] private HopeInputField walletNameField, password1Field, password2Field;
	[SerializeField] private Button saveButton, deleteButton;

	[SerializeField] private GeneralRadioButtons addressOptions;
	[SerializeField] private Transform addressListTransform;
	[SerializeField] private Button unlockButton;

	[SerializeField] private CheckBox twoFactorAuthenticationCheckbox;
	[SerializeField] private GameObject setUpSection;
	[SerializeField] private TextMeshProUGUI keyText;
	[SerializeField] private Image qrCodeImage;
	[SerializeField] private HopeInputField codeInputField;
	[SerializeField] private Button confirmButton;

	[SerializeField] private TextMeshProUGUI currentVersionText, latestVersionText;
	[SerializeField] private Button downloadUpdateButton;

	private List<Selectable> inputFields = new List<Selectable>();

	private GeneralSection general;
	private WalletSection wallet;
	private AddressSection address;
	private TwoFactorAuthenticationSection twoFactorAuthenticationSection;
	private HopeVersion hopeVersion;

	private UserWalletManager userWalletManager;
	private HopeWalletInfoManager hopeWalletInfoManager;
	private ButtonClickObserver buttonClickObserver;

	[Inject]
	public void Construct(UserWalletManager userWalletManager,
						  HopeWalletInfoManager hopeWalletInfoManager,
						  ButtonClickObserver buttonClickObserver)
	{
		this.userWalletManager = userWalletManager;
		this.hopeWalletInfoManager = hopeWalletInfoManager;
		this.buttonClickObserver = buttonClickObserver;
	}

	protected override void OnStart()
	{
		base.OnStart();

		inputFields.Add(walletNameField.InputFieldBase);
		inputFields.Add(password1Field.InputFieldBase);
		inputFields.Add(password2Field.InputFieldBase);

		buttonClickObserver.SubscribeObservable(this);

		general = new GeneralSection(defaultCurrencyDropdown, idleTimeoutTimeCheckbox, countdownTimerCheckbox, transactionNotificationCheckbox, updateNotificationCheckbox, idleTimeoutTimeInputField);
		address = new AddressSection(addressOptions, addressListTransform, unlockButton);
		twoFactorAuthenticationSection = new TwoFactorAuthenticationSection(twoFactorAuthenticationCheckbox, setUpSection, keyText, qrCodeImage, codeInputField, confirmButton);
		hopeVersion = new HopeVersion(currentVersionText, latestVersionText, downloadUpdateButton);

		if (userWalletManager.ActiveWalletType == UserWalletManager.WalletType.Hope)
			wallet = new WalletSection(hopeWalletInfoManager, userWalletManager, walletNameField, password1Field, password2Field, saveButton, deleteButton);
		else
		{
			walletCategoryButton.interactable = false;
			walletCategoryButton.GetComponent<TextMeshProUGUI>().color = UIColors.LightGrey;
		}
	}

	private void OnDestroy()
	{
		buttonClickObserver.UnsubscribeObservable(this);
		MoreDropdown.PopupClosed?.Invoke();

		//Set default currency dropdown
		SecurePlayerPrefs.SetBool("idle timeout", idleTimeoutTimeCheckbox.ToggledOn);
		SecurePlayerPrefs.SetBool("countdown timer", countdownTimerCheckbox.ToggledOn);
		SecurePlayerPrefs.SetBool("transaction notification", transactionNotificationCheckbox.ToggledOn);
		SecurePlayerPrefs.SetBool("update notification", updateNotificationCheckbox.ToggledOn);
		SecurePlayerPrefs.SetBool("two-factor authentication", twoFactorAuthenticationCheckbox.ToggledOn);

		if (idleTimeoutTimeCheckbox.ToggledOn)
			SecurePlayerPrefs.SetInt("idle time", general.IdleTimeValue);
	}

	public void TabButtonPressed(ClickType clickType)
	{
		if (clickType != ClickType.Down)
			return;

		SelectableExtensions.MoveToNextSelectable(inputFields);
	}

	public void EnterButtonPressed(ClickType clickType)
	{
		if (clickType != ClickType.Down)
			return;

		if (InputFieldUtils.GetActiveInputField() == password2Field.InputFieldBase && saveButton.interactable)
			saveButton.Press();
		else
			SelectableExtensions.MoveToNextSelectable(inputFields);
	}
}