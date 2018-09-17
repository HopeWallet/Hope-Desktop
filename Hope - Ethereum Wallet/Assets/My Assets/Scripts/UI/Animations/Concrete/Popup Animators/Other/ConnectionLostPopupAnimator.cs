using UnityEngine;

/// <summary>
/// The animator class of the ConnectionLostPopup
/// </summary>
public sealed class ConnectionLostPopupAnimator : PopupAnimator
{ 
	[SerializeField] private GameObject subText;

	protected override void AnimateUniqueElementsIn()
	{
		subText.AnimateGraphicAndScale(1f, 1f, 0.1f, FinishedAnimating);
	}
}
