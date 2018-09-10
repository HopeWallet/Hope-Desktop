using System;
using UnityEngine;
using UnityEngine.UI;

public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	public sealed class PasswordSection
	{
		private HopeInputField currentPasswordField, newPasswordField, confirmPasswordField;
		private GameObject loadingIcon;
		private Button saveButton;

		public PasswordSection(HopeInputField currentPasswordField,
							  HopeInputField newPasswordField,
							  HopeInputField confirmPasswordField,
							  GameObject loadingIcon,
							  Button saveButton)
		{
			this.currentPasswordField = currentPasswordField;
			this.newPasswordField = newPasswordField;
			this.confirmPasswordField = confirmPasswordField;
			this.loadingIcon = loadingIcon;
			this.saveButton = saveButton;

			newPasswordField.OnInputUpdated += _ => PasswordsUpdated();
			confirmPasswordField.OnInputUpdated += _ => PasswordsUpdated();
			saveButton.onClick.AddListener(SaveButtonClicked);
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
			currentPasswordField.Text = string.Empty;
		}
	}
}
