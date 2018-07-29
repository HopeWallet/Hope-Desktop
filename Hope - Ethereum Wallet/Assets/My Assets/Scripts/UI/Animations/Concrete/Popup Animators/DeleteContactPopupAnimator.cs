using UnityEngine;
using UnityEngine.UI;

public class DeleteContactPopupAnimator : UIAnimator
{

	[SerializeField] private Image blur;
	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject mainText;
	[SerializeField] private GameObject contactName;
	[SerializeField] private GameObject contactAddress;
	[SerializeField] private GameObject yesButton;
	[SerializeField] private GameObject noButton;

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		blur.AnimateMaterialBlur(0.25f, 0.2f);
		dim.AnimateGraphic(1f, 0.2f,
			() => contactName.AnimateScaleX(1f, 0.15f,
			() => contactAddress.AnimateScaleX(1f, 0.15f,
			() => yesButton.AnimateGraphicAndScale(1f, 1f, 0.1f,
			() => noButton.AnimateGraphicAndScale(1f, 1f, 0.1f, FinishedAnimating)))));

		form.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => mainText.AnimateScaleX(1f, 0.15f));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		contactName.AnimateScaleX(0f, 0.1f);

		yesButton.AnimateGraphicAndScale(0f, 0f, 0.1f,
			() => contactAddress.AnimateScaleX(0f, 0.1f,
			() => { blur.AnimateMaterialBlur(-0.25f, 0.2f); dim.AnimateGraphic(0f, 0.2f); }));

		noButton.AnimateGraphicAndScale(0f, 0f, 0.1f,
			() => mainText.AnimateScaleX(0f, 0.1f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.2f, FinishedAnimating)));
	}

}
