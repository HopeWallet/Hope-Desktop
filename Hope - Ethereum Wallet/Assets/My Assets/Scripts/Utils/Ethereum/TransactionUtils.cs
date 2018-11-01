using Hope.Utils.Promises;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections;
using System.Numerics;

namespace Hope.Utils.Ethereum
{
    /// <summary>
    /// Class which extends the nethereum unity transactions to allow for more actions on successful transactions and unsuccessful ones.
    /// </summary>
    public class TransactionUtils
    {
        private static EthereumNetworkManager EthereumNetworkManager;

        /// <summary>
        /// Initializes the <see cref="TransactionUtils"/> by assigning the reference to the active network.
        /// </summary>
        /// <param name="ethereumNetworkManager"> The active <see cref="global::EthereumNetworkManager"/>. </param>
        public TransactionUtils(EthereumNetworkManager ethereumNetworkManager)
        {
            EthereumNetworkManager = ethereumNetworkManager;
        }

        /// <summary>
        /// Gets the transaction count of an Ethereum address.
        /// </summary>
        /// <param name="address"> The Ethereum address to get the transaction count for. </param>
        /// <returns> The promise returning the transaction count. </returns>
        public static EthCallPromise<BigInteger> GetAddressTransactionCount(string address)
        {
            EthCallPromise<BigInteger> promise = new EthCallPromise<BigInteger>();
            _GetAddressTransactionCount(address, promise).StartCoroutine();
            return promise;
        }

        /// <summary>
        /// Gets the details of a transaction hash.
        /// </summary>
        /// <param name="txHash"> The transaction hash. </param>
        /// <param name="onTxReceived"> Action to call once the transaction is received. </param>
        /// <returns> The promise returning the transaction details. </returns>
        public static EthCallPromise<Transaction> GetTransactionDetails(string txHash)
        {
            EthCallPromise<Transaction> promise = new EthCallPromise<Transaction>();
            _GetTransactionDetailsCoroutine(txHash, promise).StartCoroutine();
            return promise;
        }

        /// <summary>
        /// The coroutine for getting the transaction count of an ethereum address.
        /// </summary>
        /// <param name="address"> The address to get the transaction count for. </param>
        /// <param name="promise"> Promise returning the transaction count. </param>
        /// <returns> The transaction count request to await. </returns>
        private static IEnumerator _GetAddressTransactionCount(string address, EthCallPromise<BigInteger> promise)
        {
            if (!AddressUtils.IsValidEthereumAddress(address))
                throw new ArgumentException("Expected valid Ethereum address.");

            var request = new EthGetTransactionCountUnityRequest(EthereumNetworkManager.CurrentNetwork.NetworkUrl);
            yield return request.SendRequest(address, BlockParameter.CreateLatest());

            promise.Build(request, () => request.Result.Value);
        }

        /// <summary>
        /// The coroutine for getting the details of a transaction.
        /// </summary>
        /// <param name="txHash"> The transaction hash. </param>
        /// <param name="promise"> Promise returning the transaction. </param>
        /// <returns> The transaction request to await. </returns>
        private static IEnumerator _GetTransactionDetailsCoroutine(string txHash, EthCallPromise<Transaction> promise)
        {
            if (!AddressUtils.IsValidTransactionHash(txHash))
                throw new ArgumentException("Expected valid Ethereum transaction hash.");

            var request = new EthGetTransactionByHashUnityRequest(EthereumNetworkManager.CurrentNetwork.NetworkUrl);
            yield return request.SendRequest(txHash);

            promise.Build(request, () => request.Result);
        }
    }
}