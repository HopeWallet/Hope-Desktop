
public class LoadingPopupAnimator : UIAnimator
{
	protected override void AnimateUniqueElementsIn() => gameObject.AnimateGraphicAndScale(1f, 1f, 0.1f, FinishedAnimating);

	protected override void AnimateUniqueElementsOut() => gameObject.AnimateGraphicAndScale(0f, 0f, 0.1f, FinishedAnimating);
}
