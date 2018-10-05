using UnityEngine;

/// <summary>
/// The popup that manages the modification of user's settings and preferences
/// </summary>
public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	/// <summary>
	/// The security section of the SettingsPopup
	/// </summary>
	public sealed class SecuritySection
	{
		private CheckBox idleTimeoutTimeCheckbox,
						 loginAttemptsCheckbox;

		private HopeInputField idleTimeoutTimeInputField,
							   loginAttemptsInputField,
							   lockoutTimeInputField;

		private GameObject idleTimeoutTimeSection;

		private int idleTimeValue;

		/// <summary>
		/// Sets the necessary variables
		/// </summary>
		/// <param name="idleTimeoutTimeCheckbox"> The idle timeout time checkbox </param>
		/// <param name="loginAttemptsCheckbox"> The login attempts checbox </param>
		/// <param name="idleTimeoutTimeInputField"> The idle timeout time input field </param>
		/// <param name="loginAttemptsInputField"> The login attempts input field </param>
		/// <param name="lockoutTimeInputField"> The lock out time input field </param>
		public SecuritySection(CheckBox idleTimeoutTimeCheckbox,
							   CheckBox loginAttemptsCheckbox,
							   HopeInputField idleTimeoutTimeInputField,
							   HopeInputField loginAttemptsInputField,
							   HopeInputField lockoutTimeInputField)
		{
			this.idleTimeoutTimeCheckbox = idleTimeoutTimeCheckbox;
			this.loginAttemptsCheckbox = loginAttemptsCheckbox;
			this.idleTimeoutTimeInputField = idleTimeoutTimeInputField;
			this.loginAttemptsInputField = loginAttemptsInputField;
			this.lockoutTimeInputField = lockoutTimeInputField;

			SetListeners();
		}

		private void SetListeners()
		{
			idleTimeoutTimeInputField.OnInputUpdated += IdleTimeoutFieldChanged;
			idleTimeoutTimeCheckbox.OnCheckboxClicked += IdleTimeoutCheckboxClicked;
			loginAttemptsCheckbox.OnCheckboxClicked += LoginAttemptsChecboxClicked;
			loginAttemptsInputField.OnInputUpdated += LoginAttemptsChanged;
			lockoutTimeInputField.OnInputUpdated += LockoutTimeChanged;
		}

		/// <summary>
		/// Sets the current user settings
		/// </summary>
		private void SetCurrentSettings()
		{
			idleTimeoutTimeCheckbox.SetCheckboxValue(SecurePlayerPrefs.GetBool("idle timeout"));

			if (idleTimeoutTimeCheckbox.ToggledOn)
			{
				idleTimeValue = SecurePlayerPrefs.GetInt("idle time");
				idleTimeoutTimeInputField.Text = idleTimeValue.ToString();
			}
		}

		/// <summary>
		/// The idle timeout checkbox has been clicked
		/// </summary>
		/// <param name="enabled"> Whether the setting is enabled or not </param>
		private void IdleTimeoutCheckboxClicked(bool enabled)
		{
			SecurePlayerPrefs.SetBool("idle timeout", enabled);
			idleTimeoutTimeInputField.Text = enabled ? "5" : string.Empty;

			if (enabled)
				SecurePlayerPrefs.SetInt("idle time", idleTimeValue);
		}

		/// <summary>
		/// The idle timeout input field has been changed
		/// </summary>
		/// <param name="text"> The current text in the input field </param>
		private void IdleTimeoutFieldChanged(string text)
		{
			int.TryParse(text, out idleTimeValue);

			idleTimeoutTimeInputField.Error = string.IsNullOrEmpty(text) || idleTimeValue <= 0;

			idleTimeoutTimeCheckbox.AnimateElements(!idleTimeoutTimeInputField.Error);
			idleTimeoutTimeSection.AnimateTransformY(string.IsNullOrEmpty(text) ? -60f : -75f, 0.15f);

			if (!idleTimeoutTimeInputField.Error && !idleTimeoutTimeInputField.InputFieldBase.wasCanceled)
			{
				SecurePlayerPrefs.SetBool("idle timeout", true);
				SecurePlayerPrefs.SetInt("idle time", idleTimeValue);
			}
		}

		private void LoginAttemptsChecboxClicked(bool enabled)
		{
			loginAttemptsInputField.Text = enabled ? "5" : string.Empty;
			lockoutTimeInputField.Text = enabled ? "5" : string.Empty;

			lockoutTimeInputField.gameObject.AnimateScale(enabled ? 1f : 0f, 0.2f);
		}

		private void LoginAttemptsChanged(string text)
		{

		}

		private void LockoutTimeChanged(string text)
		{

		}
	}
}
