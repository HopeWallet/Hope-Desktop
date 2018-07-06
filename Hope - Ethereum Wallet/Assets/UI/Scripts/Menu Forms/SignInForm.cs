using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignInForm : FormAnimation
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
	protected override void InitializeElements()
	{
		form.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(ExitButtonClicked);
		passwordInputField.GetComponent<TMP_InputField>().onValueChanged.AddListener(InputFieldChanged);
		signInButton.GetComponent<Button>().onClick.AddListener(SignInAttempt);
		passwordInputField.GetComponent<TMP_InputField>().text = "";

		//title.GetComponent<TextMeshProUGUI>().text = walletName;
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
			() => signInButton.AnimateScaleX(1f, 0.1f, FinishedAnimatingIn))));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		title.AnimateScaleX(0f, 0.2f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.15f, FinishedAnimatingOut));

		passwordInputField.AnimateScaleX(0f, 0.15f);
		signInButton.AnimateScaleX(0f, 0.15f);
	}

	/// <summary>
	/// Exit button is clicked
	/// </summary>
	private void ExitButtonClicked() => DisableMenu();

	/// <summary>
	/// Sets the button to interactable if the input field is not empty
	/// </summary>
	/// <param name="str"> The current string in the password input field </param>
	private void InputFieldChanged(string str)
	{
		signInButton.GetComponent<Button>().interactable = str != "" ? true : false;

		if (errorIconVisible) AnimateErrorIcon(false);
	}

	/// <summary>
	/// Checks to see if password entered is correct
	/// </summary>
	private void SignInAttempt()
	{
		//if (passwordIsCorrect)
		//	DisableMenu();
		//else
		AnimateErrorIcon(true);
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
}
