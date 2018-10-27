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
    private UserWalletManager.Settings userWalletManagerSettings;
    private HopeWalletInfoManager hopeWalletInfoManager;
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
    /// <param name="userWalletManagerSettings"> The active UserWalletManager.Settings. </param>
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
        UserWalletManager.Settings userWalletManagerSettings,
        WalletPasswordVerification walletPasswordVerification,
        ContactsManager contactsManager,
        DynamicDataCache dynamicDataCache,
        ButtonClickObserver buttonClickObserver,
        CurrencyManager currencyManager,
        LogoutHandler logoutHandler)
    {
		this.userWalletManager = userWalletManager;
        this.userWalletManagerSettings = userWalletManagerSettings;
		this.hopeWalletInfoManager = hopeWalletInfoManager;
        this.walletPasswordVerification = walletPasswordVerification;
        this.contactsManager = contactsManager;
        this.dynamicDataCache = dynamicDataCache;
        this.buttonClickObserver = buttonClickObserver;
        this.currencyManager = currencyManager;
		this.logoutHandler = logoutHandler;

		buttonClickObserver.SubscribeObservable(this);
		defaultCurrencyOptions.ButtonClicked((int)currencyManager.ActiveCurrency);

		walletName = hopeWalletInfoManager.GetWalletInfo(userWalletManager.GetWalletAddress()).WalletName;

		deleteWalletButton.onClick.AddListener(() => popupManager.GetPopup<GeneralOkCancelPopup>(true)
																 .SetSubText($"Are you sure you want to delete wallet '{walletName}'?\nThis cannot be undone!")
																 .OnOkClicked(() => DeleteWallet(userWalletManager, hopeWalletInfoManager, logoutHandler))
																 .DisableEnterButton());
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

			walletNameSection = new WalletNameSection(hopeWalletInfoManager, walletPasswordVerification, contactsManager, dynamicDataCache, userWalletManager, settingsPopupAnimator, currentPasswordSection, changeWalletNameSection, currentPasswordloadingIcon, currentPasswordField, currentWalletNameField, newWalletNameField, nextButton, saveWalletNameButton, hopeOnlyCategoryButtons);

			passwordSection = new PasswordSection(userWalletManagerSettings, userWalletManager, hopeWalletInfoManager, dynamicDataCache, settingsPopupAnimator, newPasswordField, confirmPasswordField, savePasswordButton, newPasswordLoadingIcon);
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
    /// Switches currency if needed
    /// </summary>
    private void OnDestroy()
    {
        if (currencyManager.ActiveCurrency != (CurrencyManager.CurrencyType)defaultCurrencyOptions.PreviouslyActiveButton)
            currencyManager.SwitchActiveCurrency((CurrencyManager.CurrencyType)defaultCurrencyOptions.PreviouslyActiveButton);

        SecurePlayerPrefs.SetBool(PlayerPrefConstants.SETTING_COUNTDOWN_TIMER, countdownTimerCheckbox.IsToggledOn);
        SecurePlayerPrefs.SetBool(PlayerPrefConstants.SETTING_SHOW_TOOLTIPS, showTooltipsCheckbox.IsToggledOn);
        SecurePlayerPrefs.SetBool(PlayerPrefConstants.SETTING_UPDATE_NOTIFICATIONS, updateNotificationCheckbox.IsToggledOn);
        SecurePlayerPrefs.SetBool(PlayerPrefConstants.SETTING_REQUIRE_PASSWORD_FOR_TRANSACTION, PasswordForTransactionCheckbox.IsToggledOn);
        SecurePlayerPrefs.SetBool(PlayerPrefConstants.SETTING_IDLE_TIMEOUT, idleTimeoutTimeCheckbox.IsToggledOn);
        SecurePlayerPrefs.SetBool(PlayerPrefConstants.SETTING_LOGIN_ATTEMPTS_LIMIT, loginAttemptsCheckbox.IsToggledOn);
        SecurePlayerPrefs.SetBool(PlayerPrefConstants.SETTING_START_ON_PREVIOUS_ACCOUNT, startupAccountCheckbox.IsToggledOn);

        buttonClickObserver.UnsubscribeObservable(this);
        MoreDropdown.PopupClosed?.Invoke();
    }

    /// <summary>
    /// Sets the current user settings
    /// </summary>
    private void SetCurrentSettings()
	{
		countdownTimerCheckbox.SetValue(SecurePlayerPrefs.GetBool(PlayerPrefConstants.SETTING_COUNTDOWN_TIMER));
		showTooltipsCheckbox.SetValue(SecurePlayerPrefs.GetBool(PlayerPrefConstants.SETTING_SHOW_TOOLTIPS));
		updateNotificationCheckbox.SetValue(SecurePlayerPrefs.GetBool(PlayerPrefConstants.SETTING_UPDATE_NOTIFICATIONS));
		startupAccountCheckbox.SetValue(SecurePlayerPrefs.GetBool(PlayerPrefConstants.SETTING_START_ON_PREVIOUS_ACCOUNT));
		PasswordForTransactionCheckbox.SetValue(SecurePlayerPrefs.GetBool(PlayerPrefConstants.SETTING_REQUIRE_PASSWORD_FOR_TRANSACTION));
	}

    private void DeleteWallet(UserWalletManager userWalletManager, HopeWalletInfoManager hopeWalletInfoManager, LogoutHandler logoutHandler)
    {
        var wallets = hopeWalletInfoManager.Wallets;
        var walletToDelete = hopeWalletInfoManager.GetWalletInfo(userWalletManager.GetWalletAddress());

        for (int i = wallets.IndexOf(walletToDelete) + 1; i < wallets.Count; i++)
            hopeWalletInfoManager.UpdateWalletInfo(wallets[i].WalletNum, new WalletInfo(wallets[i].EncryptedWalletData, wallets[i].WalletName, wallets[i].WalletAddresses, wallets[i].WalletNum - 1));

        hopeWalletInfoManager.DeleteWalletInfo(walletToDelete);
		logoutHandler.Logout();
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