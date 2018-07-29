using System;

/// <summary>
/// Class which contains the smart contract addresses for PRPS.
/// </summary>
public sealed class PRPS : StaticSmartContract
{
    /// <summary>
    /// Initializes <see cref="PRPS"/> by transferring references to the <see cref="StaticSmartContract"/> base.
    /// </summary>
    /// <param name="ethereumNetworkSettings"> The settings of the active <see cref="EthereumNetworkManager"/>. </param>
    /// <param name="settings"> The settings of this <see cref="PRPS"/> contract. </param>
    public PRPS(EthereumNetworkManager.Settings ethereumNetworkSettings, Settings settings) : base(ethereumNetworkSettings, settings)
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