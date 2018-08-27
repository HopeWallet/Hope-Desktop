using UnityEngine;
using UnityEngine.UI;

public class LockPRPSPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject prpsTokenSection;
	[SerializeField] private GameObject dubiTokenSection;
	[SerializeField] private GameObject line;
	[SerializeField] private GameObject purposeSection;
	[SerializeField] private GameObject transactionSpeedSection;
	[SerializeField] private GameObject timePeriodSection;
	[SerializeField] private GameObject noteText;
	[SerializeField] private GameObject lockPRPSButton;

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		prpsTokenSection.AnimateScale(1f, 0.1f);
		dubiTokenSection.AnimateScale(1f, 0.1f);
		purposeSection.AnimateScaleX(1f, 0.15f);
		transactionSpeedSection.AnimateScaleX(1f, 0.2f);
		timePeriodSection.AnimateScaleX(1f, 0.25f);
		noteText.AnimateScaleX(1f, 0.275f);
		lockPRPSButton.AnimateGraphicAndScale(1f, 1f, 0.3f, FinishedAnimating);
	}
}
