using UnityEngine;
using UnityEngine.UI;

public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	public sealed class PasswordSection
	{
		private HopeInputField currentPasswordField, newPasswordField, confirmPasswordField;
		private Button saveButton;
		private GameObject loadingIcon;

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
		}
	}
}
