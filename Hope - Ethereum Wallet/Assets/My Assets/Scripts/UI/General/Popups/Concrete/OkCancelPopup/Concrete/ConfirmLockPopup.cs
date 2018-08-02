using TMPro;

/// <summary>
/// Class used for displaying the confirmation to lock purpose.
/// </summary>
public sealed class ConfirmLockPopup : ConfirmTransactionPopupBase<ConfirmLockPopup>
{
    public TMP_Text lockPeriodText,
                    prpsAmountText,
                    dubiAmountText;

    /// <summary>
    /// Passes the amount of purpose being locked through to display.
    /// </summary>
    /// <param name="transactionInput"> The transaction input containing the amount of purpose to lock. </param>
    protected override void InternalSetConfirmationValues(object[] transactionInput) => SetLockPrpsValues((int)transactionInput[0], (decimal)transactionInput[1]);

    /// <summary>
    /// Displays the amount of purpose and time period the purpose will be locked for.
    /// </summary>
    /// <param name="lockPeriod"> The number of months the purpose will be locked for. </param>
    /// <param name="lockAmount"> The amount of purpose to lock. </param>
    private void SetLockPrpsValues(int lockPeriod, decimal lockAmount)
    {
        lockPeriodText.text = lockPeriod + " Month Lock";
        prpsAmountText.text = lockAmount.ToString().LimitEnd(13, "...");
        dubiAmountText.text = (lockAmount * ((decimal)lockPeriod / 300)).ToString().TrimEnd('0', '.').LimitEnd(13, "...");
    }
}