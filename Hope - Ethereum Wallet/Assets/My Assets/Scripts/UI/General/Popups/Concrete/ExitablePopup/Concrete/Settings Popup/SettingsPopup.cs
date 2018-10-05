using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// The popup that manages the modification of user's settings and preferences
/// </summary>
public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>, ITabButtonObservable, IEnterButtonObservable
{
    [SerializeField] private CheckBox countdownTimerCheckbox, showTooltipsCheckbox, updateNotificationCheckbox, startupAccountCheckbox;

    [SerializeField] private IconButtons defaultCurrencyOptions;

	[SerializeField] private CheckBox PasswordForTransactionCheckbox, idleTimeoutTimeCheckbox, loginAttemptsCheckbox;
	[SerializeField] private HopeInputField idleTimeoutTimeInputField, loginAttemptsInputField, lockoutTimeInputField;

	[SerializeField] private CheckBox twoFactorAuthCheckbox;
	[SerializeField] private GameObject setUpSection;
	[SerializeField] private TextMeshProUGUI keyText;
	[SerializeField] private Image qrCodeImage;
	[SerializeField] private HopeInputField codeInputField;
	[SerializeField] private Button confirmButton;

    [SerializeField] private HopeInputField currentWalletNameField, newWalletNameField;

	[SerializeField] private HopeInputField newPasswordField, confirmPasswordField;

	[SerializeField] private GameObject[] hopeOnlyCategoryButtons;
	[SerializeField] private GameObject[] categoryLines;
	[SerializeField] private HopeInputField[] currentPasswordFields;
	[SerializeField] private Button[] changingSettingButtons;
	[SerializeField] private GameObject[] loadingIcons;

	private readonly List<Selectable> selectables = new List<Selectable>();

	private SecuritySection securitySection;
    private TwoFactorAuthenticationSection twoFactorAuthenticationSection;
	private WalletNameSection walletNameSection;
	private PasswordSection passwordSection;
	private DeleteWalletSection deleteWalletSection;

	private UserWalletManager userWalletManager;
    private HopeWalletInfoManager hopeWalletInfoManager;
    private ButtonClickObserver buttonClickObserver;
    private CurrencyManager currencyManager;
	private LogoutHandler logoutHandler;

	/// <summary>
	/// Sets the necessary dependencies
	/// </summary>
	/// <param name="userWalletManager"> The active UserWalletManager </param>
	/// <param name="hopeWalletInfoManager"> The active HopeWalletInfoManager </param>
	/// <param name="buttonClickObserver"> The active ButtonClickObserver </param>
	/// <param name="currencyManager"> The active CurrencyManager </param>
	[Inject]
    public void Construct(UserWalletManager userWalletManager,
						  HopeWalletInfoManager hopeWalletInfoManager,
						  ButtonClickObserver buttonClickObserver,
						  CurrencyManager currencyManager,
						  LogoutHandler logoutHandler)
    {
		this.userWalletManager = userWalletManager;
		this.hopeWalletInfoManager = hopeWalletInfoManager;
        this.buttonClickObserver = buttonClickObserver;
        this.currencyManager = currencyManager;
		this.logoutHandler = logoutHandler;

		buttonClickObserver.SubscribeObservable(this);
		defaultCurrencyOptions.ButtonClicked((int)currencyManager.ActiveCurrency);
	}

	/// <summary>
	/// Sets the other partial classes and other necessary variables
	/// </summary>
    protected override void OnStart()
    {
        base.OnStart();

		SetCurrentSettings();

		if (userWalletManager.ActiveWalletType == UserWalletManager.WalletType.Hope)
        {
			securitySection = new SecuritySection(idleTimeoutTimeCheckbox, loginAttemptsCheckbox, idleTimeoutTimeInputField, loginAttemptsInputField, lockoutTimeInputField);
            twoFactorAuthenticationSection = new TwoFactorAuthenticationSection(twoFactorAuthCheckbox, setUpSection, keyText, qrCodeImage, codeInputField, confirmButton);
			walletNameSection = new WalletNameSection(hopeWalletInfoManager, userWalletManager, Animator as SettingsPopupAnimator, currentPasswordFields[0], currentWalletNameField, newWalletNameField, changingSettingButtons[0], loadingIcons[0]);
			passwordSection = new PasswordSection(currentPasswordFields[1], newPasswordField, confirmPasswordField, changingSettingButtons[1], loadingIcons[1]);
			deleteWalletSection = new DeleteWalletSection(hopeWalletInfoManager, userWalletManager, logoutHandler, currentPasswordFields[2], changingSettingButtons[2], loadingIcons[2]);
		}
		else
        {
			foreach (GameObject categoryButton in hopeOnlyCategoryButtons)
				categoryButton.SetActive(false);

			categoryLines[0].SetActive(false);
			categoryLines[1].SetActive(false);
		}

		//selectables.Add(walletNameField.InputFieldBase);
		//selectables.Add(newPasswordField.InputFieldBase);
		//selectables.Add(confirmPasswordField.InputFieldBase);
	}

	/// <summary>
	/// Sets the current user settings
	/// </summary>
	private void SetCurrentSettings()
	{
		countdownTimerCheckbox.SetValue(SecurePlayerPrefs.GetBool("countdown timer"));
		showTooltipsCheckbox.SetValue(SecurePlayerPrefs.GetBool("show tooltips"));
		updateNotificationCheckbox.SetValue(SecurePlayerPrefs.GetBool("update notification"));
		startupAccountCheckbox.SetValue(SecurePlayerPrefs.GetBool("start on last account"));
		PasswordForTransactionCheckbox.SetValue(SecurePlayerPrefs.GetBool("password required for transaction"));
	}

	/// <summary>
	/// Switches currency if needed
	/// </summary>
	private void OnDestroy()
    {
        buttonClickObserver.UnsubscribeObservable(this);
        MoreDropdown.PopupClosed?.Invoke();

        if (currencyManager.ActiveCurrency != (CurrencyManager.CurrencyType)defaultCurrencyOptions.PreviouslyActiveButton)
            currencyManager.SwitchActiveCurrency((CurrencyManager.CurrencyType)defaultCurrencyOptions.PreviouslyActiveButton);

		SecurePlayerPrefs.SetBool("countdown timer", countdownTimerCheckbox.IsToggledOn);
		SecurePlayerPrefs.SetBool("show tooltips", showTooltipsCheckbox.IsToggledOn);
		SecurePlayerPrefs.SetBool("update notification", updateNotificationCheckbox.IsToggledOn);
		SecurePlayerPrefs.SetBool("password required for transaction", PasswordForTransactionCheckbox.IsToggledOn);
		SecurePlayerPrefs.SetBool("idle timeout", idleTimeoutTimeCheckbox.IsToggledOn);
		SecurePlayerPrefs.SetBool("limit login attempts", loginAttemptsCheckbox.IsToggledOn);
	}

	/// <summary>
	/// Moves to the next input field
	/// </summary>
	/// <param name="clickType"> The tab button ClickType </param>
    public void TabButtonPressed(ClickType clickType)
    {
        if (clickType != ClickType.Down)
            return;

        selectables.MoveToNextSelectable();
    }

	/// <summary>
	/// Moves to next input field, unless at the last input field, then it presses the button if it is interactable
	/// </summary>
	/// <param name="clickType"> The enter button ClickType </param>
    public void EnterButtonPressed(ClickType clickType)
    {
        if (clickType != ClickType.Down)
            return;

        //if (InputFieldUtils.GetActiveInputField() == currentPasswordField.InputFieldBase && editWalletButton.interactable)
            //editWalletButton.Press();
        //else if (InputFieldUtils.GetActiveInputField() == confirmPasswordField.InputFieldBase && saveButton.interactable)
            //saveButton.Press();
        //else
            //selectables.MoveToNextSelectable();
    }
}