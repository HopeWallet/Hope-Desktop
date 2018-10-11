using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The popup that manages the modification of user's settings and preferences
/// </summary>
public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	/// <summary>
	/// The modify wallet section where the user can edit the wallet name, change the password, or delete their wallet
	/// </summary>
	public sealed class ModifyWalletSection
	{
		private readonly GameObject currentPasswordSection, walletDetailsSection, loadingIcon;
		private readonly HopeInputField currentPasswordField, currentWalletNameField, newWalletNameField, newPasswordField, confirmPasswordField;
		private readonly Button nextButton, saveWalletNameButton, savePasswordButton, deleteWalletButton;

		private readonly HopeWalletInfoManager hopeWalletInfoManager;
        private readonly HopeWalletInfoManager.Settings walletSettings;
        private readonly WalletPasswordVerification walletPasswordVerification;
        private readonly DynamicDataCache dynamicDataCache;
		private readonly SettingsPopupAnimator settingsPopupAnimator;

		private readonly string walletName;

		public ModifyWalletSection(
            HopeWalletInfoManager hopeWalletInfoManager,
            HopeWalletInfoManager.Settings walletSettings,
            WalletPasswordVerification walletPasswordVerification,
            DynamicDataCache dynamicDataCache,
            UserWalletManager userWalletManager,
            SettingsPopupAnimator settingsPopupAnimator,
            PopupManager popupManager,
            LogoutHandler logoutHandler,
            GameObject currentPasswordSection,
            GameObject walletDetailsSection,
            GameObject loadingIcon,
            HopeInputField currentPasswordField,
            HopeInputField currentWalletNameField,
            HopeInputField newWalletNameField,
            HopeInputField newPasswordField,
            HopeInputField confirmPasswordField,
            Button nextButton,
            Button saveWalletNameButton,
            Button savePasswordButton,
            Button deleteWalletButton)
        {
            this.hopeWalletInfoManager = hopeWalletInfoManager;
            this.walletSettings = walletSettings;
            this.walletPasswordVerification = walletPasswordVerification;
            this.dynamicDataCache = dynamicDataCache;
			this.settingsPopupAnimator = settingsPopupAnimator;
			this.currentPasswordSection = currentPasswordSection;
			this.walletDetailsSection = walletDetailsSection;
			this.loadingIcon = loadingIcon;
			this.currentPasswordField = currentPasswordField;
			this.currentWalletNameField = currentWalletNameField;
			this.newWalletNameField = newWalletNameField;
			this.newPasswordField = newPasswordField;
			this.confirmPasswordField = confirmPasswordField;
			this.nextButton = nextButton;
			this.saveWalletNameButton = saveWalletNameButton;
			this.savePasswordButton = savePasswordButton;
			this.deleteWalletButton = deleteWalletButton;

			walletName = hopeWalletInfoManager.GetWalletInfo(userWalletManager.GetWalletAddress()).WalletName;
			deleteWalletButton.onClick.AddListener(() => popupManager.GetPopup<GeneralOkCancelPopup>(true).SetSubText("Are you sure you want to delete " + walletName + "?").OnOkClicked(() => logoutHandler.Logout()));
			SetListeners();
		}

		/// <summary>
		/// Sets the necessary input field listeners
		/// </summary>
		private void SetListeners()
		{
			currentPasswordField.OnInputUpdated += CurrentPasswordFieldChanged;
			nextButton.onClick.AddListener(EditWalletButtonClicked);

			currentWalletNameField.Error = false;
			currentWalletNameField.Text = walletName;
			newWalletNameField.OnInputUpdated += WalletNameFieldChanged;
			saveWalletNameButton.onClick.AddListener(SaveWalletNameClicked);

			newPasswordField.OnInputUpdated += _ => PasswordsUpdated();
			confirmPasswordField.OnInputUpdated += _ => PasswordsUpdated();
			savePasswordButton.onClick.AddListener(SavePasswordButtonClicked);
		}

		/// <summary>
		/// The current password input field has been changed
		/// </summary>
		/// <param name="text"> The text in the input field </param>
		private void CurrentPasswordFieldChanged(string text)
		{
			currentPasswordField.Error = string.IsNullOrEmpty(text);
			nextButton.interactable = !currentPasswordField.Error;
		}

		/// <summary>
		/// The edit wallet button has been clicked
		/// </summary>
		private void EditWalletButtonClicked()
		{
            currentPasswordField.InputFieldBase.interactable = false;
			settingsPopupAnimator.VerifyingPassword(nextButton.gameObject, loadingIcon, true);

            walletPasswordVerification.VerifyPassword(currentPasswordField)
                                      .OnPasswordCorrect(_ =>
                                      {
                                          settingsPopupAnimator.VerifyingPassword(nextButton.gameObject, loadingIcon, false);
                                          currentPasswordSection.AnimateScale(0f, 0.15f, () => walletDetailsSection.AnimateScale(1f, 0.15f));
                                      })
                                      .OnPasswordIncorrect(() =>
                                      {
                                          nextButton.interactable = false;
                                          settingsPopupAnimator.VerifyingPassword(nextButton.gameObject, loadingIcon, false);
                                          settingsPopupAnimator.AnimateIcon(nextButton.transform.GetChild(0).gameObject);
                                      });
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

			saveWalletNameButton.interactable = !newWalletNameField.Error && !currentPasswordField.Error;
		}

		/// <summary>
		/// Loops through the saved wallets and checks if a wallet is already saved under the given name.
		/// </summary>
		/// <param name="text"> The current text in the input field </param>
		/// <returns> Whether the given walletName has been used before </returns>
		private bool WalletNameExists(string text)
		{
			for (int i = 1; ; i++)
			{
				if (hopeWalletInfoManager.GetWalletInfo(i).WalletName.EqualsIgnoreCase(text))
					return true;
				else if (string.IsNullOrEmpty(hopeWalletInfoManager.GetWalletInfo(i).WalletName))
					return false;
			}
		}

		/// <summary>
		/// Save wallet name button has been clicked
		/// </summary>
		private void SaveWalletNameClicked()
		{
			//Save new wallet name internally

			currentWalletNameField.Text = newWalletNameField.Text;
			newWalletNameField.Text = string.Empty;
			settingsPopupAnimator.AnimateIcon(saveWalletNameButton.transform.GetChild(0).gameObject);
		}

		/// <summary>
		/// Checks if the passwords valid and animates the error icon if needed
		/// </summary>
		private void PasswordsUpdated()
		{
			string password1Text = newPasswordField.Text;
			string password2Text = confirmPasswordField.Text;

			newPasswordField.Error = newPasswordField.Text.Length < 8;
			confirmPasswordField.Error = password1Text != password2Text;

			if (newPasswordField.Error)
				newPasswordField.errorMessage.text = "Password too short";

			if (confirmPasswordField.Error)
				confirmPasswordField.errorMessage.text = "Passwords do not match";

			confirmPasswordField.UpdateVisuals();
			savePasswordButton.interactable = !currentPasswordField.Error && !newPasswordField.Error && !confirmPasswordField.Error;
		}

		/// <summary>
		/// Save password button has been clicked
		/// </summary>
		private void SavePasswordButtonClicked()
		{
			//Save new wallet name internally

			newPasswordField.Text = string.Empty;
			confirmPasswordField.Text = string.Empty;
			settingsPopupAnimator.AnimateIcon(savePasswordButton.transform.GetChild(0).gameObject);
		}
	}
}
