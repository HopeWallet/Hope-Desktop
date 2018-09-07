using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	public sealed class TwoFactorAuthenticationSection
	{
		private CheckBox twoFactorAuthenticationCheckbox;
		private GameObject setUpSection;
		private TextMeshProUGUI keyText;
		private Image qrCodeImage;
		private HopeInputField codeInputField;
		private Button confirmButton;

		public TwoFactorAuthenticationSection(CheckBox twoFactorAuthenticationCheckbox,
											  GameObject setUpSection,
											  TextMeshProUGUI keyText,
											  Image qrCodeImage,
											  HopeInputField codeInputField,
											  Button confirmButton)
		{
			this.twoFactorAuthenticationCheckbox = twoFactorAuthenticationCheckbox;
			this.setUpSection = setUpSection;
			this.keyText = keyText;
			this.qrCodeImage = qrCodeImage;
			this.codeInputField = codeInputField;
			this.confirmButton = confirmButton;

			SetUpVisuals();
		}

		private void SetUpVisuals()
		{
			bool setUp2FA = !SecurePlayerPrefs.GetBool("2FA set up");

			twoFactorAuthenticationCheckbox.transform.localScale = setUp2FA ? Vector2.zero : Vector2.one;
			setUpSection.transform.localScale = setUp2FA ? Vector2.one : Vector2.zero;

			if (setUp2FA)
			{
				//Generate key and qr code image
			}

			codeInputField.OnInputUpdated += CodeInputFieldChanged;
		}

		private void CodeInputFieldChanged(string code)
		{
			codeInputField.Error = string.IsNullOrEmpty(code);
			confirmButton.interactable = !codeInputField.Error;
		}
	}
}
