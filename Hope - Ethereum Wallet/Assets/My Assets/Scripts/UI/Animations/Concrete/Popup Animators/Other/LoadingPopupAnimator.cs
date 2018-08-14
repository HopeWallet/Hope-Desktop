
public class LoadingPopupAnimator : UIAnimator
{
	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn() => gameObject.AnimateGraphicAndScale(1f, 1f, 0.1f, FinishedAnimating);
}
