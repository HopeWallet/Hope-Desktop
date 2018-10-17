using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The popup that manages the modification of user's settings and preferences
/// </summary>
public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	/// <summary>
	/// The two-factor authentication section, where user can set it up, enable or disable it
	/// </summary>
	public sealed class TwoFactorAuthenticationSection
	{
		private CheckBox twoFactorAuthenticationCheckbox;
		private GameObject setUpSection;
		private TextMeshProUGUI keyText;
		private Image qrCodeImage;
		private HopeInputField codeInputField;
		private Button confirmButton;

		/// <summary>
		/// Sets the necessary variables
		/// </summary>
		/// <param name="twoFactorAuthenticationCheckbox"> The checkbox to enable two-factor authentication or not </param>
		/// <param name="setUpSection"> The set-up section </param>
		/// <param name="keyText"> The key text element </param>
		/// <param name="qrCodeImage"> The qr code image </param>
		/// <param name="codeInputField"> The input field for the user to input the code </param>
		/// <param name="confirmButton"> The confirm button </param>
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

		/// <summary>
		/// Sets up the visuals depending on if two-factor authentication has already been set up
		/// </summary>
		private void SetUpVisuals()
		{
			bool setUp2FA = !SecurePlayerPrefs.GetBool(PlayerPrefConstants.TWO_FACTOR_AUTH_SETUP);

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
				twoFactorAuthenticationCheckbox.OnCheckboxClicked += boolean => SecurePlayerPrefs.SetBool(PlayerPrefConstants.TWO_FACTOR_AUTH_ENABLED, boolean);
				twoFactorAuthenticationCheckbox.transform.localScale = Vector2.one;
				setUpSection.transform.localScale = Vector2.zero;
			}
		}

		/// <summary>
		/// The code input field has been changed
		/// </summary>
		/// <param name="code"> the current string in the input field </param>
		private void CodeInputFieldChanged(string code)
		{
			codeInputField.Error = string.IsNullOrEmpty(code);
			confirmButton.interactable = !codeInputField.Error;
		}

		/// <summary>
		/// The confirm button has been clicked
		/// </summary>
		private void ConfirmButtonClicked()
		{
			//If code in the input field is correct:
			setUpSection.AnimateScale(0f, 0.15f, () => twoFactorAuthenticationCheckbox.gameObject.AnimateScale(1f, 0.15f));
			SecurePlayerPrefs.SetBool(PlayerPrefConstants.TWO_FACTOR_AUTH_ENABLED, true);

			//Else 
			//confirmButton.interactable = false;
			//codeInputField.Error = true;
			//codeInputField.UpdateVisuals();
		}
	}
}
