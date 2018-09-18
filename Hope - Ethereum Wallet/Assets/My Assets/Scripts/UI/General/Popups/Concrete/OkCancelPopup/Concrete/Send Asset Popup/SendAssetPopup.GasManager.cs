using Hope.Utils.Ethereum;
using Nethereum.Util;
using System;
using System.Numerics;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Class which displays the popup for sending a TradableAsset.
/// </summary>
public sealed partial class SendAssetPopup : OkCancelPopupComponent<SendAssetPopup>
{
	/// <summary>
	/// Class used for managing the gas of the <see cref="SendAssetPopup"/>.
	/// </summary>
	public sealed class GasManager : IPeriodicUpdater
	{
		public event Action OnGasChanged;

		private readonly TradableAssetManager tradableAssetManager;
		private readonly PeriodicUpdateManager periodicUpdateManager;

		private readonly Toggle advancedModeToggle;

		private readonly TransactionSpeedSlider transactionSpeedSlider;

        private readonly HopeInputField gasLimitField,
                                        gasPriceField;

        private AmountManager amountManager;

        private TMP_Text transactionFeeText;

        private GasPrice estimatedGasPrice,
						 enteredGasPrice;

		private BigInteger estimatedGasLimit,
						   enteredGasLimit;

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
		public bool Error => TransactionFee == 0;

		/// <summary>
		/// The gas price to use for the transaction.
		/// </summary>
		public GasPrice TransactionGasPrice => advancedModeToggle.IsToggledOn ? enteredGasPrice : estimatedGasPrice;

		/// <summary>
		/// The gas limit to use for the transaction.
		/// </summary>
		public BigInteger TransactionGasLimit => advancedModeToggle.IsToggledOn ? enteredGasLimit : estimatedGasLimit;

        /// <summary>
        /// Initializes the <see cref="GasManager"/> by assigning all required references.
        /// </summary>
        /// <param name="tradableAssetManager"> The active <see cref="TradableAssetManager"/>. </param>
        /// <param name="gasPriceObserver"> The active <see cref="GasPriceObserver"/>. </param>
        /// <param name="periodicUpdateManager"> The active <see cref="PeriodicUpdateManager"/>. </param>
        /// <param name="advancedModeToggle"> The toggle for switching between advanced and simple mode. </param>
        /// <param name="slider"> The slider used to control transaction speed. </param>
        /// <param name="gasLimitField"> The input field for the gas limit when in advanced mode. </param>
        /// <param name="gasPriceField"> The input field for the gas price when in advanced mode. </param>
        /// <param name="transactionFeeText"> The text component to use to set the transaction fee. </param>
        public GasManager(
			TradableAssetManager tradableAssetManager,
			GasPriceObserver gasPriceObserver,
			PeriodicUpdateManager periodicUpdateManager,
			Toggle advancedModeToggle,
			Slider slider,
			HopeInputField gasLimitField,
			HopeInputField gasPriceField,
			TMP_Text transactionFeeText)
		{
			this.tradableAssetManager = tradableAssetManager;
			this.periodicUpdateManager = periodicUpdateManager;
			this.advancedModeToggle = advancedModeToggle;
			this.gasLimitField = gasLimitField;
			this.gasPriceField = gasPriceField;
			this.transactionFeeText = transactionFeeText;

			transactionSpeedSlider = new TransactionSpeedSlider(gasPriceObserver, slider, UpdateGasPriceEstimate);

			OnGasChanged += UpdateTransactionFeeVisuals;

			AddListenersAndObservables();
			EstimateGasLimit();
			transactionSpeedSlider.Start();
		}

		/// <summary>
		/// Estimates the gas limit every PeriodicUpdate.
		/// </summary>
		public void PeriodicUpdate()
		{
			EstimateGasLimit();
		}

        /// <summary>
        /// Updates the transaction fee visuals based on the current eth amount and gas price.
        /// </summary>
		private void UpdateTransactionFeeVisuals()
		{
            if (amountManager == null)
                return;

			bool hasEnoughETH = tradableAssetManager.ActiveTradableAsset is EtherAsset
				? TransactionFee <= (tradableAssetManager.EtherAsset.AssetBalance - amountManager.SendableAmount)
				: TransactionFee < tradableAssetManager.EtherAsset.AssetBalance;

			transactionFeeText.text = hasEnoughETH ? "~ " + TransactionFee.ToString().LimitEnd(14).TrimEnd('0') + "<style=Symbol> ETH</style>" : "Not enough ETH";
			transactionFeeText.color = !hasEnoughETH ? UIColors.Red : UIColors.White;
		}

		/// <summary>
		/// Unsubscribes the gas price observable and removes the periodic updater.
		/// </summary>
		public void Destroy()
		{
			periodicUpdateManager.RemovePeriodicUpdater(this);
			transactionSpeedSlider.Stop();
		}

        public void SetupDependencies(AmountManager amountManager)
        {
            this.amountManager = amountManager;
            amountManager.OnAmountChanged += UpdateTransactionFeeVisuals;

            UpdateTransactionFeeVisuals();
        }

		/// <summary>
		/// Adds all observables/listeners/updaters.
		/// </summary>
		private void AddListenersAndObservables()
		{
			periodicUpdateManager.AddPeriodicUpdater(this);

            gasLimitField.OnInputUpdated += CheckGasLimitField;
			gasPriceField.OnInputUpdated += CheckGasPriceField;
            advancedModeToggle.AddToggleListener(UpdateTransactionFeeVisuals);
		}

		/// <summary>
		/// Checks the gas limit entered in the gas limit field.
		/// </summary>
		/// <param name="gasLimit"> The entered gas limit. </param>
		private void CheckGasLimitField(string gasLimit)
		{
			BigInteger.TryParse(gasLimit, out enteredGasLimit);
			gasLimitField.Error = string.IsNullOrEmpty(gasLimit) || enteredGasLimit.IsZero;

			OnGasChanged?.Invoke();
		}

		/// <summary>
		/// Checks the gas price entered in the gas price field.
		/// </summary>
		/// <param name="gasPrice"> The entered gas price. </param>
		private void CheckGasPriceField(string gasPrice)
		{
			decimal price;
			decimal.TryParse(gasPriceField.Text, out price);

			enteredGasPrice = new GasPrice(GasUtils.GetFunctionalGasPrice(price));
			gasPriceField.Error = string.IsNullOrEmpty(gasPriceField.Text) || price == 0;

			OnGasChanged?.Invoke();
		}

        /// <summary>
        /// Updates the estimated gas price based on the current progress of the transaction speed slider.
        /// </summary>
        /// <param name="newEstimate"> The new estimated gas price. </param>
        private void UpdateGasPriceEstimate(GasPrice newEstimate)
		{
            estimatedGasPrice = newEstimate;

            OnGasChanged?.Invoke();
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

			if (string.IsNullOrEmpty(gasLimitField.Text) || !advancedModeToggle.IsToggledOn || limit > enteredGasLimit)
				CheckGasLimitField(limit.ToString());
		}
	}
}