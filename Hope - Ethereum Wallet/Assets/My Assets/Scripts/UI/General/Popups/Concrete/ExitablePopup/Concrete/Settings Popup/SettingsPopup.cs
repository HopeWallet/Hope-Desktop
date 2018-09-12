using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	[SerializeField] private CategoryButtons categoryButtons;
	[SerializeField] private CheckBox idleTimeoutTimeCheckbox, countdownTimerCheckbox, transactionNotificationCheckbox, updateNotificationCheckbox;
	[SerializeField] private HopeInputField idleTimeoutTimeInputField;

	[SerializeField] private GeneralRadioButtons defaultCurrencyOptions;

	[SerializeField] private Button walletCategoryButton;
	[SerializeField] private HopeInputField currentPasswordField, walletNameField, newPasswordField, confirmPasswordField;
	[SerializeField] private Button editWalletButton, saveButton, deleteButton;
	[SerializeField] private GameObject checkMarkIcon;

	[SerializeField] private Button twoFactorAuthCategoryButton;
	[SerializeField] private CheckBox twoFactorAuthenticationCheckbox;
	[SerializeField] private GameObject setUpSection;
	[SerializeField] private TextMeshProUGUI keyText;
	[SerializeField] private Image qrCodeImage;
	[SerializeField] private HopeInputField codeInputField;
	[SerializeField] private Button confirmButton;

	private GeneralSection generalSection;
	private WalletSection walletSection;
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
			walletSection = new WalletSection(hopeWalletInfoManager, userWalletManager, buttonClickObserver, settingsPopupAnimator, currentPasswordField, walletNameField, newPasswordField, confirmPasswordField, editWalletButton, saveButton, deleteButton, checkMarkIcon);
			twoFactorAuthenticationSection = new TwoFactorAuthenticationSection(twoFactorAuthenticationCheckbox, setUpSection, keyText, qrCodeImage, codeInputField, confirmButton);
		}
		else
		{
			DisabledCategoryButton(walletCategoryButton);
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