using System;
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
	[SerializeField] private GameObject walletNameErrorIcon;
	[SerializeField] private GameObject passwordCheckMarkIcon;
	[SerializeField] private GameObject passwordErrorIcon;

	private GameObject progressBarFull;
	private TextMeshProUGUI passwordStrengthText;

	private CreateWalletMenu createWalletMenu;

	private bool validPassword;
	private bool validWalletName;

	/// <summary>
	/// Initializes the necessary variables that haven't already been initialized in the inspector
	/// </summary>
	private void Awake()
	{
		createWalletMenu = transform.GetComponent<CreateWalletMenu>();

		password1Field.GetComponent<TMP_InputField>().onValueChanged.AddListener(Password1FieldChanged);
		password2Field.GetComponent<TMP_InputField>().onValueChanged.AddListener(Password2FieldChanged);
		walletNameField.GetComponent<TMP_InputField>().onValueChanged.AddListener(WalletNameFieldChanged);

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
	}

	/// <summary>
	/// Checks to see if the wallet name is valid and has not been used before.
	/// </summary>
	/// <param name="walletName"> The text in the wallet name input field. </param>
	private void WalletNameFieldChanged(string walletName)
	{
		if (walletName.Length > 30)
			walletNameField.GetComponent<TMP_InputField>().text = walletName.LimitEnd(30);

		bool emptyName = string.IsNullOrEmpty(walletName.Trim());
		bool usedName = WalletNameExists(walletName);
		validWalletName = !emptyName && !usedName;

		if (emptyName)
			createWalletMenu.WalletNameErrorMessage = "This is not a valid wallet name to save as.";
		else if (usedName)
			createWalletMenu.WalletNameErrorMessage = "You already have a Hope wallet saved under this name.";

		AnimateIcon(walletNameErrorIcon, !string.IsNullOrEmpty(walletName) && !validWalletName);

		SetButtonInteractable();
	}

	/// <summary>
	/// Loops through the saved wallets and checks if a wallet is already saved under the given name.
	/// </summary>
	/// <param name="walletName"> The current wallet name in the input field. </param>
	/// <returns> Whether the given walletName has been used before </returns>
	private bool WalletNameExists(string walletName)
	{
		for (int i = 1; ; i++)
		{
			try
			{
				if (createWalletMenu.UserWalletInfoManager.GetWalletInfo(i).WalletName == walletName)
					return true;
			}
			catch
			{
				return false;
			}
		}
	}

	/// <summary>
	/// Sets the password strength progress bar width, color, and sets the passwrod strength text, and color
	/// </summary>
	/// <param name="password"> The inputted string in the password1Field </param>
	private void Password1FieldChanged(string password)
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

		PasswordsUpdated(password, password2Field.GetComponent<TMP_InputField>().text);
	}

	/// <summary>
	/// Checks to see if the two passwords match, as long as they are not empty
	/// </summary>
	/// <param name="password"> The inputted string in the password2Field </param>
	private void Password2FieldChanged(string password) => PasswordsUpdated(password1Field.GetComponent<TMP_InputField>().text, password);

	/// <summary>
	/// Checks if the passwords valid and animates the error icon if needed
	/// </summary>
	/// <param name="password1"> The text in the first password input field </param>
	/// <param name="password2"> The text in the second password input field </param>
	private void PasswordsUpdated(string password1, string password2)
	{
		bool passwordsMatch = password1 == password2;
		bool validPasswordLength = password1.Length >= AESEncryption.MIN_PASSWORD_LENGTH;

		validPassword = passwordsMatch && validPasswordLength;

		if (!passwordsMatch)
			createWalletMenu.PasswordErrorMessage = "Your passwords do not match.";
		else if (!validPasswordLength)
			createWalletMenu.PasswordErrorMessage = "Your password length must be a minimum of 8 characters. Your password is currently only " + password1.Length + " characters long.";

		AnimateIcon(passwordErrorIcon, !string.IsNullOrEmpty(password2) && !validPassword);
		AnimateIcon(passwordCheckMarkIcon, !string.IsNullOrEmpty(password2) && validPassword);

		SetButtonInteractable();
	}

	/// <summary>
	/// Checks if passwords match, are above 7 characters, and all fields are filled in
	/// </summary>
	private void SetButtonInteractable() => createButton.GetComponent<Button>().interactable = validWalletName && validPassword;

	/// <summary>
	/// Animates an icon in or out of view
	/// </summary>
	/// <param name="icon"> The icon being animated </param>
	/// <param name="animateIn"> Whether the icon is being animated in or out </param>
	private void AnimateIcon(GameObject icon, bool animateIn)
	{
		if (animateIn && icon.GetComponent<InfoMessage>().Hovered) return; 

		icon.AnimateGraphicAndScale(animateIn ? 1f : 0f, animateIn ? 1f : 0f, 0.15f);
	}
}
