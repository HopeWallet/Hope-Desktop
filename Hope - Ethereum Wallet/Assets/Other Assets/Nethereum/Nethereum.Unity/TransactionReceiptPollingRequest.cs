using System.Collections;
using Nethereum.RPC.Eth.DTOs;
using UnityEngine;

namespace Nethereum.JsonRpc.UnityClient
{
    public class TransactionReceiptPollingRequest : UnityRequest<TransactionReceipt>
    {
        private string _url;
        private readonly EthGetTransactionReceiptUnityRequest _ethGetTransactionReceipt;

        public TransactionReceiptPollingRequest(string url)
        {
            _url = url;
            _ethGetTransactionReceipt = new EthGetTransactionReceiptUnityRequest(_url);
        }

        public IEnumerator PollForReceipt(string transactionHash, float secondsToWait)
        {
            if (string.IsNullOrEmpty(transactionHash))
                yield break;

            WaitForSeconds waiter = new WaitForSeconds(secondsToWait);
            TransactionReceipt receipt = null;
            Result = null;

            while (receipt == null)
            {
                yield return _ethGetTransactionReceipt.SendRequest(transactionHash);

                if (_ethGetTransactionReceipt.Exception == null)
                {
                    receipt = _ethGetTransactionReceipt.Result;
                }
                else
                {
                    Exception = _ethGetTransactionReceipt.Exception;
                    yield break;
                }

                yield return waiter;
            }

            Result = receipt;
        }
    }
}