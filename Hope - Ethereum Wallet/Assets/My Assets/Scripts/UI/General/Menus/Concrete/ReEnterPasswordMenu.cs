using Hope.Random.Bytes;
using Hope.Security.ProtectedTypes.Types;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// The class that manages the re-login of the user after being idle
/// </summary>
public sealed class ReEnterPasswordMenu : Menu<ReEnterPasswordMenu>, IEnterButtonObservable
{
    public event Action OnPasswordVerificationStarted;
    public event Action OnPasswordEnteredCorrect;
    public event Action OnPasswordEnteredIncorrect;

    [SerializeField] private TextMeshProUGUI walletName;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button unlockButton, homeButton;
    [SerializeField] private HopeInputField passwordField;

    private WalletPasswordVerification walletPasswordVerification;
    private LogoutHandler logoutHandler;
    private DynamicDataCache dynamicDataCache;
    private ButtonClickObserver buttonClickObserver;

    /// <summary>
    /// Sets the necessary dependencies
    /// </summary>
    /// <param name="hopeWalletInfoManager"> The active HopeWalletInfoManager </param>
    /// <param name="userWalletManager"> The active UserWalletManager </param>
    /// <param name="walletPasswordVerification"> An instance of the WalletPasswordVerification class. </param>
    /// <param name="logoutHandler"> The active LogoutHandler. </param>
    /// <param name="dynamicDataCache"> The active DynamicDataCache</param>
    /// <param name="buttonClickObserver"> The active ButtonClickObserver </param>
    [Inject]
    public void Construct(
        HopeWalletInfoManager hopeWalletInfoManager,
        UserWalletManager userWalletManager,
        WalletPasswordVerification walletPasswordVerification,
        LogoutHandler logoutHandler,
        DynamicDataCache dynamicDataCache,
        ButtonClickObserver buttonClickObserver)
    {
        this.walletPasswordVerification = walletPasswordVerification;
        this.logoutHandler = logoutHandler;
        this.dynamicDataCache = dynamicDataCache;
        this.buttonClickObserver = buttonClickObserver;

        walletName.text = hopeWalletInfoManager.GetWalletInfo(userWalletManager.GetWalletAddress()).WalletName;

        SetMessageText();

        (dynamicDataCache.GetData("pass") as ProtectedString)?.SetValue(RandomBytes.Secure.SHA3.GetBytes(16));
    }

    /// <summary>
    /// Sets all the listeners
    /// </summary>
    protected override void OnAwake()
    {
        homeButton.onClick.AddListener(HomeButtonClicked);
        unlockButton.onClick.AddListener(UnlockButtonClicked);

        passwordField.OnInputUpdated += PasswordFieldChanged;
    }

    /// <summary>
    /// Subscribes the buttonClickObserver
    /// </summary>
    private void OnEnable()
    {
        buttonClickObserver.SubscribeObservable(this);
    }

    /// <summary>
    /// Unsubscribes the buttonClickObserver
    /// </summary>
    private void OnDisable()
    {
        buttonClickObserver.UnsubscribeObservable(this);
    }

    /// <summary>
    /// Sets the main message text
    /// </summary>
    private void SetMessageText()
    {
        int idleTime = SecurePlayerPrefs.GetInt("idle time");
        string minuteWord = idleTime == 1 ? " minute." : " minutes.";

        messageText.text = $"You have been idle for {idleTime}{minuteWord}{Environment.NewLine} Please re-enter your password or go back to the main menu.";
    }

    /// <summary>
    /// Password field has been changed
    /// </summary>
    /// <param name="text"> The current text in the input field </param>
    private void PasswordFieldChanged(string text)
    {
        passwordField.Error = string.IsNullOrEmpty(passwordField.InputFieldBase.text);
        unlockButton.interactable = !passwordField.Error;
    }

    /// <summary>
    /// Home button has been clicked
    /// </summary>
    private void HomeButtonClicked()
    {
        logoutHandler.Logout();
    }

    /// <summary>
    /// Unlock button has been clicked and password is checked
    /// </summary>
    private void UnlockButtonClicked()
    {
        OnPasswordVerificationStarted?.Invoke();

        walletPasswordVerification.VerifyPassword(passwordField)
                                  .OnPasswordCorrect(CorrectPassword)
                                  .OnPasswordIncorrect(IncorrectPassword);
    }

    /// <summary>
    /// The password is correct and the user is brought back to the OpenWalletMenu
    /// </summary>
    /// <param name="password"> The password string</param>
    private void CorrectPassword(string password)
    {
        (dynamicDataCache.GetData("pass") as ProtectedString)?.SetValue(password);

        OnPasswordEnteredCorrect?.Invoke();
        uiManager.CloseMenu();
    }

    /// <summary>
    /// The password is incorrect and the user is given an error
    /// </summary>
    private void IncorrectPassword()
    {
        OnPasswordEnteredIncorrect?.Invoke();
    }

    /// <summary>
    /// Clicks the unlockButton if the input field is selected and the unlock button is interactable
    /// </summary>
    /// <param name="clickType"> The enter button ClickType </param>
    public void EnterButtonPressed(ClickType clickType)
    {
        if (clickType != ClickType.Down)
            return;

        if (unlockButton.interactable && !walletPasswordVerification.VerifyingPassword)
            unlockButton.Press();
        else if (!walletPasswordVerification.VerifyingPassword)
            passwordField.InputFieldBase.SelectSelectable();
    }
}