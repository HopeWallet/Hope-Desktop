using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

public sealed partial class SendAssetPopup : OkCancelPopupComponent<SendAssetPopup>
{
    public sealed class AmountManager
    {

        private readonly SendAssetPopup sendAssetPopup;
        private readonly TradableAssetManager tradableAssetManager;

        private readonly Toggle maxToggle;

        private readonly Slider transactionSpeedSlider;

        private readonly TMP_InputField amountInputField;

        public bool IsValid { get; private set; }

        public decimal SendableAmount { get; private set; }

        public AmountManager(
            SendAssetPopup sendAssetPopup,
            TradableAssetManager tradableAssetManager,
            Toggle maxToggle,
            Slider transactionSpeedSlider,
            TMP_InputField amountInputField)
        {
            this.sendAssetPopup = sendAssetPopup;
            this.tradableAssetManager = tradableAssetManager;
            this.maxToggle = maxToggle;
            this.transactionSpeedSlider = transactionSpeedSlider;
            this.amountInputField = amountInputField;

            SetupListeners();
        }

        private void SetupListeners()
        {
            amountInputField.onValueChanged.AddListener(OnAmountChanged);
            maxToggle.AddToggleListener(ToggleChanged);
        }

        private void ToggleChanged()
        {
            amountInputField.interactable = maxToggle.IsToggledOn;
        }

        private void OnAmountChanged(string amountText)
        {
            var activeAsset = tradableAssetManager.ActiveTradableAsset;

            amountInputField.RestrictToBalance(activeAsset);
        }
    }
}