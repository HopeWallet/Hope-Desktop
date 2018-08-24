using Hope.Utils.Promises;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.UnityClient;
using System.Collections;

namespace Hope.Utils.Ethereum
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
        /// <param name="functionInput"> The input parameters of the <see cref="ContractFunction"/>. </param>
        /// <returns> Promise of the transaction result of sending the contract message. </returns>
        public static EthTransactionPromise SendContractMessage<TFunc>(
            string contractAddress,
            TransactionSignedUnityRequest signedUnityRequest,
            HexBigInteger gasPrice,
            HexBigInteger gasLimit,
            params object[] functionInput) where TFunc : ContractFunction
        {
            var promise = new EthTransactionPromise();
            _SendContractMessageCoroutine<TFunc>(promise, contractAddress, signedUnityRequest, gasPrice, gasLimit, functionInput).StartCoroutine();

            return promise;
        }

        /// <summary>
        /// Coroutine which sends a message to an ethereum smart contract.
        /// </summary>
        /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to execute on the blockchain given the contract address. </typeparam>
        /// <param name="promise"> Promise of the transaction result of sending the contract message. </param>
        /// <param name="contractAddress"> The contract address to execute the <see cref="ContractFunction"/> on. </param>
        /// <param name="signedUnityRequest"> The <see cref="TransactionSignedUnityRequest"/> to use to send the message. </param>
        /// <param name="gasPrice"> The <see cref="HexBigInteger"/> gas price to use with the transaction. </param>
        /// <param name="gasLimit"> The <see cref="HexBigInteger"/> gas limit to use with the transaction. </param>
        /// <param name="functionInput"> The input parameters of the <see cref="ContractFunction"/>. </param>
        private static IEnumerator _SendContractMessageCoroutine<TFunc>(
            EthTransactionPromise promise,
            string contractAddress,
            TransactionSignedUnityRequest signedUnityRequest,
            HexBigInteger gasPrice,
            HexBigInteger gasLimit,
            params object[] functionInput) where TFunc : ContractFunction
        {
            yield return signedUnityRequest.SignAndSendTransaction(ContractFunction.CreateFunction<TFunc>(gasPrice, gasLimit, functionInput).CreateTransactionInput(contractAddress));

            promise.Build(signedUnityRequest, () => signedUnityRequest.Result, () => EthereumNetwork.NetworkUrl);
        }

        /// <summary>
        /// Queries some data from an ethereum smart contract which is active on the blockchain.
        /// </summary>
        /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> of the smart contract to execute which will return us some data. </typeparam>
        /// <typeparam name="TOut"> The <see cref="IFunctionOutputDTO"/> which represents the data which was returned from the <see cref="ContractFunction"/>. </typeparam>
        /// <param name="contractAddress"> The contract address to execute the <see cref="ContractFunction"/> on. </param>
        /// <param name="senderAddress"> The address of the sender requesting this data. </param>
        /// <param name="functionInput"> The input parameters of the <see cref="ContractFunction"/>. </param>
        /// <returns> The promise which will return the call result. </returns>
        public static EthCallPromise<TOut> QueryContract<TFunc, TOut>(
            string contractAddress,
            string senderAddress,
            params object[] functionInput) where TFunc : ContractFunction where TOut : IFunctionOutputDTO, new()
        {
            var promise = new EthCallPromise<TOut>();
            _QueryContractCoroutine<TFunc, TOut>(promise, contractAddress, senderAddress, functionInput).StartCoroutine();

            return promise;
        }

        /// <summary>
        /// Coroutine which queries some data from an ethereum smart contract.
        /// </summary>
        /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> of the smart contract to execute which will return us some data. </typeparam>
        /// <typeparam name="TOut"> The <see cref="IFunctionOutputDTO"/> which represents the data which was returned from the <see cref="ContractFunction"/>. </typeparam>
        /// <param name="promise"> Promise of eventually returning the data from the contract query. </param>
        /// <param name="contractAddress"> The contract address to execute the <see cref="ContractFunction"/> on. </param>
        /// <param name="senderAddress"> The address of the sender requesting this data. </param>
        /// <param name="functionInput"> The input parameters of the <see cref="ContractFunction"/>. </param>
        private static IEnumerator _QueryContractCoroutine<TFunc, TOut>(
            EthCallPromise<TOut> promise,
            string contractAddress,
            string senderAddress,
            params object[] functionInput) where TFunc : ContractFunction where TOut : IFunctionOutputDTO, new()
        {
            var queryRequest = new QueryUnityRequest<TFunc, TOut>(EthereumNetwork.NetworkUrl, senderAddress);
            yield return queryRequest.Query(ContractFunction.CreateFunction<TFunc>(functionInput), contractAddress);

            promise.Build(queryRequest, () => queryRequest.Result);
        }
    }
}