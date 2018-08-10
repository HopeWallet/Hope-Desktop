using System;
using System.Reflection;
using Zenject;

/// <summary>
/// Class which contains the settings for certain classes.
/// </summary>
//[CreateAssetMenu(menuName = "Hope/App Settings")]
public class AppSettingsInstaller : ScriptableObjectInstaller<AppSettingsInstaller>
{
    public DebugManager.Settings debugSettings;
    public Settings playerPrefSettings;
    public UserWalletManager.Settings walletSettings;
    public EthereumNetworkManager.Settings ethereumNetworkSettings;
    public UIManager.Settings uiSettings;
    public SmartContractManager.Settings contractManagerSettings;

    /// <summary>
    /// Installs the bindings for all settings.
    /// </summary>
    public override void InstallBindings()
    {
        Container.BindInstance(playerPrefSettings.securePlayerPrefSettings).AsSingle().NonLazy();
        Container.BindInstance(playerPrefSettings.contactsSettings).AsSingle().NonLazy();
        Container.BindInstance(playerPrefSettings.tokenContractSettings).AsSingle().NonLazy();
        Container.BindInstance(playerPrefSettings.tokenListSettings).AsSingle().NonLazy();
        Container.BindInstance(playerPrefSettings.walletPrefSettings).AsSingle().NonLazy();

        Container.BindInstance(debugSettings).AsSingle().NonLazy();
        Container.BindInstance(walletSettings).AsSingle().NonLazy();
        Container.BindInstance(ethereumNetworkSettings).AsSingle().NonLazy();
        Container.BindInstance(uiSettings).AsSingle().NonLazy();

        InstallContractSettings();
    }

    /// <summary>
    /// Installs the bindings for the ethereum smart contract settings.
    /// </summary>
    private void InstallContractSettings()
    {
        typeof(SmartContractManager.Settings).GetFields(BindingFlags.Instance | BindingFlags.GetField | BindingFlags.Public)
                                             .ForEach(field => Container.BindInstance(field.GetValue(contractManagerSettings)).AsSingle().NonLazy());
    }

    /// <summary>
    /// Class which manages all player pref key settings.
    /// </summary>
    [Serializable]
    public sealed class Settings
    {
        public SecurePlayerPrefs.Settings securePlayerPrefSettings;
        public ContactsManager.Settings contactsSettings;
        public UserWalletInfoManager.Settings walletPrefSettings;
        public TokenContractManager.Settings tokenContractSettings;
        public TokenListManager.Settings tokenListSettings;
    }
}