using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.RPC.Eth.DTOs;
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
        public static void GetEthBalance(string address, Action<dynamic> onBalanceReceived)
        {
            _AddressEthBalanceCoroutine(address, onBalanceReceived).StartCoroutine();
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
        public static void SendEther(
            TransactionSignedUnityRequest signedUnityRequest,
            string walletAddress,
            HexBigInteger gasLimit,
            HexBigInteger gasPrice,
            string addressTo,
            decimal amount)
        {
            _SendEtherCoroutine(signedUnityRequest, walletAddress, gasLimit, gasPrice, addressTo, amount).StartCoroutine();
        }

        /// <summary>
        /// Gets the ether balance of a certain wallet.
        /// </summary>
        /// <param name="address"> The address to check the ether balance for. </param>
        /// <param name="onBalanceReceived"> Callback which is executed when the balance is received. </param>
        /// <returns> The time waited for the request to complete. </returns>
        private static IEnumerator _AddressEthBalanceCoroutine(string address, Action<dynamic> onBalanceReceived)
        {
            EthGetBalanceUnityRequest request = new EthGetBalanceUnityRequest(EthereumNetwork.NetworkUrl);
            yield return request.SendRequest(address, BlockParameter.CreateLatest());

            request.CheckTransactionResult(() => onBalanceReceived(SolidityUtils.ConvertFromUInt(request.Result.Value, 18)));
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
        /// <returns> The time waited for the request to be broadcast to the network. </returns>
        private static IEnumerator _SendEtherCoroutine(
            TransactionSignedUnityRequest signedUnityRequest,
            string walletAddress,
            HexBigInteger gasLimit,
            HexBigInteger gasPrice,
            string addressTo,
            dynamic amount)
        {
            TransactionInput transactionInput = new TransactionInput("", addressTo, walletAddress, gasLimit, gasPrice, new HexBigInteger(SolidityUtils.ConvertToUInt(amount, 18)));
            yield return signedUnityRequest.SignAndSendTransaction(transactionInput);

            signedUnityRequest.CheckTransactionResult(() => signedUnityRequest.PollForTransactionReceipt(EthereumNetwork.NetworkUrl, ()
                => Debug.Log("Successfully sent " + amount + " Ether to address " + addressTo)));
        }
    }
}
