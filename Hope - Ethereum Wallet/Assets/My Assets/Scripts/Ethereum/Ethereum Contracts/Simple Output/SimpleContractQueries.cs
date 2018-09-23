using Hope.Utils.Ethereum;
using System;
using static SimpleOutputs;
using UInt64 = SimpleOutputs.UInt64;
using UInt32 = SimpleOutputs.UInt32;
using UInt16 = SimpleOutputs.UInt16;
using String = SimpleOutputs.String;
using Hope.Utils.Promises;

/// <summary>
/// Class which contains simple output contract queries which return the result based on the query type.
/// </summary>
public static class SimpleContractQueries
{
    /// <summary>
    /// Queries a contract function which returns a <see cref="UInt256"/> type.
    /// Also valid for any uints below 256.
    /// </summary>
    /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute which will return a <see cref="UInt256"/> type. </typeparam>
    /// <param name="contractAddress"> The contract address of the function to execute. </param>
    /// <param name="senderAddress"> The address sending the query. </param>
    /// <param name="functionInput"> The input parameters to pass to the <see cref="ContractFunction"/>. </param>
    public static EthCallPromise<UInt256> QueryUInt256Output<TFunc>(
        string contractAddress,
        string senderAddress,
        params object[] functionInput) where TFunc : ContractFunction
    {
        return ContractUtils.QueryContract<TFunc, UInt256>(contractAddress, senderAddress, functionInput);
    }

    /// <summary>
    /// Queries a contract function which returns a <see cref="UInt128"/> type.
    /// Also valid for any uints below 128.
    /// </summary>
    /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute which will return a <see cref="UInt128"/> type. </typeparam>
    /// <param name="contractAddress"> The contract address of the function to execute. </param>
    /// <param name="senderAddress"> The address sending the query. </param>
    /// <param name="functionInput"> The input parameters to pass to the <see cref="ContractFunction"/>. </param>
    public static EthCallPromise<UInt128> QueryUInt128Output<TFunc>(
        string contractAddress,
        string senderAddress,
        params object[] functionInput) where TFunc : ContractFunction
    {
        return ContractUtils.QueryContract<TFunc, UInt128>(contractAddress, senderAddress, functionInput);
    }

    /// <summary>
    /// Queries a contract function which returns a <see cref="UInt64"/> type.
    /// Also valid for any uints below 64.
    /// </summary>
    /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute which will return a <see cref="UInt64"/> type. </typeparam>
    /// <param name="contractAddress"> The contract address of the function to execute. </param>
    /// <param name="senderAddress"> The address sending the query. </param>
    /// <param name="functionInput"> The input parameters to pass to the <see cref="ContractFunction"/>. </param>
    public static EthCallPromise<UInt64> QueryUInt64Output<TFunc>(
        string contractAddress,
        string senderAddress,
        params object[] functionInput) where TFunc : ContractFunction
    {
        return ContractUtils.QueryContract<TFunc, UInt64>(contractAddress, senderAddress, functionInput);
    }

    /// <summary>
    /// Queries a contract function which returns a <see cref="UInt32"/> type.
    /// Also valid for any uints below 32.
    /// </summary>
    /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute which will return a <see cref="UInt32"/> type. </typeparam>
    /// <param name="contractAddress"> The contract address of the function to execute. </param>
    /// <param name="senderAddress"> The address sending the query. </param>
    /// <param name="functionInput"> The input parameters to pass to the <see cref="ContractFunction"/>. </param>
    public static EthCallPromise<UInt32> QueryUInt32Output<TFunc>(
        string contractAddress,
        string senderAddress,
        params object[] functionInput) where TFunc : ContractFunction
    {
        return ContractUtils.QueryContract<TFunc, UInt32>(contractAddress, senderAddress, functionInput);
    }

    /// <summary>
    /// Queries a contract function which returns a <see cref="UInt16"/> type.
    /// Also valid for any uints below 16.
    /// </summary>
    /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute which will return a <see cref="UInt16"/> type. </typeparam>
    /// <param name="contractAddress"> The contract address of the function to execute. </param>
    /// <param name="senderAddress"> The address sending the query. </param>
    /// <param name="functionInput"> The input parameters to pass to the <see cref="ContractFunction"/>. </param>
    public static EthCallPromise<UInt16> QueryUInt16Output<TFunc>(
        string contractAddress,
        string senderAddress,
        params object[] functionInput) where TFunc : ContractFunction
    {
        return ContractUtils.QueryContract<TFunc, UInt16>(contractAddress, senderAddress, functionInput);
    }

    /// <summary>
    /// Queries a contract function which returns a <see cref="UInt8"/> type.
    /// Only valid for uint8 type.
    /// </summary>
    /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute which will return a <see cref="UInt8"/> type. </typeparam>
    /// <param name="contractAddress"> The contract address of the function to execute. </param>
    /// <param name="senderAddress"> The address sending the query. </param>
    /// <param name="functionInput"> The input parameters to pass to the <see cref="ContractFunction"/>. </param>
    public static EthCallPromise<UInt8> QueryUInt8Output<TFunc>(
        string contractAddress,
        string senderAddress,
        params object[] functionInput) where TFunc : ContractFunction
    {
        return ContractUtils.QueryContract<TFunc, UInt8>(contractAddress, senderAddress, functionInput);
    }

    /// <summary>
    /// Queries a contract function which returns a <see cref="String"/> type.
    /// </summary>
    /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute which will return a <see cref="String"/> type. </typeparam>
    /// <param name="contractAddress"> The contract address of the function to execute. </param>
    /// <param name="senderAddress"> The address sending the query. </param>
    /// <param name="onQueryCompleted"> Action called once the query has completed, passing the <see cref="String"/> result. </param>
    /// <param name="functionInput"> The input parameters to pass to the <see cref="ContractFunction"/>. </param>
    public static EthCallPromise<String> QueryStringOutput<TFunc>(
        string contractAddress,
        string senderAddress,
        params object[] functionInput) where TFunc : ContractFunction
    {
        return ContractUtils.QueryContract<TFunc, String>(contractAddress, senderAddress, functionInput);
    }

    /// <summary>
    /// Queries a contract function which returns a <see cref="Address"/> type.
    /// </summary>
    /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute which will return a <see cref="Address"/> type. </typeparam>
    /// <param name="contractAddress"> The contract address of the function to execute. </param>
    /// <param name="senderAddress"> The address sending the query. </param>
    /// <param name="functionInput"> The input parameters to pass to the <see cref="ContractFunction"/>. </param>
    public static EthCallPromise<Address> QueryAddressOutput<TFunc>(
        string contractAddress,
        string senderAddress,
        params object[] functionInput) where TFunc : ContractFunction
    {
        return ContractUtils.QueryContract<TFunc, Address>(contractAddress, senderAddress, functionInput);
    }

    /// <summary>
    /// Queries a contract function which returns a <see cref="Bool"/> type.
    /// </summary>
    /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute which will return a <see cref="Bool"/> type. </typeparam>
    /// <param name="contractAddress"> The contract address of the function to execute. </param>
    /// <param name="senderAddress"> The address sending the query. </param>
    /// <param name="functionInput"> The input parameters to pass to the <see cref="ContractFunction"/>. </param>
    public static EthCallPromise<Bool> QueryBoolOutput<TFunc>(
        string contractAddress,
        string senderAddress,
        params object[] functionInput) where TFunc : ContractFunction
    {
        return ContractUtils.QueryContract<TFunc, Bool>(contractAddress, senderAddress, functionInput);
    }
}