using UnityEngine;

public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	public sealed class General
	{
		private GameObject defaultCurrencyDropdown;
		private CheckBox idleTimeoutTimeCheckbox, countdownTimerCheckbox, transactionNotificationCheckbox, updateNotificationCheckbox;
		private HopeInputField idleTimeoutTimeInputField;

		public General(GameObject defaultCurrencyDropdown,
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
		}
	}
}
