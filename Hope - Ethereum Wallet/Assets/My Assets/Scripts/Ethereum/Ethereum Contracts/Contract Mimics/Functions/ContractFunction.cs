using Nethereum.Contracts;
using System.Collections.Generic;

public abstract class ContractFunction : FunctionMessage
{
    protected readonly List<object> input = new List<object>();

    protected ContractFunction(params object[] functionInput)
    {
        if (functionInput == null)
            return;

        functionInput.ForEach(param => input.Add(param));
    }
}