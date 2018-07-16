using Hope.Security.Encryption;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class which animates the CreateWalletMenu.
/// </summary>
public class CreateWalletMenuAnimator : UIAnimator
{

	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject walletNameField;
	[SerializeField] private GameObject passwordHeader;
	[SerializeField] private GameObject passwordStrength;
	[SerializeField] private GameObject passwordStrengthProgressBar;
	[SerializeField] private GameObject password1Field;
	[SerializeField] private GameObject password2Field;
	[SerializeField] private GameObject createButton;
	[SerializeField] private GameObject checkMarkIcon;
	[SerializeField] private GameObject errorIcon;

	private GameObject progressBarFull;
	private TextMeshProUGUI passwordStrengthText;

	private string walletNameText, password1Text, password2Text;

	/// <summary>
	/// Initializes the necessary variables that haven't already been initialized in the inspector
	/// </summary>
	private void Awake()
	{
		InitializeVariable(ref walletNameField);
		InitializeVariable(ref password1Field);
		InitializeVariable(ref password2Field);

		password1Field.GetComponent<TMP_InputField>().onValueChanged.AddListener(GetPasswordStrength);

		progressBarFull = passwordStrengthProgressBar.transform.GetChild(1).gameObject;
		passwordStrengthText = passwordStrength.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
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
			() => passwordStrength.AnimateScaleX(1f, 0.15f,
			() => password1Field.AnimateScaleX(1f, 0.15f,
			() => password2Field.AnimateScaleX(1f, 0.15f, FinishedAnimating)))));

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
		passwordStrength.AnimateScaleX(0f, 0.15f);
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
		inputField.onValueChanged.AddListener(_ => SetText());
		inputField.onValueChanged.AddListener(_ => SetButtonInteractable());
	}

	/// <summary>
	/// Sets the string variables to the current component's text
	/// </summary>
	private void SetText()
	{
		walletNameText = walletNameField.GetComponent<TMP_InputField>().text.Trim();
		password1Text = password1Field.GetComponent<TMP_InputField>().text;
		password2Text = password2Field.GetComponent<TMP_InputField>().text;
	}

	/// <summary>
	/// Sets the password strength progress bar width, color, and sets the passwrod strength text, and color
	/// </summary>
	/// <param name="password"> The string in the password1Field </param>
	private void GetPasswordStrength(string password)
	{
		passwordStrengthProgressBar.AnimateTransformX(string.IsNullOrEmpty(password) ? 0f : -35f, 0.15f);
		passwordStrengthText.gameObject.AnimateGraphic(string.IsNullOrEmpty(password) ? 0f : 1f, 0.15f);

		float lengthPercentage = 0.05f * password.Length;
		float colorPercentage = lengthPercentage >= 0.5f ? (lengthPercentage - 0.5f) * 2f : lengthPercentage * 2f;

		Color strengthColor = Color.Lerp(lengthPercentage >= 0.5f ? UIColors.Yellow : UIColors.Red, lengthPercentage >= 0.5f ? UIColors.Green : UIColors.Yellow, colorPercentage);

		if (lengthPercentage <= 1f)
		{
			progressBarFull.AnimateScaleX(lengthPercentage, 0.1f);
			progressBarFull.GetComponent<Image>().color = strengthColor;
		}

		passwordStrengthText.text = lengthPercentage == 0f ? "" : lengthPercentage < 0.4f ? "Too Short" : lengthPercentage < 0.55f ? "Weak" : lengthPercentage < 0.7f ? "Fair" : lengthPercentage < 0.85f ? "Strong" : "Very Strong";
	}

	/// <summary>
	/// Checks if passwords match, are above 7 characters, and all fields are filled in
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
