
/// <summary>
/// The animator class of the LoadingPopup
/// </summary>
public sealed class LoadingPopupAnimator : UIAnimator
{
	protected override void AnimateIn()
	{
		gameObject.AnimateGraphicAndScale(1f, 1f, 0.1f, FinishedAnimating);
	}

	protected override void AnimateOut()
	{
		gameObject.AnimateGraphicAndScale(0f, 0f, 0.1f, FinishedAnimating);
	}

	protected override void AnimateUniqueElementsIn()
	{
		gameObject.AnimateGraphicAndScale(1f, 1f, 0.1f, FinishedAnimating);
	}
}
