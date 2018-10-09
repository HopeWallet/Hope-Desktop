using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class AboutPopup : ExitablePopupComponent<AboutPopup>
{
	[SerializeField] private TextMeshProUGUI currentVersionText, latestVersionText;
	[SerializeField] private Button downloadUpdateButton, websiteButton, githubButton, redditButton, discordButton;

	private float currentVersion = 1.05f, latestVersion = 1.07f;

	/// <summary>
	/// Checks if there is a new hope version available
	/// </summary>
	protected override void Awake()
	{
		base.Awake();

		currentVersionText.text = "Current Hope version: (v " + currentVersion.ToString("0.00") + ")";

		bool upToDate = latestVersion == currentVersion;

		latestVersionText.text = !upToDate ? "New Hope version available! (v " + latestVersion.ToString("0.00") + ")" : "Software is up to date!";
		downloadUpdateButton.gameObject.SetActive(!upToDate);

		if (!upToDate)
			downloadUpdateButton.onClick.AddListener(() => Application.OpenURL("http://www.hopewallet.io/"));

		websiteButton.onClick.AddListener(() => Application.OpenURL("http://www.hopewallet.io/"));
		githubButton.onClick.AddListener(() => Application.OpenURL("https://github.com/HopeWallet/"));
	}

	/// <summary>
	/// Calls the PopupClosed action
	/// </summary>
	private void OnDestroy() => MoreDropdown.PopupClosed?.Invoke();
}
