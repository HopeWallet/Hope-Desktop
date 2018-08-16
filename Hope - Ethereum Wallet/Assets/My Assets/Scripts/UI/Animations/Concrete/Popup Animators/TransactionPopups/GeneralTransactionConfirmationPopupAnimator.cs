public sealed class GeneralTransactionConfirmationPopupAnimator : UIAnimator
{
	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
        FinishedAnimating();
	}

    protected override void ResetElementValues()
    {
        FinishedAnimating();
    }
}