using UnityEngine;

public class TransactionSentPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject mainText;
	[SerializeField] private GameObject subText;
	[SerializeField] private GameObject okButton;

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		mainText.AnimateGraphicAndScale(0.84f, 1f, 0.2f);
		subText.AnimateGraphicAndScale(0.64f, 1f, 0.25f);
		okButton.AnimateGraphicAndScale(1f, 1f, 0.3f, FinishedAnimating);
	}
}
