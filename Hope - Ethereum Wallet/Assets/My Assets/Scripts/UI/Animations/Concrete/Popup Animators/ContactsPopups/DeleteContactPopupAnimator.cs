using UnityEngine;

/// <summary>
/// The animator class of the DeleteContactPopup
/// </summary>
public sealed class DeleteContactPopupAnimator : PopupAnimator
{
	[SerializeField] private GameObject contactName;
	[SerializeField] private GameObject contactAddress;
	[SerializeField] private GameObject yesButton;
	[SerializeField] private GameObject noButton;

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		contactName.AnimateScaleX(1f, 0.2f);
		contactAddress.AnimateScaleX(1f, 0.25f);
		yesButton.AnimateGraphicAndScale(1f, 1f, 0.3f);
		noButton.AnimateGraphicAndScale(1f, 1f, 0.3f, FinishedAnimating);
	}
}
