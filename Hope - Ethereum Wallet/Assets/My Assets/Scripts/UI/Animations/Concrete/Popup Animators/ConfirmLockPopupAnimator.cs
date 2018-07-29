using UnityEngine;
using UnityEngine.UI;

public class ConfirmLockPopupAnimator : CountdownTimerAnimator
{

	[SerializeField] private Image blur;
	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject prpsSection;
	[SerializeField] private GameObject dubiSection;

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		blur.AnimateMaterialBlur(0.25f, 0.15f);
		dim.AnimateGraphic(1f, 0.15f);
		form.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => title.AnimateGraphicAndScale(0.85f, 1f, 0.15f,
			() => prpsSection.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => dubiSection.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => { confirmButton.AnimateScaleX(1f, 0.15f, StartTimerAnimation); cancelButton.AnimateScaleX(1f, 0.15f, FinishedAnimating); }))));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		timerText.AnimateGraphicAndScale(0f, 0f, 0.2f);

		cancelButton.AnimateScaleX(0f, 0.2f,
			() => prpsSection.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => blur.AnimateMaterialBlur(-0.25f, 0.15f))));

		confirmButton.AnimateScaleX(0f, 0.2f,
			() => dubiSection.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => title.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => dim.AnimateGraphic(0f, 0.15f, FinishedAnimating))));
	}
}
