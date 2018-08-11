using Hope.Utils.Ethereum;
using System;
using static SimpleOutputs;
using UInt64 = SimpleOutputs.UInt64;
using UInt32 = SimpleOutputs.UInt32;
using UInt16 = SimpleOutputs.UInt16;
using String = SimpleOutputs.String;

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
    /// <param name="onQueryCompleted"> Action called once the query has completed, passing the <see cref="UInt256"/> result. </param>
    /// <param name="functionInput"> The input parameters to pass to the <see cref="ContractFunction"/>. </param>
    public static void QueryUInt256Output<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<UInt256> onQueryCompleted,
        params object[] functionInput) where TFunc : ContractFunction
    {
        ContractUtils.QueryContract<TFunc, UInt256>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }

    /// <summary>
    /// Queries a contract function which returns a <see cref="UInt128"/> type.
    /// Also valid for any uints below 128.
    /// </summary>
    /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute which will return a <see cref="UInt128"/> type. </typeparam>
    /// <param name="contractAddress"> The contract address of the function to execute. </param>
    /// <param name="senderAddress"> The address sending the query. </param>
    /// <param name="onQueryCompleted"> Action called once the query has completed, passing the <see cref="UInt128"/> result. </param>
    /// <param name="functionInput"> The input parameters to pass to the <see cref="ContractFunction"/>. </param>
    public static void QueryUInt128Output<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<UInt128> onQueryCompleted,
        params object[] functionInput) where TFunc : ContractFunction
    {
        ContractUtils.QueryContract<TFunc, UInt128>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }

    /// <summary>
    /// Queries a contract function which returns a <see cref="UInt64"/> type.
    /// Also valid for any uints below 64.
    /// </summary>
    /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute which will return a <see cref="UInt64"/> type. </typeparam>
    /// <param name="contractAddress"> The contract address of the function to execute. </param>
    /// <param name="senderAddress"> The address sending the query. </param>
    /// <param name="onQueryCompleted"> Action called once the query has completed, passing the <see cref="UInt64"/> result. </param>
    /// <param name="functionInput"> The input parameters to pass to the <see cref="ContractFunction"/>. </param>
    public static void QueryUInt64Output<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<UInt64> onQueryCompleted,
        params object[] functionInput) where TFunc : ContractFunction
    {
        ContractUtils.QueryContract<TFunc, UInt64>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }

    /// <summary>
    /// Queries a contract function which returns a <see cref="UInt32"/> type.
    /// Also valid for any uints below 32.
    /// </summary>
    /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute which will return a <see cref="UInt32"/> type. </typeparam>
    /// <param name="contractAddress"> The contract address of the function to execute. </param>
    /// <param name="senderAddress"> The address sending the query. </param>
    /// <param name="onQueryCompleted"> Action called once the query has completed, passing the <see cref="UInt32"/> result. </param>
    /// <param name="functionInput"> The input parameters to pass to the <see cref="ContractFunction"/>. </param>
    public static void QueryUInt32Output<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<UInt32> onQueryCompleted,
        params object[] functionInput) where TFunc : ContractFunction
    {
        ContractUtils.QueryContract<TFunc, UInt32>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }

    /// <summary>
    /// Queries a contract function which returns a <see cref="UInt16"/> type.
    /// Also valid for any uints below 16.
    /// </summary>
    /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute which will return a <see cref="UInt16"/> type. </typeparam>
    /// <param name="contractAddress"> The contract address of the function to execute. </param>
    /// <param name="senderAddress"> The address sending the query. </param>
    /// <param name="onQueryCompleted"> Action called once the query has completed, passing the <see cref="UInt16"/> result. </param>
    /// <param name="functionInput"> The input parameters to pass to the <see cref="ContractFunction"/>. </param>
    public static void QueryUInt16Output<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<UInt16> onQueryCompleted,
        params object[] functionInput) where TFunc : ContractFunction
    {
        ContractUtils.QueryContract<TFunc, UInt16>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }

    /// <summary>
    /// Queries a contract function which returns a <see cref="UInt8"/> type.
    /// Only valid for uint8 type.
    /// </summary>
    /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute which will return a <see cref="UInt8"/> type. </typeparam>
    /// <param name="contractAddress"> The contract address of the function to execute. </param>
    /// <param name="senderAddress"> The address sending the query. </param>
    /// <param name="onQueryCompleted"> Action called once the query has completed, passing the <see cref="UInt8"/> result. </param>
    /// <param name="functionInput"> The input parameters to pass to the <see cref="ContractFunction"/>. </param>
    public static void QueryUInt8Output<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<UInt8> onQueryCompleted,
        params object[] functionInput) where TFunc : ContractFunction
    {
        ContractUtils.QueryContract<TFunc, UInt8>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }

    /// <summary>
    /// Queries a contract function which returns a <see cref="String"/> type.
    /// </summary>
    /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute which will return a <see cref="String"/> type. </typeparam>
    /// <param name="contractAddress"> The contract address of the function to execute. </param>
    /// <param name="senderAddress"> The address sending the query. </param>
    /// <param name="onQueryCompleted"> Action called once the query has completed, passing the <see cref="String"/> result. </param>
    /// <param name="functionInput"> The input parameters to pass to the <see cref="ContractFunction"/>. </param>
    public static void QueryStringOutput<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<String> onQueryCompleted,
        params object[] functionInput) where TFunc : ContractFunction
    {
        ContractUtils.QueryContract<TFunc, String>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }

    /// <summary>
    /// Queries a contract function which returns a <see cref="Address"/> type.
    /// </summary>
    /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute which will return a <see cref="Address"/> type. </typeparam>
    /// <param name="contractAddress"> The contract address of the function to execute. </param>
    /// <param name="senderAddress"> The address sending the query. </param>
    /// <param name="onQueryCompleted"> Action called once the query has completed, passing the <see cref="Address"/> result. </param>
    /// <param name="functionInput"> The input parameters to pass to the <see cref="ContractFunction"/>. </param>
    public static void QueryAddressOutput<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<Address> onQueryCompleted,
        params object[] functionInput) where TFunc : ContractFunction
    {
        ContractUtils.QueryContract<TFunc, Address>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }

    /// <summary>
    /// Queries a contract function which returns a <see cref="Bool"/> type.
    /// </summary>
    /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute which will return a <see cref="Bool"/> type. </typeparam>
    /// <param name="contractAddress"> The contract address of the function to execute. </param>
    /// <param name="senderAddress"> The address sending the query. </param>
    /// <param name="onQueryCompleted"> Action called once the query has completed, passing the <see cref="Bool"/> result. </param>
    /// <param name="functionInput"> The input parameters to pass to the <see cref="ContractFunction"/>. </param>
    public static void QueryBoolOutput<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<Bool> onQueryCompleted,
        params object[] functionInput) where TFunc : ContractFunction
    {
        ContractUtils.QueryContract<TFunc, Bool>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }
}