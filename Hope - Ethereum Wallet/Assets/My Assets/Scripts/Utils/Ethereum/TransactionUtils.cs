using Nethereum.JsonRpc.UnityClient;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hope.Utils.EthereumUtils
{

    /// <summary>
    /// Class which extends the nethereum unity transactions to allow for more actions on successful transactions and unsuccessful ones.
    /// </summary>
    public static class TransactionUtils
    {

        private static readonly WaitForSeconds Waiter = new WaitForSeconds(WAIT_INTERVAL);
        private const float WAIT_INTERVAL = 5f;

        /// <summary>
        /// Checks the details of a transaction hash.
        /// </summary>
        /// <param name="txHash"> The transaction hash. </param>
        /// <param name="onTxReceived"> Action to call once the transaction is received. </param>
        public static void CheckTransactionDetails(string txHash, Action<Transaction> onTxReceived) => _GetTransactionDetailsCoroutine(txHash, onTxReceived).StartCoroutine();

        /// <summary>
        /// Checks a UnityRequest transaction, and executes an action if it was successful.
        /// </summary>
        /// <typeparam name="T"> The type of the return value of the transaction result. </typeparam>
        /// <param name="unityRequest"> The transaction request. </param>
        /// <param name="onSuccessfulTransaction"> Action called if the transaction was successful. </param>
        /// <param name="readExceptionMessage"> Whether the exception message should be displayed. </param>
        public static void CheckTransactionResult<T>(this UnityRequest<T> unityRequest, Action onSuccessfulTransaction, bool readExceptionMessage = true)
        {
            var exception = unityRequest.Exception;
            var result = unityRequest.Result;

            if (exception == null && !EqualityComparer<T>.Default.Equals(result, default(T)))
                onSuccessfulTransaction?.Invoke();
            else if (readExceptionMessage)
                ExceptionManager.DisplayException(exception);
        }

        /// <summary>
        /// Waits for a transaction to be mined and then executes an action.
        /// </summary>
        /// <param name="unityRequest"> The unity request with a tx hash result to wait for mining. </param>
        /// <param name="networkUrl"> The network to listen to the transaction on. </param>
        /// <param name="onMiningFinished"> Action to execute once the transaction has successfully been mined. </param>
        public static void PollForTransactionReceipt(this UnityRequest<string> unityRequest, string networkUrl, Action onMiningFinished)
            => _PollForTransactionReceiptCoroutine(unityRequest.Result, networkUrl, onMiningFinished).StartCoroutine();

        /// <summary>
        /// Coroutine which will iteratively check a transaction hash to see if it has been mined or not.
        /// </summary>
        /// <param name="txHash"> The transaction hash to check. </param>
        /// <param name="networkUrl"> The network to listen to the transaction on. </param>
        /// <param name="onMiningFinished"> Action to execute once the transaction has successfully been mined. </param>
        /// <returns></returns>
        private static IEnumerator _PollForTransactionReceiptCoroutine(string txHash, string networkUrl, Action onMiningFinished)
        {
            var request = new EthGetTransactionReceiptUnityRequest(networkUrl);
            var mined = false;

            while (!mined)
            {
                yield return Waiter;
                yield return request.SendRequest(txHash);
                request.CheckTransactionResult(() => mined = true, false);
            }

            if (request.Result?.Status?.Value == 1)
                onMiningFinished?.Invoke();
            else
                ExceptionManager.DisplayException(new Exception("Transaction failed."));
        }

        /// <summary>
        /// Starts the coroutine for getting the details of a transaction.
        /// </summary>
        /// <param name="txHash"> The transaction hash. </param>
        /// <param name="onTxReceived"> Action to call once the transaction details have been received. </param>
        /// <returns> The transaction request to await. </returns>
        private static IEnumerator _GetTransactionDetailsCoroutine(string txHash, Action<Transaction> onTxReceived)
        {
            var request = new EthGetTransactionByHashUnityRequest(EthereumNetworkManager.Instance.CurrentNetwork.NetworkUrl);

            yield return request.SendRequest(txHash);

            onTxReceived?.Invoke(request.Result);
        }
    }
}