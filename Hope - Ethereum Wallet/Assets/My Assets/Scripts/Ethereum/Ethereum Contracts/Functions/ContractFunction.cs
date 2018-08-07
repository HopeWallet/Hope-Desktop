using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using System;
using System.Collections.Generic;

/// <summary>
/// Base class to use for all ethereum contract functions that will be executed.
/// </summary>
public abstract class ContractFunction : FunctionMessage
{
    protected readonly List<object> input = new List<object>();

    /// <summary>
    /// Creates an instance of a <see cref="ContractFunction"/> of type <typeparamref name="T"/> to use with a query.
    /// </summary>
    /// <typeparam name="T"> The concrete type of <see cref="ContractFunction"/>. </typeparam>
    /// <param name="functionInput"> The input parameters to pass through the function. </param>
    /// <returns> The newly created instance of <see cref="ContractFunction"/> of type <typeparamref name="T"/>. </returns>
    public static T CreateFunction<T>(params object[] functionInput) where T : ContractFunction
    {
        return (T)Activator.CreateInstance(typeof(T), functionInput);
    }

    /// <summary>
    /// Creates an instance of a <see cref="ContractFunction"/> of type <typeparamref name="T"/> to use for gas limit estimates.
    /// </summary>
    /// <typeparam name="T"> The concrete type of <see cref="ContractFunction"/>. </typeparam>
    /// <param name="callerAddress"> The address of the function caller. </param>
    /// <param name="functionInput"> The input parameters to pass through the function. </param>
    /// <returns> The newly created instance of <see cref="ContractFunction"/> of type <typeparamref name="T"/>. </returns>
    public static T CreateFunction<T>(string callerAddress, params object[] functionInput) where T : ContractFunction
    {
        T func = CreateFunction<T>(functionInput);
        func.FromAddress = callerAddress;
        return func;
    }

    /// <summary>
    /// Creates an instance of a <see cref="ContractFunction"/> of type <typeparamref name="T"/> to use to send a contract message.
    /// </summary>
    /// <typeparam name="T"> The concrete type of <see cref="ContractFunction"/>. </typeparam>
    /// <param name="gasPrice"> The <see cref="HexBigInteger"/> gas price to use with the transaction when the function is executed. </param>
    /// <param name="gasLimit"> The <see cref="HexBigInteger"/> gas limit to use with the transaction when the function is executed. </param>
    /// <param name="functionInput"> The input parameters to pass through the function. </param>
    /// <returns> The newly created instance of <see cref="ContractFunction"/> of type <typeparamref name="T"/>. </returns>
    public static T CreateFunction<T>(HexBigInteger gasPrice, HexBigInteger gasLimit, params object[] functionInput) where T : ContractFunction
    {
        T func = CreateFunction<T>(functionInput);
        func.GasPrice = gasPrice.Value;
        func.Gas = gasLimit.Value;

        return func;
    }

    /// <summary>
    /// Initializes the <see cref="ContractFunction"/> with the function input parameters.
    /// </summary>
    /// <param name="functionInput"> The input parameters to pass through the function. </param>
    protected ContractFunction(params object[] functionInput)
    {
        functionInput?.ForEach(param => input.Add(param));
    }
}