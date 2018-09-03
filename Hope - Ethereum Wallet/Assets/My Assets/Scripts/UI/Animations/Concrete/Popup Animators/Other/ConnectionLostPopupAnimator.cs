using UnityEngine;

public class ConnectionLostPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject subText;

	protected override void AnimateUniqueElementsIn() => subText.AnimateGraphicAndScale(1f, 1f, 0.1f, FinishedAnimating);
}
