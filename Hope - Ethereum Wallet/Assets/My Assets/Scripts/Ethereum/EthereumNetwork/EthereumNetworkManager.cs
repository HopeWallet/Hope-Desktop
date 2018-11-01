using System;

/// <summary>
/// Class which manages the different ethereum networks.
/// </summary>
public sealed class EthereumNetworkManager : InjectableSingleton<EthereumNetworkManager>
{
    private readonly Settings settings;

    private readonly EthereumNetwork rinkeby,
                                     mainnet;

    /// <summary>
    /// Gets the currently active network.
    /// </summary>
    public EthereumNetwork CurrentNetwork => GetCurrentNetwork();

    /// <summary>
    /// Initializes the network manager by setting up the different networks.
    /// </summary>
    /// <param name="settings"> The <see cref="Settings"/> to apply to this <see cref="EthereumNetworkManager"/>. </param>
    public EthereumNetworkManager(Settings settings)
    {
        this.settings = settings;

        mainnet = new EthereumNetwork("https://mainnet.infura.io");
        rinkeby = new EthereumNetwork("https://rinkeby.infura.io");
    }

    /// <summary>
    /// Gets the currently active network.
    /// </summary>
    /// <returns> The currently active EthereumNetwork. </returns>
    private EthereumNetwork GetCurrentNetwork()
    {
        switch (settings.networkType)
        {
            case NetworkType.Mainnet:
                return mainnet;
            default:
            case NetworkType.Rinkeby:
                return rinkeby;
        }
    }

    /// <summary>
    /// Class which contains the settings for the ethereum network manager.
    /// </summary>
    [Serializable]
    public class Settings
    {
        public event Action<NetworkType> OnNetworkChanged;

        public NetworkType networkType;

        /// <summary>
        /// Changes the network type to another network.
        /// </summary>
        /// <param name="networkType"> The new ethereum network type. </param>
        public void ChangeNetwork(NetworkType networkType)
        {
            this.networkType = networkType;
            OnNetworkChanged?.Invoke(networkType);
        }
    }

    /// <summary>
    /// Enum which holds the different network types.
    /// </summary>
    [Serializable]
    public enum NetworkType { Mainnet = 1, Rinkeby = 4 };
}
