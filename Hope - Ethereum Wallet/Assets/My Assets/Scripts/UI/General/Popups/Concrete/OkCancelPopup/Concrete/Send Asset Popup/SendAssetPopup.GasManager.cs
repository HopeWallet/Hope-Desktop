using Hope.Utils.EthereumUtils;
using Nethereum.Hex.HexTypes;
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

        private readonly TMP_Text transactionFeeText;

        private GasPrice estimatedGasPrice,
                         enteredGasPrice;

        private BigInteger estimatedGasLimit,
                           enteredGasLimit;

        private Action onGasChanged;

        private const int GAS_FIELD_MAX_LENGTH = 8;
        private const string RAND_ADDRESS = "0x0278018340138741034781903741800348314013";

        public float UpdateInterval => 10f;

        public decimal TransactionFee => UnitConversion.Convert.FromWei(TransactionGasLimit * (TransactionGasPrice.FunctionalGasPrice == null ? 0 : TransactionGasPrice.FunctionalGasPrice.Value));

        public bool IsValid => TransactionFee != 0;

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
            TMP_InputField gasPriceField,
            TMP_Text transactionFeeText)
        {
            this.tradableAssetManager = tradableAssetManager;
            this.gasPriceObserver = gasPriceObserver;
            this.periodicUpdateManager = periodicUpdateManager;
            this.advancedModeToggle = advancedModeToggle;
            this.transactionSpeedSlider = transactionSpeedSlider;
            this.gasLimitField = gasLimitField;
            this.gasPriceField = gasPriceField;
            this.transactionFeeText = transactionFeeText;

            AddGasListener(() => this.transactionFeeText.text = "~ " + TransactionFee.ToString() + " ETH");

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

            onGasChanged?.Invoke();
        }

        public void AddGasListener(Action gasChanged)
        {
            if (onGasChanged == null)
                onGasChanged = gasChanged;
            else
                onGasChanged += gasChanged;
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
            BigInteger.TryParse(gasLimit, out enteredGasLimit);
            gasLimitField.text = RestrictToNumbers(gasLimit).LimitEnd(GAS_FIELD_MAX_LENGTH);

            onGasChanged?.Invoke();
        }

        private void CheckGasPriceField(string gasPrice)
        {
            gasPriceField.text = RestrictToNumbersAndDots(gasPrice).LimitEnd(GAS_FIELD_MAX_LENGTH);

            decimal price;
            decimal.TryParse(gasPriceField.text, out price);

            enteredGasPrice = new GasPrice(GasUtils.GetFunctionalGasPrice(price));

            onGasChanged?.Invoke();
        }

        private void CheckTransactionSpeedSlider(float value)
        {
            decimal multiplier = decimal.Round((decimal)Mathf.Lerp(0.6f, 1.4f, value) * (decimal)Mathf.Lerp(1f, 4f, value - 0.45f), 2, MidpointRounding.AwayFromZero);
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

            if (string.IsNullOrEmpty(gasLimitField.text) || !advancedModeToggle.IsToggledOn || limit > enteredGasLimit)
                CheckGasLimitField(limit.ToString());
        }

        private string RestrictToNumbers(string str) => new string(str.Where(c => char.IsDigit(c)).ToArray());

        private string RestrictToNumbersAndDots(string str) => new string(str.Where(c => char.IsDigit(c) || c == '.').ToArray());
    }
}