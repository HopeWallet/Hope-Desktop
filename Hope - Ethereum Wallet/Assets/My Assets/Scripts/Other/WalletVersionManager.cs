using System;
using UniRx;

public sealed class WalletVersionManager
{
    private const string GITHUB_RELEASES_LINK = "https://api.github.com/repos/HopeWallet/Hope.Security/releases";

    private readonly Settings versionSettings;

    public string CurrentWalletVersion { get; private set; }

    public string NewWalletVersion { get; private set; }

    public string WalletReleaseUrl { get; private set; }

    public WalletVersionManager(Settings versionSettings)
    {
        this.versionSettings = versionSettings;

        Observable.WhenAll(ObservableWWW.Get(GITHUB_RELEASES_LINK))
                  .Subscribe(results => OnReleasesPageDownloaded(results[0]));
    }

    private void OnReleasesPageDownloaded(string releasesJson)
    {
        var currentRelease = JsonUtils.DeserializeDynamicCollection(releasesJson)[0];

        WalletReleaseUrl = (string)currentRelease.html_url;
        NewWalletVersion = ((string)currentRelease.tag_name).TrimStart('v');

        if (!SecurePlayerPrefs.HasKey(PlayerPrefConstants.VERSION_PREF))
            SecurePlayerPrefs.SetString(PlayerPrefConstants.VERSION_PREF, versionSettings.version);
        else if (!(CurrentWalletVersion = SecurePlayerPrefs.GetString(PlayerPrefConstants.VERSION_PREF).TrimStart('v')).EqualsIgnoreCase(NewWalletVersion))
            NewVersionLocated();
    }

    private void NewVersionLocated()
    {
        CurrentWalletVersion.Log();
        NewWalletVersion.Log();
        WalletReleaseUrl.Log();
    }

    [Serializable]
    public sealed class Settings
    {
        public string version;
    }
}