using UnityEngine;

public class LoadingPopupAnimator : UIAnimator
{

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn() => gameObject.AnimateGraphicAndScale(1f, 1f, 0.1f, FinishedAnimating);

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut() => gameObject.AnimateGraphicAndScale(0f, 0f, 0.1f, FinishedAnimating);
}
