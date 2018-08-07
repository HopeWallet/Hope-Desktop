using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class DynamicSmartContract
{
    public string ContractAddress { get; }

    protected DynamicSmartContract(string contractAddress)
    {
        ContractAddress = contractAddress;
    }
}