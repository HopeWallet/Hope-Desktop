using UnityEngine;
using UnityEngine.UI;

public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	public sealed class WalletSection
	{
		private HopeInputField walletNameField, newPasswordField, confirmPasswordField;
		private Button saveButton, deleteButton;

		public WalletSection(HopeInputField walletNameField,
					  HopeInputField newPasswordField,
					  HopeInputField confirmPasswordField,
					  Button saveButton,
					  Button deleteButton)
		{
			this.walletNameField = walletNameField;
			this.newPasswordField = newPasswordField;
			this.confirmPasswordField = confirmPasswordField;
			this.saveButton = saveButton;
			this.deleteButton = deleteButton;
		}
	}
}
