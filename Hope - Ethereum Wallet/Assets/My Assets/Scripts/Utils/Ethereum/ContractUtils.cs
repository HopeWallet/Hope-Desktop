using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.UnityClient;
using System;
using System.Collections;

namespace Hope.Utils.EthereumUtils
{
    /// <summary>
    /// Class which contains useful utility methods for sending messages to smart contracts or querying data from them.
    /// </summary>
    public class ContractUtils
    {
        private static EthereumNetwork EthereumNetwork;

        /// <summary>
        /// Initializes the <see cref="ContractUtils"/> by assigning the reference to the active network.
        /// </summary>
        /// <param name="ethereumNetworkManager"> The active <see cref="EthereumNetworkManager"/>. </param>
        public ContractUtils(EthereumNetworkManager ethereumNetworkManager)
        {
            EthereumNetwork = ethereumNetworkManager.CurrentNetwork;
        }

        /// <summary>
        /// Sends a message to an ethereum smart contract with the intent to change a part of the contract on the blockchain.
        /// </summary>
        /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute on the blockchain given the contract address. </typeparam>
        /// <param name="contractAddress"> The contract address to execute the <see cref="ContractFunction"/> on. </param>
        /// <param name="signedUnityRequest"> The <see cref="TransactionSignedUnityRequest"/> to use to send the message. </param>
        /// <param name="gasPrice"> The <see cref="HexBigInteger"/> gas price to use with the transaction. </param>
        /// <param name="gasLimit"> The <see cref="HexBigInteger"/> gas limit to use with the transaction. </param>
        /// <param name="onMessageExecuted"> Action called if the message is successfully executed on the blockchain. </param>
        /// <param name="functionInput"> The input parameters of the <see cref="ContractFunction"/>. </param>
        public static void SendContractMessage<TFunc>(
            string contractAddress,
            TransactionSignedUnityRequest signedUnityRequest,
            HexBigInteger gasPrice,
            HexBigInteger gasLimit,
            Action onMessageExecuted,
            params object[] functionInput) where TFunc : ContractFunction
        {
            _SendContractMessageCoroutine<TFunc>(contractAddress, signedUnityRequest, gasPrice, gasLimit, onMessageExecuted, functionInput).StartCoroutine();
        }

        /// <summary>
        /// Coroutine which sends a message to an ethereum smart contract.
        /// </summary>
        /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute on the blockchain given the contract address. </typeparam>
        /// <param name="contractAddress"> The contract address to execute the <see cref="ContractFunction"/> on. </param>
        /// <param name="signedUnityRequest"> The <see cref="TransactionSignedUnityRequest"/> to use to send the message. </param>
        /// <param name="gasPrice"> The <see cref="HexBigInteger"/> gas price to use with the transaction. </param>
        /// <param name="gasLimit"> The <see cref="HexBigInteger"/> gas limit to use with the transaction. </param>
        /// <param name="onMessageExecuted"> Action called if the message is successfully executed on the blockchain. </param>
        /// <param name="functionInput"> The input parameters of the <see cref="ContractFunction"/>. </param>
        private static IEnumerator _SendContractMessageCoroutine<TFunc>(
            string contractAddress,
            TransactionSignedUnityRequest signedUnityRequest,
            HexBigInteger gasPrice,
            HexBigInteger gasLimit,
            Action onMessageExecuted,
            params object[] functionInput) where TFunc : ContractFunction
        {
            yield return signedUnityRequest.SignAndSendTransaction(ContractFunction.CreateFunction<TFunc>(gasPrice, gasLimit, functionInput).CreateTransactionInput(contractAddress));

            signedUnityRequest.PollForTransactionReceipt(EthereumNetwork.NetworkUrl, () => signedUnityRequest.CheckTransactionResult(onMessageExecuted));
        }

        /// <summary>
        /// Queries some data from an ethereum smart contract which is active on the blockchain.
        /// </summary>
        /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> of the smart contract to execute which will return us some data. </typeparam>
        /// <typeparam name="TOut"> The <see cref="IFunctionOutputDTO"/> which represents the data which was returned from the <see cref="ContractFunction"/>. </typeparam>
        /// <param name="contractAddress"> The contract address to execute the <see cref="ContractFunction"/> on. </param>
        /// <param name="senderAddress"> The address of the sender requesting this data. </param>
        /// <param name="onQueryCompleted"> Action called once the query is completed successfully, passing in <typeparamref name="TOut"/>. </param>
        /// <param name="functionInput"> The input parameters of the <see cref="ContractFunction"/>. </param>
        public static void QueryContract<TFunc, TOut>(
            string contractAddress,
            string senderAddress,
            Action<TOut> onQueryCompleted,
            params object[] functionInput) where TFunc : ContractFunction where TOut : IFunctionOutputDTO, new()
        {
            _QueryContractCoroutine<TFunc, TOut>(contractAddress, senderAddress, onQueryCompleted, functionInput).StartCoroutine();
        }

        /// <summary>
        /// Coroutine which queries some data from an ethereum smart contract.
        /// </summary>
        /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> of the smart contract to execute which will return us some data. </typeparam>
        /// <typeparam name="TOut"> The <see cref="IFunctionOutputDTO"/> which represents the data which was returned from the <see cref="ContractFunction"/>. </typeparam>
        /// <param name="contractAddress"> The contract address to execute the <see cref="ContractFunction"/> on. </param>
        /// <param name="senderAddress"> The address of the sender requesting this data. </param>
        /// <param name="onQueryCompleted"> Action called once the query is completed successfully, passing in <typeparamref name="TOut"/>. </param>
        /// <param name="functionInput"> The input parameters of the <see cref="ContractFunction"/>. </param>
        private static IEnumerator _QueryContractCoroutine<TFunc, TOut>(
            string contractAddress,
            string senderAddress,
            Action<TOut> onQueryCompleted,
            params object[] functionInput) where TFunc : ContractFunction where TOut : IFunctionOutputDTO, new()
        {
            var queryRequest = new QueryUnityRequest<TFunc, TOut>(EthereumNetwork.NetworkUrl, senderAddress);
            yield return queryRequest.Query(ContractFunction.CreateFunction<TFunc>(functionInput), contractAddress);

            queryRequest.CheckTransactionResult(() => onQueryCompleted?.Invoke(queryRequest.Result));
        }
    }
}