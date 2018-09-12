using UnityEngine;

public class AboutPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject hopeVersionSection;

	protected override void AnimateUniqueElementsIn()
	{
		hopeVersionSection.AnimateScale(1f, 0.15f, FinishedAnimating);
	}
}
