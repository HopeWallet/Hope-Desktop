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
	[SerializeField] private HopeInputField idleTimeoutTimeInputField, loginAttemptsInputField;

	[SerializeField] private CheckBox twoFactorAuthCheckbox;
	[SerializeField] private GameObject setUpSection;
	[SerializeField] private TextMeshProUGUI keyText;
	[SerializeField] private Image qrCodeImage;
	[SerializeField] private HopeInputField codeInputField;
	[SerializeField] private Button confirmButton;

	[SerializeField] private GameObject currentPasswordSection, changeWalletNameSection, currentPasswordloadingIcon, newPasswordLoadingIcon;
	[SerializeField] private HopeInputField currentPasswordField, currentWalletNameField, newWalletNameField, newPasswordField, confirmPasswordField;
	[SerializeField] private Button nextButton, saveWalletNameButton, savePasswordButton, deleteWalletButton;

	[SerializeField] private GameObject[] hopeOnlyCategoryButtons;
	[SerializeField] private GameObject[] categoryLines;

	private string walletName;

	private readonly List<Selectable> selectables = new List<Selectable>();

	private SecuritySection securitySection;
    private TwoFactorAuthenticationSection twoFactorAuthenticationSection;
	private WalletNameSection walletNameSection;
	private PasswordSection passwordSection;

	private UserWalletManager userWalletManager;
    private HopeWalletInfoManager hopeWalletInfoManager;
    private HopeWalletInfoManager.Settings walletSettings;
    private WalletPasswordVerification walletPasswordVerification;
    private ContactsManager contactsManager;
    private DynamicDataCache dynamicDataCache;
    private ButtonClickObserver buttonClickObserver;
    private CurrencyManager currencyManager;
	private LogoutHandler logoutHandler;
	private SettingsPopupAnimator settingsPopupAnimator;

    /// <summary>
    /// Sets the necessary dependencies
    /// </summary>
    /// <param name="userWalletManager"> The active UserWalletManager </param>
    /// <param name="hopeWalletInfoManager"> The active HopeWalletInfoManager </param>
    /// <param name="walletSettings"> The active HopeWalletInfoManager.Settings. </param>
    /// <param name="walletPasswordVerification"> An instance of WalletPasswordVerification. </param>
    /// <param name="contactsManager"> The active ContactsManager. </param>
    /// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
    /// <param name="buttonClickObserver"> The active ButtonClickObserver </param>
    /// <param name="currencyManager"> The active CurrencyManager </param>
    /// <param name="logoutHandler"> The active LogoutHandler. </param>
    [Inject]
    public void Construct(
        UserWalletManager userWalletManager,
        HopeWalletInfoManager hopeWalletInfoManager,
        HopeWalletInfoManager.Settings walletSettings,
        WalletPasswordVerification walletPasswordVerification,
        ContactsManager contactsManager,
        DynamicDataCache dynamicDataCache,
        ButtonClickObserver buttonClickObserver,
        CurrencyManager currencyManager,
        LogoutHandler logoutHandler)
    {
		this.userWalletManager = userWalletManager;
		this.hopeWalletInfoManager = hopeWalletInfoManager;
        this.walletSettings = walletSettings;
        this.walletPasswordVerification = walletPasswordVerification;
        this.contactsManager = contactsManager;
        this.dynamicDataCache = dynamicDataCache;
        this.buttonClickObserver = buttonClickObserver;
        this.currencyManager = currencyManager;
		this.logoutHandler = logoutHandler;

		buttonClickObserver.SubscribeObservable(this);
		defaultCurrencyOptions.ButtonClicked((int)currencyManager.ActiveCurrency);

		walletName = hopeWalletInfoManager.GetWalletInfo(userWalletManager.GetWalletAddress()).WalletName;
		deleteWalletButton.onClick.AddListener(() => popupManager.GetPopup<GeneralOkCancelPopup>(true).SetSubText("Are you sure you want to delete " + walletName + "?").OnOkClicked(() => logoutHandler.Logout()));
	}

	/// <summary>
	/// Sets the other partial classes and other necessary variables
	/// </summary>
	protected override void OnStart()
    {
        base.OnStart();

		SetCurrentSettings();

		settingsPopupAnimator = Animator as SettingsPopupAnimator;

		if (userWalletManager.ActiveWalletType == UserWalletManager.WalletType.Hope)
        {
			securitySection = new SecuritySection(idleTimeoutTimeCheckbox, loginAttemptsCheckbox, idleTimeoutTimeInputField, loginAttemptsInputField);

            twoFactorAuthenticationSection = new TwoFactorAuthenticationSection(twoFactorAuthCheckbox, setUpSection, keyText, qrCodeImage, codeInputField, confirmButton);

			walletNameSection = new WalletNameSection(hopeWalletInfoManager, walletSettings, walletPasswordVerification, contactsManager, dynamicDataCache, userWalletManager, settingsPopupAnimator, currentPasswordSection, changeWalletNameSection, currentPasswordloadingIcon, currentPasswordField, currentWalletNameField, newWalletNameField, nextButton, saveWalletNameButton, hopeOnlyCategoryButtons);

			passwordSection = new PasswordSection(settingsPopupAnimator, newPasswordField, confirmPasswordField, savePasswordButton, newPasswordLoadingIcon);
		}
		else
        {
			foreach (GameObject categoryButton in hopeOnlyCategoryButtons)
				categoryButton.SetActive(false);

			categoryLines[0].SetActive(false);
			categoryLines[1].SetActive(false);
		}

		selectables.Add(newPasswordField.InputFieldBase);
		selectables.Add(confirmPasswordField.InputFieldBase);
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
		if (currencyManager.ActiveCurrency != (CurrencyManager.CurrencyType)defaultCurrencyOptions.PreviouslyActiveButton)
			currencyManager.SwitchActiveCurrency((CurrencyManager.CurrencyType)defaultCurrencyOptions.PreviouslyActiveButton);

		SecurePlayerPrefs.SetBool("countdown timer", countdownTimerCheckbox.IsToggledOn);
		SecurePlayerPrefs.SetBool("show tooltips", showTooltipsCheckbox.IsToggledOn);
		SecurePlayerPrefs.SetBool("update notification", updateNotificationCheckbox.IsToggledOn);
		SecurePlayerPrefs.SetBool("password required for transaction", PasswordForTransactionCheckbox.IsToggledOn);
		SecurePlayerPrefs.SetBool("idle timeout", idleTimeoutTimeCheckbox.IsToggledOn);
		SecurePlayerPrefs.SetBool("limit login attempts", loginAttemptsCheckbox.IsToggledOn);

		buttonClickObserver.UnsubscribeObservable(this);
        MoreDropdown.PopupClosed?.Invoke();
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

		if (InputFieldUtils.GetActiveInputField() == currentPasswordField.InputFieldBase && nextButton.interactable)
			nextButton.Press();
		else if (InputFieldUtils.GetActiveInputField() == newWalletNameField.InputFieldBase && saveWalletNameButton.interactable)
			saveWalletNameButton.Press();
		else if (InputFieldUtils.GetActiveInputField() == confirmPasswordField.InputFieldBase && savePasswordButton.interactable)
			savePasswordButton.Press();
		else
			selectables.MoveToNextSelectable();
	}
}