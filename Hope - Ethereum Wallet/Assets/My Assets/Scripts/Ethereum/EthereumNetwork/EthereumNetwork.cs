using System;

/// <summary>
/// Class which contains the info needed to connect to one of ethereum's networks.
/// </summary>
[Serializable]
public class EthereumNetwork
{

    /// <summary>
    /// The infura link of this EthereumNetwork.
    /// </summary>
    public string NetworkUrl { get; }

    /// <summary>
    /// The EthereumAPI object of this network.
    /// </summary>
    public EthereumAPI Api { get; }

    /// <summary>
    /// Initializes this network with the infura link and website used for the api.
    /// </summary>
    /// <param name="infuraNetworkUrl"> The infura network url. </param>
    /// <param name="apiUrl"> The api url to retrieve ethereum data from. </param>
    public EthereumNetwork(string infuraNetworkUrl, string apiUrl)
    {
        NetworkUrl = infuraNetworkUrl;
        Api = new EthereumAPI(apiUrl);
    }
}