using UnityEngine;

/// <summary>
/// The animator class of the HopeUpdatePopup
/// </summary>
public sealed class HopeUpdatePopupAnimator : PopupAnimator
{
	/// <summary>
	/// Animate the unique elements of the form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		FinishedAnimating();
	}
}
