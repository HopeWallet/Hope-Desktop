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

		public int IdleTimeValue, MaxLoginAttempts, LockoutTime;

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
			SetCurrentSettings();
		}

		private void SetListeners()
		{
			idleTimeoutTimeCheckbox.OnCheckboxClicked += IdleTimeoutCheckboxClicked;
			idleTimeoutTimeInputField.OnInputUpdated += IdleTimeoutFieldChanged;
			loginAttemptsCheckbox.OnCheckboxClicked += LoginAttemptsChecboxClicked;
			loginAttemptsInputField.OnInputUpdated += LoginAttemptsFielChanged;
			lockoutTimeInputField.OnInputUpdated += LockoutTimeFieldChanged;
		}

		private void SetCurrentSettings()
		{
			if (SecurePlayerPrefs.GetBool("idle timeout"))
			{
				idleTimeoutTimeCheckbox.SetValue(true);
				idleTimeoutTimeInputField.Text = SecurePlayerPrefs.GetInt("idle time").ToString();
			}

			lockoutTimeInputField.Text = SecurePlayerPrefs.GetInt("lock out time").ToString();

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
			int.TryParse(text, out IdleTimeValue);
			idleTimeoutTimeInputField.Error = string.IsNullOrEmpty(text) || IdleTimeValue <= 0;

			if (!idleTimeoutTimeInputField.InputFieldBase.wasCanceled)
			{
				idleTimeoutTimeCheckbox.ToggleCheckbox(!idleTimeoutTimeInputField.Error);

				if (!idleTimeoutTimeInputField.Error)
					SecurePlayerPrefs.SetInt("idle time", IdleTimeValue);
			}
		}

		private void LoginAttemptsChecboxClicked(bool enabled)
		{
			loginAttemptsInputField.Text = enabled ? SecurePlayerPrefs.GetInt("max login attempts").ToString() : string.Empty;
		}

		private void LoginAttemptsFielChanged(string text)
		{
			int.TryParse(text, out MaxLoginAttempts);
			loginAttemptsInputField.Error = string.IsNullOrEmpty(text) || MaxLoginAttempts <= 0;

			if (!loginAttemptsInputField.InputFieldBase.wasCanceled)
			{
				loginAttemptsCheckbox.ToggleCheckbox(!loginAttemptsInputField.Error);

				lockoutTimeInputField.gameObject.AnimateScale(!loginAttemptsInputField.Error ? 1f : 0f, 0.2f);

				if (!loginAttemptsInputField.Error)
					SecurePlayerPrefs.SetInt("max login attempts", MaxLoginAttempts);
			}
		}

		private void LockoutTimeFieldChanged(string text)
		{
			int.TryParse(text, out LockoutTime);
			lockoutTimeInputField.Error = string.IsNullOrEmpty(text) || LockoutTime <= 0;

			if (!lockoutTimeInputField.InputFieldBase.wasCanceled)
			{
				loginAttemptsCheckbox.ToggleCheckbox(!lockoutTimeInputField.Error);

				if (!lockoutTimeInputField.Error)
					SecurePlayerPrefs.SetInt("lock out time", LockoutTime);
			}
		}
	}
}
