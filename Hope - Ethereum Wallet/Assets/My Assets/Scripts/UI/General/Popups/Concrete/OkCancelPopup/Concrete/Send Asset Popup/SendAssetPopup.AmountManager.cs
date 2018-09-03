using System;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Class which displays the popup for sending a TradableAsset.
/// </summary>
public sealed partial class SendAssetPopup : OkCancelPopupComponent<SendAssetPopup>
{
	/// <summary>
	/// Class which manages the sendable amount entered in the input field.
	/// </summary>
	public sealed class AmountManager
	{
		public event Action OnAmountChanged;

		private TMP_Text currencyText, oppositeCurrencyAmountText;

		private Button currencyButton;

        private GasManager gasManager;
        private AssetManager assetManager;
        private CurrencyManager currencyManager;
        private TradableAssetPriceManager tradableAssetPriceManager;

		private bool usingTokenCurrency = true;
		private decimal oppositeCurrencyValue;

        private readonly string tradableTokenSymbol;

		private readonly Toggle maxToggle;

		private readonly HopeInputField amountInputField;

		/// <summary>
		/// Whether the amount input field is empty or not.
		/// </summary>
		public bool IsEmpty { get { return string.IsNullOrEmpty(amountInputField.Text); } }

		/// <summary>
		/// The amount that will be sent.
		/// </summary>
		public decimal SendableAmount { get; private set; }

		/// <summary>
		/// The max sendable amount given the current asset balance.
		/// </summary>
		private decimal MaxSendableAmount
		{
			get
			{
				dynamic activeAssetBalance = assetManager.ActiveAssetBalance;
				dynamic max = assetManager.ActiveAsset is EtherAsset ? activeAssetBalance - gasManager.TransactionFee : activeAssetBalance;

				return max < 0 ? 0 : max;
			}
		}

        /// <summary>
        /// Initializes the <see cref="AmountManager"/> by assigning the references to the popup, max toggle, and amount input field.
        /// </summary>
        /// <param name="currencyManager"> The active <see cref="CurrencyManager"/>. </param>
        /// <param name="tradableAssetPriceManager"> The active <see cref="TradableAssetPriceManager"/>. </param>
        /// <param name="maxToggle"> The toggle for switching between maximum sendable amount and the entered amount. </param>
        /// <param name="amountInputField"> The input field used for entering the sendable amount. </param>
        /// <param name="currencyText"> The currency text object. </param>
        /// <param name="oppositeCurrencyAmountText"> The opposite currency amount text object. </param>
        /// <param name="currencyButton"> The currency button. </param>
        /// <param name="tokenSymbol"> The token symbol. </param>
        public AmountManager(
            CurrencyManager currencyManager,
            TradableAssetPriceManager tradableAssetPriceManager,
			Toggle maxToggle,
			HopeInputField amountInputField,
			TMP_Text currencyText,
			TMP_Text oppositeCurrencyAmountText,
			Button currencyButton,
			string tokenSymbol)
		{
			this.maxToggle = maxToggle;
			this.amountInputField = amountInputField;
			this.currencyText = currencyText;
			this.oppositeCurrencyAmountText = oppositeCurrencyAmountText;
			this.currencyButton = currencyButton;
            this.currencyManager = currencyManager;
            this.tradableAssetPriceManager = tradableAssetPriceManager;
			tradableTokenSymbol = tokenSymbol;

            currencyText.text = currencyManager.ActiveCurrency.ToString();

            amountInputField.SetPlaceholderText("Amount (" + tokenSymbol + ")");
			SetupListeners();
		}

		/// <summary>
		/// Sets up the dependencies for the this instance of the AmountManager.
		/// </summary>
		/// <param name="gasManager"> The GasManager dependency. </param>
		/// <param name="assetManager"> The AssetManager dependency. </param>
		public void SetupDependencies(GasManager gasManager, AssetManager assetManager)
		{
			this.gasManager = gasManager;
			this.assetManager = assetManager;

            currencyButton.interactable = tradableAssetPriceManager.GetPrice(assetManager.ActiveAsset.AssetSymbol) > 0;

            gasManager.OnGasChanged += MaxChanged;
            assetManager.OnAssetBalanceChanged += MaxChanged;
        }

