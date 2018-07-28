using Zenject;

/// <summary>
/// Class which contains the settings for certain classes.
/// </summary>
//[CreateAssetMenu(menuName = "Hope/App Settings")]
public class AppSettingsInstaller : ScriptableObjectInstaller<AppSettingsInstaller>
{
    public DebugManager.Settings debugSettings;
    public UserWalletManager.Settings walletSettings;
    public TokenContractManager.Settings tokenContractSettings;
    public EthereumNetworkManager.Settings ethereumNetworkSettings;
    public UIManager.Settings uiSettings;
    public SmartContractManager.Settings contractManagerSettings;

    /// <summary>
    /// Installs the bindings for all settings.
    /// </summary>
    public override void InstallBindings()
    {
        Container.BindInstance(debugSettings).AsSingle().NonLazy();
        Container.BindInstance(walletSettings).AsSingle().NonLazy();
        Container.BindInstance(tokenContractSettings).AsSingle().NonLazy();
        Container.BindInstance(ethereumNetworkSettings).AsSingle().NonLazy();
        Container.BindInstance(uiSettings).AsSingle().NonLazy();

        InstallContractSettings();
    }

    /// <summary>
    /// Installs the bindings for the ethereum smart contract settings.
    /// </summary>
    private void InstallContractSettings()
    {
        Container.BindInstance(contractManagerSettings.hodlerSettings).AsSingle().NonLazy();
    }

}
