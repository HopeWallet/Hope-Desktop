using Hope.Utils.EthereumUtils;
using System;

public static class SimpleOutputQueries
{
    public static void QueryUInt256Output<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<SimpleOutputs.UInt256> onQueryCompleted,
        params object[] functionInput) where TFunc : ContractFunction
    {
        ContractUtils.QueryContract<TFunc, SimpleOutputs.UInt256>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }

    public static void QueryUInt128Output<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<SimpleOutputs.UInt128> onQueryCompleted,
        params object[] functionInput) where TFunc : ContractFunction
    {
        ContractUtils.QueryContract<TFunc, SimpleOutputs.UInt128>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }

    public static void QueryUInt64Output<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<SimpleOutputs.UInt64> onQueryCompleted,
        params object[] functionInput) where TFunc : ContractFunction
    {
        ContractUtils.QueryContract<TFunc, SimpleOutputs.UInt64>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }

    public static void QueryUInt32Output<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<SimpleOutputs.UInt32> onQueryCompleted,
        params object[] functionInput) where TFunc : ContractFunction
    {
        ContractUtils.QueryContract<TFunc, SimpleOutputs.UInt32>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }

    public static void QueryUInt16Output<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<SimpleOutputs.UInt16> onQueryCompleted,
        params object[] functionInput) where TFunc : ContractFunction
    {
        ContractUtils.QueryContract<TFunc, SimpleOutputs.UInt16>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }

    public static void QueryUInt8Output<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<SimpleOutputs.UInt8> onQueryCompleted,
        params object[] functionInput) where TFunc : ContractFunction
    {
        ContractUtils.QueryContract<TFunc, SimpleOutputs.UInt8>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }

    public static void QueryStringOutput<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<SimpleOutputs.String> onQueryCompleted,
        params object[] functionInput) where TFunc : ContractFunction
    {
        ContractUtils.QueryContract<TFunc, SimpleOutputs.String>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }

    public static void QueryBoolOutput<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<SimpleOutputs.Bool> onQueryCompleted,
        params object[] functionInput) where TFunc : ContractFunction
    {
        ContractUtils.QueryContract<TFunc, SimpleOutputs.Bool>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }
}