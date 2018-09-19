using Hope.Security.PBKDF2;
using Hope.Security.PBKDF2.Engines.Blake2b;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class ReEnterPasswordMenu : Menu<ReEnterPasswordMenu>
{
    public event Action OnPasswordVerificationStarted;
    public event Action OnPasswordEnteredCorrect;
    public event Action OnPasswordEnteredIncorrect;

	[SerializeField] private TextMeshProUGUI walletName;

	[SerializeField] private Button unlockButton, homeButton;
	[SerializeField] private HopeInputField passwordField;

    private HopeWalletInfoManager.Settings walletSettings;

    private int walletNum;

	[Inject]
	public void Construct(
        HopeWalletInfoManager hopeWalletInfoManager,
        UserWalletManager userWalletManager,
        HopeWalletInfoManager.Settings walletSettings)
	{
        this.walletSettings = walletSettings;

        var walletInfo = hopeWalletInfoManager.GetWalletInfo(userWalletManager.WalletAddress);
        walletName.text = walletInfo.WalletName;
        walletNum = walletInfo.WalletNum + 1;
	}

	private void Awake()
	{
		homeButton.onClick.AddListener(HomeButtonClicked);
		unlockButton.onClick.AddListener(UnlockButtonClicked);

		passwordField.OnInputUpdated += PasswordFieldChanged;
	}

	private void PasswordFieldChanged(string text)
	{
		passwordField.Error = string.IsNullOrEmpty(text);
		unlockButton.interactable = !passwordField.Error;
	}

    private void HomeButtonClicked()
    {
        SceneManager.LoadScene("Hope Wallet");
    }

    private void UnlockButtonClicked()
	{
        OnPasswordVerificationStarted?.Invoke();

        var saltedHash = SecurePlayerPrefs.GetString(walletSettings.walletPasswordPrefName + walletNum);
        var pbkdf2 = new PBKDF2PasswordHashing(new Blake2b_512_Engine());

        Observable.WhenAll(Observable.Start(() => string.IsNullOrEmpty(passwordField.Text) ? false : pbkdf2.VerifyPassword(passwordField.Text, saltedHash)))
                  .ObserveOnMainThread()
                  .Subscribe(correctPass =>
                  {
                      if (!correctPass[0])
                          IncorrectPassword();
                      else
                          CorrectPassword();
                  });
    }

    private void CorrectPassword()
    {
        OnPasswordEnteredCorrect?.Invoke();
        uiManager.CloseMenu();
    }

    private void IncorrectPassword()
    {
        OnPasswordEnteredIncorrect?.Invoke();
    }
}