using TMPro;
using UnityEngine.UI;

public sealed partial class LockPRPSPopup
{
    public sealed class TimeManager
    {
        private readonly AmountManager amountManager;

        private readonly Button threeMonthsButton,
                                sixMonthsButton,
                                twelveMonthsButton;

        private readonly TMP_Text dubiRewardText;

        private decimal multiplier;

        public bool IsValid => multiplier >= 0.01m;

        public int MonthsToLock => (int)(multiplier * 300);

        public TimeManager(
            AmountManager amountManager,
            Button threeMonthsButton,
            Button sixMonthsButton,
            Button twelveMonthsButton,
            TMP_Text dubiRewardText)
        {
            this.amountManager = amountManager;
            this.threeMonthsButton = threeMonthsButton;
            this.sixMonthsButton = sixMonthsButton;
            this.twelveMonthsButton = twelveMonthsButton;
            this.dubiRewardText = dubiRewardText;

            amountManager.OnLockAmountChanged += UpdateDUBIReward;
            threeMonthsButton.onClick.AddListener(() => ButtonPressed(threeMonthsButton));
            sixMonthsButton.onClick.AddListener(() => ButtonPressed(sixMonthsButton));
            twelveMonthsButton.onClick.AddListener(() => ButtonPressed(twelveMonthsButton));
        }

        public void Stop()
        {
            amountManager.OnLockAmountChanged -= UpdateDUBIReward;
        }

        private void ButtonPressed(Button button)
        {
            threeMonthsButton.interactable = true;
            sixMonthsButton.interactable = true;
            twelveMonthsButton.interactable = true;

            button.interactable = false;

            if (button == threeMonthsButton)
                multiplier = 0.01m;
            else if (button == sixMonthsButton)
                multiplier = 0.02m;
            else
                multiplier = 0.04m;

            UpdateDUBIReward();
        }

        private void UpdateDUBIReward()
        {
            if (multiplier < 0.01m)
                return;

            if (amountManager.AmountToLock <= 0.0000000000000001m)
            {
                dubiRewardText.gameObject.SetActive(false);
            }
            else
            {
                dubiRewardText.text = "(+" + (amountManager.AmountToLock * multiplier).ToString().LimitEnd(10, "...") + ")";
                dubiRewardText.gameObject.SetActive(true);
            }
        }
    }
}