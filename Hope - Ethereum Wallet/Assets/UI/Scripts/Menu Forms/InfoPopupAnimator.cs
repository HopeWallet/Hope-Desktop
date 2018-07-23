using DG.Tweening;

public class InfoPopupAnimator : UIAnimator
{

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn() => gameObject.AnimateScaleX(1f, 0.1f, FinishedAnimating);

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut() => gameObject.AnimateScaleX(0f, 0.1f, FinishedAnimating);
}
