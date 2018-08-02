using Nethereum.Util;
using System.Numerics;
using TMPro;
using UnityEngine.UI;

public sealed partial class LockPRPSPopup
{
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

        public void Stop()
        {
            transactionSpeedSlider.Stop();
        }

        private void UpdateGasPriceEstimate(GasPrice gasPrice)
        {
            TransactionGasPrice = gasPrice;
            transactionFeeText.text = "~ " + TransactionFee.ToString().LimitEnd(14).TrimEnd('0') + " ETH";
        }
    }
}