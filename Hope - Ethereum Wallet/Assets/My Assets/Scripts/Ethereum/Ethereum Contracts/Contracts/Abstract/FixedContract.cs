using System;

/// <summary>
/// Class which is used as a fixed scriptable object version of a ContractBase.
/// </summary>
/// <typeparam name="T"> The type of the contract as a CotnractBase. </typeparam>
[Serializable]
public abstract class FixedContract<T> : FixedContractBase where T : ContractBase
{

    /// <summary>
    /// Creates the contract.
    /// </summary>
    public override ContractBase CreateContract() => (T)Activator.CreateInstance(typeof(T), contractAddress, contractAbi);

}
