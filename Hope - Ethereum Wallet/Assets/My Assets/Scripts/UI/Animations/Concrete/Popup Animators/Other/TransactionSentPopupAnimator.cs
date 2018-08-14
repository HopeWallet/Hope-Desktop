using UnityEngine;

public class TransactionSentPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject mainText;
	[SerializeField] private GameObject subText;
	[SerializeField] private GameObject okButton;

	protected override void AnimateUniqueElementsIn()
	{
		mainText.AnimateGraphicAndScale(0.84f, 1f, 0.2f);
		subText.AnimateGraphicAndScale(0.64f, 1f, 0.25f);
		okButton.AnimateGraphicAndScale(1f, 1f, 0.3f, FinishedAnimating);
	}

	protected override void AnimateUniqueElementsOut()
	{
		okButton.AnimateGraphicAndScale(0f, 0f, 0.2f, () => AnimateBasicElements(false));
		subText.AnimateGraphicAndScale(0f, 0f, 0.25f);
		mainText.AnimateGraphicAndScale(0f, 0f, 0.3f);
	}
}
