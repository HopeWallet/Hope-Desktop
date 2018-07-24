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
    public sealed class GasManager : IStandardGasPriceObservable, IPeriodicUpdater
    {
        private readonly TradableAssetManager tradableAssetManager;
        private readonly GasPriceObserver gasPriceObserver;
        private readonly PeriodicUpdateManager periodicUpdateManager;

        private readonly Toggle advancedModeToggle;

        private readonly Slider transactionSpeedSlider;

        private readonly TMP_InputField gasLimitField,
                                        gasPriceField;

        private GasPrice estimatedGasPrice,
                         enteredGasPrice;

        private BigInteger estimatedGasLimit,
                           enteredGasLimit;

        private const string RAND_ADDRESS = "0x0278018340138741034781903741800348314013";

        public float UpdateInterval => 10f;

        public decimal TransactionCost => UnitConversion.Convert.FromWei(TransactionGasLimit * TransactionGasPrice.FunctionalGasPrice.Value);

        public bool IsValid => TransactionCost != 0;

        public GasPrice TransactionGasPrice => advancedModeToggle.IsToggledOn ? enteredGasPrice : estimatedGasPrice;

        public BigInteger TransactionGasLimit => advancedModeToggle.IsToggledOn ? enteredGasLimit : estimatedGasLimit;

        public GasPrice StandardGasPrice { get; set; }

        public GasManager(
            TradableAssetManager tradableAssetManager,
            GasPriceObserver gasPriceObserver,
            PeriodicUpdateManager periodicUpdateManager,
            Toggle advancedModeToggle,
            Slider transactionSpeedSlider,
            TMP_InputField gasLimitField,
            TMP_InputField gasPriceField)
        {
            this.tradableAssetManager = tradableAssetManager;
            this.gasPriceObserver = gasPriceObserver;
            this.periodicUpdateManager = periodicUpdateManager;
            this.advancedModeToggle = advancedModeToggle;
            this.transactionSpeedSlider = transactionSpeedSlider;
            this.gasLimitField = gasLimitField;
            this.gasPriceField = gasPriceField;

            AddListenersAndObservables();
            EstimateGasLimit();
            CheckTransactionSpeedSlider(0.5f);
        }

        public void PeriodicUpdate()
        {
            EstimateGasLimit();
        }

        public void Destroy()
        {
            gasPriceObserver.UnsubscribeObservable(this);
            periodicUpdateManager.RemovePeriodicUpdater(this);
        }

        public void OnGasPricesUpdated()
        {
            if (!advancedModeToggle.IsToggledOn)
                gasPriceField.text = estimatedGasPrice.ReadableGasPrice.ToString();
        }

        private void AddListenersAndObservables()
        {
            gasPriceObserver.SubscribeObservable(this);
            periodicUpdateManager.AddPeriodicUpdater(this);

            transactionSpeedSlider.onValueChanged.AddListener(CheckTransactionSpeedSlider);
            gasLimitField.onValueChanged.AddListener(CheckGasLimitField);
            gasPriceField.onValueChanged.AddListener(CheckGasPriceField);
        }

        private void CheckGasLimitField(string gasLimit)
        {
            gasLimit = RestrictToNumbers(gasLimit);

            BigInteger newGasLimit;
            BigInteger.TryParse(gasLimit, out newGasLimit);

            if (!string.IsNullOrEmpty(gasLimitField.text) && advancedModeToggle.IsToggledOn && newGasLimit <= enteredGasLimit)
                return;

            gasLimitField.text = gasLimit;
            enteredGasLimit = newGasLimit;
        }

        private void CheckGasPriceField(string gasPrice)
        {
            gasPriceField.text = RestrictToNumbersAndDots(gasPrice).LimitEnd(8);

            decimal price;
            decimal.TryParse(gasPriceField.text, out price);

            enteredGasPrice = new GasPrice(GasUtils.GetFunctionalGasPrice(price));
        }

        private void CheckTransactionSpeedSlider(float value)
        {
            decimal multiplier = decimal.Round((decimal)Mathf.Lerp(0.5f, 1.5f, value) * (decimal)Mathf.Lerp(1f, 4f, value - 0.5f), 2, MidpointRounding.AwayFromZero);
            estimatedGasPrice = new GasPrice(new BigInteger(multiplier * (decimal)StandardGasPrice.FunctionalGasPrice.Value));

            OnGasPricesUpdated();
        }

        private void EstimateGasLimit()
        {
            tradableAssetManager.ActiveTradableAsset.GetTransferGasLimit(RAND_ADDRESS, (decimal)tradableAssetManager.ActiveTradableAsset.AssetBalance, OnGasLimitReceived);
        }

        private void OnGasLimitReceived(BigInteger limit)
        {
            estimatedGasLimit = limit;
            CheckGasLimitField(limit.ToString());
        }

        private string RestrictToNumbers(string str) => new string(str.Where(c => char.IsDigit(c)).ToArray());

        private string RestrictToNumbersAndDots(string str) => new string(str.Where(c => char.IsDigit(c) || c == '.').ToArray());
    }
}