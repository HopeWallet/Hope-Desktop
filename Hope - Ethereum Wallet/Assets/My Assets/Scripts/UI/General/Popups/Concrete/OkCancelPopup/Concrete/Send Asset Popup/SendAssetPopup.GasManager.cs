using Hope.Utils.EthereumUtils;
using Nethereum.Util;
using System;
using System.Linq;
using System.Numerics;
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

        public decimal TransactionCost
        {
            get
            {
                BigInteger gasLimit = advancedModeToggle.IsToggledOn ? EnteredGasLimit : EstimatedGasLimit;
                BigInteger gasPrice = advancedModeToggle.IsToggledOn ? EnteredGasPrice.FunctionalGasPrice.Value : EstimatedGasPrice.FunctionalGasPrice.Value;

                return UnitConversion.Convert.FromWei(gasLimit * gasPrice);
            }
        }

        public bool IsValid => TransactionCost != 0;

        public GasPrice StandardGasPrice { get; set; }

        public GasPrice EstimatedGasPrice => new GasPrice(_estimatedGasPrice);

        public GasPrice EnteredGasPrice => new GasPrice(_enteredGasPrice);

        public BigInteger EstimatedGasLimit => _estimatedGasLimit;

        public BigInteger EnteredGasLimit => _enteredGasLimit;

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
            gasPriceField.text = RestrictToNumbersAndDots(gasPrice).LimitEnd(8);

            decimal price;
            decimal.TryParse(gasPriceField.text, out price);

            _enteredGasPrice = GasUtils.GetFunctionalGasPrice(price);
        }

        private void CheckTransactionSpeedSlider(float value)
        {
            decimal multiplier = decimal.Round((decimal)Mathf.Lerp(0.5f, 1.5f, value) * (decimal)Mathf.Lerp(1f, 4f, value - 0.5f), 2, MidpointRounding.AwayFromZero);
            _estimatedGasPrice = new BigInteger(multiplier * (decimal)StandardGasPrice.FunctionalGasPrice.Value);

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