using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// The main settings popup class
/// </summary>
public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>, ITabButtonObservable, IEnterButtonObservable
{
    [SerializeField] private IconButtons categoryButtons;
    [SerializeField] private CheckBox idleTimeoutTimeCheckbox, countdownTimerCheckbox, showTooltipsCheckbox, updateNotificationCheckbox;
    [SerializeField] private HopeInputField idleTimeoutTimeInputField;

    [SerializeField] private IconButtons defaultCurrencyOptions;

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

    private readonly List<Selectable> selectables = new List<Selectable>();

    private GeneralSection generalSection;
    private WalletSection walletSection;
    private TwoFactorAuthenticationSection twoFactorAuthenticationSection;

    private UserWalletManager userWalletManager;
    private HopeWalletInfoManager hopeWalletInfoManager;
    private ButtonClickObserver buttonClickObserver;
    private CurrencyManager currencyManager;

	/// <summary>
	/// Sets the necessary dependencies
	/// </summary>
	/// <param name="userWalletManager"> The active UserWalletManager </param>
	/// <param name="hopeWalletInfoManager"> The active HopeWalletInfoManager </param>
	/// <param name="buttonClickObserver"> The active ButtonClickObserver </param>
	/// <param name="currencyManager"> The active CurrencyManager </param>
    [Inject]
    public void Construct(
        UserWalletManager userWalletManager,
        HopeWalletInfoManager hopeWalletInfoManager,
        ButtonClickObserver buttonClickObserver,
        CurrencyManager currencyManager)
    {
        this.userWalletManager = userWalletManager;
        this.hopeWalletInfoManager = hopeWalletInfoManager;
        this.buttonClickObserver = buttonClickObserver;
        this.currencyManager = currencyManager;

		buttonClickObserver.SubscribeObservable(this);
        defaultCurrencyOptions.ButtonClicked((int)currencyManager.ActiveCurrency);
    }

	/// <summary>
	/// Sets the other partial classes and other necessary variables
	/// </summary>
    protected override void OnStart()
    {
        base.OnStart();

		bool usingHopeWallet = userWalletManager.ActiveWalletType == UserWalletManager.WalletType.Hope;

		generalSection = new GeneralSection(idleTimeoutTimeCheckbox, countdownTimerCheckbox, showTooltipsCheckbox, updateNotificationCheckbox, idleTimeoutTimeInputField, usingHopeWallet);

        if (usingHopeWallet)
        {
            walletSection = new WalletSection(hopeWalletInfoManager, userWalletManager, popupManager, Animator as SettingsPopupAnimator, currentPasswordField, walletNameField, newPasswordField, confirmPasswordField, editWalletButton, saveButton, deleteButton, checkMarkIcon);
            twoFactorAuthenticationSection = new TwoFactorAuthenticationSection(twoFactorAuthenticationCheckbox, setUpSection, keyText, qrCodeImage, codeInputField, confirmButton);
        }
        else
        {
            DisabledCategoryButton(walletCategoryButton);
            DisabledCategoryButton(twoFactorAuthCategoryButton);
        }

        selectables.Add(walletNameField.InputFieldBase);
        selectables.Add(newPasswordField.InputFieldBase);
        selectables.Add(confirmPasswordField.InputFieldBase);
    }

	/// <summary>
	/// Disables a given category button on the popup
	/// </summary>
	/// <param name="categoryButton"> The category button being disabled </param>
    private void DisabledCategoryButton(Button categoryButton)
    {
        categoryButton.interactable = false;
        categoryButton.GetComponent<TextMeshProUGUI>().color = UIColors.LightGrey;
    }

	/// <summary>
	/// Switches currency if needed
	/// </summary>
    private void OnDestroy()
    {
        buttonClickObserver.UnsubscribeObservable(this);
        MoreDropdown.PopupClosed?.Invoke();

        if (currencyManager.ActiveCurrency != (CurrencyManager.CurrencyType)defaultCurrencyOptions.previouslySelectedButton)
            currencyManager.SwitchActiveCurrency((CurrencyManager.CurrencyType)defaultCurrencyOptions.previouslySelectedButton);
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

        if (InputFieldUtils.GetActiveInputField() == currentPasswordField.InputFieldBase && editWalletButton.interactable)
            editWalletButton.Press();
        else if (InputFieldUtils.GetActiveInputField() == confirmPasswordField.InputFieldBase && saveButton.interactable)
            saveButton.Press();
        else
            selectables.MoveToNextSelectable();
    }
}