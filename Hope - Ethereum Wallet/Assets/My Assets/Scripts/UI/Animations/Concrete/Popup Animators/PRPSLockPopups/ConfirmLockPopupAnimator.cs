using UnityEngine;

public class ConfirmLockPopupAnimator : CountdownTimerAnimator
{
	[SerializeField] private GameObject prpsSection;
	[SerializeField] private GameObject dubiSection;
	[SerializeField] private GameObject noteText;

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		prpsSection.AnimateScale(1f, 0.2f);
		dubiSection.AnimateScale(1f, 0.2f, StartTimerAnimation);
		noteText.AnimateScaleX(1f, 0.25f);
		confirmButton.AnimateGraphicAndScale(1f, 1f, 0.3f);
		cancelButton.AnimateGraphicAndScale(1f, 1f, 0.3f, FinishedAnimating);
	}
}
