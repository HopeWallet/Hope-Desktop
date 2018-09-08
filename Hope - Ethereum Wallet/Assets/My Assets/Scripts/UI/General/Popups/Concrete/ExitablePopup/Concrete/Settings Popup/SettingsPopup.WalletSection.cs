using UnityEngine;
using UnityEngine.UI;

public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	public sealed class WalletSection
	{
		private HopeInputField walletNameField;
		private Button saveButton, deleteButton;
		private GameObject saveButtonText;

		private HopeWalletInfoManager hopeWalletInfoManager;
		private UserWalletManager userWalletManager;

		private string walletName;

		public WalletSection(HopeWalletInfoManager hopeWalletInfoManager,
							 UserWalletManager userWalletManager,
							 HopeInputField walletNameField,
							 Button saveButton,
							 Button deleteButton)
		{
			this.hopeWalletInfoManager = hopeWalletInfoManager;
			this.userWalletManager = userWalletManager;
			this.walletNameField = walletNameField;
			this.saveButton = saveButton;
			this.deleteButton = deleteButton;

			saveButton.onClick.AddListener(SaveButtonClicked);
			deleteButton.onClick.AddListener(DeleteButtonClicked);
			saveButtonText = saveButton.transform.GetChild(0).gameObject;

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

		private void SetSaveButtonInteractable()
		{
			saveButton.interactable = !walletNameField.Error;
			saveButtonText.AnimateColor(saveButton.interactable ? UIColors.White : UIColors.DarkGrey, 0.15f);
		}

		private void SaveButtonClicked()
		{
			//Change wallet name

			walletNameField.Text = string.Empty;
		}

		private void DeleteButtonClicked()
		{

		}
	}
}
