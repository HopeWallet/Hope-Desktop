using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using System;
using System.Collections.Generic;

public abstract class ContractFunction : FunctionMessage
{
    protected readonly List<object> input = new List<object>();

    public static T CreateFunction<T>(params object[] functionInput) where T : ContractFunction => (T)Activator.CreateInstance(typeof(T), functionInput);

    public static T CreateFunction<T>(HexBigInteger gasPrice, HexBigInteger gasLimit, params object[] functionInput) where T : ContractFunction
    {
        T func = CreateFunction<T>(functionInput);
        func.GasPrice = gasPrice.Value;
        func.Gas = gasLimit.Value;

        return func;
    }

    protected ContractFunction(params object[] functionInput)
    {
        if (functionInput == null)
            return;

        functionInput.ForEach(param => input.Add(param));
    }
}