using System;
using TMPro;
using UnityEngine;
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

		private TMP_Text currencyButtonText, oppositeCurrencyAmountText;

		private Button currencyButton;

		private bool usingTokenCurrency;
		private string tradableTokenSymbol, defaultCurrency = "USD";
		private decimal oppositeCurrencyValue, currentTokenPrice = 281.81m;

		private readonly SendAssetPopup sendAssetPopup;

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
				AssetManager assetManager = sendAssetPopup.Asset;
				dynamic activeAssetBalance = assetManager.ActiveAssetBalance;
				dynamic max = assetManager.ActiveAsset is EtherAsset ? activeAssetBalance - sendAssetPopup.Gas.TransactionFee : activeAssetBalance;

				return max < 0 ? 0 : max;
			}
		}

		/// <summary>
		/// Initializes the <see cref="AmountManager"/> by assigning the references to the popup, max toggle, and amount input field.
		/// </summary>
		/// <param name="sendAssetPopup"> The active <see cref="SendAssetPopup"/>. </param>
		/// <param name="maxToggle"> The toggle for switching between maximum sendable amount and the entered amount. </param>
		/// <param name="amountInputField"> The input field used for entering the sendable amount. </param>
		/// <param name="currencyText"> The currency text object. </param>
		/// <param name="oppositeCurrencyAmountText"> The opposite currency amount text object. </param>
		/// <param name="currencyButton"> The currency button. </param>
		/// <param name="tokenSymbol"> The token symbol. </param>
		public AmountManager(
			SendAssetPopup sendAssetPopup,
			Toggle maxToggle,
			HopeInputField amountInputField,
			TMP_Text currencyText,
			TMP_Text oppositeCurrencyAmountText,
			Button currencyButton,
			string tokenSymbol)
		{
			this.sendAssetPopup = sendAssetPopup;
			this.maxToggle = maxToggle;
			this.amountInputField = amountInputField;
			this.currencyButtonText = currencyText;
			this.oppositeCurrencyAmountText = oppositeCurrencyAmountText;
			this.currencyButton = currencyButton;
			tradableTokenSymbol = tokenSymbol;

			amountInputField.SetPlaceholderText("Amount (" + tokenSymbol + ")");
			usingTokenCurrency = true;
			currencyText.text = defaultCurrency;
			SetupListeners();
		}

		/// <summary>
		/// Sets up all listeners.
		/// </summary>
		private void SetupListeners()
		{
			sendAssetPopup.Asset.OnAssetBalanceChanged += MaxChanged;
			sendAssetPopup.Gas.OnGasChanged += MaxChanged;
			maxToggle.AddToggleListener(MaxChanged);
			amountInputField.OnInputUpdated += _ => AmountFieldChanged();
			currencyButton.onClick.AddListener(CurrencyChanged);
		}

		private void CurrencyChanged()
		{
			usingTokenCurrency = !usingTokenCurrency;

			amountInputField.SetPlaceholderText("Amount (" + currencyButtonText.text + ")");
			currencyButtonText.text = usingTokenCurrency ? defaultCurrency : tradableTokenSymbol;

			if (!string.IsNullOrEmpty(amountInputField.Text))
				amountInputField.Text = oppositeCurrencyValue.ToString();
		}

		/// <summary>
		/// Called when the maximum sendable amount is changed or activated through the toggle.
		/// </summary>
		private void MaxChanged()
		{
			if (maxToggle.IsToggledOn)
				amountInputField.Text = usingTokenCurrency ? MaxSendableAmount.ToString() : (MaxSendableAmount * currentTokenPrice).ToString();

			CheckIfValidAmount();
		}

		/// <summary>
		/// Called once the amount input field is edited and changed.
		/// </summary>
		private void AmountFieldChanged()
		{
			amountInputField.inputFieldBase.RestrictDecimalValue(usingTokenCurrency ? 18 : 2);

			decimal newSendableAmount;
			decimal.TryParse(amountInputField.Text, out newSendableAmount);

			SendableAmount = newSendableAmount;

			oppositeCurrencyAmountText.gameObject.AnimateGraphicAndScale(string.IsNullOrEmpty(amountInputField.Text) ? 0f : 1f, string.IsNullOrEmpty(amountInputField.Text) ? 0f : 1f, 0.15f);
			ChangeOppositeCurrencyValue();

			if (maxToggle.IsToggledOn != (SendableAmount == MaxSendableAmount))
			{
				maxToggle.IsToggledOn = SendableAmount == MaxSendableAmount;
				maxToggle.AnimateImages();
			}

			CheckIfValidAmount();
		}

		private void ChangeOppositeCurrencyValue()
		{
			oppositeCurrencyValue = usingTokenCurrency ? SendableAmount * currentTokenPrice : SendableAmount / currentTokenPrice;

			string oppositeCurrencyValueText = usingTokenCurrency ? oppositeCurrencyValue.ToString("0.00").LimitEnd(8, "...") : oppositeCurrencyValue.ToString().LimitEnd(8, "...");

			oppositeCurrencyAmountText.text = usingTokenCurrency ? "= $" + oppositeCurrencyValueText + " " + defaultCurrency : "= " + oppositeCurrencyValueText + " " + tradableTokenSymbol;
		}

		/// <summary>
		/// Checks if the amount entered in the input field is valid and updates the amount changed listeners.
		/// </summary>
		private void CheckIfValidAmount()
		{
			amountInputField.Error = SendableAmount == 0 || (usingTokenCurrency ? SendableAmount > MaxSendableAmount : (SendableAmount / currentTokenPrice) > MaxSendableAmount);
			OnAmountChanged?.Invoke();
		}
	}
}