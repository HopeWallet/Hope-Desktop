using System;
using TMPro;
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

	[Inject]
	public void Construct(HopeWalletInfoManager hopeWalletInfoManager, UserWalletManager userWalletManager)
	{
		walletName.text = hopeWalletInfoManager.GetWalletInfo(userWalletManager.WalletAddress).WalletName;
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

	private void HomeButtonClicked() => SceneManager.LoadScene("Hope Wallet");

	private void UnlockButtonClicked()
	{
        OnPasswordVerificationStarted?.Invoke();

		CoroutineUtils.ExecuteAfterWait(1f, () =>
		{
			bool passwordIsCorrect = true;

            if (passwordIsCorrect)
                uiManager.CloseMenu();
            else
                OnPasswordEnteredIncorrect?.Invoke();
		});

		//Check if password is correct
		//bool passwordIsCorrect = true;

		//if (passwordIsCorrect)
		//	uiManager.CloseMenu();
		//else
		//	reEnterPasswordMenuAnimator.PasswordIncorrect();
	}
}
