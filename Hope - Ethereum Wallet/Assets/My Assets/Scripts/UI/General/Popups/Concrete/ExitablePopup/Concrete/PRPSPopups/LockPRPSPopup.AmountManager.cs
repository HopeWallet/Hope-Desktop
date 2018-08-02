using System;
using TMPro;

public sealed partial class LockPRPSPopup
{
    public sealed class AmountManager
    {
        public event Action OnLockAmountChanged;

        private readonly LockPRPSManager lockPRPSManager;

        private readonly Toggle maxToggle;

        private readonly TMP_InputField amountInputField;

        private readonly TMP_Text amountPlaceholderText,
                                  prpsBalanceText,
                                  dubiBalanceText,
                                  dubiRewardText;

        /// <summary>
        /// Whether the amount entered in the input field is a valid sendable amount.
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// Whether the amount input field is empty or not.
        /// </summary>
        public bool IsEmpty { get { return string.IsNullOrEmpty(amountInputField.text); } }

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
        /// <param name="lockPRPSManager"></param>
        /// <param name="maxToggle"> The toggle for switching between maximum sendable amount and the entered amount. </param>
        /// <param name="amountInputField"> The input field used for entering the sendable amount. </param>
        public AmountManager(
            LockPRPSManager lockPRPSManager,
            Toggle maxToggle,
            TMP_InputField amountInputField,
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

            amountPlaceholderText = amountInputField.placeholder.GetComponent<TMP_Text>();

            lockPRPSManager.OnAmountsUpdated += BalancesUpdated;

            maxToggle.AddToggleListener(MaxChanged);
            amountInputField.onValueChanged.AddListener(AmountFieldChanged);

            BalancesUpdated();
        }

        public void Stop()
        {
            lockPRPSManager.OnAmountsUpdated -= BalancesUpdated;
        }

        /// <summary>
        /// Called when the maximum sendable amount is changed or activated through the toggle.
        /// </summary>
        private void MaxChanged()
        {
            AmountToLock = maxToggle.IsToggledOn ? MaxSendableAmount : AmountToLock;

            amountPlaceholderText.text = maxToggle.IsToggledOn ? AmountToLock.ToString() + " (Max)" : "Enter amount...";
            amountInputField.text = string.IsNullOrEmpty(amountInputField.text) && !maxToggle.IsToggledOn ? "" : AmountToLock.ToString();

            amountInputField.textComponent.enabled = !maxToggle.IsToggledOn;
            amountPlaceholderText.enabled = string.IsNullOrEmpty(amountInputField.text) || maxToggle.IsToggledOn;

            amountInputField.interactable = !maxToggle.IsToggledOn;

            CheckIfValidAmount();
        }

        /// <summary>
        /// Called once the amount input field is edited and changed.
        /// </summary>
        /// <param name="amountText"> The text entered in the amount input field. </param>
        private void AmountFieldChanged(string amountText)
        {
            amountInputField.RestrictDecimalValue(18);

            decimal newLockableAmount;
            decimal.TryParse(amountInputField.text, out newLockableAmount);

            AmountToLock = newLockableAmount;

            CheckIfValidAmount();
        }

        /// <summary>
        /// Checks if the amount entered in the input field is valid and updates the amount changed listeners.
        /// </summary>
        private void CheckIfValidAmount()
        {
            IsValid = AmountToLock <= MaxSendableAmount && AmountToLock >= 0.0000000000000001m;
        }

        private void BalancesUpdated()
        {
            prpsBalanceText.text = StringUtils.LimitEnd(lockPRPSManager.PRPSBalance.ToString(), 10, "...");
            dubiBalanceText.text = StringUtils.LimitEnd(lockPRPSManager.DUBIBalance.ToString(), 10, "...");
        }
    }
}