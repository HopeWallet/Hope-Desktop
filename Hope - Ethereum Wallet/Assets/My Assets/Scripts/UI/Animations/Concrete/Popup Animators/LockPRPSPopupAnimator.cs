using UnityEngine;
using UnityEngine.UI;

public class LockPRPSPopupAnimator : UIAnimator
{

	[SerializeField] private Image blur;
	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject prpsTokenSection;
	[SerializeField] private GameObject dubiTokenSection;
	[SerializeField] private GameObject purposeSection;
	[SerializeField] private GameObject transactionSpeedSection;
	[SerializeField] private GameObject timePeriodSection;
	[SerializeField] private GameObject noteText;
	[SerializeField] private GameObject lockPRPSButton;

	protected override void AnimateIn()
	{
		blur.AnimateMaterialBlur(0.5f, 0.15f,
			() => title.AnimateScaleX(1f, 0.15f,
			() => dubiTokenSection.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => transactionSpeedSection.AnimateScaleX(1f, 0.15f,
			() => noteText.AnimateScaleX(1f, 0.15f,
			() => lockPRPSButton.AnimateGraphicAndScale(1f, 1f, 0.15f, FinishedAnimating))))));

		dim.AnimateGraphic(1f, 0.15f,
			() => form.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => prpsTokenSection.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => purposeSection.AnimateScaleX(1f, 0.15f,
			() => timePeriodSection.AnimateScaleX(1f, 0.15f)))));
	}

	protected override void AnimateOut()
	{
		
	}
}
