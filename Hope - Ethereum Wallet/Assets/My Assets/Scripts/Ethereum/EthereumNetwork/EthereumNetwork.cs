using System;

/// <summary>
/// Class which contains the info needed to connect to one of ethereum's networks.
/// </summary>
[Serializable]
public sealed class EthereumNetwork
{
    /// <summary>
    /// The infura link of this EthereumNetwork.
    /// </summary>
    public string NetworkUrl { get; }

    /// <summary>
    /// Initializes this network with the infura link and website used for the api.
    /// </summary>
    /// <param name="infuraNetworkUrl"> The infura network url. </param>
    public EthereumNetwork(string infuraNetworkUrl)
    {
        NetworkUrl = infuraNetworkUrl;
    }
}