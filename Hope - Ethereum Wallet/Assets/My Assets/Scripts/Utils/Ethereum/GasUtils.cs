using Nethereum.Contracts;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.Util;
using System;
using System.Collections;
using System.Numerics;

namespace Hope.Utils.EthereumUtils
{

    /// <summary>
    /// Class used for anything related to ethereum transaction gas.
    /// </summary>
    public static class GasUtils
    {

        /// <summary>
        /// Enum for the type of gas price to aim for.
        /// </summary>
        public enum GasPriceTarget { Slow, Standard, Fast };

        private const int ETH_SEND_LIMIT = 21000;

        /// <summary>
        /// Estimates the gas limit for a contract function.
        /// </summary>
        /// <param name="function"> The contract function to get the gas limit for. </param>
        /// <param name="callerAddress"> The address of the function caller. (msg.sender) </param>
        /// <param name="onGasReceived"> Action to execute once the gas limit has been received. </param>
        /// <param name="input"> The function input. </param>
        public static void EstimateGasLimit(Function function, string callerAddress,
            Action<BigInteger> onGasReceived, params object[] input) => _EstimateGasLimitCoroutine(function, callerAddress, onGasReceived, input).StartCoroutine();

        /// <summary>
        /// Estimates the gas limit for a basic ether transaction.
        /// </summary>
        /// <param name="onGasReceived"> Action to execute once the eth gas limit has been received. </param>
        public static void EstimateGasLimit(Action<BigInteger> onGasReceived) => onGasReceived(new BigInteger(ETH_SEND_LIMIT));

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
        /// <param name="function"> The function to estimate the gas limit for. </param>
        /// <param name="callerAddress"> The address of the one calling this function. </param>
        /// <param name="onGasReceived"> Callback which executes an action with the gas limit as a parameter. </param>
        /// <param name="input"> The input of the function. </param>
        /// <returns> The time taken to retrieve the estimated gas limit. </returns>
        private static IEnumerator _EstimateGasLimitCoroutine(Function function, string callerAddress, Action<BigInteger> onGasReceived, params object[] input)
        {
            var request = new EthEstimateGasUnityRequest(EthereumNetworkManager.Instance.CurrentNetwork.NetworkUrl);

            yield return request.SendRequest(function.CreateTransactionInput(callerAddress, input));

            request.CheckTransactionRequest(() => onGasReceived((request.Result.Value * 10) / 9));
        }

        /// <summary>
        /// Estimates the gas price based on current network congestion.
        /// </summary>
        /// <param name="gasPriceTarget"> The GasPriceTarget to aim for. </param>
        /// <param name="onGasPriceReceived"> Action to execute once the gas price has been received. </param>
        /// <returns> The time taken to retrieve the estimated gas limit. </returns>
        private static IEnumerator _EstimateGasPriceCoroutine(GasPriceTarget gasPriceTarget, Action<BigInteger> onGasPriceReceived)
        {
            var request = new EthGasPriceUnityRequest(EthereumNetworkManager.Instance.CurrentNetwork.NetworkUrl);

            yield return request.SendRequest();

            request.CheckTransactionRequest(() => onGasPriceReceived(ModifyGasPrice(gasPriceTarget, request.Result.Value)));
        }

    }

}