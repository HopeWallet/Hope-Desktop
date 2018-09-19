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

	/// <summary>
	/// Initializes the necessary variables that haven't already been initialized in the inspector
	/// </summary>
	private void Awake()
	{
		passwordInputField.GetComponent<HopeInputField>().OnInputUpdated += _ => InputFieldChanged();
		unlockButton.GetComponent<Button>().onClick.AddListener(VerifyingPassword);
        GetComponent<UnlockWalletPopup>().OnPasswordEnteredIncorrect += PasswordIncorrect;
	}

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		passwordInputField.InputFieldBase.ActivateInputField();
		passwordInputField.gameObject.AnimateScaleX(1f, 0.15f);
		unlockButton.AnimateGraphicAndScale(1f, 1f, 0.25f, FinishedAnimating);
	}

	/// <summary>
	/// Sets the button to interactable if the input field is not empty
	/// </summary>
	private void InputFieldChanged()
	{
		passwordInputField.Error = string.IsNullOrEmpty(passwordInputField.Text);
		unlockButton.GetComponent<Button>().interactable = !passwordInputField.Error;
	}

	/// <summary>
	/// Called if the password is incorrect
	/// </summary>
	public void PasswordIncorrect()
	{
		passwordInputField.Error = true;
		passwordInputField.UpdateVisuals();
		unlockButton.GetComponent<Button>().interactable = false;
		VerifyingPassword();
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
