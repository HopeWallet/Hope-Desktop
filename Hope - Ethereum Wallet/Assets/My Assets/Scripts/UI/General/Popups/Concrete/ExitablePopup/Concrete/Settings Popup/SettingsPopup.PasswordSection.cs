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

		private SettingsPopupAnimator settingsPopupAnimator;

		/// <summary>
		/// Sets the necessary variables and dependencies
		/// </summary>
		/// <param name="settingsPopupAnimator"> The SettingsPopupAnimator </param>
		/// <param name="currentPasswordField"> The current password input field </param>
		/// <param name="newPasswordField"> The new password field </param>
		/// <param name="confirmPasswordField"> The confirm password field</param>
		/// <param name="saveButton"> The save button </param>
		/// <param name="loadingIcon"> The loading icon </param>
		public PasswordSection(SettingsPopupAnimator settingsPopupAnimator,
							   HopeInputField currentPasswordField,
							   HopeInputField newPasswordField,
							   HopeInputField confirmPasswordField,
							   Button saveButton,
							   GameObject loadingIcon)
		{
			this.settingsPopupAnimator = settingsPopupAnimator;
			this.currentPasswordField = currentPasswordField;
			this.newPasswordField = newPasswordField;
			this.confirmPasswordField = confirmPasswordField;
			this.saveButton = saveButton;
			this.loadingIcon = loadingIcon;

			SetListeners();
		}

		/// <summary>
		/// Sets the necessary listeners
		/// </summary>
		private void SetListeners()
		{
			currentPasswordField.OnInputUpdated += CurrentPasswordFieldChanged;
			newPasswordField.OnInputUpdated += _ => PasswordsUpdated();
			confirmPasswordField.OnInputUpdated += _ => PasswordsUpdated();
			saveButton.onClick.AddListener(SaveButtonClicked);
		}

		/// <summary>
		/// The current password input field has been changed
		/// </summary>
		/// <param name="text"> The text in the input field </param>
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

		/// <summary>
		/// Saves the new password if the current password is correct, else it gives an error
		/// </summary>
		private void SaveButtonClicked()
		{
			settingsPopupAnimator.VerifyingPassword(saveButton.gameObject, loadingIcon, true);

			//check if passwordIsCorrect
			bool passwordIsCorrect = true;

			if (passwordIsCorrect)
			{
				currentPasswordField.Text = string.Empty;
				newPasswordField.Text = string.Empty;
				confirmPasswordField.Text = string.Empty;
			}
			else
			{
				currentPasswordField.Error = true;
				currentPasswordField.UpdateVisuals();
				saveButton.interactable = false;
			}

			//Implement passwordIsCorrect ternary operator after Hope wallet is done checking password >>>>>>>>>>>>>>>>>>>>>>>>>
			settingsPopupAnimator.PasswordVerificationFinished(saveButton.gameObject, loadingIcon, saveButton.transform.GetChild(passwordIsCorrect ? 0 : 1).gameObject);
		}
	}
}
