using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class ReEnterPasswordMenu : Menu<ReEnterPasswordMenu>
{
	[SerializeField] private TextMeshProUGUI walletName;

	[SerializeField] private Button unlockButton, homeButton;
	[SerializeField] private HopeInputField passwordField;

	private ReEnterPasswordMenuAnimator reEnterPasswordMenuAnimator;

	[Inject]
	public void Construct(HopeWalletInfoManager hopeWalletInfoManager, UserWalletManager userWalletManager)
	{
		walletName.text = hopeWalletInfoManager.GetWalletInfo(userWalletManager.WalletAddress).WalletName;
	}

	private void Awake()
	{
		reEnterPasswordMenuAnimator = Animator as ReEnterPasswordMenuAnimator;

		homeButton.onClick.AddListener(HomeButtonClicked);
		unlockButton.onClick.AddListener(UnlockButtonClicked);

		passwordField.OnInputUpdated += PasswordFieldChanged;
	}

	private void PasswordFieldChanged(string text)
	{
		passwordField.Error = string.IsNullOrEmpty(text);
		unlockButton.interactable = !passwordField.Error;
	}

	private void HomeButtonClicked() => uiManager.OpenMenu<ChooseWalletMenu>();

	private void UnlockButtonClicked()
	{
		reEnterPasswordMenuAnimator.VerifyingPassword();

		CoroutineUtils.ExecuteAfterWait(1f, () =>
		{
			bool passwordIsCorrect = true;

			if (passwordIsCorrect)
				uiManager.CloseMenu();
			else
				reEnterPasswordMenuAnimator.PasswordIncorrect();
		});

		//Check if password is correct
		//bool passwordIsCorrect = true;

		//if (passwordIsCorrect)
		//	uiManager.CloseMenu();
		//else
		//	reEnterPasswordMenuAnimator.PasswordIncorrect();
	}
}
