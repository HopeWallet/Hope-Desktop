using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

public sealed partial class SendAssetPopup : OkCancelPopupComponent<SendAssetPopup>
{
    public sealed class GasManager : IStandardGasPriceObservable
    {

        private readonly TradableAssetManager tradableAssetManager;
        private readonly GasPriceObserver gasPriceObserver;

        private readonly Slider transactionSpeedSlider;

        private readonly TMP_InputField gasLimitField,
                                        gasPriceField;

        private const string RAND_ADDRESS = "0x0278018340138741034781903741800348314013";

        public GasPrice StandardGasPrice { get; set; }

        public BigInteger EstimatedGasPrice { get; private set; }

        public BigInteger EnteredGasPrice { get; private set; }

        public BigInteger EstimatedGasLimit { get; private set; }

        public BigInteger EnteredGasLimit { get; private set; }

        public bool IsValid { get; private set; }

        public GasManager(
            TradableAssetManager tradableAssetManager,
            GasPriceObserver gasPriceObserver,
            Slider transactionSpeedSlider,
            TMP_InputField gasLimitField,
            TMP_InputField gasPriceField)
        {
            this.tradableAssetManager = tradableAssetManager;
            this.gasPriceObserver = gasPriceObserver;
            this.transactionSpeedSlider = transactionSpeedSlider;
            this.gasLimitField = gasLimitField;
            this.gasPriceField = gasPriceField;

            AddListenersAndObservables();
            EstimateGasLimit();
            CheckTransactionSpeedSlider(0.5f);
        }

        public void Destroy()
        {
            gasPriceObserver.UnsubscribeObservable(this);
        }

        public void OnGasPricesUpdated()
        {
        }

        private void AddListenersAndObservables()
        {
            gasPriceObserver.SubscribeObservable(this);

            transactionSpeedSlider.onValueChanged.AddListener(CheckTransactionSpeedSlider);
            gasLimitField.onValueChanged.AddListener(CheckGasLimitField);
            gasPriceField.onValueChanged.AddListener(CheckGasPriceField);
        }

        private void CheckGasLimitField(string gasLimit)
        {
        }

        private void CheckGasPriceField(string gasPrice)
        {
        }

        private void CheckTransactionSpeedSlider(float value)
        {
        }

        private void EstimateGasLimit()
        {
            tradableAssetManager.ActiveTradableAsset.GetTransferGasLimit(RAND_ADDRESS, (decimal)tradableAssetManager.ActiveTradableAsset.AssetBalance, OnGasLimitReceived);
        }

        private void OnGasLimitReceived(BigInteger limit)
        {
            EstimatedGasLimit = limit;
        }
    }
}