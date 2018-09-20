using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// The wallet section of the settings popup
/// </summary>
public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	/// <summary>
	/// The wallet section where the user can edit the wallet name or password
	/// </summary>
	public sealed class WalletSection
	{
		private HopeInputField currentPasswordField, walletNameField, newPasswordField, confirmPasswordField;
		private Button editWalletButton, saveButton, deleteButton;
		private GameObject saveButtonText, checkMarkIcon;

		private HopeWalletInfoManager hopeWalletInfoManager;
		private UserWalletManager userWalletManager;
		private SettingsPopupAnimator settingsPopupAnimator;
		private PopupManager popupManager;

		private string walletName;
		private bool animatingIcon;

		private bool AnimatingIcon
		{
			set
			{
				animatingIcon = value;
				saveButton.interactable = !animatingIcon && (!walletNameField.Error || (!newPasswordField.Error && !confirmPasswordField.Error));
				saveButtonText.AnimateColor(saveButton.interactable ? UIColors.White : UIColors.DarkGrey, 0.15f);
			}
		}

		/// <summary>
		/// Sets the necessary variables and dependencies
		/// </summary>
		/// <param name="hopeWalletInfoManager"> The active HopeWalletInfoManager </param>
		/// <param name="userWalletManager"> The active UserWalletManager </param>
		/// <param name="popupManager"> The active PopupManager </param>
		/// <param name="settingsPopupAnimator"> The active SettingPopupAnimator </param>
		/// <param name="currentPasswordField"> The current password intput field </param>
		/// <param name="walletNameField"> The wallet name input field </param>
		/// <param name="newPasswordField"> The new password input field </param>
		/// <param name="confirmPasswordField"> The confirm password input field</param>
		/// <param name="editWalletButton"></param>
		/// <param name="saveButton"> The save button </param>
		/// <param name="deleteButton"> The delete button </param>
		/// <param name="checkMarkIcon"> The chechmark icon for if the save button was clicked </param>
		public WalletSection(HopeWalletInfoManager hopeWalletInfoManager,
							 UserWalletManager userWalletManager,
							 PopupManager popupManager,
							 SettingsPopupAnimator settingsPopupAnimator,
							 HopeInputField currentPasswordField,
							 HopeInputField walletNameField,
							 HopeInputField newPasswordField,
							 HopeInputField confirmPasswordField,
							 Button editWalletButton,
							 Button saveButton,
							 Button deleteButton,
							 GameObject checkMarkIcon)
		{
			this.hopeWalletInfoManager = hopeWalletInfoManager;
			this.userWalletManager = userWalletManager;
			this.popupManager = popupManager;
			this.settingsPopupAnimator = settingsPopupAnimator;
			this.currentPasswordField = currentPasswordField;
			this.walletNameField = walletNameField;
			this.newPasswordField = newPasswordField;
			this.confirmPasswordField = confirmPasswordField;
			this.editWalletButton = editWalletButton;
			this.saveButton = saveButton;
			this.deleteButton = deleteButton;
			this.checkMarkIcon = checkMarkIcon;

			SetListeners();
		}

		/// <summary>
		/// Sets all the necessary listeners
		/// </summary>
		private void SetListeners()
		{
			saveButton.onClick.AddListener(SaveButtonClicked);
			deleteButton.onClick.AddListener(DeleteButtonClicked);
			editWalletButton.onClick.AddListener(EditWalletButtonClicked);

			currentPasswordField.OnInputUpdated += (text) => editWalletButton.interactable = !string.IsNullOrEmpty(text);
			newPasswordField.OnInputUpdated += _ => PasswordsUpdated();
			confirmPasswordField.OnInputUpdated += _ => PasswordsUpdated();
			walletNameField.OnInputUpdated += WalletNameFieldChanged;

			currentPasswordField.Error = false;
			saveButtonText = saveButton.transform.GetChild(0).gameObject;
			walletName = hopeWalletInfoManager.GetWalletInfo(userWalletManager.GetWalletAddress()).WalletName;
			walletNameField.Text = walletName;
		}

		/// <summary>
		/// The edit wallet button was clicked and the password is verified
		/// </summary>
		private void EditWalletButtonClicked()
		{
			settingsPopupAnimator.VerifyingPassword.Invoke(true);

			//Check if current password is correct or not
			bool passwordCorrect = true;

			if (passwordCorrect)
			{
				settingsPopupAnimator.EditWallet();
			}
			else
			{
				currentPasswordField.Error = true;
				settingsPopupAnimator.VerifyingPassword(false);
			}
		}

		/// <summary>
		/// Wallet name field has been changed
		/// </summary>
		/// <param name="text"> The current text in the input field</param>
		private void WalletNameFieldChanged(string text)
		{
			bool emptyName = string.IsNullOrEmpty(text.Trim());
			bool usedName = WalletNameExists(text);
			walletNameField.Error = emptyName || usedName;

			if (emptyName)
				walletNameField.errorMessage.text = "Invalid wallet name";
			else if (usedName)
				walletNameField.errorMessage.text = "Wallet name in use";

			SetSaveButtonInteractable();
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
                if (hopeWalletInfoManager.GetWalletInfo(i).WalletName.EqualsIgnoreCase(textInField) && walletName != textInField)
                    return true;
                else if (string.IsNullOrEmpty(hopeWalletInfoManager.GetWalletInfo(i).WalletName))
                    return false;
			}
		}

		/// <summary>
		/// Sets the save button to interactable depending on if the input fields have errors or not
		/// </summary>
		private void SetSaveButtonInteractable()
		{
			if (animatingIcon)
				return;

			saveButton.interactable = !walletNameField.Error || (!newPasswordField.Error && !confirmPasswordField.Error);
			saveButtonText.AnimateColor(saveButton.interactable ? UIColors.White : UIColors.DarkGrey, 0.15f);
		}

		/// <summary>
		/// The save button is clicked, and the necessary wallet information is changed
		/// </summary>
		private void SaveButtonClicked()
		{
			//Change wallet details

			AnimateCheckmarkIcon();
			newPasswordField.Text = string.Empty;
			confirmPasswordField.Text = string.Empty;
		}

		/// <summary>
		/// Delete button has been clicked and opens up a confirmation popup
		/// </summary>
		private void DeleteButtonClicked()
		{
			deleteButton.interactable = false;
			popupManager.GetPopup<GeneralOkCancelPopup>().SetSubText("Are you sure you want to delete " + walletName + "?").OnOkClicked(() => SceneManager.LoadScene("HopeWallet")).OnFinish(() => deleteButton.interactable = true);
		}

		/// <summary>
		/// Animates an icon in and out of view
		/// </summary>
		public void AnimateCheckmarkIcon()
		{
			AnimatingIcon = true;
			checkMarkIcon.transform.localScale = new Vector3(0, 0, 1);

			checkMarkIcon.AnimateGraphicAndScale(1f, 1f, 0.15f,
				() => CoroutineUtils.ExecuteAfterWait(0.6f, () => { if (checkMarkIcon != null) checkMarkIcon.AnimateGraphic(0f, 0.25f, () => AnimatingIcon = false); }));
		}
	}
}
