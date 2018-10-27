using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public sealed class AboutPopup : ExitablePopupComponent<AboutPopup>
{
	[SerializeField] private TextMeshProUGUI currentVersionText, latestVersionText;
	[SerializeField] private Button downloadUpdateButton, websiteButton, githubButton, redditButton, discordButton;

    private WalletVersionManager walletVersionManager;

    private const string WEBSITE_URL = "http://www.hopewallet.io/";
    private const string GITHUB_URL = "https://github.com/HopeWallet/";

    [Inject]
    public void Construct(WalletVersionManager walletVersionManager)
    {
        this.walletVersionManager = walletVersionManager;
    }

	/// <summary>
	/// Checks if there is a new hope version available
	/// </summary>
	protected override void Awake()
	{
		base.Awake();

		currentVersionText.text = $"Current Hope version: (v{walletVersionManager.LocalVersion})";

		bool upToDate = !walletVersionManager.NewVersionExists;

		latestVersionText.text = !upToDate ? $"New Hope version available! (v{walletVersionManager.LatestVersion})" : "Wallet is up to date!";
		downloadUpdateButton.gameObject.SetActive(!upToDate);

		if (!upToDate)
			downloadUpdateButton.onClick.AddListener(() => Application.OpenURL(walletVersionManager.LatestVersionUrl));

		websiteButton.onClick.AddListener(() => Application.OpenURL(WEBSITE_URL));
		githubButton.onClick.AddListener(() => Application.OpenURL(GITHUB_URL));
	}

	/// <summary>
	/// Calls the PopupClosed action
	/// </summary>
	private void OnDestroy() => MoreDropdown.PopupClosed?.Invoke();
}
