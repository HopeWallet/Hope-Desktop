using Hope.Security.Encryption;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class which animates the CreateWalletMenu.
/// </summary>
public class CreateWalletMenuAnimator : MenuAnimator
{

	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject walletNameField;
	[SerializeField] private GameObject passwordHeader;
	[SerializeField] private GameObject password1Field;
	[SerializeField] private GameObject password2Field;
	[SerializeField] private GameObject createButton;
	[SerializeField] private GameObject checkMarkIcon;
	[SerializeField] private GameObject errorIcon;

	private string walletNameText, password1Text, password2Text;

	/// <summary>
	/// Initializes the necessary variables that haven't already been initialized in the inspector
	/// </summary>
	private void Awake()
	{
		InitializeVariable(ref walletNameField);
		InitializeVariable(ref password1Field);
		InitializeVariable(ref password2Field);
	}

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		form.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => title.AnimateGraphicAndScale(0.85f, 1f, 0.2f,
			() => createButton.AnimateGraphicAndScale(1f, 1f, 0.2f)));

		walletNameField.AnimateScaleX(1f, 0.15f,
			() => passwordHeader.AnimateScaleX(1f, 0.15f,
			() => password1Field.AnimateScaleX(1f, 0.15f,
			() => password2Field.AnimateScaleX(1f, 0.15f, FinishedAnimating))));

	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		createButton.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => title.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.15f, FinishedAnimating)));

		walletNameField.AnimateScaleX(0f, 0.15f);
		passwordHeader.AnimateScaleX(0f, 0.15f);
		password1Field.AnimateScaleX(0f, 0.15f);
		password2Field.AnimateScaleX(0f, 0.15f);

		errorIcon.AnimateGraphic(0f, 0.1f);
		checkMarkIcon.AnimateGraphic(0f, 0.1f);
	}

	/// <summary>
	/// Adds listeners to a given input field
	/// </summary>
	/// <param name="gameObject"> The gameobject input field </param>
	private void InitializeVariable(ref GameObject gameObject)
	{
		TMP_InputField inputField = gameObject.GetComponent<TMP_InputField>();
		inputField.onValueChanged.AddListener(str => SetText());
		inputField.onValueChanged.AddListener(str => SetButtonInteractable());
	}

	/// <summary>
	/// Sets the string variables to the current component's text
	/// </summary>
	private void SetText()
	{
		walletNameText = walletNameField.GetComponent<TMP_InputField>().text;
		password1Text = password1Field.GetComponent<TMP_InputField>().text;
		password2Text = password2Field.GetComponent<TMP_InputField>().text;
	}

	/// <summary>
	/// Checks if passwords match, are above 7 characters, and all fieldss are filled in
	/// </summary>
	private void SetButtonInteractable()
	{
		bool passwordsValid = password1Text == password2Text && password1Text.Length >= AESEncryption.MIN_PASSWORD_LENGTH;

		if (!string.IsNullOrEmpty(password1Text) && !string.IsNullOrEmpty(password2Text))
		{
			errorIcon.AnimateGraphic(passwordsValid ? 0f : 1f, 0.25f);
			checkMarkIcon.AnimateGraphic(passwordsValid ? 1f : 0f, 0.25f);
		}
		else
		{
			errorIcon.AnimateGraphic(0f, 0.25f);
			checkMarkIcon.AnimateGraphic(0f, 0.25f);
		}

		createButton.GetComponent<Button>().interactable = !string.IsNullOrEmpty(walletNameText) && passwordsValid;
	}
}
