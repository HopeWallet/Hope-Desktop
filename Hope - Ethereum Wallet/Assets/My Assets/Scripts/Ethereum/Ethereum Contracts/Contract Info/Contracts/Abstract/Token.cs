using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Token : DynamicSmartContract
{

    public string Name { get; private set; }

    public string Symbol { get; private set; }

    public int Decimals { get; private set; }

    public Token(string contractAddress) : base(contractAddress)
    {
    }
}