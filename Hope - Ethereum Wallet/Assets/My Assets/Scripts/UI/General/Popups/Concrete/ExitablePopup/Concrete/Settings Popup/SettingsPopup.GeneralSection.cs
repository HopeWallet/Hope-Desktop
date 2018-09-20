using UnityEngine;

/// <summary>
/// The general section of the settings popup
/// </summary>
public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	/// <summary>
	/// The generel settings section
	/// </summary>
	public sealed class GeneralSection
	{
		private GameObject idleTimeoutTimeSection;
		private CheckBox idleTimeoutTimeCheckbox, countdownTimerCheckbox, transactionNotificationCheckbox, updateNotificationCheckbox;
		private HopeInputField idleTimeoutTimeInputField;

		private int idleTimeValue;

		/// <summary>
		/// Sets the necessary variables
		/// </summary>
		/// <param name="idleTimeoutTimeCheckbox"> The idle timeout time checkbox </param>
		/// <param name="countdownTimerCheckbox"> The countdown timer checkbox</param>
		/// <param name="transactionNotificationCheckbox"> The transaction notification checkbox </param>
		/// <param name="updateNotificationCheckbox"> The update notification checkbox </param>
		/// <param name="idleTimeoutTimeInputField"> The idle timeout time input field </param>
		/// <param name="usingHopeWallet"> Whether the user is using Hope or not </param>
		public GeneralSection(CheckBox idleTimeoutTimeCheckbox,
							  CheckBox countdownTimerCheckbox,
							  CheckBox transactionNotificationCheckbox,
							  CheckBox updateNotificationCheckbox,
							  HopeInputField idleTimeoutTimeInputField,
							  bool usingHopeWallet)
		{
			this.idleTimeoutTimeCheckbox = idleTimeoutTimeCheckbox;
			this.countdownTimerCheckbox = countdownTimerCheckbox;
			this.transactionNotificationCheckbox = transactionNotificationCheckbox;
			this.updateNotificationCheckbox = updateNotificationCheckbox;
			this.idleTimeoutTimeInputField = idleTimeoutTimeInputField;
			idleTimeoutTimeSection = idleTimeoutTimeInputField.transform.parent.gameObject;

			SetListeners();
			SetCurrentSettings();

			if (!usingHopeWallet)
			{
				idleTimeoutTimeCheckbox.gameObject.SetActive(false);
				idleTimeoutTimeInputField.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Sets all the necessary listeners
		/// </summary>
		private void SetListeners()
		{
			idleTimeoutTimeInputField.OnInputUpdated += IdleTimeoutFieldChanged;
			idleTimeoutTimeCheckbox.OnCheckboxClicked += IdleTimeoutCheckboxClicked;
			countdownTimerCheckbox.OnCheckboxClicked += boolean => SecurePlayerPrefs.SetBool("countdown timer", boolean);
			transactionNotificationCheckbox.OnCheckboxClicked += boolean => SecurePlayerPrefs.SetBool("transaction notification", boolean);
			updateNotificationCheckbox.OnCheckboxClicked += boolean => SecurePlayerPrefs.SetBool("update notification", boolean);
		}

		/// <summary>
		/// Sets the current checkbox and input field values to what is currently saved
		/// </summary>
		private void SetCurrentSettings()
		{
			idleTimeoutTimeCheckbox.SetCheckboxValue(SecurePlayerPrefs.GetBool("idle timeout"));
			countdownTimerCheckbox.SetCheckboxValue(SecurePlayerPrefs.GetBool("countdown timer"));
			transactionNotificationCheckbox.SetCheckboxValue(SecurePlayerPrefs.GetBool("transaction notification"));
			updateNotificationCheckbox.SetCheckboxValue(SecurePlayerPrefs.GetBool("update notification"));

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
			idleTimeoutTimeSection.AnimateTransformY(string.IsNullOrEmpty(text) ? 142f : 132f, 0.15f);

			if (!idleTimeoutTimeInputField.Error && !idleTimeoutTimeInputField.InputFieldBase.wasCanceled)
			{
				SecurePlayerPrefs.SetBool("idle timeout", true);
				SecurePlayerPrefs.SetInt("idle time", idleTimeValue);
			}
		}
	}
}
