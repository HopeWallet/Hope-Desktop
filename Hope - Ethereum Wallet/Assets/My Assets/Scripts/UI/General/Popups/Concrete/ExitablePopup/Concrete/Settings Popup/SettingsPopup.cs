using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	[SerializeField] private CheckBox idleTimeoutTimeCheckbox, countdownTimerCheckbox, transactionNotificationCheckbox, updateNotificationCheckbox;
	[SerializeField] private HopeInputField idleTimeoutTimeInputField;

	[SerializeField] private GeneralRadioButtons defaultCurrencyOptions;

	[SerializeField] private Button walletCategoryButton;
	[SerializeField] private HopeInputField currentWalletNameField, newWalletNameField;
	[SerializeField] private Button walletSaveButton, deleteButton;

	[SerializeField] private Button passwordCategoryButton;
	[SerializeField] private HopeInputField currentPasswordField, newPasswordField, confirmPasswordField;
	[SerializeField] private Button passwordSaveButton, nextButton;
	[SerializeField] private GameObject loadingIcon;

	[SerializeField] private Button twoFactorAuthCategoryButton;
	[SerializeField] private CheckBox twoFactorAuthenticationCheckbox;
	[SerializeField] private GameObject setUpSection;
	[SerializeField] private TextMeshProUGUI keyText;
	[SerializeField] private Image qrCodeImage;
	[SerializeField] private HopeInputField codeInputField;
	[SerializeField] private Button confirmButton;

	private GeneralSection generalSection;
	private WalletSection walletSection;
	private PasswordSection passwordSection;
	private TwoFactorAuthenticationSection twoFactorAuthenticationSection;

	private UserWalletManager userWalletManager;
	private HopeWalletInfoManager hopeWalletInfoManager;
	private ButtonClickObserver buttonClickObserver;
	private CurrencyManager currencyManager;

	[Inject]
	public void Construct(UserWalletManager userWalletManager,
						  HopeWalletInfoManager hopeWalletInfoManager,
						  ButtonClickObserver buttonClickObserver,
						  CurrencyManager currencyManager)
	{
		this.userWalletManager = userWalletManager;
		this.hopeWalletInfoManager = hopeWalletInfoManager;
		this.buttonClickObserver = buttonClickObserver;
		this.currencyManager = currencyManager;

		defaultCurrencyOptions.ButtonClicked((int)currencyManager.ActiveCurrency);
	}

	protected override void OnStart()
	{
		base.OnStart();

		SettingsPopupAnimator settingsPopupAnimator = Animator as SettingsPopupAnimator;

		generalSection = new GeneralSection(idleTimeoutTimeCheckbox, countdownTimerCheckbox, transactionNotificationCheckbox, updateNotificationCheckbox, idleTimeoutTimeInputField);

		if (userWalletManager.ActiveWalletType == UserWalletManager.WalletType.Hope)
		{
			walletSection = new WalletSection(hopeWalletInfoManager, userWalletManager, currentWalletNameField, newWalletNameField, walletSaveButton, deleteButton);
			passwordSection = new PasswordSection(currentPasswordField, newPasswordField, confirmPasswordField, loadingIcon, passwordSaveButton, nextButton, settingsPopupAnimator);
			twoFactorAuthenticationSection = new TwoFactorAuthenticationSection(twoFactorAuthenticationCheckbox, setUpSection, keyText, qrCodeImage, codeInputField, confirmButton);
		}
		else
		{
			DisabledCategoryButton(walletCategoryButton);
			DisabledCategoryButton(passwordCategoryButton);
			DisabledCategoryButton(twoFactorAuthCategoryButton);
		}
	}

	private void DisabledCategoryButton(Button categoryButton)
	{
		categoryButton.interactable = false;
		categoryButton.GetComponent<TextMeshProUGUI>().color = UIColors.LightGrey;
	}

	private void OnDestroy()
	{
		MoreDropdown.PopupClosed?.Invoke();
		currencyManager.SwitchActiveCurrency((CurrencyManager.CurrencyType) defaultCurrencyOptions.previouslySelectedButton);
	}
}