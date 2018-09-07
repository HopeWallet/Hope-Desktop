using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	public sealed class HopeVersion
	{
		private TextMeshProUGUI currentVersionText, latestVersionText;
		private Button downloadUpdateButton;

		private float currentVersion, latestVersion;

		public HopeVersion(TextMeshProUGUI currentVersionText,
						   TextMeshProUGUI latestVersionText,
						   Button downloadUpdateButton,
						   float currentVersion,
						   float latestVersion)
		{
			this.currentVersionText = currentVersionText;
			this.latestVersionText = latestVersionText;
			this.downloadUpdateButton = downloadUpdateButton;
			this.currentVersion = currentVersion;
			this.latestVersion = latestVersion;

			SetTextValues();
		}

		private void SetTextValues()
		{
			currentVersionText.text = "Current Hope version: (v " + currentVersion.ToString("0.00") + ")";

			bool upToDate = latestVersion == currentVersion;

			latestVersionText.text = !upToDate ? "New Hope version available! (v " + latestVersion.ToString("0.00") + ")" : "Software is up to date!";
			downloadUpdateButton.gameObject.SetActive(!upToDate);

			if (!upToDate)
				downloadUpdateButton.onClick.AddListener(() => Application.OpenURL("http://www.hopewallet.io/"));
		}
	}
}
