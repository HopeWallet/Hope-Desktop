using UnityEngine;
using System.Linq;

public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	public sealed class GeneralSection
	{
		private GameObject idleTimeoutTimeSection;
		private CheckBox idleTimeoutTimeCheckbox, countdownTimerCheckbox, transactionNotificationCheckbox, updateNotificationCheckbox;
		private HopeInputField idleTimeoutTimeInputField;

		private int idleTimeValue;

		public GeneralSection(CheckBox idleTimeoutTimeCheckbox,
							  CheckBox countdownTimerCheckbox,
							  CheckBox transactionNotificationCheckbox,
							  CheckBox updateNotificationCheckbox,
							  HopeInputField idleTimeoutTimeInputField)
		{
			this.idleTimeoutTimeCheckbox = idleTimeoutTimeCheckbox;
			this.countdownTimerCheckbox = countdownTimerCheckbox;
			this.transactionNotificationCheckbox = transactionNotificationCheckbox;
			this.updateNotificationCheckbox = updateNotificationCheckbox;
			this.idleTimeoutTimeInputField = idleTimeoutTimeInputField;
			idleTimeoutTimeSection = idleTimeoutTimeInputField.transform.parent.gameObject;

			SetListeners();
			SetCurrentSettings();
		}

		private void SetListeners()
		{
			idleTimeoutTimeInputField.OnInputUpdated += IdleTimeoutFieldChanged;
			idleTimeoutTimeCheckbox.OnCheckboxClicked += IdleTimeoutCheckboxClicked;
			countdownTimerCheckbox.OnCheckboxClicked += boolean => SecurePlayerPrefs.SetBool("countdown timer", boolean);
			transactionNotificationCheckbox.OnCheckboxClicked += boolean => SecurePlayerPrefs.SetBool("transaction notification", boolean);
			updateNotificationCheckbox.OnCheckboxClicked += boolean => SecurePlayerPrefs.SetBool("update notification", boolean);
		}

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

		private void IdleTimeoutCheckboxClicked(bool enabled)
		{
			SecurePlayerPrefs.SetBool("idle timeout", enabled);
			idleTimeoutTimeInputField.Text = enabled ? "5" : string.Empty;

			if (enabled)
			{
				SecurePlayerPrefs.SetInt("idle time", idleTimeValue);
				IdleTimeoutManager.IdleTimeoutEnabled?.Invoke();
			}
		}

		private void IdleTimeoutFieldChanged(string text)
		{
			int.TryParse(text, out idleTimeValue);

			idleTimeoutTimeInputField.Error = string.IsNullOrEmpty(text) || idleTimeValue <= 0;

			idleTimeoutTimeCheckbox.AnimateElements(!idleTimeoutTimeInputField.Error);
			idleTimeoutTimeSection.AnimateTransformY(string.IsNullOrEmpty(text) ? 142f : 132f, 0.15f);

			if (!idleTimeoutTimeInputField.InputFieldBase.wasCanceled)
			{
				SecurePlayerPrefs.SetBool("idle timeout", !idleTimeoutTimeInputField.Error);
				SecurePlayerPrefs.SetInt("idle time", idleTimeValue);
			}
		}
	}
}
