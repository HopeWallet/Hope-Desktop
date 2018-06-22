using System;
using UnityEngine;
using static EthereumNetworkManager;

/// <summary>
/// Base class for all fixed contracts.
/// </summary>
[Serializable]
public abstract class FixedContractBase : ScriptableObject
{

    [SerializeField] protected string contractAddress;
    [SerializeField] protected string contractAbi;
    [SerializeField] protected NetworkType networkType;

    /// <summary>
    /// The network this contract will exist on.
    /// </summary>
    public NetworkType NetworkType => networkType;

    /// <summary>
    /// Method which creates an instance of the contract belonging to this scriptable object.
    /// </summary>
    /// <returns> The contract belonging to this object. </returns>
    public abstract ContractBase CreateContract();
}