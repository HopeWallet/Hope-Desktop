using Hope.Security.Encryption;
using Hope.Security.ProtectedTypes.Types;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Menu which lets the user create a new wallet by first choosing a password and name for the wallet.
/// </summary>
public sealed class CreateWalletMenu : Menu<CreateWalletMenu>
{
	[SerializeField] private Button createWalletButton, backButton;

	[SerializeField] private TMP_InputField walletNameField, password1Field, password2Field;

	[SerializeField] private InteractableIcon menuInfoIcon,
											  walletNameErrorIcon,
											  passwordErrorIcon,
											  passwordCheckMarkIcon;

	private CreateWalletMenuAnimator createWalletMenuAnimator;
    private DynamicDataCache dynamicDataCache;

	private UserWalletInfoManager userWalletInfoManager;

	private bool validPassword, validWalletName;

	/// <summary>
	/// Adds the required dependencies into this class.
	/// </summary>
	/// <param name="dynamicDataCache"> The active ProtectedStringDataCache. </param>
	/// <param name="userWalletInfoManager"> The active UserWalletInfoManager. </param>
	[Inject]
    public void Construct(DynamicDataCache dynamicDataCache, UserWalletInfoManager userWalletInfoManager, PopupManager popupManager)
	{
		this.dynamicDataCache = dynamicDataCache;
		this.userWalletInfoManager = userWalletInfoManager;

		menuInfoIcon.PopupManager = popupManager;
		walletNameErrorIcon.PopupManager = popupManager;
		passwordErrorIcon.PopupManager = popupManager;
	}

	/// <summary>
	/// Adds the button listeners.
	/// </summary>
	protected override void OnAwake()
	{
		createWalletMenuAnimator = transform.GetComponent<CreateWalletMenuAnimator>();

		backButton.onClick.AddListener(GoBack);
		password1Field.onValueChanged.AddListener(Password1FieldChanged);
		password2Field.onValueChanged.AddListener(Password2FieldChanged);
		walletNameField.onValueChanged.AddListener(WalletNameFieldChanged);
		createWalletButton.onClick.AddListener(CreateWalletNameAndPass);
	}

	/// <summary>
	/// Sets up the wallet name and password and opens the next menu.
	/// </summary>
	private void CreateWalletNameAndPass()
    {
        dynamicDataCache.SetData("pass", new ProtectedString(password1Field.text));
        dynamicDataCache.SetData("name", walletNameField.text);

        uiManager.OpenMenu<ImportOrCreateMnemonicMenu>();
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
			walletNameErrorIcon.infoText = "This is not a valid wallet name to save as.";
		else if (usedName)
			walletNameErrorIcon.infoText = "You already have a Hope wallet saved under this name.";

		createWalletMenuAnimator.AnimateIcon(walletNameErrorIcon, !string.IsNullOrEmpty(walletName) && !validWalletName);

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
				if (userWalletInfoManager.GetWalletInfo(i).WalletName.EqualsIgnoreCase(walletName))
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
		createWalletMenuAnimator.AnimatePasswordStrengthBar(password);
		PasswordsUpdated(password, password2Field.text);
	}

	/// <summary>
	/// Checks to see if the two passwords match, as long as they are not empty
	/// </summary>
	/// <param name="password"> The inputted string in the password2Field </param>
	private void Password2FieldChanged(string password) => PasswordsUpdated(password1Field.text, password);

	/// <summary>
	/// Checks if the passwords valid and animates the error icon if needed
	/// </summary>
	/// <param name="password1"> The text in the first password input field </param>
	/// <param name="password2"> The text in the second password input field </param>
	private void PasswordsUpdated(string password1, string password2)
	{
		bool passwordsMatch = password1 == password2;
		bool validPasswordLength = password1.Length >= PasswordUtils.MIN_LENGTH;

		validPassword = passwordsMatch && validPasswordLength;

		if (!passwordsMatch)
			passwordErrorIcon.infoText = "Your passwords do not match.";
		else if (!validPasswordLength)
			passwordErrorIcon.infoText = "Your password length must be a minimum of 8 characters. Your password is currently only " + password1.Length + " characters long.";

		createWalletMenuAnimator.AnimateIcon(passwordErrorIcon, !string.IsNullOrEmpty(password2) && !validPassword);
		createWalletMenuAnimator.AnimateIcon(passwordCheckMarkIcon, !string.IsNullOrEmpty(password2) && validPassword);

		SetButtonInteractable();
	}

	/// <summary>
	/// Checks if passwords match, are above 7 characters, and all fields are filled in
	/// </summary>
	private void SetButtonInteractable() => createWalletButton.interactable = validWalletName && validPassword;
}