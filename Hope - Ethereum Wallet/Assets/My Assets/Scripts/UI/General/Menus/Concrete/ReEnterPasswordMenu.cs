using Hope.Random.Bytes;
using Hope.Security.ProtectedTypes.Types;
using System;
using System.Collections;
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
	public event Action<bool> AnimateLockedOutSection;

	[SerializeField] private TextMeshProUGUI formTitle;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button unlockButton, homeButton;
    [SerializeField] private HopeInputField passwordField;
	[SerializeField] private TextMeshProUGUI timerText;

	private bool lockedOut;
	private string walletName;

	private readonly WaitForSeconds waiter = new WaitForSeconds(1f);

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

		walletName = hopeWalletInfoManager.GetWalletInfo(userWalletManager.GetWalletAddress()).WalletName;
		formTitle.text = walletName;

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

		if (!SecurePlayerPrefs.GetBool(PlayerPrefConstants.LOGIN_ATTEMPTS_LIMIT))
			return;

		if (!SecurePlayerPrefs.HasKey(walletName + PlayerPrefConstants.CURRENT_LOGIN_ATTEMPT))
			SecurePlayerPrefs.SetInt(walletName + PlayerPrefConstants.CURRENT_LOGIN_ATTEMPT, 1);

		if (!SecurePlayerPrefs.HasKey(walletName + PlayerPrefConstants.LAST_FAILED_LOGIN_ATTEMPT))
			SecurePlayerPrefs.SetString(walletName + PlayerPrefConstants.LAST_FAILED_LOGIN_ATTEMPT, DateTimeUtils.GetCurrentUnixTime().ToString());

		TimerChecker().StartCoroutine();
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
        int idleTime = SecurePlayerPrefs.GetInt(PlayerPrefConstants.IDLE_TIME);
        string minuteWord = idleTime == 1 ? " minute." : " minutes.";

        messageText.text = $"You have been idle for {idleTime}{minuteWord}{Environment.NewLine} Please re-enter your password or go back to the main menu.";
    }

	/// <summary>
	/// Updates the password field placeholder text with how many login attempts the user has before being locked out
	/// </summary>
	private void UpdatePlaceHolderText()
	{
		int currentLoginAttempt = SecurePlayerPrefs.GetInt(walletName + PlayerPrefConstants.CURRENT_LOGIN_ATTEMPT);
		int attemptsLeft = SecurePlayerPrefs.GetInt(PlayerPrefConstants.MAX_LOGIN_ATTEMPTS) - currentLoginAttempt + 1;

		if (currentLoginAttempt != 1)
		{
			string word = attemptsLeft == 1 ? " try " : " tries ";

			passwordField.SetPlaceholderText("Password (" + attemptsLeft + word + "left)");
		}
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

		if (passwordField.InputFieldBase.inputType == InputField.InputType.Standard)
			passwordField.EyeClicked();

		walletPasswordVerification.VerifyPassword(passwordField)
                                  .OnPasswordCorrect(CorrectPassword)
                                  .OnPasswordIncorrect(IncorrectPassword);
    }

    /// <summary>
    /// The password is correct and the user is brought back to the OpenWalletMenu
    /// </summary>
    /// <param name="password"> The password string</param>
    private void CorrectPassword(byte[] password)
    {
        (dynamicDataCache.GetData("pass") as ProtectedString)?.SetValue(password);

		SecurePlayerPrefs.SetInt(walletName + PlayerPrefConstants.CURRENT_LOGIN_ATTEMPT, 1);

		OnPasswordEnteredCorrect?.Invoke();
        uiManager.CloseMenu();
    }

    /// <summary>
    /// The password is incorrect and the user is given an error
    /// </summary>
    private void IncorrectPassword()
    {
        OnPasswordEnteredIncorrect?.Invoke();

		if (!SecurePlayerPrefs.GetBool(PlayerPrefConstants.LOGIN_ATTEMPTS_LIMIT))
			return;

		SecurePlayerPrefs.SetInt(walletName + PlayerPrefConstants.CURRENT_LOGIN_ATTEMPT, SecurePlayerPrefs.GetInt(walletName + PlayerPrefConstants.CURRENT_LOGIN_ATTEMPT) + 1);
		SecurePlayerPrefs.SetString(walletName + PlayerPrefConstants.LAST_FAILED_LOGIN_ATTEMPT, DateTimeUtils.GetCurrentUnixTime().ToString());

		if ((SecurePlayerPrefs.GetInt(PlayerPrefConstants.MAX_LOGIN_ATTEMPTS) - SecurePlayerPrefs.GetInt(walletName + PlayerPrefConstants.CURRENT_LOGIN_ATTEMPT) + 1) == 0)
		{
			lockedOut = true;
			AnimateLockedOutSection?.Invoke(true);
		}
		else
		{
			UpdatePlaceHolderText();
		}
	}

	/// <summary>
	/// Updates the timer text and checks to see if the user can be granted access to the password field again, 
	/// or gets the full login attempts amount reset again
	/// </summary>
	/// <returns> Returns one WaitForSeconds </returns>
	private IEnumerator TimerChecker()
	{
		long currentTime, lastFailedAttempt;
		long.TryParse(DateTimeUtils.GetCurrentUnixTime().ToString(), out currentTime);
		long.TryParse(SecurePlayerPrefs.GetString(walletName + PlayerPrefConstants.LAST_FAILED_LOGIN_ATTEMPT), out lastFailedAttempt);

		if ((currentTime - lastFailedAttempt) >= 300)
		{
			SecurePlayerPrefs.SetInt(walletName + PlayerPrefConstants.CURRENT_LOGIN_ATTEMPT, 1);
			passwordField.SetPlaceholderText("Password");

			if (lockedOut)
			{
				lockedOut = false;
				passwordField.Text = string.Empty;
				passwordField.UpdateVisuals();
				passwordField.InputFieldBase.ActivateInputField();
				AnimateLockedOutSection?.Invoke(false);
			}
		}
		else
		{
			timerText.text = DateTimeUtils.GetAnalogTime(300 - (currentTime - lastFailedAttempt));
		}

		yield return waiter;

		if (this != null)
			TimerChecker().StartCoroutine();
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