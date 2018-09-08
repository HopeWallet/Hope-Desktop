using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	[SerializeField] private GameObject defaultCurrencyDropdown;
	[SerializeField] private CheckBox idleTimeoutTimeCheckbox, countdownTimerCheckbox, transactionNotificationCheckbox, updateNotificationCheckbox;
	[SerializeField] private HopeInputField idleTimeoutTimeInputField;

	[SerializeField] private Button walletCategoryButton;
	[SerializeField] private HopeInputField walletNameField;
	[SerializeField] private Button saveButton, deleteButton;

	[SerializeField] private CheckBox twoFactorAuthenticationCheckbox;
	[SerializeField] private GameObject setUpSection;
	[SerializeField] private TextMeshProUGUI keyText;
	[SerializeField] private Image qrCodeImage;
	[SerializeField] private HopeInputField codeInputField;
	[SerializeField] private Button confirmButton;

	private GeneralSection general;
	private WalletSection wallet;
	private PasswordSection address;
	private TwoFactorAuthenticationSection twoFactorAuthenticationSection;

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

		general = new GeneralSection(idleTimeoutTimeCheckbox, countdownTimerCheckbox, transactionNotificationCheckbox, updateNotificationCheckbox, idleTimeoutTimeInputField);
		twoFactorAuthenticationSection = new TwoFactorAuthenticationSection(twoFactorAuthenticationCheckbox, setUpSection, keyText, qrCodeImage, codeInputField, confirmButton);

		if (userWalletManager.ActiveWalletType == UserWalletManager.WalletType.Hope)
			wallet = new WalletSection(hopeWalletInfoManager, userWalletManager, walletNameField, saveButton, deleteButton);
		else
		{
			walletCategoryButton.interactable = false;
			walletCategoryButton.GetComponent<TextMeshProUGUI>().color = UIColors.LightGrey;
		}
	}

	private void OnDestroy() => MoreDropdown.PopupClosed?.Invoke();
}