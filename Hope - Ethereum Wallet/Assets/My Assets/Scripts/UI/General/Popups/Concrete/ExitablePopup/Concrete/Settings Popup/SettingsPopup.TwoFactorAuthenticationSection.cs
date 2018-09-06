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
		}
	}
}
