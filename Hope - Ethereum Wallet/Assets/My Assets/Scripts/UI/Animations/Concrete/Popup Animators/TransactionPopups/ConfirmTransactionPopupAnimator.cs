using UnityEngine;
using UnityEngine.UI;

public sealed class ConfirmTransactionPopupAnimator : CountdownTimerAnimator
{
	[SerializeField] private GameObject tokenIcon;
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
		fromAddressSection.AnimateScaleX(1f, 0.18f);
		toAddressSection.AnimateScaleX(1f, 0.22f);
		line.AnimateScaleX(1f, 0.235f);
		transactionSection.AnimateScaleX(1f, 0.25f);
		feeSection.AnimateScaleX(1f, 0.27f, StartTimerAnimation);
		confirmButton.AnimateGraphicAndScale(1f, 1f, 0.3f);
		cancelButton.AnimateGraphicAndScale(1f, 1f, 0.3f, FinishedAnimating);
	}

	/// <summary>
	/// Animates the unique elements of this form out of view
	/// </summary>
	protected override void AnimateUniqueElementsOut()
	{
		cancelButton.AnimateGraphicAndScale(0f, 0f, 0.15f);
		confirmButton.AnimateGraphicAndScale(0f, 0f, 0.15f);
		feeSection.AnimateScaleX(0f, 0.18f);
		transactionSection.AnimateScaleX(0f, 0.22f, () => AnimateBasicElements(false));
		line.AnimateScaleX(0f, 0.235f);
		fromAddressSection.AnimateScaleX(0f, 0.25f);
		toAddressSection.AnimateScaleX(0f, 0.27f);
		tokenIcon.AnimateGraphicAndScale(0f, 0f, 0.3f);
	}
}
