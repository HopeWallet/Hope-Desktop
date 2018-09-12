using System;
using UnityEngine;
using UnityEngine.UI;

public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	public sealed class WalletSection : ITabButtonObservable, IEnterButtonObservable
	{
		private HopeInputField currentPasswordField, walletNameField, newPasswordField, confirmPasswordField;
		private Button editWalletButton, saveButton, deleteButton;
		private GameObject saveButtonText, checkMarkIcon;

		private HopeWalletInfoManager hopeWalletInfoManager;
		private UserWalletManager userWalletManager;
		private ButtonClickObserver buttonClickObserver;
		private SettingsPopupAnimator settingsPopupAnimator;

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

		public WalletSection(HopeWalletInfoManager hopeWalletInfoManager,
							 UserWalletManager userWalletManager,
							 ButtonClickObserver buttonClickObserver,
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
			this.buttonClickObserver = buttonClickObserver;
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

		private void SetListeners()
		{
			buttonClickObserver.SubscribeObservable(this);

			saveButton.onClick.AddListener(SaveButtonClicked);
			deleteButton.onClick.AddListener(DeleteButtonClicked);
			editWalletButton.onClick.AddListener(EditWalletButtonClicked);

			currentPasswordField.OnInputUpdated += (text) => editWalletButton.interactable = !string.IsNullOrEmpty(text);
			newPasswordField.OnInputUpdated += _ => PasswordsUpdated();
			confirmPasswordField.OnInputUpdated += _ => PasswordsUpdated();
			walletNameField.OnInputUpdated += WalletNameFieldChanged;

			currentPasswordField.Error = false;
			saveButtonText = saveButton.transform.GetChild(0).gameObject;
			walletName = hopeWalletInfoManager.GetWalletInfo(userWalletManager.WalletAddress).WalletName;
			walletNameField.Text = walletName;
		}

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

		private void SetSaveButtonInteractable()
		{
			if (animatingIcon)
				return;

			saveButton.interactable = !walletNameField.Error || (!newPasswordField.Error && !confirmPasswordField.Error);
			saveButtonText.AnimateColor(saveButton.interactable ? UIColors.White : UIColors.DarkGrey, 0.15f);
		}

		private void SaveButtonClicked()
		{
			//Change wallet details

			AnimateCheckmarkIcon();
			newPasswordField.Text = string.Empty;
			confirmPasswordField.Text = string.Empty;
		}

		private void DeleteButtonClicked()
		{

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

		public void TabButtonPressed(ClickType clickType)
		{
			throw new System.NotImplementedException();
		}

		public void EnterButtonPressed(ClickType clickType)
		{
			throw new System.NotImplementedException();
		}
	}
}
