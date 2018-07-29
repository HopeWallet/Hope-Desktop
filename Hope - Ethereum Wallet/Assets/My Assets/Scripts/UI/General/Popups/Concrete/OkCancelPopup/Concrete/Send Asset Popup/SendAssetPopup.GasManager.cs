using Hope.Utils.EthereumUtils;
using Nethereum.Util;
using System;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class which displays the popup for sending a TradableAsset.
/// </summary>
public sealed partial class SendAssetPopup : OkCancelPopupComponent<SendAssetPopup>
{
	/// <summary>
	/// Class used for managing the gas of the <see cref="SendAssetPopup"/>.
	/// </summary>
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

		//private const int GAS_FIELD_MAX_LENGTH = 8;
		private const string RAND_ADDRESS = "0x0278018340138741034781903741800348314013";

		/// <summary>
		/// The interval in which the gas limit should be retrieved again.
		/// </summary>
		public float UpdateInterval => 10f;

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
		public GasPrice TransactionGasPrice => advancedModeToggle.IsToggledOn ? enteredGasPrice : estimatedGasPrice;

		/// <summary>
		/// The gas limit to use for the transaction.
		/// </summary>
		public BigInteger TransactionGasLimit => advancedModeToggle.IsToggledOn ? enteredGasLimit : estimatedGasLimit;

		/// <summary>
		/// The current estimated standard gas price.
		/// </summary>
		public GasPrice StandardGasPrice { get; set; }

		/// <summary>
		/// Initializes the <see cref="GasManager"/> by assigning all required references.
		/// </summary>
		/// <param name="tradableAssetManager"> The active <see cref="TradableAssetManager"/>. </param>
		/// <param name="gasPriceObserver"> The active <see cref="GasPriceObserver"/>. </param>
		/// <param name="periodicUpdateManager"> The active <see cref="PeriodicUpdateManager"/>. </param>
		/// <param name="advancedModeToggle"> The toggle for switching between advanced and simple mode. </param>
		/// <param name="transactionSpeedSlider"> The slider for the transaction speed. </param>
		/// <param name="gasLimitField"> The input field for the gas limit when in advanced mode. </param>
		/// <param name="gasPriceField"> The input field for the gas price when in advanced mode. </param>
		/// <param name="transactionFeeText"> The text component to use to set the transaction fee. </param>
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

			AddGasListener(() => this.transactionFeeText.text = "~ " + TransactionFee + " ETH");

			AddListenersAndObservables();
			EstimateGasLimit();
			UpdateGasPriceEstimate(0.5f);
		}

		/// <summary>
		/// Estimates the gas limit every PeriodicUpdate.
		/// </summary>
		public void PeriodicUpdate()
		{
			EstimateGasLimit();
		}

		/// <summary>
		/// Unsubscribes the gas price observable and removes the periodic updater.
		/// </summary>
		public void Destroy()
		{
			gasPriceObserver.UnsubscribeObservable(this);
			periodicUpdateManager.RemovePeriodicUpdater(this);
		}

		/// <summary>
		/// Called once the gas prices were updated again from the <see cref="GasPriceObserver"/>/
		/// </summary>
		public void OnGasPricesUpdated()
		{
			if (!advancedModeToggle.IsToggledOn)
				gasPriceField.text = estimatedGasPrice.ReadableGasPrice.ToString();

			onGasChanged?.Invoke();
		}

		/// <summary>
		/// Adds a listener which is called once the gas price or gas limit is changed.
		/// </summary>
		/// <param name="gasChanged"> Action to call once the gas price or gas limit is changed. </param>
		public void AddGasListener(Action gasChanged)
		{
			if (onGasChanged == null)
				onGasChanged = gasChanged;
			else
				onGasChanged += gasChanged;
		}

		/// <summary>
		/// Adds all observables/listeners/updaters.
		/// </summary>
		private void AddListenersAndObservables()
		{
			gasPriceObserver.SubscribeObservable(this);
			periodicUpdateManager.AddPeriodicUpdater(this);

			transactionSpeedSlider.onValueChanged.AddListener(UpdateGasPriceEstimate);
			gasLimitField.onValueChanged.AddListener(CheckGasLimitField);
			gasPriceField.onValueChanged.AddListener(CheckGasPriceField);
		}

		/// <summary>
		/// Checks the gas limit entered in the gas limit field.
		/// </summary>
		/// <param name="gasLimit"> The entered gas limit. </param>
		private void CheckGasLimitField(string gasLimit)
		{
			BigInteger.TryParse(gasLimit, out enteredGasLimit);
            gasLimitField.text = gasLimit;

			onGasChanged?.Invoke();
		}

		/// <summary>
		/// Checks the gas price entered in the gas price field.
		/// </summary>
		/// <param name="gasPrice"> The entered gas price. </param>
		private void CheckGasPriceField(string gasPrice)
		{
            gasPriceField.text = gasPrice;

			decimal price;
			decimal.TryParse(gasPriceField.text, out price);

			enteredGasPrice = new GasPrice(GasUtils.GetFunctionalGasPrice(price));

			onGasChanged?.Invoke();
		}

		/// <summary>
		/// Updates the estimated gas price based on the current progress of the transaction speed slider.
		/// </summary>
		/// <param name="value"> The current value of the transaction speed slider. </param>
		private void UpdateGasPriceEstimate(float value)
		{
			decimal multiplier = decimal.Round((decimal)Mathf.Lerp(0.6f, 1.4f, value) * (decimal)Mathf.Lerp(1f, 4f, value - 0.45f), 2, MidpointRounding.AwayFromZero);
			estimatedGasPrice = new GasPrice(new BigInteger(multiplier * (decimal)StandardGasPrice.FunctionalGasPrice.Value));

			OnGasPricesUpdated();
		}

		/// <summary>
		/// Estimates the gas limit for the current <see cref="TradableAsset"/>.
		/// </summary>
		private void EstimateGasLimit()
		{
			tradableAssetManager.ActiveTradableAsset.GetTransferGasLimit(RAND_ADDRESS, (decimal)tradableAssetManager.ActiveTradableAsset.AssetBalance, OnGasLimitReceived);
		}

		/// <summary>
		/// Called once the gas limit has been estimated.
		/// </summary>
		/// <param name="limit"> The newly estimated gas limit for the active asset. </param>
		private void OnGasLimitReceived(BigInteger limit)
		{
			estimatedGasLimit = limit;

			if (string.IsNullOrEmpty(gasLimitField.text) || !advancedModeToggle.IsToggledOn || limit > enteredGasLimit)
				CheckGasLimitField(limit.ToString());
		}

		/// <summary>
		/// Restricts a string value to numbers only.
		/// </summary>
		/// <param name="str"> The string to restrict the values for. </param>
		/// <returns> The entered string restricted to only numbers. </returns>
		private string RestrictToNumbers(string str) => new string(str.Where(c => char.IsDigit(c)).ToArray());

		/// <summary>
		/// Restricts a string value to numbers and dots only.
		/// </summary>
		/// <param name="str"> The string to restrict the values for. </param>
		/// <returns> The entered string restricted to numbers and dots. </returns>
		private string RestrictToNumbersAndDots(string str) => new string(str.Where(c => char.IsDigit(c) || c == '.').ToArray());
	}
}