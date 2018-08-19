using Hope.Security.ProtectedTypes.Types;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Menu which lets the user create a new wallet by first choosing a password and name for the wallet.
/// </summary>
public sealed class CreateWalletMenu : Menu<CreateWalletMenu>
{
	[SerializeField] private Button nextButton;

	[SerializeField] private HopeInputField walletNameField,
											password1Field,
											password2Field;

	private DynamicDataCache dynamicDataCache;
	private UserWalletInfoManager userWalletInfoManager;

	/// <summary>
	/// Adds the required dependencies into this class.
	/// </summary>
	/// <param name="dynamicDataCache"> The active ProtectedStringDataCache. </param>
	/// <param name="userWalletInfoManager"> The active UserWalletInfoManager. </param>
	[Inject]
    public void Construct(DynamicDataCache dynamicDataCache, UserWalletInfoManager userWalletInfoManager)
	{
		this.dynamicDataCache = dynamicDataCache;
		this.userWalletInfoManager = userWalletInfoManager;
	}

	/// <summary>
	/// Adds the button listeners.
	/// </summary>
	protected override void OnAwake()
	{
		password1Field.OnInputUpdated += PasswordsUpdated;
		password2Field.OnInputUpdated += PasswordsUpdated;
		walletNameField.OnInputUpdated += WalletNameFieldChanged;
		nextButton.onClick.AddListener(CreateWalletNameAndPass);
	}

	/// <summary>
	/// Sets up the wallet name and password and opens the next menu.
	/// </summary>
	private void CreateWalletNameAndPass()
    {
        dynamicDataCache.SetData("pass", new ProtectedString(password1Field.Input));
        dynamicDataCache.SetData("name", walletNameField.Input);

        uiManager.OpenMenu<ImportOrCreateMnemonicMenu>();
    }

	/// <summary>
	/// Checks to see if the wallet name is valid and has not been used before.
	/// </summary>
	/// <param name="walletName"> The text in the wallet name input field. </param>
	private void WalletNameFieldChanged()
	{
		string walletName = walletNameField.Input;

		bool emptyName = string.IsNullOrEmpty(walletName.Trim());
		bool usedName = WalletNameExists(walletName);
		walletNameField.Error = emptyName || usedName;

		if (emptyName)
			walletNameField.errorMessage.text = "Invalid wallet name.";
		else if (usedName)
			walletNameField.errorMessage.text = "Wallet name in use.";

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
	/// Checks if the passwords valid and animates the error icon if needed
	/// </summary>
	private void PasswordsUpdated()
	{
		string password1 = password1Field.Input;
		string password2 = password2Field.Input;

		password1Field.Error = password1Field.Input.Length < 8;
		password2Field.Error = password1 != password2;

		if (password1Field.Error)
			password1Field.errorMessage.text = "Password too short.";

		if (password2Field.Error)
			password2Field.errorMessage.text = "Passwords do not match.";

		password2Field.UpdateVisuals(string.IsNullOrEmpty(password2Field.Input));
		SetButtonInteractable();
	}

	/// <summary>
	/// Checks if passwords match, are above 7 characters, and all fields are filled in
	/// </summary>
	private void SetButtonInteractable() => nextButton.interactable = !walletNameField.Error && !password1Field.Error && !password2Field.Error;
}