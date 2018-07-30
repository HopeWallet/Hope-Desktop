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

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		blur.AnimateMaterialBlur(0.75f, 0.15f,
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

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut() => AnimateBottomSectionOut();

	/// <summary>
	/// Animates the entire bottom section out, including the lockPRPSButton, noteText, timePeriodSection,
	/// transactionSpeedSection, and the purposeSection
	/// </summary>
	private void AnimateBottomSectionOut()
	{
		lockPRPSButton.AnimateGraphicAndScale(0f, 0f, 0.15f);
		noteText.AnimateScaleX(0f, 0.15f,
			() => transactionSpeedSection.AnimateScaleX(0f, 0.15f));
		timePeriodSection.AnimateScaleX(0f, 0.15f,
			() => purposeSection.AnimateScaleX(0f, 0.15f, AnimateTopSectionOut));
	}

	/// <summary>
	/// Animates the entier top section including the prps and dubi tokens, title, and finally the form, dim, and blur
	/// </summary>
	private void AnimateTopSectionOut()
	{
		prpsTokenSection.AnimateGraphicAndScale(0f, 0f, 0.15f);

		dubiTokenSection.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => title.AnimateScaleX(0f, 0.15f,
			() => { form.AnimateGraphicAndScale(0f, 0f, 0.2f); blur.AnimateMaterialBlur(-0.75f, 0.2f); dim.AnimateGraphic(0f, 0.2f, FinishedAnimating); }));
	}
}
