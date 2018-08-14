using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class which animates the UnlockWalletPopup.
/// </summary>
public class UnlockWalletPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject passwordInputField;
	[SerializeField] private GameObject signInButton;
	[SerializeField] private GameObject loadingIcon;
	[SerializeField] private InteractableIcon passwordErrorIcon;

	private bool errorIconVisible;

	/// <summary>
	/// Makes button interactable if the errorIcon is set to visible.
	/// </summary>
	private bool ErrorIconVisible
	{
		set
		{
			errorIconVisible = value;
			if (errorIconVisible) signInButton.GetComponent<Button>().interactable = false;
		}
	}

	/// <summary>
	/// Initializes the necessary variables that haven't already been initialized in the inspector.
	/// </summary>
	private void Awake()
	{
		passwordInputField.GetComponent<TMP_InputField>().onValueChanged.AddListener(InputFieldChanged);
		passwordInputField.GetComponent<TMP_InputField>().text = "";
	}

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		passwordInputField.AnimateScaleX(1f, 0.15f);
		signInButton.AnimateGraphicAndScale(1f, 1f, 0.25f, FinishedAnimating);
	}

	/// <summary>
	/// Sets the button to interactable if the input field is not empty.
	/// </summary>
	/// <param name="str"> The current string in the password input field. </param>
	private void InputFieldChanged(string str)
	{
		signInButton.GetComponent<Button>().interactable = !string.IsNullOrEmpty(str);

		if (errorIconVisible) AnimateErrorIcon(false);
	}

	/// <summary>
	/// Called if the password is incorrect
	/// </summary>
	public void PasswordIncorrect()
	{
		AnimateErrorIcon(true);
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
			passwordInputField.GetComponent<TMP_InputField>().interactable = false;
            Animating = true;
		}

		loadingIcon.AnimateGraphicAndScale(startingProcess ? 1f : 0f, startingProcess ? 1f : 0f, 0.2f);

		if (!startingProcess)
		{
			loadingIcon.SetActive(false);
			passwordInputField.GetComponent<TMP_InputField>().interactable = true;
			Animating = false;
		}
	}

	/// <summary>
	/// Animates the error icon on or off screen.
	/// </summary>
	/// <param name="animateIn"> Checks if animating the error icon in or out. </param>
	private void AnimateErrorIcon(bool animateIn)
	{
		passwordErrorIcon.AnimateIcon(animateIn ? 1f : 0f);
		ErrorIconVisible = animateIn;
	}
}
