using Nethereum.Util;
using System.Numerics;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Class used for locking purpose.
/// </summary>
public sealed partial class LockPRPSPopup
{
    /// <summary>
    /// Class used for managing the gas costs for locking purpose.
    /// </summary>
    public sealed class GasManager
    {
        private readonly LockPRPSManager lockPRPSManager;
        private readonly TransactionSpeedSlider transactionSpeedSlider;
        private readonly TMP_Text transactionFeeText;

        /// <summary>
        /// The approximate transaction fee of the transaction based on the gas limit and gas price.
        /// </summary>
        public decimal TransactionFee => UnitConversion.Convert.FromWei(TransactionGasLimit * (TransactionGasPrice.FunctionalGasPrice == null ? 0 : TransactionGasPrice.FunctionalGasPrice.Value));

        /// <summary>
        /// Is valid if the transaction can be sent based on the current gas price and gas limit.
        /// </summary>
        public bool IsValid => TransactionFee > 0;

        /// <summary>
        /// The gas price to use for the transaction.
        /// </summary>
        public GasPrice TransactionGasPrice { get; private set; }

        /// <summary>
        /// The gas limit to use for the transaction.
        /// </summary>
        public BigInteger TransactionGasLimit => lockPRPSManager.GasLimit;

        /// <summary>
        /// Initializes the GasMAnager by assigning required dependencies.
        /// </summary>
        /// <param name="lockPRPSManager"> The active LockPRPSManager. </param>
        /// <param name="gasPriceObserver"> The active GasPriceObserver. </param>
        /// <param name="slider"> The slider for controlling the gas prices. </param>
        /// <param name="transactionFeeText"> The text used for displaying the gas cost of the transaction. </param>
        public GasManager(
            LockPRPSManager lockPRPSManager,
            GasPriceObserver gasPriceObserver,
            Slider slider,
            TMP_Text transactionFeeText)
        {
            this.lockPRPSManager = lockPRPSManager;
            this.transactionFeeText = transactionFeeText;

            transactionSpeedSlider = new TransactionSpeedSlider(gasPriceObserver, slider, UpdateGasPriceEstimate);
            transactionSpeedSlider.Start();
        }

        /// <summary>
        /// Stops the GasManager.
        /// </summary>
        public void Stop()
        {
            transactionSpeedSlider.Stop();
        }

        /// <summary>
        /// Updates the gas price estimate given the new GasPrice.
        /// </summary>
        /// <param name="gasPrice"> The new GasPrice to set. </param>
        private void UpdateGasPriceEstimate(GasPrice gasPrice)
        {
            TransactionGasPrice = gasPrice;
            transactionFeeText.text = "~ " + TransactionFee.ToString().LimitEnd(14).TrimEnd('0') + " ETH";
        }
    }
}