using UnityEngine;

public class ExitConfirmationPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject noteText;
	[SerializeField] private GameObject yesButton;
	[SerializeField] private GameObject noButton;

	protected override void AnimateUniqueElementsIn()
	{
		noteText.AnimateScaleX(1f, 0.2f);
		yesButton.AnimateGraphicAndScale(1f, 1f, 0.3f);
		noButton.AnimateGraphicAndScale(1f, 1f, 0.3f, FinishedAnimating);
	}

	protected override void AnimateUniqueElementsOut()
	{
		noButton.AnimateGraphicAndScale(0f, 0f, 0.2f, () => AnimateBasicElements(false));
		yesButton.AnimateGraphicAndScale(0f, 0f, 0.2f);
		noteText.AnimateScaleX(0f, 0.3f);
	}
}
