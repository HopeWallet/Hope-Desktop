using System;
using UnityEngine;

/// <summary>
/// Class which is used as a fixed scriptable object version of a ContractBase.
/// </summary>
/// <typeparam name="T"> The type of the contract. </typeparam>
[Serializable]
public abstract class FixedContract<T> : ScriptableObject where T : ContractBase
{

    [SerializeField] protected string contractAddress;
    [SerializeField] protected string contractAbi;

    /// <summary>
    /// The usable ContractBase.
    /// </summary>
    public T Contract { get; private set; }

    /// <summary>
    /// Creates the contract.
    /// </summary>
    public void Create() => Contract = Contract ?? (T)Activator.CreateInstance(typeof(T), contractAddress, contractAbi);
}