		/// <summary>
		/// Sets up all listeners.
		/// </summary>
		private void SetupListeners()
		{
			maxToggle.AddToggleListener(MaxChanged);
			amountInputField.OnInputUpdated += _ => AmountFieldChanged();
			currencyButton.onClick.AddListener(CurrencyChanged);
		}

		/// <summary>
		/// Changes the currency and the inputted text according to the currency
		/// </summary>
		private void CurrencyChanged()
		{
			usingTokenCurrency = !usingTokenCurrency;

			amountInputField.SetPlaceholderText("Amount (" + currencyText.text + ")");
			currencyText.text = usingTokenCurrency ? currencyManager.ActiveCurrency.ToString() : tradableTokenSymbol;

			if (!string.IsNullOrEmpty(amountInputField.Text))
				amountInputField.Text = oppositeCurrencyValue.ToString();

			maxToggle.SetInteractable(usingTokenCurrency);
		}

		/// <summary>
		/// Called when the maximum sendable amount is changed or activated through the toggle.
		/// </summary>
		private void MaxChanged()
		{
			if (maxToggle.IsToggledOn)
				amountInputField.Text = usingTokenCurrency ? MaxSendableAmount.ToString() : (MaxSendableAmount * tradableAssetPriceManager.GetPrice(assetManager.ActiveAsset.AssetSymbol)).ToString();

			CheckIfValidAmount();
		}

		/// <summary>
		/// Called once the amount input field is edited and changed.
		/// </summary>
		private void AmountFieldChanged()
		{
			amountInputField.InputFieldBase.RestrictDecimalValue(usingTokenCurrency ? assetManager.ActiveAsset.AssetDecimals : 2);

			decimal newSendableAmount;
			decimal.TryParse(amountInputField.Text, out newSendableAmount);

			oppositeCurrencyAmountText.gameObject.AnimateGraphicAndScale(string.IsNullOrEmpty(amountInputField.Text) ? 0f : 1f, string.IsNullOrEmpty(amountInputField.Text) ? 0f : 1f, 0.15f);
			ChangeOppositeCurrencyValue(newSendableAmount);

			SendableAmount = usingTokenCurrency ? newSendableAmount : oppositeCurrencyValue;

			if (maxToggle.IsToggledOn != (SendableAmount == MaxSendableAmount))
			{
				maxToggle.IsToggledOn = SendableAmount == MaxSendableAmount;
				maxToggle.AnimateImages();
			}

			CheckIfValidAmount();
		}

		/// <summary>
		/// Changes the opposite currency value text
		/// </summary>
		/// <param name="newSendableAmount"> The new sendable amount entered in the input field. </param>
		private void ChangeOppositeCurrencyValue(decimal newSendableAmount)
		{
            decimal currentTokenPrice = tradableAssetPriceManager.GetPrice(assetManager.ActiveAsset.AssetSymbol);
            oppositeCurrencyValue = usingTokenCurrency ? newSendableAmount * currentTokenPrice : newSendableAmount / currentTokenPrice;

			string oppositeCurrencyValueText = usingTokenCurrency ? oppositeCurrencyValue.ToString("0.00").LimitEnd(8, "...") : oppositeCurrencyValue.ToString().LimitEnd(8, "...");

			oppositeCurrencyAmountText.text = usingTokenCurrency ? "= $" + oppositeCurrencyValueText + " " + currencyManager.ActiveCurrency.ToString() : "= " + oppositeCurrencyValueText + " " + tradableTokenSymbol;
		}

		/// <summary>
		/// Checks if the amount entered in the input field is valid and updates the amount changed listeners.
		/// </summary>
		private void CheckIfValidAmount()
		{
			bool emptyField = string.IsNullOrEmpty(amountInputField.Text);

			amountInputField.Error = emptyField || SendableAmount == 0 || (usingTokenCurrency ? SendableAmount > MaxSendableAmount : (SendableAmount / tradableAssetPriceManager.GetPrice(assetManager.ActiveAsset.AssetSymbol)) > MaxSendableAmount);

			if (!emptyField)
				amountInputField.errorMessage.text = SendableAmount == 0 ? "Invalid amount" : "Exceeds " + tradableTokenSymbol + " balance";

            OnAmountChanged?.Invoke();
		}
	}
}