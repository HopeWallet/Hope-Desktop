using System;
using UnityEngine;
using UnityEngine.UI;

public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	public sealed class PasswordSection
	{
		private HopeInputField currentPasswordField, newPasswordField, confirmPasswordField;
		private GameObject loadingIcon;
		private Button saveButton, nextButton;

		private SettingsPopupAnimator settingsPopupAnimator;

		public bool creatingNewPassword { get; private set; }

		public PasswordSection(HopeInputField currentPasswordField,
							  HopeInputField newPasswordField,
							  HopeInputField confirmPasswordField,
							  GameObject loadingIcon,
							  Button saveButton,
							  Button nextButton,
							  SettingsPopupAnimator settingsPopupAnimator)
		{
			this.currentPasswordField = currentPasswordField;
			this.newPasswordField = newPasswordField;
			this.confirmPasswordField = confirmPasswordField;
			this.loadingIcon = loadingIcon;
			this.saveButton = saveButton;
			this.nextButton = nextButton;
			this.settingsPopupAnimator = settingsPopupAnimator;

			currentPasswordField.OnInputUpdated += CurrentPasswordFieldChanged;
			newPasswordField.OnInputUpdated += _ => PasswordsUpdated();
			confirmPasswordField.OnInputUpdated += _ => PasswordsUpdated();
			saveButton.onClick.AddListener(SaveButtonClicked);
			nextButton.onClick.AddListener(NextButtonClicked);

			currentPasswordField.InputFieldBase.ActivateInputField();
		}

		private void CurrentPasswordFieldChanged(string text)
		{
			currentPasswordField.Error = string.IsNullOrEmpty(text);
			nextButton.interactable = !currentPasswordField.Error;

			if (creatingNewPassword)
			{
				creatingNewPassword = false;
				settingsPopupAnimator.CreateNewPassword(false);
				newPasswordField.Text = string.Empty;
				confirmPasswordField.Text = string.Empty;
			}
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
			saveButton.interactable = !newPasswordField.Error && !confirmPasswordField.Error;
		}

		private void SaveButtonClicked()
		{
			//Change password

			currentPasswordField.Text = string.Empty;
			newPasswordField.Text = string.Empty;
			confirmPasswordField.Text = string.Empty;
		}

		private void NextButtonClicked()
		{
			//Check if current password is correct or not

			settingsPopupAnimator.VerifyingPassword.Invoke(true);

			bool passwordCorrect = true;

			if (passwordCorrect)
			{
				settingsPopupAnimator.CreateNewPassword.Invoke(true);
				creatingNewPassword = true;
				newPasswordField.InputFieldBase.ActivateInputField();

			}
			else
			{
				currentPasswordField.Error = true;
				settingsPopupAnimator.VerifyingPassword.Invoke(false);
			}
		}
	}
}
