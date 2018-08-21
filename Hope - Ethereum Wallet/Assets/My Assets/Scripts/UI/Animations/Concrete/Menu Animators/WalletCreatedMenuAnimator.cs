using UnityEngine;

public class WalletCreatedMenuAnimator : UIAnimator
{
	protected override void AnimateUniqueElementsIn()
	{
		FinishedAnimating();
	}

	protected override void ResetElementValues()
	{
		FinishedAnimating();
	}
}
