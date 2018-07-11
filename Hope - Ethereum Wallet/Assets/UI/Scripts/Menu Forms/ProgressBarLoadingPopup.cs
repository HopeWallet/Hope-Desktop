using UnityEngine;

public class ProgressBarLoadingPopup : UIAnimator
{

	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject text;
	[SerializeField] private GameObject loadingBar;

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		dim.AnimateGraphic(1f, 0.1f,
			() => form.AnimateGraphicAndScale(1f, 1f, 0.1f,
			() => text.AnimateGraphicAndScale(1f, 0.85f, 0.1f,
			() => loadingBar.AnimateScaleX(1f, 0.1f, FinishedAnimating))));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		loadingBar.AnimateScaleX(0f, 0.1f,
			() => text.AnimateGraphicAndScale(0f, 0f, 0.1f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.1f,
			() => dim.AnimateGraphic(0f, 0.1f, FinishedAnimating))));
	}
}
