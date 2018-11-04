using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The animator class of the UnlockWalletPopup
/// </summary>
public class UnlockWalletPopupAnimator : PopupAnimator
{
	[SerializeField] private HopeInputField passwordInputField;
	[SerializeField] private GameObject unlockButton;
	[SerializeField] private GameObject loadingIcon;
	[SerializeField] private GameObject lockedOutSection;

	private UnlockWalletPopup unlockWalletPopup;

	/// <summary>
	/// Initializes the necessary variables that haven't already been initialized in the inspector
	/// </summary>
	private void Awake()
	{
		unlockWalletPopup = GetComponent<UnlockWalletPopup>();

		unlockWalletPopup.AnimateLockedOutSection += AnimateLockedOutSection;
		unlockWalletPopup.OnPasswordEnteredIncorrect += PasswordIncorrect;

		passwordInputField.GetComponent<HopeInputField>().OnInputUpdated += _ => InputFieldChanged();
		unlockButton.GetComponent<Button>().onClick.AddListener(VerifyingPassword);
	}

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
        bool hasMaxLoginPref = SecurePlayerPrefs.HasKey(PlayerPrefConstants.SETTING_MAX_LOGIN_ATTEMPTS);
        bool hasCurrentLoginAttemptPref = SecurePlayerPrefs.HasKey(unlockWalletPopup.WalletName + PlayerPrefConstants.SETTING_CURRENT_LOGIN_ATTEMPT);

		if (hasMaxLoginPref && hasCurrentLoginAttemptPref && !string.IsNullOrEmpty(unlockWalletPopup.WalletName) && (SecurePlayerPrefs.GetInt(PlayerPrefConstants.SETTING_MAX_LOGIN_ATTEMPTS) - SecurePlayerPrefs.GetInt(unlockWalletPopup.WalletName + PlayerPrefConstants.SETTING_CURRENT_LOGIN_ATTEMPT) + 1) == 0)
		{
			lockedOutSection.AnimateScale(1f, 0.15f);
			unlockWalletPopup.LockedOut = true;
		}
		else
		{
			passwordInputField.InputFieldBase.ActivateInputField();
			passwordInputField.gameObject.AnimateScaleX(1f, 0.15f);
		}

		unlockButton.AnimateGraphicAndScale(1f, 1f, 0.25f, FinishedAnimating);
	}

	/// <summary>
	/// Sets the button to interactable if the input field is not empty
	/// </summary>
	private void InputFieldChanged()
	{
		passwordInputField.Error = string.IsNullOrEmpty(passwordInputField.InputFieldBase.text);
		unlockButton.GetComponent<Button>().interactable = !passwordInputField.Error;
	}

	/// <summary>
	/// Called if the password is incorrect
	/// </summary>
	private void PasswordIncorrect()
	{
		passwordInputField.Error = true;
		passwordInputField.UpdateVisuals();
		unlockButton.GetComponent<Button>().interactable = false;
		VerifyingPassword();
	}

	/// <summary>
	/// Animates the locked out section in or out of view
	/// </summary>
	/// <param name="userLockedOut"> Whether the locked out section should be shown or not </param>
	private void AnimateLockedOutSection(bool userLockedOut)
	{
		if (userLockedOut)
			passwordInputField.gameObject.AnimateScaleX(0f, 0.15f, () => lockedOutSection.AnimateScale(1f, 0.15f));
		else
			lockedOutSection.gameObject.AnimateScale(0f, 0.15f, () => passwordInputField.gameObject.AnimateScaleX(1f, 0.15f));
	}

	/// <summary>
	/// Animates the loadingIcon while in or out of view
	/// </summary>
	private void VerifyingPassword()
	{
		bool startingProcess = !loadingIcon.activeInHierarchy;

		if (startingProcess)
		{
			loadingIcon.SetActive(true);
			Animating = true;
			unlockButton.AnimateGraphicAndScale(0f, 0f, 0.15f, () => loadingIcon.AnimateGraphicAndScale(1f, 1f, 0.15f));
			passwordInputField.InputFieldBase.DeactivateInputField();
		}

		if (!startingProcess)
		{
			loadingIcon.AnimateGraphicAndScale(0f, 0f, 0.15f, () =>
			{
				loadingIcon.SetActive(false);
				unlockButton.AnimateGraphicAndScale(1f, 1f, 0.15f);
				Animating = false;
			});
		}
	}
}
