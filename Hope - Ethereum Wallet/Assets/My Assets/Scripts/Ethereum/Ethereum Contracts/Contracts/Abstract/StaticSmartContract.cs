/// <summary>
/// Base class for all fixed smart contracts to inherit.
/// </summary>
public abstract class StaticSmartContract
{
    /// <summary>
    /// The address of this smart contract.
    /// </summary>
    public string ContractAddress { get; }

    /// <summary>
    /// Initializes the <see cref="StaticSmartContract"/> by assigning the references to the required settings.
    /// </summary>
    /// <param name="ethereumNetworkSettings"> The current <see cref="EthereumNetworkManager.Settings"/> instance. </param>
    /// <param name="settings"> The settings of this smart contract. </param>
    protected StaticSmartContract(EthereumNetworkManager.Settings ethereumNetworkSettings, SettingsBase settings)
    {
        ContractAddress = ethereumNetworkSettings.networkType == EthereumNetworkManager.NetworkType.Mainnet ? settings.mainnetAddress.ToLower() : settings.rinkebyAddress.ToLower();
    }

    /// <summary>
    /// Base class which contains the mainnet and testnet addresses for this smart contract.
    /// </summary>
    public abstract class SettingsBase
    {
        public string mainnetAddress;
        public string rinkebyAddress;
    }
}