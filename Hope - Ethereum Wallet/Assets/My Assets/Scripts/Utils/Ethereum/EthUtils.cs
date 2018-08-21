using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.RPC.Eth.DTOs;
using RandomNET.Integers;
using System;
using System.Collections;
using Debug = UnityEngine.Debug;

namespace Hope.Utils.Ethereum
{
    /// <summary>
    /// Utility class used for sending ether and getting the ether balance of certain addresses.
    /// </summary>
    public sealed class EthUtils
    {
        private static EthereumNetwork EthereumNetwork;

        /// <summary>
        /// Initializes the <see cref="WalletUtils"/> by assigning the reference to the active network.
        /// </summary>
        /// <param name="ethereumNetworkManager"> The active <see cref="EthereumNetworkManager"/>. </param>
        public EthUtils(EthereumNetworkManager ethereumNetworkManager)
        {
            EthereumNetwork = ethereumNetworkManager.CurrentNetwork;
        }

        /// <summary>
        /// Gets the amount of ether in a user's wallet.
        /// </summary>
        /// <param name="address"> The address to check for the ether balance. </param>
        /// <param name="onBalanceReceived"> Called when the eth balance has been received. </param>
        public static EthCallPromise<dynamic> GetEtherBalance(string address)
        {
            int id = RandomInt.Fast.GetInt();
            _AddressEthBalanceCoroutine(address, id).StartCoroutine();
            return EthCallPromise<dynamic>.GetPromise(id);
        }

        /// <summary>
        /// Sends ether from this wallet to a given address.
        /// </summary>
        /// <param name="signedUnityRequest"> The signed request to send the ether with. </param>
        /// <param name="walletAddress"> The address of the wallet sending the ether. </param>
        /// <param name="gasLimit"> The gas limit of the ether send transaction. </param>
        /// <param name="gasPrice"> The gas price of the ether send transaction. </param>
        /// <param name="addressTo"> The address to send the ether to. </param>
        /// <param name="amount"> The amount of ether to send. </param>
        public static EthTransactionPromise SendEther(
            TransactionSignedUnityRequest signedUnityRequest,
            string walletAddress,
            HexBigInteger gasLimit,
            HexBigInteger gasPrice,
            string addressTo,
            decimal amount)
        {
            int id = RandomInt.Fast.GetInt();
            _SendEtherCoroutine(signedUnityRequest, walletAddress, gasLimit, gasPrice, addressTo, amount, id).StartCoroutine();
            return EthTransactionPromise.GetPromise(id);
        }

        /// <summary>
        /// Gets the ether balance of a certain wallet.
        /// </summary>
        /// <param name="address"> The address to check the ether balance for. </param>
        /// <param name="promiseId"> The id of the promise which will contain the result for this eth balance call. </param>
        /// <returns> The time waited for the request to complete. </returns>
        private static IEnumerator _AddressEthBalanceCoroutine(string address, int promiseId)
        {
            EthGetBalanceUnityRequest request = new EthGetBalanceUnityRequest(EthereumNetwork.NetworkUrl);
            yield return request.SendRequest(address, BlockParameter.CreateLatest());

            EthCallPromise<dynamic>.GetPromise(promiseId).Build(request, () => SolidityUtils.ConvertFromUInt(request.Result.Value, 18));
        }

        /// <summary>
        /// Sends ether from one address to another.
        /// </summary>
        /// <param name="signedUnityRequest"> The signed request to send the ether with. </param>
        /// <param name="walletAddress"> The address of the wallet sending the ether. </param>
        /// <param name="gasLimit"> The gas limit of the ether send transaction. </param>
        /// <param name="gasPrice"> The gas price of the ether send transaction. </param>
        /// <param name="addressTo"> The address to send the ether to. </param>
        /// <param name="amount"> The amount to send in ether. </param>
        /// <param name="promiseId"> The id of the promise containing the result of this transaction. </param>
        /// <returns> The time waited for the request to be broadcast to the network. </returns>
        private static IEnumerator _SendEtherCoroutine(
            TransactionSignedUnityRequest signedUnityRequest,
            string walletAddress,
            HexBigInteger gasLimit,
            HexBigInteger gasPrice,
            string addressTo,
            dynamic amount,
            int promiseId)
        {
            TransactionInput transactionInput = new TransactionInput("", addressTo, walletAddress, gasLimit, gasPrice, new HexBigInteger(SolidityUtils.ConvertToUInt(amount, 18)));
            yield return signedUnityRequest.SignAndSendTransaction(transactionInput);

            EthTransactionPromise.GetPromise(promiseId).Build(signedUnityRequest, () => signedUnityRequest.Result, () => EthereumNetwork.NetworkUrl);
            //signedUnityRequest.CheckTransactionResult(() => signedUnityRequest.PollForTransactionReceipt(EthereumNetwork.NetworkUrl, ()
            //    => Debug.Log("Successfully sent " + amount + " Ether to address " + addressTo)));
        }
    }
}
