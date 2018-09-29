using Hope.Utils.Ethereum;
using Nethereum.JsonRpc.UnityClient;
using System;
using System.Collections;
using UnityEngine;

namespace Hope.Utils.Promises
{
    /// <summary>
    /// Class used for retrieving the eventual status of a transaction after it has been sent to the ethereum network.
    /// </summary>
    public sealed class EthTransactionPromise : Promise<EthTransactionPromise, string>
    {
        private readonly WaitForSeconds Waiter = new WaitForSeconds(UPDATE_INTERVAL);

        private readonly EthereumPendingTransactionManager ethereumPendingTransactionManager;
        private readonly string message;

        private const float UPDATE_INTERVAL = 5f;

        /// <summary>
        /// Initializes the EthTransactionPromise with the pending transaction manager.
        /// </summary>
        /// <param name="ethereumPendingTransactionManager"> The active EthereumPendingTransactionManager. </param>
        /// <param name="message"> The message representing the transaction. </param>
        public EthTransactionPromise(EthereumPendingTransactionManager ethereumPendingTransactionManager, string message)
        {
            this.ethereumPendingTransactionManager = ethereumPendingTransactionManager;
            this.message = message;
        }

        /// <summary>
        /// Initializes the EthTransactionPromise without any EthereumPendingTransactionManager.
        /// </summary>
        public EthTransactionPromise()
        {
        }

        /// <summary>
        /// Starts the transaction receipt polling system.
        /// </summary>
        /// <param name="args"> The EthTransactionPromise arguments which contain the transaction hash and the ethereum network. </param>
        protected override void InternalBuild(params Func<object>[] args)
        {
            PollForTransactionReceipt((string)args[0]?.Invoke(), (string)args[1]?.Invoke()).StartCoroutine();
        }

        /// <summary>
        /// Polls for the transaction receipt of a transaction until a result is found.
        /// </summary>
        /// <param name="txHash"> The transaction hash to poll for a receipt. </param>
        /// <param name="networkUrl"> The network url of the transaction hash. </param>
        private IEnumerator PollForTransactionReceipt(string txHash, string networkUrl)
        {
            if (!AddressUtils.IsValidTransactionHash(txHash))
            {
                InternalInvokeError("Invalid transaction hash");
                yield return null;
            }

            if (ethereumPendingTransactionManager != null)
                ethereumPendingTransactionManager.StartNewPendingTransaction(this, txHash, message);

            var request = new EthGetTransactionReceiptUnityRequest(networkUrl);

            do
            {
                yield return Waiter;
                yield return request.SendRequest(txHash);
            }
            while (request.Exception != null || request.Result == null);

            if (request.Result?.Status?.Value == 1)
                InternalInvokeSuccess("Transaction successful!");
            else
                InternalInvokeError(request?.Exception?.Message);
        }
    }
}