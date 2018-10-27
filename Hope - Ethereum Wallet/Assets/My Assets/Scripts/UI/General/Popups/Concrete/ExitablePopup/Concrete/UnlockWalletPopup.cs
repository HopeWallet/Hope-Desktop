using Hope.Security.ProtectedTypes.Types;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Popup used for entering the password to unlock a specific wallet.
/// </summary>
public sealed class UnlockWalletPopup : ExitablePopupComponent<UnlockWalletPopup>, ITabButtonObservable, IEnterButtonObservable
{
	public event Action OnPasswordEnteredIncorrect;
	public event Action<bool> AnimateLockedOutSection;

	private Action popupClosed;

	[SerializeField] private TextMeshProUGUI formTitle;
	[SerializeField] private HopeInputField passwordField;
	[SerializeField] private Button unlockWalletButton;
	[SerializeField] private TextMeshProUGUI timerText;

	private readonly WaitForSeconds waiter = new WaitForSeconds(1f);

	private UIManager uiManager;
	private UserWalletManager userWalletManager;
	private DynamicDataCache dynamicDataCache;
	private ButtonClickObserver buttonClickObserver;
	private WalletPasswordVerification walletPasswordVerification;

	/// <summary>
	/// If the user is locked out from logging in or not
	/// </summary>
	public bool LockedOut { get; set; }

	/// <summary>
	/// The wallet name
	/// </summary>
	public string WalletName { get; private set; }

	/// <summary>
	/// Adds the required dependencies to this popup.
	/// </summary>
	/// <param name="uiManager"> The active UIManager. </param>
	/// <param name="userWalletManager"> The active UserWalletManager. </param>
	/// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
	/// <param name="buttonClickObserver"> The active ButtonClickObserver. </param>
	/// <param name="walletPasswordVerification"> An instance of the WalletPasswordVerification class. </param>
	[Inject]
	public void Construct(
		UIManager uiManager,
		UserWalletManager userWalletManager,
		DynamicDataCache dynamicDataCache,
		ButtonClickObserver buttonClickObserver,
		WalletPasswordVerification walletPasswordVerification)
	{
		this.uiManager = uiManager;
		this.userWalletManager = userWalletManager;
		this.dynamicDataCache = dynamicDataCache;
		this.buttonClickObserver = buttonClickObserver;
		this.walletPasswordVerification = walletPasswordVerification;
	}

	/// <summary>
	/// Sets the wallet name and popupClosed action
	/// </summary>
	/// <param name="walletName"> The wallet name </param>
	/// <param name="popupClosed"> The finishing action </param>
	public void SetVariables(string walletName, Action popupClosed)
	{
		this.WalletName = walletName;
		this.popupClosed = popupClosed;
		formTitle.text = $"Unlock <size=90%>'{walletName.LimitEnd(17, "...")}'</size>";
	}

	/// <summary>
	/// Adds the button listener and sets up the limit login attempts feature if the setting is enabled
	/// </summary>
	protected override void OnStart()
	{
		unlockWalletButton.onClick.AddListener(CheckPassword);

		if (!SecurePlayerPrefs.GetBool(PlayerPrefConstants.SETTING_LOGIN_ATTEMPTS_LIMIT))
			return;

		if (!SecurePlayerPrefs.HasKey(WalletName + PlayerPrefConstants.SETTING_CURRENT_LOGIN_ATTEMPT))
			SecurePlayerPrefs.SetInt(WalletName + PlayerPrefConstants.SETTING_CURRENT_LOGIN_ATTEMPT, 1);

		if (!SecurePlayerPrefs.HasKey(WalletName + PlayerPrefConstants.SETTING_LAST_FAILED_LOGIN_ATTEMPT))
			SecurePlayerPrefs.SetString(WalletName + PlayerPrefConstants.SETTING_LAST_FAILED_LOGIN_ATTEMPT, DateTimeUtils.GetCurrentUnixTime().ToString());

		UpdatePlaceHolderText();
		TimerChecker().StartCoroutine();
	}

	/// <summary>
	/// Adds the OnWalletLoad method to the UserWallet.OnWalletLoadSuccessful event.
	/// </summary>
	private void OnEnable()
	{
		UserWalletManager.OnWalletLoadSuccessful += OnWalletLoad;
		buttonClickObserver.SubscribeObservable(this);
	}

	/// <summary>
	/// Removes the OnWalletLoad method from the UserWallet.OnWalletLoadSuccessful event.
	/// </summary>
	private void OnDisable()
	{
		UserWalletManager.OnWalletLoadSuccessful -= OnWalletLoad;
		buttonClickObserver.UnsubscribeObservable(this);
		popupClosed?.Invoke();
	}

	/// <summary>
	/// Enables the open wallet gui once the user wallet has been successfully loaded.
	/// </summary>
	private void OnWalletLoad()
	{
		uiManager.OpenMenu<OpenWalletMenu>();
		uiManager.DestroyUnusedMenus();
	}

