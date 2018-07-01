using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using System;
using System.Collections;
using UnityEngine;

namespace Hope.Utils.EthereumUtils
{

    /// <summary>
    /// Class with a bunch of extensions which allow for smoother wallet functionality.
    /// </summary>
    public static class WalletUtils
    {

        private const string PATH_TWELVE_WORDS = "m/44'/60'/0'/0/x";
        private const string PATH_TWENTYFOUR_WORDS = "m/44'/60'/0'/x";

        /// <summary>
        /// Determines the correct path to use with the wallet derivation based on how many words are in the mnemonic phrase.
        /// </summary>
        /// <param name="mnemonicPhrase"> The mnemonic phrase to derive the path from. </param>
        /// <returns> The correct path to derive the wallet from. </returns>
        public static string DetermineCorrectPath(string mnemonicPhrase)
        {
            var wordCount = mnemonicPhrase.GetMnemonicWords().Length;

            if (wordCount == 12)
                return PATH_TWELVE_WORDS;
            else if (wordCount == 24)
                return PATH_TWENTYFOUR_WORDS;
            else
                return null;
        }

		/// <summary>
		/// Gets the individual words of a mnemonic phrase.
		/// </summary>
		/// <param name="str"> The string which contains the words. </param>
		/// <returns> The array of individual words. </returns>
		public static string[] GetMnemonicWords(this string str) => str.Trim().Split(' ', '\t', '\n');

		/// <summary>
		/// Gets the amount of ether in a user's wallet.
		/// </summary>
		/// <param name="wallet"> The wallet to check for the ether amount. </param>
		/// <param name="onBalanceReceived"> Called when the eth balance has been received. </param>
		public static void GetEthBalance(UserWallet wallet, Action<dynamic> onBalanceReceived)
            => _AddressEthBalanceCoroutine(wallet.Address, onBalanceReceived).StartCoroutine();

        /// <summary>
        /// Sends ether from this wallet to a given address.
        /// </summary>
        /// <param name="signedUnityRequest"> The signed request to send the ether with. </param>
        /// <param name="walletAddress"> The address of the wallet sending the ether. </param>
        /// <param name="gasLimit"> The gas limit of the ether send transaction. </param>
        /// <param name="gasPrice"> The gas price of the ether send transaction. </param>
        /// <param name="address"> The address to send the ether to. </param>
        /// <param name="amount"> The amount of ether to send. </param>
        public static void SendEther(TransactionSignedUnityRequest signedUnityRequest, string walletAddress,
            HexBigInteger gasLimit, HexBigInteger gasPrice, string address, decimal amount)
        {
            _SendEtherCoroutine(signedUnityRequest, walletAddress, gasLimit, gasPrice, address, amount).StartCoroutine();
        }

        /// <summary>
        /// Gets the ether balance of a certain wallet.
        /// </summary>
        /// <param name="address"> The address to check the ether balance for. </param>
        /// <param name="onBalanceReceived"> Callback which is executed when the balance is received. </param>
        /// <returns> The time waited for the request to complete. </returns>
        private static IEnumerator _AddressEthBalanceCoroutine(string address, Action<dynamic> onBalanceReceived)
        {
            var request = new EthGetBalanceUnityRequest(EthereumNetworkManager.Instance.CurrentNetwork.NetworkUrl);

            yield return request.SendRequest(address, BlockParameter.CreateLatest());

            request.CheckTransactionRequest(() => onBalanceReceived(UnitConversion.Convert.FromWei(request.Result, 18)));
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
        /// <returns> The time waited for the request to be broadcast ot the network. </returns>
        private static IEnumerator _SendEtherCoroutine(TransactionSignedUnityRequest signedUnityRequest, string walletAddress,
            HexBigInteger gasLimit, HexBigInteger gasPrice, string addressTo, dynamic amount)
        {
            var networkUrl = EthereumNetworkManager.Instance.CurrentNetwork.NetworkUrl;
            var transactionInput = new TransactionInput("", addressTo, walletAddress, gasLimit, gasPrice, new HexBigInteger(UnitConversion.Convert.ToWei(amount)));

            yield return signedUnityRequest.SignAndSendTransaction(transactionInput);

            signedUnityRequest.CheckTransactionRequest(() => signedUnityRequest.WaitForTransactionMining(networkUrl, ()
                => Debug.Log("Successfully sent " + amount + " Ether to address " + addressTo)));
        }

    }

}