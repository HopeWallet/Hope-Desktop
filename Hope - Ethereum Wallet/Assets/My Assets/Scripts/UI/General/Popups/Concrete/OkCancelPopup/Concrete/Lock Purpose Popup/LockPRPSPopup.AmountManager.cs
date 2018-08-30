using System;
using TMPro;

/// <summary>
/// Class used for locking purpose.
/// </summary>
public sealed partial class LockPRPSPopup
{
	/// <summary>
	/// Class used for managing the amount to lock.
	/// </summary>
	public sealed class AmountManager
	{
		public event Action OnLockAmountChanged;

		private readonly LockPRPSManager lockPRPSManager;

		private readonly Toggle maxToggle;

		private readonly HopeInputField amountInputField;

		private readonly TMP_Text prpsBalanceText,
								  dubiBalanceText,
								  dubiRewardText;

		/// <summary>
		/// Whether the amount input field is empty or not.
		/// </summary>
		public bool IsEmpty { get { return string.IsNullOrEmpty(amountInputField.Text); } }

		/// <summary>
		/// The amount that will be sent.
		/// </summary>
		public decimal AmountToLock { get; private set; }

		/// <summary>
		/// The max sendable amount given the current asset balance.
		/// </summary>
		private decimal MaxSendableAmount => lockPRPSManager.PRPSBalance;

		/// <summary>
		/// Initializes the <see cref="AmountManager"/> by assigning the references to the popup, max toggle, and amount input field.
		/// </summary>
		/// <param name="lockPRPSManager"> The active LockPRPSManager. </param>
		/// <param name="maxToggle"> The toggle for switching between maximum sendable amount and the entered amount. </param>
		/// <param name="amountInputField"> The input field used for entering the sendable amount. </param>
		/// <param name="prpsBalanceText"> Text component used for displaying the current purpose balance. </param>
		/// <param name="dubiBalanceText"> Text component used for displaying the current dubi balance. </param>
		/// <param name="dubiRewardText"> Text component used for displaying the dubi reward. </param>
		public AmountManager(
			LockPRPSManager lockPRPSManager,
			Toggle maxToggle,
			HopeInputField amountInputField,
			TMP_Text prpsBalanceText,
			TMP_Text dubiBalanceText,
			TMP_Text dubiRewardText)
		{
			this.lockPRPSManager = lockPRPSManager;
			this.maxToggle = maxToggle;
			this.amountInputField = amountInputField;
			this.prpsBalanceText = prpsBalanceText;
			this.dubiBalanceText = dubiBalanceText;
			this.dubiRewardText = dubiRewardText;

			lockPRPSManager.OnAmountsUpdated += BalancesUpdated;

			maxToggle.AddToggleListener(MaxChanged);
			amountInputField.OnInputUpdated += _ => AmountFieldChanged();

			BalancesUpdated();
		}

		/// <summary>
		/// Stops the AmountManager.
		/// </summary>
		public void Stop() => lockPRPSManager.OnAmountsUpdated -= BalancesUpdated;

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

			AmountToLock = newLockableAmount;

			if (maxToggle.IsToggledOn != (AmountToLock == MaxSendableAmount))
			{
				maxToggle.IsToggledOn = AmountToLock == MaxSendableAmount;
				maxToggle.AnimateImages();
			}

			CheckIfValidAmount();
        }

        /// <summary>
        /// Checks if the amount entered in the input field is valid and updates the amount changed listeners.
        /// </summary>
        private void CheckIfValidAmount()
        {
			amountInputField.Error = string.IsNullOrEmpty(amountInputField.Text) || AmountToLock > MaxSendableAmount || AmountToLock < 0.0000000000000001m;
			OnLockAmountChanged?.Invoke();
        }

        /// <summary>
        /// Updates the purpose and dubi balance text.
        /// </summary>
        private void BalancesUpdated()
        {
			prpsBalanceText.text = StringUtils.LimitEnd(StringUtils.ConvertDecimalToString(lockPRPSManager.PRPSBalance), 10, "...");
			dubiBalanceText.text = StringUtils.LimitEnd(StringUtils.ConvertDecimalToString(lockPRPSManager.DUBIBalance), 10, "...");
        }
    }
}