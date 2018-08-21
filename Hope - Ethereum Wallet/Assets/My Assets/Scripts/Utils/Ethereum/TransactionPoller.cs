using Nethereum.JsonRpc.UnityClient;
using System;
using System.Collections;
using UnityEngine;

namespace Hope.Utils.Ethereum
{
    public sealed class TransactionPoller
    {
        private readonly WaitForSeconds Waiter = new WaitForSeconds(UPDATE_INTERVAL);

        private event Action OnTransactionSuccess;
        private event Action OnTransactionFail;

        private const float UPDATE_INTERVAL = 5f;

        public TransactionPoller(string txHash, string url)
        {
            PollForTransactionReceipt(txHash, url).StartCoroutine();
        }

        public TransactionPoller OnTransactionSuccessful(Action onTransactionSuccess)
        {
            OnTransactionSuccess += onTransactionSuccess;
            return this;
        }

        public TransactionPoller OnTransactionFailure(Action onTransactionFail)
        {
            OnTransactionFail += onTransactionFail;
            return this;
        }

        private IEnumerator PollForTransactionReceipt(string txHash, string networkUrl)
        {
            EthGetTransactionReceiptUnityRequest request = new EthGetTransactionReceiptUnityRequest(networkUrl);

            do
            {
                yield return Waiter;
                yield return request.SendRequest(txHash);
            }
            while (request.Exception != null || request.Result == null);

            if (request.Result?.Status?.Value == 1)
                OnTransactionSuccess?.Invoke();
            else
                OnTransactionFail?.Invoke();
        }
    }
}