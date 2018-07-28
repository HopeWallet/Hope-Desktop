using Hope.Utils.EthereumUtils;
using System;

/// <summary>
/// Class which contains simple output contract queries which return the result based on the query type.
/// </summary>
public static class SimpleContractQueries
{
    /// <summary>
    /// Queries a contract function which returns a <see cref="SimpleOutputs.UInt256"/> type.
    /// Also valid for any uints below 256.
    /// </summary>
    /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute which will return a <see cref="SimpleOutputs.UInt256"/> type. </typeparam>
    /// <param name="contractAddress"> The contract address of the function to execute. </param>
    /// <param name="senderAddress"> The address sending the query. </param>
    /// <param name="onQueryCompleted"> Action called once the query has completed, passing the <see cref="SimpleOutputs.UInt256"/> result. </param>
    /// <param name="functionInput"> The input parameters to pass to the <see cref="ContractFunction"/>. </param>
    public static void QueryUInt256Output<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<SimpleOutputs.UInt256> onQueryCompleted,
        params object[] functionInput) where TFunc : ContractFunction
    {
        ContractUtils.QueryContract<TFunc, SimpleOutputs.UInt256>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }

    /// <summary>
    /// Queries a contract function which returns a <see cref="SimpleOutputs.UInt128"/> type.
    /// Also valid for any uints below 128.
    /// </summary>
    /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute which will return a <see cref="SimpleOutputs.UInt128"/> type. </typeparam>
    /// <param name="contractAddress"> The contract address of the function to execute. </param>
    /// <param name="senderAddress"> The address sending the query. </param>
    /// <param name="onQueryCompleted"> Action called once the query has completed, passing the <see cref="SimpleOutputs.UInt128"/> result. </param>
    /// <param name="functionInput"> The input parameters to pass to the <see cref="ContractFunction"/>. </param>
    public static void QueryUInt128Output<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<SimpleOutputs.UInt128> onQueryCompleted,
        params object[] functionInput) where TFunc : ContractFunction
    {
        ContractUtils.QueryContract<TFunc, SimpleOutputs.UInt128>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }

    /// <summary>
    /// Queries a contract function which returns a <see cref="SimpleOutputs.UInt64"/> type.
    /// Also valid for any uints below 64.
    /// </summary>
    /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute which will return a <see cref="SimpleOutputs.UInt64"/> type. </typeparam>
    /// <param name="contractAddress"> The contract address of the function to execute. </param>
    /// <param name="senderAddress"> The address sending the query. </param>
    /// <param name="onQueryCompleted"> Action called once the query has completed, passing the <see cref="SimpleOutputs.UInt64"/> result. </param>
    /// <param name="functionInput"> The input parameters to pass to the <see cref="ContractFunction"/>. </param>
    public static void QueryUInt64Output<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<SimpleOutputs.UInt64> onQueryCompleted,
        params object[] functionInput) where TFunc : ContractFunction
    {
        ContractUtils.QueryContract<TFunc, SimpleOutputs.UInt64>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }

    /// <summary>
    /// Queries a contract function which returns a <see cref="SimpleOutputs.UInt32"/> type.
    /// Also valid for any uints below 32.
    /// </summary>
    /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute which will return a <see cref="SimpleOutputs.UInt32"/> type. </typeparam>
    /// <param name="contractAddress"> The contract address of the function to execute. </param>
    /// <param name="senderAddress"> The address sending the query. </param>
    /// <param name="onQueryCompleted"> Action called once the query has completed, passing the <see cref="SimpleOutputs.UInt32"/> result. </param>
    /// <param name="functionInput"> The input parameters to pass to the <see cref="ContractFunction"/>. </param>
    public static void QueryUInt32Output<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<SimpleOutputs.UInt32> onQueryCompleted,
        params object[] functionInput) where TFunc : ContractFunction
    {
        ContractUtils.QueryContract<TFunc, SimpleOutputs.UInt32>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }

    /// <summary>
    /// Queries a contract function which returns a <see cref="SimpleOutputs.UInt16"/> type.
    /// Also valid for any uints below 16.
    /// </summary>
    /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute which will return a <see cref="SimpleOutputs.UInt16"/> type. </typeparam>
    /// <param name="contractAddress"> The contract address of the function to execute. </param>
    /// <param name="senderAddress"> The address sending the query. </param>
    /// <param name="onQueryCompleted"> Action called once the query has completed, passing the <see cref="SimpleOutputs.UInt16"/> result. </param>
    /// <param name="functionInput"> The input parameters to pass to the <see cref="ContractFunction"/>. </param>
    public static void QueryUInt16Output<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<SimpleOutputs.UInt16> onQueryCompleted,
        params object[] functionInput) where TFunc : ContractFunction
    {
        ContractUtils.QueryContract<TFunc, SimpleOutputs.UInt16>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }

    /// <summary>
    /// Queries a contract function which returns a <see cref="SimpleOutputs.UInt8"/> type.
    /// Only valid for uint8 type.
    /// </summary>
    /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute which will return a <see cref="SimpleOutputs.UInt8"/> type. </typeparam>
    /// <param name="contractAddress"> The contract address of the function to execute. </param>
    /// <param name="senderAddress"> The address sending the query. </param>
    /// <param name="onQueryCompleted"> Action called once the query has completed, passing the <see cref="SimpleOutputs.UInt8"/> result. </param>
    /// <param name="functionInput"> The input parameters to pass to the <see cref="ContractFunction"/>. </param>
    public static void QueryUInt8Output<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<SimpleOutputs.UInt8> onQueryCompleted,
        params object[] functionInput) where TFunc : ContractFunction
    {
        ContractUtils.QueryContract<TFunc, SimpleOutputs.UInt8>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }

    /// <summary>
    /// Queries a contract function which returns a <see cref="SimpleOutputs.String"/> type.
    /// </summary>
    /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute which will return a <see cref="SimpleOutputs.String"/> type. </typeparam>
    /// <param name="contractAddress"> The contract address of the function to execute. </param>
    /// <param name="senderAddress"> The address sending the query. </param>
    /// <param name="onQueryCompleted"> Action called once the query has completed, passing the <see cref="SimpleOutputs.String"/> result. </param>
    /// <param name="functionInput"> The input parameters to pass to the <see cref="ContractFunction"/>. </param>
    public static void QueryStringOutput<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<SimpleOutputs.String> onQueryCompleted,
        params object[] functionInput) where TFunc : ContractFunction
    {
        ContractUtils.QueryContract<TFunc, SimpleOutputs.String>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }

    /// <summary>
    /// Queries a contract function which returns a <see cref="SimpleOutputs.Address"/> type.
    /// </summary>
    /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute which will return a <see cref="SimpleOutputs.Address"/> type. </typeparam>
    /// <param name="contractAddress"> The contract address of the function to execute. </param>
    /// <param name="senderAddress"> The address sending the query. </param>
    /// <param name="onQueryCompleted"> Action called once the query has completed, passing the <see cref="SimpleOutputs.Address"/> result. </param>
    /// <param name="functionInput"> The input parameters to pass to the <see cref="ContractFunction"/>. </param>
    public static void QueryAddressOutput<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<SimpleOutputs.Address> onQueryCompleted,
        params object[] functionInput) where TFunc : ContractFunction
    {
        ContractUtils.QueryContract<TFunc, SimpleOutputs.Address>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }

    /// <summary>
    /// Queries a contract function which returns a <see cref="SimpleOutputs.Bool"/> type.
    /// </summary>
    /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute which will return a <see cref="SimpleOutputs.Bool"/> type. </typeparam>
    /// <param name="contractAddress"> The contract address of the function to execute. </param>
    /// <param name="senderAddress"> The address sending the query. </param>
    /// <param name="onQueryCompleted"> Action called once the query has completed, passing the <see cref="SimpleOutputs.Bool"/> result. </param>
    /// <param name="functionInput"> The input parameters to pass to the <see cref="ContractFunction"/>. </param>
    public static void QueryBoolOutput<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<SimpleOutputs.Bool> onQueryCompleted,
        params object[] functionInput) where TFunc : ContractFunction
    {
        ContractUtils.QueryContract<TFunc, SimpleOutputs.Bool>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }
}