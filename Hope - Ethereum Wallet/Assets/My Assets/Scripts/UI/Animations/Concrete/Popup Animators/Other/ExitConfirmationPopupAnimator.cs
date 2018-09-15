using UnityEngine;

public class ExitConfirmationPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject subText;
	[SerializeField] private GameObject yesButton;
	[SerializeField] private GameObject noButton;

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		subText.AnimateScaleX(1f, 0.2f);
		yesButton.AnimateGraphicAndScale(1f, 1f, 0.25f);
		noButton.AnimateGraphicAndScale(1f, 1f, 0.3f, FinishedAnimating);
	}
}
