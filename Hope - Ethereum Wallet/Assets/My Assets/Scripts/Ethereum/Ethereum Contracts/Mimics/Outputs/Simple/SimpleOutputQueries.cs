using Hope.Utils.EthereumUtils;
using System;

public static class SimpleOutputQueries
{
    public static void QueryUInt256Output<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<SimpleOutputs.UInt256> onQueryCompleted,
        params object[] functionInput) where TFunc : QueryFunction
    {
        ContractUtils.QueryContract<TFunc, SimpleOutputs.UInt256>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }

    public static void QueryStringOutput<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<SimpleOutputs.String> onQueryCompleted,
        params object[] functionInput) where TFunc : QueryFunction
    {
        ContractUtils.QueryContract<TFunc, SimpleOutputs.String>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }

    public static void QueryBoolOutput<TFunc>(
        string contractAddress,
        string senderAddress,
        Action<SimpleOutputs.Bool> onQueryCompleted,
        params object[] functionInput) where TFunc : QueryFunction
    {
        ContractUtils.QueryContract<TFunc, SimpleOutputs.Bool>(contractAddress, senderAddress, onQueryCompleted, functionInput);
    }
}