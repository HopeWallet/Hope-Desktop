using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	public sealed class HopeVersion
	{
		private TextMeshProUGUI currentVersionText, latestVersionText;
		private Button downloadUpdateButton;

		public HopeVersion(TextMeshProUGUI currentVersionText,
						   TextMeshProUGUI latestVersionText,
						   Button downloadUpdateButton)
		{
			this.currentVersionText = currentVersionText;
			this.latestVersionText = latestVersionText;
			this.downloadUpdateButton = downloadUpdateButton;

			if (downloadUpdateButton.gameObject.activeInHierarchy)
				downloadUpdateButton.onClick.AddListener(() => Application.OpenURL("http://www.hopewallet.io/"));
		}
	}
}
