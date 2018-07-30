using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class which animates the UnlockWalletPopup.
/// </summary>
public class UnlockWalletPopupAnimator : UIAnimator
{

	[SerializeField] private Image blur;
	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject passwordInputField;
	[SerializeField] private GameObject signInButton;
	[SerializeField] private GameObject loadingIcon;
	[SerializeField] private GameObject errorIcon;

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
	/// Animates the UI elements of the form into view.
	/// </summary>
	protected override void AnimateIn()
	{
		blur.AnimateMaterialBlur(1f, 0.2f);
		dim.AnimateGraphic(1f, 0.2f);
		form.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => title.AnimateScaleX(1f, 0.1f, 
			() => passwordInputField.AnimateScaleX(1f, 0.1f,
			() => signInButton.AnimateScaleX(1f, 0.1f, FinishedAnimating))));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view.
	/// </summary>
	protected override void AnimateOut()
	{
		title.AnimateScaleX(0f, 0.2f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => { blur.AnimateMaterialBlur(-1f, 0.15f); dim.AnimateGraphic(0f, 0.15f, FinishedAnimating); }));

		passwordInputField.AnimateScaleX(0f, 0.15f);
		signInButton.AnimateScaleX(0f, 0.15f);

		if (errorIconVisible) AnimateErrorIcon(false);
		if (loadingIcon.activeInHierarchy) VerifyingPassword();
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
	/// <param name="animatingIn"> Checks if animating the error icon in or out. </param>
	private void AnimateErrorIcon(bool animatingIn)
	{
		errorIcon.AnimateGraphicAndScale(animatingIn ? 1f : 0f, animatingIn ? 1f : 0f, 0.2f);
		ErrorIconVisible = animatingIn;
	}
}
