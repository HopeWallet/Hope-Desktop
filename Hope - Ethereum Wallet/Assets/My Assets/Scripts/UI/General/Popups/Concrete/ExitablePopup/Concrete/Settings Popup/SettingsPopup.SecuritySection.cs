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

		private int idleTimeValue, maxLoginAttempts, lockoutTime;

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

			loginAttemptsCheckbox.SetCheckboxValue(SecurePlayerPrefs.GetBool("limit login attempts"));

			if (loginAttemptsCheckbox.ToggledOn)
				loginAttemptsInputField.Text = SecurePlayerPrefs.GetInt("max login attempts").ToString();

			lockoutTimeInputField.Text = SecurePlayerPrefs.GetInt("lock out time").ToString();
		}

		/// <summary>
		/// The idle timeout checkbox has been clicked
		/// </summary>
		/// <param name="enabled"> Whether the setting is enabled or not </param>
		private void IdleTimeoutCheckboxClicked(bool enabled)
		{
			SecurePlayerPrefs.SetBool("idle timeout", enabled);
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
				idleTimeoutTimeCheckbox.AnimateElements(!idleTimeoutTimeInputField.Error);
				SecurePlayerPrefs.SetBool("idle timeout", !idleTimeoutTimeInputField.Error);

				if (!idleTimeoutTimeInputField.Error)
					SecurePlayerPrefs.SetInt("idle time", idleTimeValue);
			}
		}

		private void LoginAttemptsChecboxClicked(bool enabled)
		{
			loginAttemptsCheckbox.SetCheckboxValue(enabled);
			loginAttemptsInputField.Text = enabled ? SecurePlayerPrefs.GetInt("max login attempts").ToString() : string.Empty;
		}

		private void LoginAttemptsChanged(string text)
		{
			int.TryParse(text, out maxLoginAttempts);
			loginAttemptsInputField.Error = string.IsNullOrEmpty(text) || maxLoginAttempts <= 0;

			if (!loginAttemptsInputField.InputFieldBase.wasCanceled)
			{
				loginAttemptsCheckbox.AnimateElements(!loginAttemptsInputField.Error);
				SecurePlayerPrefs.SetBool("limit login attempts", !loginAttemptsInputField.Error);

				lockoutTimeInputField.gameObject.AnimateScale(!loginAttemptsInputField.Error ? 1f : 0f, 0.2f);
				if (string.IsNullOrEmpty(lockoutTimeInputField.Text))
					lockoutTimeInputField.Text = "5";

				if (!loginAttemptsInputField.Error)
					SecurePlayerPrefs.SetInt("max login attempts", maxLoginAttempts);
			}
		}

		private void LockoutTimeChanged(string text)
		{
			int.TryParse(text, out lockoutTime);
			lockoutTimeInputField.Error = string.IsNullOrEmpty(text) || lockoutTime <= 0;

			if (!lockoutTimeInputField.InputFieldBase.wasCanceled)
			{
				loginAttemptsCheckbox.AnimateElements(!lockoutTimeInputField.Error);
				SecurePlayerPrefs.SetBool("limit login attempts", !lockoutTimeInputField.Error);

				if (!lockoutTimeInputField.Error)
					SecurePlayerPrefs.SetInt("lock out time", lockoutTime);
			}
		}
	}
}
