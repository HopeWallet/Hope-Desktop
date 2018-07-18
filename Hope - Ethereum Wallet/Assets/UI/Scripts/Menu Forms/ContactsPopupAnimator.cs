using UnityEngine;

public class ContactsPopupAnimator : UIAnimator
{
	protected override void AnimateIn()
	{
		FinishedAnimating();
	}

	protected override void AnimateOut()
	{
		FinishedAnimating();
	}
}
