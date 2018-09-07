using UnityEngine;
using UnityEngine.UI;

public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	public sealed class WalletSection
	{
		private HopeInputField walletNameField, password1Field, password2Field;
		private Button saveButton, deleteButton;
		private GameObject saveButtonText;

		private HopeWalletInfoManager hopeWalletInfoManager;
		private UserWalletManager userWalletManager;

		private string walletName;

		public WalletSection(HopeWalletInfoManager hopeWalletInfoManager,
							 UserWalletManager userWalletManager,
							 HopeInputField walletNameField,
							 HopeInputField password1Field,
							 HopeInputField password2Field,
							 Button saveButton,
							 Button deleteButton)
		{
			this.hopeWalletInfoManager = hopeWalletInfoManager;
			this.userWalletManager = userWalletManager;
			this.walletNameField = walletNameField;
			this.password1Field = password1Field;
			this.password2Field = password2Field;
			this.saveButton = saveButton;
			this.deleteButton = deleteButton;

			saveButton.onClick.AddListener(SaveButtonClicked);
			deleteButton.onClick.AddListener(DeleteButtonClicked);
			saveButtonText = saveButton.transform.GetChild(0).gameObject;

			password1Field.OnInputUpdated += _ => PasswordsUpdated();
			password2Field.OnInputUpdated += _ => PasswordsUpdated();
			walletNameField.OnInputUpdated += WalletNameFieldChanged;

			walletName = hopeWalletInfoManager.GetWalletInfo(userWalletManager.WalletAddress).WalletName;
			walletNameField.Text = walletName;
		}

		private void WalletNameFieldChanged(string textInField)
		{
			bool emptyName = string.IsNullOrEmpty(textInField.Trim());
			bool usedName = WalletNameExists(textInField);
			walletNameField.Error = emptyName || usedName;

			if (emptyName)
				walletNameField.errorMessage.text = "Invalid wallet name";
			else if (usedName)
				walletNameField.errorMessage.text = "Wallet name in use";

			SetSaveButtonInteractable();
		}

		/// <summary>
		/// Loops through the saved wallets and checks if a wallet is already saved under the given name.
		/// </summary>
		/// <param name="textInField"> The current wallet name in the input field. </param>
		/// <returns> Whether the given walletName has been used before </returns>
		private bool WalletNameExists(string textInField)
		{
			for (int i = 1; ; i++)
			{
				try
				{
					if (hopeWalletInfoManager.GetWalletInfo(i).WalletName.EqualsIgnoreCase(textInField) && walletName != textInField)
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
			string password1Text = password1Field.Text;
			string password2Text = password2Field.Text;

			password1Field.Error = password1Field.Text.Length < 8;
			password2Field.Error = password1Text != password2Text;

			if (password1Field.Error)
				password1Field.errorMessage.text = "Password too short";

			if (password2Field.Error)
				password2Field.errorMessage.text = "Passwords do not match";

			password2Field.UpdateVisuals();
			SetSaveButtonInteractable();
		}

		private void SetSaveButtonInteractable()
		{
			saveButton.interactable = !walletNameField.Error && !password1Field.Error && !password2Field.Error;
			saveButtonText.AnimateColor(saveButton.interactable ? UIColors.White : UIColors.DarkGrey, 0.15f);
		}

		private void SaveButtonClicked()
		{
			//Change wallet name and password

			walletNameField.Text = string.Empty;
			password1Field.Text = string.Empty;
			password2Field.Text = string.Empty;
		}

		private void DeleteButtonClicked()
		{

		}
	}
}
