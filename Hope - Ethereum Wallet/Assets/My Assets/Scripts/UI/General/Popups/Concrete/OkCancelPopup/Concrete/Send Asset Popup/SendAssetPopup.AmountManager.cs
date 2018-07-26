using System;
using TMPro;

public sealed partial class SendAssetPopup : OkCancelPopupComponent<SendAssetPopup>
{
    public sealed class AmountManager
    {
        private readonly SendAssetPopup sendAssetPopup;

        private readonly Toggle maxToggle;

        private readonly TMP_InputField amountInputField;
        private readonly TextMeshProUGUI amountPlaceholderText;

        private Action onAmountChanged;

        public bool IsValid { get; private set; }

        public bool AmountChanged { get; private set; }

        public decimal SendableAmount { get; private set; }

        public decimal MaxSendableAmount
        {
            get
            {
                AssetManager assetManager = sendAssetPopup.Asset;
                dynamic activeAssetBalance = assetManager.ActiveAssetBalance;
                dynamic max = assetManager.ActiveAsset is EtherAsset ? activeAssetBalance - sendAssetPopup.Gas.TransactionCost : activeAssetBalance;

                return max < 0 ? 0 : max;
            }
        }

        public AmountManager(
            SendAssetPopup sendAssetPopup,
            Toggle maxToggle,
            TMP_InputField amountInputField)
        {
            this.sendAssetPopup = sendAssetPopup;
            this.maxToggle = maxToggle;
            this.amountInputField = amountInputField;
            amountPlaceholderText = amountInputField.placeholder.GetComponent<TextMeshProUGUI>();

            SetupListeners();
        }

        public void AddSendAmountListener(Action amountChanged)
        {
            if (onAmountChanged == null)
                onAmountChanged = amountChanged;
            else
                onAmountChanged += amountChanged;
        }

        private void SetupListeners()
        {
            sendAssetPopup.Asset.AddAssetBalanceListener(MaxChanged);
            sendAssetPopup.Gas.AddGasListener(MaxChanged);
            maxToggle.AddToggleListener(MaxChanged);
            amountInputField.onValueChanged.AddListener(OnAmountChanged);
        }

        private void MaxChanged()
        {
            SendableAmount = maxToggle.IsToggledOn ? MaxSendableAmount : SendableAmount;

            amountPlaceholderText.text = maxToggle.IsToggledOn ? SendableAmount.ToString() + " (Max)" : "Enter amount...";
            amountInputField.text = string.IsNullOrEmpty(amountInputField.text) && !maxToggle.IsToggledOn ? "" : SendableAmount.ToString();

            amountInputField.textComponent.enabled = !maxToggle.IsToggledOn;
            amountPlaceholderText.enabled = string.IsNullOrEmpty(amountInputField.text) || maxToggle.IsToggledOn;

            amountInputField.interactable = !maxToggle.IsToggledOn;

            CheckIfValidAmount();
        }

        private void OnAmountChanged(string amountText)
        {
            AmountChanged = true;

            amountInputField.RestrictToBalance(sendAssetPopup.Asset.ActiveAsset);

            decimal newSendAmount;
            decimal.TryParse(amountInputField.text, out newSendAmount);

            SendableAmount = newSendAmount;

            CheckIfValidAmount();
        }

        private void CheckIfValidAmount()
        {
            IsValid = SendableAmount <= MaxSendableAmount && SendableAmount > 0;
            onAmountChanged?.Invoke();
        }
    }
}