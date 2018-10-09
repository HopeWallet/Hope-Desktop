using UnityEngine;

/// <summary>
/// The animator class of the ConfirmTransactionPopup
/// </summary>
public sealed class ConfirmTransactionPopupAnimator : CountdownTimerAnimator
{
	[SerializeField] private GameObject tokenIcon;
	[SerializeField] private GameObject outIcon;
	[SerializeField] private GameObject fromAddressSection;
	[SerializeField] private GameObject toAddressSection;
	[SerializeField] private GameObject line;
	[SerializeField] private GameObject transactionSection;
	[SerializeField] private GameObject feeSection;

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		tokenIcon.AnimateGraphicAndScale(1f, 1f, 0.15f);
		outIcon.AnimateGraphicAndScale(1f, 1f, 0.15f);
		fromAddressSection.AnimateScaleX(1f, 0.18f);
		toAddressSection.AnimateScaleX(1f, 0.22f);
		line.AnimateScaleX(1f, 0.235f);
		transactionSection.AnimateScaleX(1f, 0.25f);
		feeSection.AnimateScaleX(1f, 0.27f, StartTimerAnimation);

		if (confirmText.activeInHierarchy)
		{
			confirmText.AnimateGraphicAndScale(1f, 1f, 0.3f, FinishedAnimating);
		}
		else
		{
			confirmButton.AnimateGraphicAndScale(1f, 1f, 0.3f);
			cancelButton.AnimateGraphicAndScale(1f, 1f, 0.3f, FinishedAnimating);
		}
	}
}
