using UnityEngine;
using UnityEngine.UI;

public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	public sealed class PasswordSection : ITabButtonObservable, IEnterButtonObservable
	{
		private HopeInputField currentPasswordField, newPasswordField, confirmPasswordField;
		private GameObject loadingIcon;
		private Button saveButton, nextButton;

		private SettingsPopupAnimator settingsPopupAnimator;
		private ButtonClickObserver buttonClickObserver;

		public PasswordSection(HopeInputField currentPasswordField,
							  HopeInputField newPasswordField,
							  HopeInputField confirmPasswordField,
							  GameObject loadingIcon,
							  Button saveButton,
							  Button nextButton,
							  SettingsPopupAnimator settingsPopupAnimator,
							  ButtonClickObserver buttonClickObserver)
		{
			this.currentPasswordField = currentPasswordField;
			this.newPasswordField = newPasswordField;
			this.confirmPasswordField = confirmPasswordField;
			this.loadingIcon = loadingIcon;
			this.saveButton = saveButton;
			this.nextButton = nextButton;
			this.settingsPopupAnimator = settingsPopupAnimator;
			this.buttonClickObserver = buttonClickObserver;

			SetListeners();
		}

		private void SetListeners()
		{
			buttonClickObserver.SubscribeObservable(this);

			currentPasswordField.Error = false;
			currentPasswordField.OnInputUpdated += (text) => nextButton.interactable = !string.IsNullOrEmpty(text);
			newPasswordField.OnInputUpdated += _ => PasswordsUpdated();
			confirmPasswordField.OnInputUpdated += _ => PasswordsUpdated();
			saveButton.onClick.AddListener(SaveButtonClicked);
			nextButton.onClick.AddListener(NextButtonClicked);
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
			currentPasswordField.InputFieldBase.interactable = true;
		}

		private void NextButtonClicked()
		{
			settingsPopupAnimator.VerifyingPassword.Invoke(true);

			//Check if current password is correct or not
			bool passwordCorrect = true;

			if (passwordCorrect)
			{
				settingsPopupAnimator.CreateNewPassword.Invoke(true);
				currentPasswordField.InputFieldBase.interactable = false;
				newPasswordField.InputFieldBase.ActivateInputField();

			}
			else
			{
				currentPasswordField.Error = true;
				settingsPopupAnimator.VerifyingPassword.Invoke(false);
			}
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
