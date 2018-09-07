using UnityEngine;
using System.Linq;

public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	public sealed class GeneralSection
	{
		private GameObject defaultCurrencyDropdown, idleTimeoutTimeSection;
		private CheckBox idleTimeoutTimeCheckbox, countdownTimerCheckbox, transactionNotificationCheckbox, updateNotificationCheckbox;
		private HopeInputField idleTimeoutTimeInputField;

		public int IdleTimeValue;

		public GeneralSection(GameObject defaultCurrencyDropdown,
							  CheckBox idleTimeoutTimeCheckbox,
							  CheckBox countdownTimerCheckbox,
							  CheckBox transactionNotificationCheckbox,
							  CheckBox updateNotificationCheckbox,
							  HopeInputField idleTimeoutTimeInputField)
		{
			this.defaultCurrencyDropdown = defaultCurrencyDropdown;
			this.idleTimeoutTimeCheckbox = idleTimeoutTimeCheckbox;
			this.countdownTimerCheckbox = countdownTimerCheckbox;
			this.transactionNotificationCheckbox = transactionNotificationCheckbox;
			this.updateNotificationCheckbox = updateNotificationCheckbox;
			this.idleTimeoutTimeInputField = idleTimeoutTimeInputField;
			idleTimeoutTimeSection = idleTimeoutTimeInputField.transform.parent.gameObject;

			idleTimeoutTimeInputField.OnInputUpdated += IdleTimeoutTimeChanged;
			idleTimeoutTimeCheckbox.OnCheckboxClicked += SetInputFieldText;
			SetCurrentSettings();
		}

		private void SetCurrentSettings()
		{
			//Set default currency dropdown
			idleTimeoutTimeCheckbox.SetCheckboxValue(SecurePlayerPrefs.GetBool("idle timeout"));
			countdownTimerCheckbox.SetCheckboxValue(SecurePlayerPrefs.GetBool("countdown timer"));
			transactionNotificationCheckbox.SetCheckboxValue(SecurePlayerPrefs.GetBool("transaction notification"));
			updateNotificationCheckbox.SetCheckboxValue(SecurePlayerPrefs.GetBool("update notification"));

			if (idleTimeoutTimeCheckbox.ToggledOn)
			{
				IdleTimeValue = SecurePlayerPrefs.GetInt("idle time");
				idleTimeoutTimeInputField.Text = IdleTimeValue.ToString();
			}
		}

		private void SetInputFieldText(bool toggledOn) => idleTimeoutTimeInputField.Text = toggledOn ? "10" : string.Empty;

		private void IdleTimeoutTimeChanged(string text)
		{
			int.TryParse(text, out IdleTimeValue);

			idleTimeoutTimeInputField.Error = string.IsNullOrEmpty(text) || IdleTimeValue <= 0;

			idleTimeoutTimeCheckbox.AnimateElements(!idleTimeoutTimeInputField.Error);
			idleTimeoutTimeSection.AnimateTransformY(string.IsNullOrEmpty(text) ? 95f : 81f, 0.15f);
		}
	}
}
