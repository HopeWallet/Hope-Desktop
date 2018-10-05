using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The popup that manages the modification of user's settings and preferences
/// </summary>
public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	/// <summary>
	/// The wallet section where the user can edit the wallet name
	/// </summary>
	public sealed class WalletNameSection
	{
		private HopeInputField currentPasswordField, currentWalletNameField, newWalletNameField;
		private Button saveButton;
		private GameObject loadingIcon;

		private HopeWalletInfoManager hopeWalletInfoManager;
		private UserWalletManager userWalletManager;
		private SettingsPopupAnimator settingsPopupAnimator;

		private string walletName;

		/// <summary>
		/// Sets the necessary variables and dependencies
		/// </summary>
		/// <param name="hopeWalletInfoManager"> The active HopeWalletInfoManager </param>
		/// <param name="userWalletManager"> The active UserWalletManager </param>
		/// <param name="settingsPopupAnimator"> The SettingsPopupAnimator </param>
		/// <param name="currentPasswordField"> The current password input field </param>
		/// <param name="currentWalletNameField"> The current wallet name field </param>
		/// <param name="newWalletNameField"> The new wallet name field </param>
		/// <param name="saveButton"> The save changes button </param>
		/// <param name="loadingIcon"> The loading icon </param>
		public WalletNameSection(HopeWalletInfoManager hopeWalletInfoManager,
								 UserWalletManager userWalletManager,
								 SettingsPopupAnimator settingsPopupAnimator,
								 HopeInputField currentPasswordField,
								 HopeInputField currentWalletNameField,
								 HopeInputField newWalletNameField,
								 Button saveButton,
								 GameObject loadingIcon)
		{
			this.hopeWalletInfoManager = hopeWalletInfoManager;
			this.userWalletManager = userWalletManager;
			this.settingsPopupAnimator = settingsPopupAnimator;
			this.currentPasswordField = currentPasswordField;
			this.currentWalletNameField = currentWalletNameField;
			this.newWalletNameField = newWalletNameField;
			this.saveButton = saveButton;
			this.loadingIcon = loadingIcon;

			
			saveButton.onClick.AddListener(SaveButtonClicked);

			currentPasswordField.OnInputUpdated += CurrentPasswordFieldChanged;
			newWalletNameField.OnInputUpdated += WalletNameFieldChanged;
			walletName = hopeWalletInfoManager.GetWalletInfo(userWalletManager.GetWalletAddress()).WalletName;
			currentWalletNameField.Text = walletName;
			currentWalletNameField.Error = false;
			currentWalletNameField.UpdateVisuals();
		}

		private void CurrentPasswordFieldChanged(string text)
		{
			currentPasswordField.Error = string.IsNullOrEmpty(text);
			saveButton.interactable = !newWalletNameField.Error && !currentPasswordField.Error;
		}

		/// <summary>
		/// Wallet name field has been changed
		/// </summary>
		/// <param name="text"> The current text in the input field</param>
		private void WalletNameFieldChanged(string text)
		{
			bool emptyName = string.IsNullOrEmpty(text.Trim());
			bool usedName = WalletNameExists(text);
			newWalletNameField.Error = emptyName || usedName;

			if (emptyName)
				newWalletNameField.errorMessage.text = "Invalid wallet name";
			else if (usedName)
				newWalletNameField.errorMessage.text = "Wallet name in use";

			saveButton.interactable = !newWalletNameField.Error && !currentPasswordField.Error;
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
                if (hopeWalletInfoManager.GetWalletInfo(i).WalletName.EqualsIgnoreCase(textInField) && walletName != textInField)
                    return true;
                else if (string.IsNullOrEmpty(hopeWalletInfoManager.GetWalletInfo(i).WalletName))
                    return false;
			}
		}

		/// <summary>
		/// The save button is clicked, and the necessary wallet information is changed
		/// </summary>
		private void SaveButtonClicked()
		{
			//Change wallet details

			currentWalletNameField.Text = newWalletNameField.Text;
			newWalletNameField.Text = string.Empty;
		}
	}
}
