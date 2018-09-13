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

				codeInputField.OnInputUpdated += CodeInputFieldChanged;
				confirmButton.onClick.AddListener(ConfirmButtonClicked);
			}
			else
			{
				twoFactorAuthenticationCheckbox.OnCheckboxClicked += boolean => SecurePlayerPrefs.SetBool("two-factor authentication", boolean);
				twoFactorAuthenticationCheckbox.transform.localScale = Vector2.one;
				setUpSection.transform.localScale = Vector2.zero;
			}
		}

		private void CodeInputFieldChanged(string code)
		{
			codeInputField.Error = string.IsNullOrEmpty(code);
			confirmButton.interactable = !codeInputField.Error;
		}

		private void ConfirmButtonClicked()
		{
			//If code is correct:
			setUpSection.AnimateScale(0f, 0.15f, () => twoFactorAuthenticationCheckbox.gameObject.AnimateScale(1f, 0.15f));
			SecurePlayerPrefs.SetBool("two-factor authentication", true);

			//Else codeInputField.Error = true;
		}
	}
}
