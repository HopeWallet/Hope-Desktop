using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The popup that manages the modification of user's settings and preferences
/// </summary>
public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	/// <summary>
	/// The section where the user can change their password
	/// </summary>
	public sealed class PasswordSection
	{
		private readonly HopeInputField newPasswordField, confirmPasswordField;
		private readonly Button saveButton;
		private readonly GameObject loadingIcon;

		private readonly SettingsPopupAnimator settingsPopupAnimator;

		public PasswordSection(
			SettingsPopupAnimator settingsPopupAnimator,
			HopeInputField newPasswordField,
			HopeInputField confirmPasswordField,
			Button saveButton,
			GameObject loadingIcon)
		{
			this.settingsPopupAnimator = settingsPopupAnimator;
			this.newPasswordField = newPasswordField;
			this.confirmPasswordField = confirmPasswordField;
			this.saveButton = saveButton;
			this.loadingIcon = loadingIcon;

			newPasswordField.OnInputUpdated += _ => PasswordsUpdated();
			confirmPasswordField.OnInputUpdated += _ => PasswordsUpdated();
			saveButton.onClick.AddListener(SavePasswordButtonClicked);
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

		/// <summary>
		/// Save password button has been clicked
		/// </summary>
		private void SavePasswordButtonClicked()
		{
			settingsPopupAnimator.ShowLoadingIcon(saveButton.gameObject, loadingIcon, true);

			//Change password internally

			//WHEN IT IS FINISHED:
			newPasswordField.Text = string.Empty;
			confirmPasswordField.Text = string.Empty;
			settingsPopupAnimator.ShowLoadingIcon(saveButton.gameObject, loadingIcon, false);
			settingsPopupAnimator.AnimateIcon(saveButton.transform.GetChild(0).gameObject);
		}
	}
}
