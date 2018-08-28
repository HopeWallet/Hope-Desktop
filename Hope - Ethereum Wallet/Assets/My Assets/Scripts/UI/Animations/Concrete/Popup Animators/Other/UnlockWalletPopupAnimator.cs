using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class which animates the UnlockWalletPopup.
/// </summary>
public class UnlockWalletPopupAnimator : UIAnimator
{
	[SerializeField] private HopeInputField passwordInputField;
	[SerializeField] private GameObject signInButton;
	[SerializeField] private GameObject loadingIcon;

    /// <summary>
    /// Initializes the necessary variables that haven't already been initialized in the inspector.
    /// </summary>
    private void Awake()
    {
        passwordInputField.GetComponent<HopeInputField>().OnInputUpdated += InputFieldChanged;
        signInButton.GetComponent<Button>().onClick.AddListener(VerifyingPassword);
    }

    /// <summary>
    /// Animates the unique elements of this form into view
    /// </summary>
    protected override void AnimateUniqueElementsIn()
	{
		passwordInputField.inputFieldBase.ActivateInputField();
		passwordInputField.gameObject.AnimateScaleX(1f, 0.15f);
		signInButton.AnimateGraphicAndScale(1f, 1f, 0.25f, FinishedAnimating);
	}

	/// <summary>
	/// Sets the button to interactable if the input field is not empty.
	/// </summary>
	private void InputFieldChanged()
	{
		passwordInputField.Error = string.IsNullOrEmpty(passwordInputField.Text);
		signInButton.GetComponent<Button>().interactable = !passwordInputField.Error;
	}

	/// <summary>
	/// Called if the password is incorrect
	/// </summary>
	public void PasswordIncorrect()
	{
		passwordInputField.Error = true;
		passwordInputField.UpdateVisuals();
		signInButton.GetComponent<Button>().interactable = false;
		VerifyingPassword();
	}

	/// <summary>
	/// Animates the loadingIcon while starting and finished the verify password.
	/// </summary>
	/// <param name="startingProcess"> Checks if animating in or out. </param>
	public void VerifyingPassword()
	{
		bool startingProcess = !loadingIcon.activeInHierarchy;

		if (startingProcess)
		{
			loadingIcon.SetActive(true);
            Animating = true;
			passwordInputField.inputFieldBase.DeactivateInputField();
		}

		loadingIcon.AnimateGraphicAndScale(startingProcess ? 1f : 0f, startingProcess ? 1f : 0f, 0.15f);
		signInButton.AnimateGraphicAndScale(startingProcess ? 0f : 1f, startingProcess ? 0f : 1f, 0.15f);

		if (!startingProcess)
		{
			loadingIcon.SetActive(false);
			Animating = false;
		}
	}
}
