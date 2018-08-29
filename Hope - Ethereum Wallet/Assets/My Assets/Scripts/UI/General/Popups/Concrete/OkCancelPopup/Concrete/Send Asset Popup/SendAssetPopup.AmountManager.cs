using System;

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
		public AmountManager(
			SendAssetPopup sendAssetPopup,
			Toggle maxToggle,
			HopeInputField amountInputField)
		{
			this.sendAssetPopup = sendAssetPopup;
			this.maxToggle = maxToggle;
			this.amountInputField = amountInputField;

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
			amountInputField.OnInputUpdated += AmountFieldChanged;
		}

		/// <summary>
		/// Called when the maximum sendable amount is changed or activated through the toggle.
		/// </summary>
		private void MaxChanged()
		{
			if (maxToggle.IsToggledOn)
				amountInputField.Text = MaxSendableAmount.ToString();

			CheckIfValidAmount();
		}

		/// <summary>
		/// Called once the amount input field is edited and changed.
		/// </summary>
		private void AmountFieldChanged()
		{
			amountInputField.inputFieldBase.RestrictDecimalValue(18);

			decimal newLockableAmount;
			decimal.TryParse(amountInputField.Text, out newLockableAmount);

			SendableAmount = newLockableAmount;

			if (maxToggle.IsToggledOn != (SendableAmount == MaxSendableAmount))
			{
				maxToggle.IsToggledOn = SendableAmount == MaxSendableAmount;
				maxToggle.AnimateImages();
			}

			CheckIfValidAmount();
		}

		/// <summary>
		/// Checks if the amount entered in the input field is valid and updates the amount changed listeners.
		/// </summary>
		private void CheckIfValidAmount()
		{
			amountInputField.Error = SendableAmount == 0 || SendableAmount > MaxSendableAmount;
            OnAmountChanged?.Invoke();
        }
    }
}