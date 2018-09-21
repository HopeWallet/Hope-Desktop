using Hope.Security.PBKDF2;
using Hope.Security.PBKDF2.Engines.Blake2b;
using Hope.Security.ProtectedTypes.Types;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class ReEnterPasswordMenu : Menu<ReEnterPasswordMenu>, IEnterButtonObservable
{
    public event Action OnPasswordVerificationStarted;
    public event Action OnPasswordEnteredCorrect;
    public event Action OnPasswordEnteredIncorrect;

    [SerializeField] private TextMeshProUGUI walletName;
	[SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button unlockButton, homeButton;
    [SerializeField] private HopeInputField passwordField;

    private HopeWalletInfoManager.Settings walletSettings;
    private DynamicDataCache dynamicDataCache;
    private ButtonClickObserver buttonClickObserver;

    private bool checkingPassword;
    private int walletNum;

	/// <summary>
	/// Sets the necessary dependencies
	/// </summary>
	/// <param name="hopeWalletInfoManager"> The active HopeWalletInfoManager </param>
	/// <param name="userWalletManager"> The active UserWalletManager </param>
	/// <param name="walletSettings"> The active HopeWalletInfoManager.Settings </param>
	/// <param name="dynamicDataCache"> The active DynamicDataCache</param>
	/// <param name="buttonClickObserver"> The active ButtonClickObserver </param>
    [Inject]
    public void Construct(
        HopeWalletInfoManager hopeWalletInfoManager,
        UserWalletManager userWalletManager,
        HopeWalletInfoManager.Settings walletSettings,
        DynamicDataCache dynamicDataCache,
        ButtonClickObserver buttonClickObserver)
    {
        this.walletSettings = walletSettings;
        this.dynamicDataCache = dynamicDataCache;
        this.buttonClickObserver = buttonClickObserver;

        var walletInfo = hopeWalletInfoManager.GetWalletInfo(userWalletManager.GetWalletAddress());
        walletName.text = walletInfo.WalletName;
        walletNum = walletInfo.WalletNum + 1;

		SetMessageText();

		(dynamicDataCache.GetData("pass") as ProtectedString)?.Dispose();
        dynamicDataCache.SetData("pass", null);
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
	private void OnEnable() => buttonClickObserver.SubscribeObservable(this);

	/// <summary>
	/// Unsubscribes the buttonClickObserver
	/// </summary>
	private void OnDisable() => buttonClickObserver.UnsubscribeObservable(this);

	/// <summary>
	/// Sets the main message text
	/// </summary>
	private void SetMessageText()
	{
		int idleTime = SecurePlayerPrefs.GetInt("idle time");
		string minuteWord;

		if (idleTime == 1)
			minuteWord = " minute";
		else
			minuteWord = " minutes";

		messageText.text = "You have been idle for " + idleTime  + minuteWord + ", please re-enter your password or go back to the main menu.";
	}

	/// <summary>
	/// Password field has been changed
	/// </summary>
	/// <param name="text"> The current text in the input field </param>
	private void PasswordFieldChanged(string text)
    {
        passwordField.Error = string.IsNullOrEmpty(text);
        unlockButton.interactable = !passwordField.Error;
    }

	/// <summary>
	/// Home button has been clicked
	/// </summary>
    private void HomeButtonClicked()
    {
        SceneManager.LoadScene("Hope Wallet");
    }

	/// <summary>
	/// Unlock button has been clicked and password is checked
	/// </summary>
    private void UnlockButtonClicked()
    {
        OnPasswordVerificationStarted?.Invoke();

        var saltedHash = SecurePlayerPrefs.GetString(walletSettings.walletPasswordPrefName + walletNum);
        var pbkdf2 = new PBKDF2PasswordHashing(new Blake2b_512_Engine());

        checkingPassword = true;
        passwordField.InputFieldBase.interactable = false;

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
	/// The password is correct and the user is brought back to the OpenWalletMenu
	/// </summary>
	/// <param name="password"> The password string</param>
    private void CorrectPassword(string password)
    {
        dynamicDataCache.SetData("pass", new ProtectedString(password));

        OnPasswordEnteredCorrect?.Invoke();
		uiManager.CloseMenu();
    }

	/// <summary>
	/// The password is incorrect and the user is given an error
	/// </summary>
    private void IncorrectPassword()
    {
        OnPasswordEnteredIncorrect?.Invoke();
        passwordField.InputFieldBase.interactable = true;
        checkingPassword = false;
    }

	/// <summary>
	/// Clicks the unlockButton if the input field is selected and the unlock button is interactable
	/// </summary>
	/// <param name="clickType"> The enter button ClickType </param>
    public void EnterButtonPressed(ClickType clickType)
    {
        if (clickType != ClickType.Down)
            return;

        if (unlockButton.interactable && !checkingPassword)
            unlockButton.Press();
        else if (!checkingPassword)
            passwordField.InputFieldBase.SelectSelectable();
    }
}