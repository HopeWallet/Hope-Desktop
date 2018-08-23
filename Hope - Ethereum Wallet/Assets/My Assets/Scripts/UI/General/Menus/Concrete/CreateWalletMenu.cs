using Hope.Security.ProtectedTypes.Types;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Menu which lets the user create a new wallet by first choosing a password and name for the wallet.
/// </summary>
public sealed class CreateWalletMenu : Menu<CreateWalletMenu>, IEnterButtonObservable, ITabButtonObservable
{
	[SerializeField] private Button nextButton;

	[SerializeField]
	private HopeInputField walletNameField,
											password1Field,
											password2Field;

	private DynamicDataCache dynamicDataCache;
	private UserWalletInfoManager userWalletInfoManager;
	private ButtonClickObserver buttonClickObserver;

	private List<Selectable> inputFields = new List<Selectable>();

	/// <summary>
	/// Adds the required dependencies into this class.
	/// </summary>
	/// <param name="dynamicDataCache"> The active ProtectedStringDataCache. </param>
	/// <param name="userWalletInfoManager"> The active UserWalletInfoManager. </param>
	/// <param name="buttonClickObserver"> The active ButtonClickObserver. </param>
	[Inject]
	public void Construct(DynamicDataCache dynamicDataCache,
						  UserWalletInfoManager userWalletInfoManager,
						  ButtonClickObserver buttonClickObserver)
	{
		this.dynamicDataCache = dynamicDataCache;
		this.userWalletInfoManager = userWalletInfoManager;
		this.buttonClickObserver = buttonClickObserver;
	}

	/// <summary>
	/// Subscribes the the current buttonClickObserver
	/// </summary>
	private void OnEnable() => buttonClickObserver.SubscribeObservable(this);

	/// <summary>
	/// Unsubscribes the the current buttonClickObserver
	/// </summary>
	private void OnDisable() => buttonClickObserver.UnsubscribeObservable(this);

	/// <summary>
	/// Adds the button listeners.
	/// </summary>
	protected override void OnAwake()
	{
		inputFields.Add(walletNameField.inputFieldBase);
		inputFields.Add(password1Field.inputFieldBase);
		inputFields.Add(password2Field.inputFieldBase);

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
		dynamicDataCache.SetData("pass", new ProtectedString(password1Field.Text));
		dynamicDataCache.SetData("name", walletNameField.Text);

		uiManager.OpenMenu<ImportOrCreateMnemonicMenu>();
	}

	/// <summary>
	/// Checks to see if the wallet name is valid and has not been used before.
	/// </summary>
	/// <param name="walletName"> The text in the wallet name input field. </param>
	private void WalletNameFieldChanged()
	{
		string walletName = walletNameField.Text;

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
		string password1 = password1Field.Text;
		string password2 = password2Field.Text;

		password1Field.Error = password1Field.Text.Length < 8;
		password2Field.Error = password1 != password2;

		if (password1Field.Error)
			password1Field.errorMessage.text = "Password too short.";

		if (password2Field.Error)
			password2Field.errorMessage.text = "Passwords do not match.";

		password2Field.UpdateVisuals();
		SetButtonInteractable();
	}

	/// <summary>
	/// Checks if passwords match, are above 7 characters, and all fields are filled in
	/// </summary>
	private void SetButtonInteractable() => nextButton.interactable = !walletNameField.Error && !password1Field.Error && !password2Field.Error;

	public void EnterButtonPressed(ClickType clickType)
	{
		if (clickType != ClickType.Down)
			return;

		if (InputFieldUtils.GetActiveInputField() == password2Field.inputFieldBase && nextButton.interactable)
			nextButton.Press();
		else
			SelectableExtensions.MoveToNextSelectable(inputFields);
	}

	public void TabButtonPressed(ClickType clickType)
	{
		if (clickType != ClickType.Down)
			return;

		SelectableExtensions.MoveToNextSelectable(inputFields);
	}
}