using Hope.Security.PBKDF2;
using Hope.Security.PBKDF2.Engines.Blake2b;
using Hope.Security.ProtectedTypes.Types;
using System;
using System.Collections;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Popup used for entering the password to unlock a specific wallet.
/// </summary>
public sealed class UnlockWalletPopup : ExitablePopupComponent<UnlockWalletPopup>, ITabButtonObservable, IEnterButtonObservable
{
	public Action OnPasswordEnteredIncorrect;
	public Action<bool> AnimateLockedOutSection;

	private Action popupClosed;

	[SerializeField] private Button unlockWalletButton;

	[SerializeField] private HopeInputField passwordField;

	[SerializeField] private TextMeshProUGUI timerText;

	private readonly WaitForSeconds waiter = new WaitForSeconds(1f);

	/// <summary>
	/// If the user is locked out from logging in or not
	/// </summary>
	public bool LockedOut { get; set; }

	private UIManager uiManager;
	private UserWalletManager userWalletManager;
	private HopeWalletInfoManager.Settings walletSettings;
	private DynamicDataCache dynamicDataCache;
	private ButtonClickObserver buttonClickObserver;

	private bool checkingPassword;

	/// <summary>
	/// Adds the required dependencies to this popup.
	/// </summary>
	/// <param name="uiManager"> The active UIManager. </param>
	/// <param name="userWalletManager"> The active UserWalletManager. </param>
	/// <param name="walletSettings"> The wallet settings. </param>
	/// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
	/// <param name="buttonClickObserver"> The active ButtonClickObserver. </param>
	[Inject]
	public void Construct(
		UIManager uiManager,
		UserWalletManager userWalletManager,
		HopeWalletInfoManager.Settings walletSettings,
		DynamicDataCache dynamicDataCache,
		ButtonClickObserver buttonClickObserver)
	{
		this.uiManager = uiManager;
		this.userWalletManager = userWalletManager;
		this.dynamicDataCache = dynamicDataCache;
		this.buttonClickObserver = buttonClickObserver;
		this.walletSettings = walletSettings;

		if (SecurePlayerPrefs.GetBool("limit login attempts"))
		{
			UpdatePlaceHolderText();
			TimerChecker().StartCoroutine();
		}
	}

	/// <summary>
	/// Updates the password field placeholder text with how many login attempts the user has before being locked out
	/// </summary>
	private void UpdatePlaceHolderText()
	{
		int currentLoginAttempt = SecurePlayerPrefs.GetInt("current login attempt");
		int attemptsLeft = SecurePlayerPrefs.GetInt("max login attempts") - currentLoginAttempt + 1;

		if (currentLoginAttempt != 1)
		{
			string word = attemptsLeft == 1 ? " try " : " tries ";

			passwordField.SetPlaceholderText("Password (" + attemptsLeft + word + "left)");
		}
	}

	/// <summary>
	/// Updates the timer text and checks to see if the user can be granted access to the password field again
	/// </summary>
	/// <returns> Returns one WaitForSeconds </returns>
	private IEnumerator TimerChecker()
	{
		long currentTime, lastFailedAttempt;
		long.TryParse(DateTimeUtils.GetCurrentUnixTime().ToString(), out currentTime);
		long.TryParse(SecurePlayerPrefs.GetString("last failed login attempt"), out lastFailedAttempt);

		if ((currentTime - lastFailedAttempt) >= 300)
		{
			SecurePlayerPrefs.SetInt("current login attempt", 1);

			if (LockedOut)
			{
				LockedOut = false;
				passwordField.SetPlaceholderText("Password");
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
	/// Sets the popupClosed action to be called when the popup is closed
	/// </summary>
	/// <param name="popupClosed"> The finishing action </param>
	public void SetOnCloseAction(Action popupClosed) => this.popupClosed = popupClosed;

	/// <summary>
	/// Adds the button listener.
	/// </summary>
	protected override void OnStart()
	{
		unlockWalletButton.onClick.AddListener(CheckPassword);
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
		string text = passwordField.Text;

		var saltedHash = SecurePlayerPrefs.GetString(walletSettings.walletPasswordPrefName + (int)dynamicDataCache.GetData("walletnum"));
		var pbkdf2 = new PBKDF2PasswordHashing(new Blake2b_512_Engine());

		passwordField.InputFieldBase.interactable = false;
		checkingPassword = true;

		Observable.WhenAll(Observable.Start(() => string.IsNullOrEmpty(passwordField.Text) ? false : pbkdf2.VerifyPassword(passwordField.Text, saltedHash)))
				  .ObserveOnMainThread()
				  .Subscribe(correctPass =>
				  {
					  if (!correctPass[0])
						  IncorrectPassword();
					  else
						  CorrectPassword(passwordField.Text);
				  });
	}

	/// <summary>
	/// Called when the correct password is entered.
	/// </summary>
	/// <param name="password"> The correct password. </param>
	private void CorrectPassword(string password)
	{
		if (dynamicDataCache.GetData("pass") != null && dynamicDataCache.GetData("pass") is ProtectedString)
			((ProtectedString)dynamicDataCache.GetData("pass")).SetValue(password);
		else
			dynamicDataCache.SetData("pass", new ProtectedString(password));

		SecurePlayerPrefs.SetInt("current login attempt", 1);

		userWalletManager.UnlockWallet();
	}

	/// <summary>
	/// Called when the incorrect password is entered.
	/// </summary>
	private void IncorrectPassword()
	{
		OnPasswordEnteredIncorrect?.Invoke();
		passwordField.InputFieldBase.interactable = true;
		checkingPassword = false;

		if (!SecurePlayerPrefs.GetBool("limit login attempts"))
			return;

		SecurePlayerPrefs.SetInt("current login attempt", SecurePlayerPrefs.GetInt("current login attempt") + 1);

		if ((SecurePlayerPrefs.GetInt("max login attempts") - SecurePlayerPrefs.GetInt("current login attempt") + 1) == 0)
		{
			LockedOut = true;
			AnimateLockedOutSection(true);
		}
		else
		{
			SecurePlayerPrefs.SetString("last failed login attempt", DateTimeUtils.GetCurrentUnixTime().ToString());
			UpdatePlaceHolderText();
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

		if (!checkingPassword)
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

		if (unlockWalletButton.interactable && !checkingPassword)
			unlockWalletButton.Press();
		else if (!checkingPassword)
			passwordField.InputFieldBase.SelectSelectable();
	}
}