using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class AboutPopup : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI currentVersionText, latestVersionText;
	[SerializeField] private Button downloadUpdateButton;

	private float currentVersion, latestVersion;

	private void Awake()
	{
		currentVersionText.text = "Current Hope version: (v " + currentVersion.ToString("0.00") + ")";

		bool upToDate = latestVersion == currentVersion;

		latestVersionText.text = !upToDate ? "New Hope version available! (v " + latestVersion.ToString("0.00") + ")" : "Software is up to date!";
		downloadUpdateButton.gameObject.SetActive(!upToDate);

		if (!upToDate)
			downloadUpdateButton.onClick.AddListener(() => Application.OpenURL("http://www.hopewallet.io/"));
	}
}
