using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections;

namespace Hope.Utils.EthereumUtils
{

    /// <summary>
    /// Class which has extension methods for executing certain actions on contracts.
    /// </summary>
    public static class ContractUtils
    {
        /// <summary>
        /// Calls a "view" or "constant" function which returns multiple values or a struct.
        /// </summary>
        /// <typeparam name="T"> The value returned from the solidity contract function. </typeparam>
        /// <param name="contract"> The contract you are executing this function from. </param>
        /// <param name="function"> The function to execute. </param>
        /// <param name="onValueReceived"> The callback to execute when the value(s) is received, and passed as a parameter. </param>
        /// <param name="input"> The input parameters to pass into the function. </param>
        public static void ComplexContractViewCall<T>(this ContractBase contract, Function function, Action<T> onValueReceived, params object[] input) where T : new()
            => _ComplexContractViewFunctionCoroutine(function, onValueReceived, input).StartCoroutine();

        /// <summary>
        /// Executes the function of a given smart contract.
        /// </summary>
        /// <param name="contract"> The contract the function exists on. </param>
        /// <param name="function"> The function to execute. </param>
        /// <param name="signedUnityRequest"> The signed unity request to execute the contract function with. </param>
        /// <param name="walletAddress"> The address of the wallet sending the request. </param>
        /// <param name="gasLimit"> The gas limit to use when executing this contract function. </param>
        /// <param name="gasPrice"> The gas price to use when executing this contract function. </param>
        /// <param name="onTransactionMined"> Action to execute when the transaction has been mined. </param>
        /// <param name="input"> The function input arguments. </param>
        public static void ExecuteContractFunction(this ContractBase contract, Function function, TransactionSignedUnityRequest signedUnityRequest,
            string walletAddress, HexBigInteger gasLimit, HexBigInteger gasPrice, Action onTransactionMined, params object[] input)
        {
            _ExecuteContractFunctionCoroutine(function, signedUnityRequest, walletAddress, gasLimit, gasPrice, new HexBigInteger(0), onTransactionMined, input).StartCoroutine();
        }

        public static void QueryContract<TFunc, TOut>(
            string contractAddress,
            string senderAddress,
            Action<TOut> onQueryCompleted,
            params object[] functionInput) where TFunc : QueryFunction where TOut : IFunctionOutputDTO, new()
        {
            _QueryContractCoroutine<TFunc, TOut>(contractAddress, senderAddress, onQueryCompleted, functionInput).StartCoroutine();
        }

        private static IEnumerator _QueryContractCoroutine<TFunc, TOut>(
            string contractAddress,
            string senderAddress,
            Action<TOut> onQueryCompleted,
            params object[] functionInput) where TFunc : QueryFunction where TOut : IFunctionOutputDTO, new()
        {
            var queryRequest = new QueryUnityRequest<TFunc, TOut>(EthereumNetworkManager.Instance.CurrentNetwork.NetworkUrl, senderAddress);
            yield return queryRequest.Query((TFunc)Activator.CreateInstance(typeof(TFunc), functionInput), contractAddress);

            queryRequest.CheckTransactionRequest(() => onQueryCompleted?.Invoke(queryRequest.Result));
        }

        /// <summary>
        /// Calls a "view" or "constant" function which returns a series of values or a struct.
        /// </summary>
        /// <typeparam name="T"> The value returned from the solidity contract function. </typeparam>
        /// <param name="function"> The function to execute. </param>
        /// <param name="onValueReceived"> The callback to execute when the value is received, and passed as a parameter. </param>
        /// <param name="input"> The input parameters to pass into the function. </param>
        /// <returns> The time waited for the request to complete. </returns>
        private static IEnumerator _ComplexContractViewFunctionCoroutine<T>(Function function, Action<T> onValueReceived, params object[] input) where T : new()
        {
            var request = new EthCallUnityRequest(EthereumNetworkManager.Instance.CurrentNetwork.NetworkUrl);

            yield return request.SendRequest(function.CreateCallInput(input), BlockParameter.CreateLatest());

            request.CheckTransactionRequest(() => onValueReceived(function.DecodeDTOTypeOutput<T>(request.Result)));
        }

        /// <summary>
        /// Executes a function from a contract.
        /// This function execution will actually change stuff on the blockchain, so it requires a signature to perform the transaction.
        /// </summary>
        /// <param name="function"> The function to execute. </param>
        /// <param name="signedUnityRequest"> The signed unity request to execute the contract function with. </param>
        /// <param name="walletAddress"> The address of the wallet sending the request. </param>
        /// <param name="gasLimit"> The gas limit to send the transaction with. </param>
        /// <param name="gasPrice"> The gas price to send the transaction with. </param>
        /// <param name="payableAmount"> The amount of ether to send to the function along with the regular function input. </param>
        /// <param name="onTransactionMined"> Action to execute when the transaction has been mined. </param>
        /// <param name="input"> The input of the function. </param>
        /// <returns> The time taken to send the transaction. </returns>
        private static IEnumerator _ExecuteContractFunctionCoroutine(Function function, TransactionSignedUnityRequest signedUnityRequest, string walletAddress,
            HexBigInteger gasLimit, HexBigInteger gasPrice, HexBigInteger payableAmount, Action onTransactionMined, params object[] input)
        {
            var network = EthereumNetworkManager.Instance.CurrentNetwork.NetworkUrl;
            var transactionInput = function.CreateTransactionInput(walletAddress, gasLimit, gasPrice, payableAmount, input);

            yield return signedUnityRequest.SignAndSendTransaction(transactionInput);

            signedUnityRequest.CheckTransactionRequest(() => signedUnityRequest.WaitForTransactionMining(network, onTransactionMined));
        }
    }

}