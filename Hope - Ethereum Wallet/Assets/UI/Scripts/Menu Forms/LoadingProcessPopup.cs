using UnityEngine;

public class LoadingProcessPopup : UIAnimator
{

	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject text;
	[SerializeField] private GameObject loadingIcon;

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		dim.AnimateGraphic(1f, 0.1f,
			() => form.AnimateGraphicAndScale(1f, 1f, 0.1f,
			() => text.AnimateScaleX(1f, 0.1f,
			() => loadingIcon.AnimateGraphicAndScale(1f, 0.75f, 0.1f, FinishedAnimating))));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		loadingIcon.AnimateGraphicAndScale(0f, 0f, 0.1f,
			() => text.AnimateScaleX(0f, 0.1f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.1f,
			() => dim.AnimateGraphic(0f, 0.1f, FinishedAnimating))));
	}
}
