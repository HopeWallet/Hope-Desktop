
public class OpenLedgerWalletMenuAnimator : UIAnimator
{
	/// <summary>
	/// Animates the unique elements into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		FinishedAnimating();
	}

	/// <summary>
	/// Resets the unique elements of the form back to the starting positions
	/// </summary>
	protected override void ResetElementValues()
	{
		FinishedAnimating();
	}
}