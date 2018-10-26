using System;
using UniRx;

public sealed class WalletVersionManager
{
    private const string GITHUB_RELEASES_LINK = "https://api.github.com/repos/HopeWallet/Hope.Security/releases";

    private readonly Settings versionSettings;
    private readonly PopupManager popupManager;

    public string CurrentWalletVersion { get; private set; }

    public string LatestWalletVersion { get; private set; }

    public string LatestWalletVersionUrl { get; private set; }

    public bool NewVersionExists { get; private set; }

    public WalletVersionManager(Settings versionSettings, PopupManager popupManager)
    {
        this.versionSettings = versionSettings;
        this.popupManager = popupManager;

        Observable.WhenAll(ObservableWWW.Get(GITHUB_RELEASES_LINK))
                  .Subscribe(results => OnReleasesPageDownloaded(results[0]));
    }

    private void OnReleasesPageDownloaded(string releasesJson)
    {
        var currentRelease = JsonUtils.DeserializeDynamicCollection(releasesJson)[0];

        LatestWalletVersionUrl = (string)currentRelease.html_url;
        LatestWalletVersion = ((string)currentRelease.tag_name).TrimStart('v');

        if (!SecurePlayerPrefs.HasKey(PlayerPrefConstants.VERSION_PREF))
            SecurePlayerPrefs.SetString(PlayerPrefConstants.VERSION_PREF, versionSettings.version);
        else if (NewVersionExists = !(CurrentWalletVersion = SecurePlayerPrefs.GetString(PlayerPrefConstants.VERSION_PREF).TrimStart('v')).EqualsIgnoreCase(LatestWalletVersion))
            popupManager.GetPopup<HopeUpdatePopup>();
    }

    [Serializable]
    public sealed class Settings
    {
        public string version;
    }
}