using UnityEngine;
using UnityEngine.UI;

public class ExitConfirmationPopupAnimator : UIAnimator
{

	[SerializeField] private Image blur;
	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject noteText;
	[SerializeField] private GameObject yesButton;
	[SerializeField] private GameObject noButton;

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		blur.AnimateMaterialBlur(0.25f, 0.2f);
		dim.AnimateGraphic(1f, 0.2f);
		form.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => title.AnimateGraphicAndScale(0.85f, 1f, 0.15f,
			() => noteText.AnimateScaleX(1f, 0.15f,
			() => yesButton.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => noButton.AnimateGraphicAndScale(1f, 1f, 0.15f, FinishedAnimating)))));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		noteText.AnimateScaleX(0f, 0.15f,
			() => title.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => { blur.AnimateMaterialBlur(-0.25f, 0.2f); dim.AnimateGraphic(0, 0.2f, FinishedAnimating); })));

		noButton.AnimateGraphicAndScale(0f, 0f, 0.15f);
		yesButton.AnimateGraphicAndScale(0f, 0f, 0.15f);
	}
}
