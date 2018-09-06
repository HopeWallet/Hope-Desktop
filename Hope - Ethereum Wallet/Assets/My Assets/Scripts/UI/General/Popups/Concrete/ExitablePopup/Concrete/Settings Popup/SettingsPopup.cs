using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	[SerializeField] private GameObject defaultCurrencyDropdown;
	[SerializeField] private CheckBox idleTimeoutTimeCheckbox, countdownTimerCheckbox, transactionNotificationCheckbox, updateNotificationCheckbox;
	[SerializeField] private HopeInputField idleTimeoutTimeInputField;

	[SerializeField] private HopeInputField walletNameField, newPasswordField, confirmPasswordField;
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

	private GeneralSection general;
	private WalletSection wallet;
	private AddressSection address;
	private TwoFactorAuthenticationSection twoFactorAuthenticationSection;
	private HopeVersion hopeVersion;

	protected override void OnStart()
	{
		base.OnStart();

		general = new GeneralSection(defaultCurrencyDropdown, idleTimeoutTimeCheckbox, countdownTimerCheckbox, transactionNotificationCheckbox, updateNotificationCheckbox, idleTimeoutTimeInputField);
		wallet = new WalletSection(walletNameField, newPasswordField, confirmPasswordField, saveButton, deleteButton);
		address = new AddressSection(addressOptions, addressListTransform, unlockButton);
		twoFactorAuthenticationSection = new TwoFactorAuthenticationSection(twoFactorAuthenticationCheckbox, setUpSection, keyText, qrCodeImage, codeInputField, confirmButton);
		hopeVersion = new HopeVersion(currentVersionText, latestVersionText, downloadUpdateButton);
	}

	private void OnDestroy()
	{
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
}