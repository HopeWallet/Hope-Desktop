using System;
using UniRx;

/// <summary>
/// Class which manages the version of the current wallet with the latest version uploaded online.
/// </summary>
public sealed class WalletVersionManager
{
    private const string GITHUB_RELEASES_LINK = "https://api.github.com/repos/HopeWallet/Hope-Desktop/releases";

    private readonly Settings versionSettings;
    private readonly PopupManager popupManager;

    /// <summary>
    /// The current local wallet version.
    /// </summary>
    public string LocalVersion { get; private set; }

    /// <summary>
    /// The latest wallet version.
    /// </summary>
    public string LatestVersion { get; private set; }

    /// <summary>
    /// The url to the latest wallet version.
    /// </summary>
    public string LatestVersionUrl { get; private set; }

    /// <summary>
    /// Whether a newer wallet version than the local version exists.
    /// </summary>
    public bool NewVersionExists { get; private set; }

    /// <summary>
    /// Initializes the WalletVersionManager and gets the latest version uploaded.
    /// </summary>
    /// <param name="versionSettings"> The Settings of the WalletVersionManager including the current version and the pref containing the current version. </param>
    /// <param name="popupManager"> The active PopupManager. </param>
    public WalletVersionManager(Settings versionSettings, PopupManager popupManager)
    {
        this.versionSettings = versionSettings;
        this.popupManager = popupManager;

        Observable.WhenAll(ObservableWWW.Get(GITHUB_RELEASES_LINK))
                  .Subscribe(results => OnReleasesPageDownloaded(results[0]));
    }

    /// <summary>
    /// Called when the releases page is downloaded, and the latest wallet version is retrieved.
    /// Opens the HopeUpdatePopup if the local wallet version does not match the latest uploaded version.
    /// </summary>
    /// <param name="releasesJson"> The json string containing the latest wallet version. </param>
    private void OnReleasesPageDownloaded(string releasesJson)
    {
        var releases = JsonUtils.DeserializeDynamicCollection(releasesJson);

        if (releases == null || releases.Count == 0)
            return;

        var currentRelease = releases[0];

        LatestVersionUrl = (string)currentRelease.html_url;
        LatestVersion = ((string)currentRelease.tag_name).TrimStart('v');

        if ((NewVersionExists = !(LocalVersion = versionSettings.version.TrimStart('v')).EqualsIgnoreCase(LatestVersion))
            && SecurePlayerPrefs.HasKey(PlayerPrefConstants.SETTING_UPDATE_NOTIFICATIONS) && SecurePlayerPrefs.GetBool(PlayerPrefConstants.SETTING_UPDATE_NOTIFICATIONS))
        {
            popupManager.GetPopup<HopeUpdatePopup>();
        }
    }

    /// <summary>
    /// Class used to contain the settings related to the WalletVersionManager.
    /// </summary>
    [Serializable]
    public sealed class Settings
    {
        public string version;
    }
}