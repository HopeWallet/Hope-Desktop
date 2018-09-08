using UnityEngine;
using UnityEngine.UI;

public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	public sealed class PasswordSection
	{
		[SerializeField] private GeneralRadioButtons addressOptions;
		[SerializeField] private Transform addressListTransform;
		[SerializeField] private Button unlockButton;

		public PasswordSection(GeneralRadioButtons addressOptions,
							  Transform addressListTransform,
							  Button unlockButton)
		{
			this.addressOptions = addressOptions;
			this.addressListTransform = addressListTransform;
			this.unlockButton = unlockButton;
		}
	}
}
