using Nethereum.JsonRpc.UnityClient;
using System;
using System.Collections;
using UnityEngine;

namespace Hope.Utils.Ethereum
{
    public sealed class EthTransactionPromise : Promise<EthTransactionPromise, string>
    {
        private readonly WaitForSeconds Waiter = new WaitForSeconds(UPDATE_INTERVAL);

        private const float UPDATE_INTERVAL = 5f;

        protected override void InternalBuild(params Func<object>[] args)
        {
            PollForTransactionReceipt((string)args[0]?.Invoke(), (string)args[1]?.Invoke()).StartCoroutine();
        }

        private IEnumerator PollForTransactionReceipt(string txHash, string networkUrl)
        {
            if (!AddressUtils.IsValidTransactionHash(txHash))
            {
                InternalInvokeError("Invalid transaction hash");
                yield return null;
            }

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
                InternalInvokeError(request.Exception.Message);
        }
    }
}