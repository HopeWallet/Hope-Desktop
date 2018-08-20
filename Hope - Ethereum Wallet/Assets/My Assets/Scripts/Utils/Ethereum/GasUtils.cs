using Nethereum.Contracts.Extensions;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using System;
using System.Collections;
using System.Numerics;

namespace Hope.Utils.Ethereum
{
    /// <summary>
    /// Class used for anything related to ethereum transaction gas.
    /// </summary>
    public class GasUtils
    {
        /// <summary>
        /// Enum for the type of gas price to aim for.
        /// </summary>
        public enum GasPriceTarget { Slow, Standard, Fast };

        private static EthereumNetwork EthereumNetwork;

        private const int ETH_GAS_LIMIT = 21000;

        /// <summary>
        /// Initializes the <see cref="GasUtils"/> by assigning the reference to the active network.
        /// </summary>
        /// <param name="ethereumNetworkManager"> The active <see cref="EthereumNetworkManager"/>. </param>
        public GasUtils(EthereumNetworkManager ethereumNetworkManager)
        {
            EthereumNetwork = ethereumNetworkManager.CurrentNetwork;
        }

        /// <summary>
        /// Estimates the gas limit of a <see cref="ContractFunction"/>.
        /// </summary>
        /// <typeparam name="TFunc"> The <see cref="ContractFunction"/> to estimate the gas limit for. </typeparam>
        /// <param name="contractAddress"> The address of the contract function to estimate. </param>
        /// <param name="callerAddress"> The address of the sender. </param>
        /// <param name="onGasReceived"> Action called with the estimated gas limit. </param>
        /// <param name="input"> The input parameters of the function. </param>
        public static void EstimateGasLimit<TFunc>(
            string contractAddress,
            string callerAddress,
            Action<BigInteger> onGasReceived,
            params object[] input) where TFunc : ContractFunction
        {
            _EstimateGasLimitCoroutine(ContractFunction.CreateFunction<TFunc>(callerAddress, input).CreateTransactionInput(contractAddress), onGasReceived).StartCoroutine();
        }

        /// <summary>
        /// Estimates the gas limit for a basic ether transaction.
        /// </summary>
        /// <param name="onGasReceived"> Action to execute once the eth gas limit has been received. </param>
        public static void EstimateGasLimit(Action<BigInteger> onGasReceived) => onGasReceived(new BigInteger(ETH_GAS_LIMIT));

        /// <summary>
        /// Estimates the gas price given the GasPriceTarget.
        /// </summary>
        /// <param name="gasPriceTarget">  The target gas price to aim for. </param>
        /// <param name="onGasPriceReceived"> Action to execute once the gas price has been received. </param>
        public static void EstimateGasPrice(GasPriceTarget gasPriceTarget, Action<BigInteger> onGasPriceReceived)
            => _EstimateGasPriceCoroutine(gasPriceTarget, onGasPriceReceived).StartCoroutine();

        /// <summary>
        /// Gets the readable gas price from the regular gwei form of the gas price.
        /// </summary>
        /// <param name="gasPrice"> The gas price to convert. </param>
        /// <returns> The readable gas price converted from gwei to wei.  </returns>
        public static decimal GetReadableGasPrice(BigInteger gasPrice) => UnitConversion.Convert.FromWei(gasPrice, UnitConversion.EthUnit.Gwei);

        /// <summary>
        /// Gets the functional gas price used for transaction input.
        /// </summary>
        /// <param name="gasPrice"> The gas price in wei to convert to the form used in transaction input. </param>
        /// <returns> The functional gas price. </returns>
        public static BigInteger GetFunctionalGasPrice(decimal gasPrice) => UnitConversion.Convert.ToWei(gasPrice, UnitConversion.EthUnit.Gwei);

        /// <summary>
        /// Calculates the maximum gas cost of a transaction given the gas price and the gas limit.
        /// </summary>
        /// <param name="gasPrice"> The functional gas price. </param>
        /// <param name="gasLimit"> The gas limit. </param>
        /// <returns> The maximum gas cost of in ether. </returns>
        public static decimal CalculateMaximumGasCost(BigInteger gasPrice, BigInteger gasLimit) => UnitConversion.Convert.FromWei(gasPrice * gasLimit);

        /// <summary>
        /// Modifies the current gas price to reflect the GasPriceTarget.
        /// </summary>
        /// <param name="priceTarget"> The target to modify the gas price towards. </param>
        /// <param name="currentPrice"> The current functional gas price to modify. </param>
        /// <returns> The new functional gas price after modification. </returns>
        private static BigInteger ModifyGasPrice(GasPriceTarget priceTarget, BigInteger currentPrice)
        {
            switch (priceTarget)
            {
                case GasPriceTarget.Slow:
                    return (currentPrice * 2) / 3;
                case GasPriceTarget.Fast:
                    return currentPrice * 2;
                default:
                    return currentPrice;
            }
        }

        /// <summary>
        /// Estimates the gas limit of a certain function of a contract.
        /// </summary>
        /// <param name="transactionInput"> The transaction input to estimate the gas limit for. </param>
        /// <param name="onGasReceived"> Callback which executes an action with the gas limit as a parameter. </param>
        /// <returns> The time taken to retrieve the estimated gas limit. </returns>
        private static IEnumerator _EstimateGasLimitCoroutine(TransactionInput transactionInput, Action<BigInteger> onGasReceived)
        {
            var request = new EthEstimateGasUnityRequest(EthereumNetwork.NetworkUrl);
            yield return request.SendRequest(transactionInput);

            request.CheckTransactionResult(() => onGasReceived((request.Result.Value * 100) / 90));
        }

        /// <summary>
        /// Estimates the gas price based on current network congestion.
        /// </summary>
        /// <param name="gasPriceTarget"> The GasPriceTarget to aim for. </param>
        /// <param name="onGasPriceReceived"> Action to execute once the gas price has been received. </param>
        /// <returns> The time taken to retrieve the estimated gas limit. </returns>
        private static IEnumerator _EstimateGasPriceCoroutine(GasPriceTarget gasPriceTarget, Action<BigInteger> onGasPriceReceived)
        {
            var request = new EthGasPriceUnityRequest(EthereumNetwork.NetworkUrl);
            yield return request.SendRequest();

            request.CheckTransactionResult(() => onGasPriceReceived(ModifyGasPrice(gasPriceTarget, request.Result.Value)));
        }
    }
}