using UnityEngine;

/// <summary>
/// The animator class of the TransactionWarningPopup
/// </summary>
public sealed class TransactionWarningPopupAnimator : PopupAnimator
{
	[SerializeField] private GameObject errorIcon;
	[SerializeField] private GameObject transactionPendingText;
	[SerializeField] private GameObject warningText;
	[SerializeField] private GameObject questionText;
	[SerializeField] private GameObject yesButton;
	[SerializeField] private GameObject noButton;

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		errorIcon.AnimateGraphicAndScale(1f, 1f, 0.1f);
		transactionPendingText.AnimateScaleX(1f, 0.15f);
		warningText.AnimateScaleX(1f, 0.2f);
		questionText.AnimateScaleX(1f, 0.25f);
		yesButton.AnimateGraphicAndScale(1f, 1f, 0.3f);
		noButton.AnimateGraphicAndScale(1f, 1f, 0.3f, FinishedAnimating);
	}
}
