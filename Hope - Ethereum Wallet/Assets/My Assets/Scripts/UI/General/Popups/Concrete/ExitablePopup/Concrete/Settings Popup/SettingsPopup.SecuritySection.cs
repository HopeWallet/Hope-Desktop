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
							   loginAttemptsInputField;

		private int idleTimeValue, maxLoginAttempts;

		/// <summary>
		/// Sets the necessary variables
		/// </summary>
		/// <param name="idleTimeoutTimeCheckbox"> The idle timeout time checkbox </param>
		/// <param name="loginAttemptsCheckbox"> The login attempts checbox </param>
		/// <param name="idleTimeoutTimeInputField"> The idle timeout time input field </param>
		/// <param name="loginAttemptsInputField"> The login attempts input field </param>
		public SecuritySection(CheckBox idleTimeoutTimeCheckbox,
							   CheckBox loginAttemptsCheckbox,
							   HopeInputField idleTimeoutTimeInputField,
							   HopeInputField loginAttemptsInputField)
		{
			this.idleTimeoutTimeCheckbox = idleTimeoutTimeCheckbox;
			this.loginAttemptsCheckbox = loginAttemptsCheckbox;
			this.idleTimeoutTimeInputField = idleTimeoutTimeInputField;
			this.loginAttemptsInputField = loginAttemptsInputField;

			SetListeners();
			SetCurrentSettings();
		}

		/// <summary>
		/// Sets the necessary button and input field listeners
		/// </summary>
		private void SetListeners()
		{
			idleTimeoutTimeCheckbox.OnCheckboxClicked += IdleTimeoutCheckboxClicked;
			idleTimeoutTimeInputField.OnInputUpdated += IdleTimeoutFieldChanged;
			loginAttemptsCheckbox.OnCheckboxClicked += LoginAttemptsChecboxClicked;
			loginAttemptsInputField.OnInputUpdated += LoginAttemptsFielChanged;
		}

		/// <summary>
		/// Sets the current settings of this section
		/// </summary>
		private void SetCurrentSettings()
		{
			if (SecurePlayerPrefs.GetBool("idle timeout"))
			{
				idleTimeoutTimeCheckbox.SetValue(true);
				idleTimeoutTimeInputField.Text = SecurePlayerPrefs.GetInt("idle time").ToString();
			}

			loginAttemptsCheckbox.ToggleCheckbox(SecurePlayerPrefs.GetBool("limit login attempts"));

			if (loginAttemptsCheckbox.IsToggledOn)
				loginAttemptsInputField.Text = SecurePlayerPrefs.GetInt("max login attempts").ToString();
		}

		/// <summary>
		/// The idle timeout checkbox has been clicked
		/// </summary>
		/// <param name="enabled"> Whether the setting is enabled or not </param>
		private void IdleTimeoutCheckboxClicked(bool enabled)
		{
			idleTimeoutTimeInputField.Text = enabled ? SecurePlayerPrefs.GetInt("idle time").ToString() : string.Empty;
		}

		/// <summary>
		/// The idle timeout input field has been changed
		/// </summary>
		/// <param name="text"> The current text in the input field </param>
		private void IdleTimeoutFieldChanged(string text)
		{
			int.TryParse(text, out idleTimeValue);
			idleTimeoutTimeInputField.Error = string.IsNullOrEmpty(text) || idleTimeValue <= 0;

			if (!idleTimeoutTimeInputField.InputFieldBase.wasCanceled)
			{
				idleTimeoutTimeCheckbox.ToggleCheckbox(!idleTimeoutTimeInputField.Error);

				if (!idleTimeoutTimeInputField.Error)
					SecurePlayerPrefs.SetInt("idle time", idleTimeValue);
			}
		}

		/// <summary>
		/// The limit login attempts checkbox has been toggled
		/// </summary>
		/// <param name="enabled"> Whether the checkbox is toggled on or not </param>
		private void LoginAttemptsChecboxClicked(bool enabled)
		{
			loginAttemptsInputField.Text = enabled ? SecurePlayerPrefs.GetInt("max login attempts").ToString() : string.Empty;
		}

		/// <summary>
		/// The max login attempts field has been changed
		/// </summary>
		/// <param name="text"> The text in the inpu field </param>
		private void LoginAttemptsFielChanged(string text)
		{
			int.TryParse(text, out maxLoginAttempts);
			loginAttemptsInputField.Error = string.IsNullOrEmpty(text) || maxLoginAttempts <= 0;

			if (!loginAttemptsInputField.InputFieldBase.wasCanceled)
			{
				loginAttemptsCheckbox.ToggleCheckbox(!loginAttemptsInputField.Error);

				if (!loginAttemptsInputField.Error)
					SecurePlayerPrefs.SetInt("max login attempts", maxLoginAttempts);
			}
		}
	}
}
