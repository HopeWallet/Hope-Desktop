using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The popup that manages the modification of user's settings and preferences
/// </summary>
public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	/// <summary>
	/// The password section where the user can change their password
	/// </summary>
	public sealed class PasswordSection
	{
		private HopeInputField currentPasswordField, newPasswordField, confirmPasswordField;
		private Button saveButton;
		private GameObject loadingIcon;

		/// <summary>
		/// Sets the necessary variables and dependencies
		/// </summary>
		/// <param name="currentPasswordField"> The current password input field </param>
		/// <param name="newPasswordField"> The new password field </param>
		/// <param name="confirmPasswordField"> The confirm password field</param>
		/// <param name="saveButton"> The save button </param>
		/// <param name="loadingIcon"> The loading icon </param>
		public PasswordSection(HopeInputField currentPasswordField,
							   HopeInputField newPasswordField,
							   HopeInputField confirmPasswordField,
							   Button saveButton,
							   GameObject loadingIcon)
		{
			this.currentPasswordField = currentPasswordField;
			this.newPasswordField = newPasswordField;
			this.confirmPasswordField = confirmPasswordField;
			this.saveButton = saveButton;
			this.loadingIcon = loadingIcon;

			SetListeners();
		}

		private void SetListeners()
		{
			currentPasswordField.OnInputUpdated += CurrentPasswordFieldChanged;
			newPasswordField.OnInputUpdated += _ => PasswordsUpdated();
			confirmPasswordField.OnInputUpdated += _ => PasswordsUpdated();
		}

		private void CurrentPasswordFieldChanged(string text)
		{
			currentPasswordField.Error = string.IsNullOrEmpty(text);
			saveButton.interactable = !currentPasswordField.Error && !newPasswordField.Error && !confirmPasswordField.Error;
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
			saveButton.interactable = !currentPasswordField.Error && !newPasswordField.Error && !confirmPasswordField.Error;
		}
	}
}
