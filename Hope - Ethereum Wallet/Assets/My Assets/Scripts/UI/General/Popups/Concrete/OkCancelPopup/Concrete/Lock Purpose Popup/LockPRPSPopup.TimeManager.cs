using TMPro;
using UnityEngine.UI;

/// <summary>
/// Class used for locking purpose.
/// </summary>
public sealed partial class LockPRPSPopup
{
    /// <summary>
    /// Class which manages the different time periods for locking purpose.
    /// </summary>
    public sealed class TimeManager
    {
        private readonly AmountManager amountManager;

        private readonly Button threeMonthsButton,
                                sixMonthsButton,
                                twelveMonthsButton;

        private readonly TMP_Text dubiRewardText;

        private decimal multiplier;

        /// <summary>
        /// Whether the time period to lock for is valid.
        /// </summary>
        public bool IsValid => multiplier >= 0.01m;

        /// <summary>
        /// The integer number of months to lock purpose for.
        /// </summary>
        public int MonthsToLock => (int)(multiplier * 300);

        /// <summary>
        /// Initializes the TimeManager by assigning all buttons and other dependencies.
        /// </summary>
        /// <param name="amountManager"> The LockPRPSPopup.AmountManager. </param>
        /// <param name="threeMonthsButton"> The button used to represent a three month purpose lock. </param>
        /// <param name="sixMonthsButton"> The button used to represent a six month purpose lock. </param>
        /// <param name="twelveMonthsButton"> The button used to represent a twelve month purpose lock. </param>
        /// <param name="dubiRewardText"> The text component used for displaying the dubi reward. </param>
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

        /// <summary>
        /// Stops the TimeManager.
        /// </summary>
        public void Stop()
        {
            amountManager.OnLockAmountChanged -= UpdateDUBIReward;
        }

        /// <summary>
        /// Called when a new monthly lock button is pressed.
        /// </summary>
        /// <param name="button"> The monthly lock button which was pressed. </param>
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

        /// <summary>
        /// Updates the dubi reward text based on the button that was pressed.
        /// </summary>
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
                dubiRewardText.text = "(+" + (amountManager.AmountToLock * multiplier).ConvertDecimalToString().LimitEnd(10, "...") + ")";
                dubiRewardText.gameObject.SetActive(true);
            }
        }
    }
}