using System;

/// <summary>
/// Class which contains the smart contract addresses for DUBI.
/// </summary>
public sealed class DUBI : StaticSmartContract
{
    /// <summary>
    /// Initializes <see cref="DUBI"/> by transferring references to the <see cref="StaticSmartContract"/> base.
    /// </summary>
    /// <param name="ethereumNetworkSettings"> The settings of the active <see cref="EthereumNetworkManager"/>. </param>
    /// <param name="settings"> The settings of this <see cref="DUBI"/> contract. </param>
    public DUBI(EthereumNetworkManager.Settings ethereumNetworkSettings, Settings settings) : base(ethereumNetworkSettings, settings)
    {
    }

    /// <summary>
    /// Class which contains the contract addresses for PRPS.
    /// </summary>
    [Serializable]
    public sealed class Settings : SettingsBase
    {
    }
}