	/// <summary>
	/// Attempts to unlock the wallet with the password entered in the field.
	/// </summary>
	private void CheckPassword()
	{
		if (passwordField.InputFieldBase.inputType == InputField.InputType.Standard)
			passwordField.EyeClicked();

		walletPasswordVerification.VerifyPassword(passwordField)
								  .OnPasswordCorrect(CorrectPassword)
								  .OnPasswordIncorrect(IncorrectPassword);
	}

	/// <summary>
	/// Called when the correct password is entered.
	/// </summary>
	/// <param name="password"> The correct password. </param>
	private void CorrectPassword(byte[] password)
	{
        if (dynamicDataCache.GetData("pass") != null && dynamicDataCache.GetData("pass") is ProtectedString)
            ((ProtectedString)dynamicDataCache.GetData("pass")).SetValue(password);
        else
            dynamicDataCache.SetData("pass", new ProtectedString(password));

        SecurePlayerPrefs.SetInt(WalletName + PlayerPrefConstants.SETTING_CURRENT_LOGIN_ATTEMPT, 1);

		userWalletManager.UnlockWallet();
	}

	/// <summary>
	/// Called when the incorrect password is entered.
	/// </summary>
	private void IncorrectPassword()
	{
		OnPasswordEnteredIncorrect?.Invoke();

		if (!SecurePlayerPrefs.GetBool(PlayerPrefConstants.SETTING_LOGIN_ATTEMPTS_LIMIT))
			return;

		SecurePlayerPrefs.SetInt(WalletName + PlayerPrefConstants.SETTING_CURRENT_LOGIN_ATTEMPT, SecurePlayerPrefs.GetInt(WalletName + PlayerPrefConstants.SETTING_CURRENT_LOGIN_ATTEMPT) + 1);
		SecurePlayerPrefs.SetString(WalletName + PlayerPrefConstants.SETTING_LAST_FAILED_LOGIN_ATTEMPT, DateTimeUtils.GetCurrentUnixTime().ToString());

		if ((SecurePlayerPrefs.GetInt(PlayerPrefConstants.SETTING_MAX_LOGIN_ATTEMPTS) - SecurePlayerPrefs.GetInt(WalletName + PlayerPrefConstants.SETTING_CURRENT_LOGIN_ATTEMPT) + 1) == 0)
		{
			LockedOut = true;
			AnimateLockedOutSection?.Invoke(true);
		}
		else
		{
			UpdatePlaceHolderText();
		}
	}

	/// <summary>
	/// Updates the password field placeholder text with how many login attempts the user has before being locked out
	/// </summary>
	private void UpdatePlaceHolderText()
	{
		int currentLoginAttempt = SecurePlayerPrefs.GetInt(WalletName + PlayerPrefConstants.SETTING_CURRENT_LOGIN_ATTEMPT);
		int attemptsLeft = SecurePlayerPrefs.GetInt(PlayerPrefConstants.SETTING_MAX_LOGIN_ATTEMPTS) - currentLoginAttempt + 1;

		if (currentLoginAttempt != 1)
		{
			string word = attemptsLeft == 1 ? " try " : " tries ";

			passwordField.SetPlaceholderText("Password (" + attemptsLeft + word + "left)");
		}
	}

	/// <summary>
	/// Moves the cursor to the password input field when tab is pressed.
	/// </summary>
	/// <param name="clickType"> The tab button click type. </param>
	public void TabButtonPressed(ClickType clickType)
	{
		if (clickType != ClickType.Down)
			return;

		if (!walletPasswordVerification.VerifyingPassword)
			passwordField.InputFieldBase.SelectSelectable();
	}

	/// <summary>
	/// Attempts to open the wallet when enter is pressed.
	/// </summary>
	/// <param name="clickType"> The enter button click type. </param>
	public void EnterButtonPressed(ClickType clickType)
	{
		if (clickType != ClickType.Down)
			return;

		if (unlockWalletButton.interactable && !walletPasswordVerification.VerifyingPassword)
			unlockWalletButton.Press();
		else if (!walletPasswordVerification.VerifyingPassword)
			passwordField.InputFieldBase.SelectSelectable();
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
		long.TryParse(SecurePlayerPrefs.GetString(WalletName + PlayerPrefConstants.SETTING_LAST_FAILED_LOGIN_ATTEMPT), out lastFailedAttempt);

		if ((currentTime - lastFailedAttempt) >= 300)
		{
			SecurePlayerPrefs.SetInt(WalletName + PlayerPrefConstants.SETTING_CURRENT_LOGIN_ATTEMPT, 1);
			passwordField.SetPlaceholderText("Password");

			if (LockedOut)
			{
				LockedOut = false;
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
}