using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hodler : FixedContract<HodlerContract>
{

}

public class HodlerContract : ContractBase
{
    public HodlerContract(string contractAddress, string abi) : base(contractAddress, abi)
    {
    }

    protected override string[] FunctionNames
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    protected override void InitializeExtra(Action<ContractBase, string> onContractInitialized)
    {
    }
}

[Serializable]
public class FixedContract<T> : ScriptableObject where T : ContractBase
{

    [SerializeField] protected string contractAddress;
    [SerializeField] protected string contractAbi;

    public T Contract { get; private set; } 

    public void Create() => Contract = (T)Activator.CreateInstance(typeof(T), contractAddress, contractAbi);

}