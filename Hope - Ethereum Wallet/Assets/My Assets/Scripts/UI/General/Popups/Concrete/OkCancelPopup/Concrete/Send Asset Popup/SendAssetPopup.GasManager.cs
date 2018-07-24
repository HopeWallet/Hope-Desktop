using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed partial class SendAssetPopup : OkCancelPopupComponent<SendAssetPopup>
{
    public sealed class GasManager : IStandardGasPriceObservable
    {

        private readonly TradableAssetManager tradableAssetManager;
        private readonly GasPriceObserver gasPriceObserver;

        private readonly Toggle advancedModeToggle;

        private readonly Slider transactionSpeedSlider;

        private readonly TMP_InputField gasLimitField,
                                        gasPriceField;

        private BigInteger _estimatedGasPrice;
        private BigInteger _enteredGasPrice;
        private BigInteger _estimatedGasLimit;
        private BigInteger _enteredGasLimit;

        private const string RAND_ADDRESS = "0x0278018340138741034781903741800348314013";

        public GasPrice StandardGasPrice { get; set; }

        public GasPrice EstimatedGasPrice => new GasPrice(_estimatedGasPrice);

        public GasPrice EnteredGasPrice => new GasPrice(_enteredGasPrice);

        public BigInteger EstimatedGasLimit => _estimatedGasLimit;

        public BigInteger EnteredGasLimit => _enteredGasLimit;

        public bool IsValid { get; private set; }

        public GasManager(
            TradableAssetManager tradableAssetManager,
            GasPriceObserver gasPriceObserver,
            Toggle advancedModeToggle,
            Slider transactionSpeedSlider,
            TMP_InputField gasLimitField,
            TMP_InputField gasPriceField)
        {
            this.tradableAssetManager = tradableAssetManager;
            this.gasPriceObserver = gasPriceObserver;
            this.advancedModeToggle = advancedModeToggle;
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
            if (!advancedModeToggle.IsToggledOn)
                gasPriceField.text = EstimatedGasPrice.ReadableGasPrice.ToString();
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
            gasLimitField.text = RestrictToNumbers(gasLimit);
            BigInteger.TryParse(gasLimitField.text, out _enteredGasLimit);
        }

        private void CheckGasPriceField(string gasPrice)
        {
            gasPriceField.text = RestrictToNumbersAndDots(gasPrice);
            BigInteger.TryParse(gasPriceField.text, out _enteredGasPrice);
        }

        private void CheckTransactionSpeedSlider(float value)
        {
            _estimatedGasPrice = new BigInteger((decimal)Mathf.Lerp(0.25f, 4f, value) * (decimal)StandardGasPrice.FunctionalGasPrice.Value);
            Debug.Log(EstimatedGasPrice.ReadableGasPrice + " Gwei => " + StandardGasPrice.ReadableGasPrice + " Gwei");
            OnGasPricesUpdated();
        }

        private void EstimateGasLimit()
        {
            tradableAssetManager.ActiveTradableAsset.GetTransferGasLimit(RAND_ADDRESS, (decimal)tradableAssetManager.ActiveTradableAsset.AssetBalance, OnGasLimitReceived);
        }

        private void OnGasLimitReceived(BigInteger limit)
        {
            _estimatedGasLimit = limit;
            CheckGasLimitField(limit.ToString());
        }

        private string RestrictToNumbers(string str) => new string(str.Where(c => char.IsDigit(c)).ToArray());

        private string RestrictToNumbersAndDots(string str) => new string(str.Where(c => char.IsDigit(c) || c == '.').ToArray());
    }
}