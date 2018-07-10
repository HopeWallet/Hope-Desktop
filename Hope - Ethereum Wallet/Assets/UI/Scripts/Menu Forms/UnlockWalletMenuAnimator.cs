using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockWalletMenuAnimator : MenuAnimator
{

	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject passwordInputField;
	[SerializeField] private GameObject signInButton;
	[SerializeField] private GameObject errorIcon;

	private bool errorIconVisible;

	/// <summary>
	/// Makes button interactable if the errorIcon is set to visible
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
	/// Initializes the necessary variables that haven't already been initialized in the inspector
	/// </summary>
	private void Awake()
	{
		passwordInputField.GetComponent<TMP_InputField>().onValueChanged.AddListener(InputFieldChanged);
		signInButton.GetComponent<Button>().onClick.AddListener(SignInClicked);

		passwordInputField.GetComponent<TMP_InputField>().text = "";
	}

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		dim.AnimateGraphic(1f, 0.2f);
		form.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => title.AnimateScaleX(1f, 0.1f, 
			() => passwordInputField.AnimateScaleX(1f, 0.1f,
			() => signInButton.AnimateScaleX(1f, 0.1f, FinishedAnimating))));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		title.AnimateScaleX(0f, 0.2f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.15f, FinishedAnimating));

		passwordInputField.AnimateScaleX(0f, 0.15f);
		signInButton.AnimateScaleX(0f, 0.15f);
	}

	/// <summary>
	/// Animates the error icon in or out of view
	/// </summary>
	/// <param name="animatingIn"> Checks if animating the icon in or out </param>
	private void AnimateErrorIcon(bool animatingIn)
	{
		Animating = true;

		errorIcon.AnimateGraphicAndScale(animatingIn ? 1f : 0f, animatingIn ? 1f : 0f, 0.2f, () => Animating = false);

		ErrorIconVisible = animatingIn;
	}

	/// <summary>
	/// Sets the button to interactable if the input field is not empty
	/// </summary>
	/// <param name="str"> The current string in the password input field </param>
	private void InputFieldChanged(string str)
	{
		signInButton.GetComponent<Button>().interactable = !string.IsNullOrEmpty(str);

		if (errorIconVisible) AnimateErrorIcon(false);
	}

	/// <summary>
	/// Checks to see if password entered is correct
	/// </summary>
	private void SignInClicked()
	{
		//if (passwordIsCorrect)
		//{
		//	DisableMenu();
		//	walletListForm.DisableWalletListForm();
		// 	//GO TO NEXT FORM
		//else
		//	AnimateErrorIcon(true);
	}
}
