using UnityEngine;

public class ConfirmTransactionPopupAnimator : CountdownTimerAnimator
{
	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject tokenIcon;
	[SerializeField] private GameObject fromAddressSection;
	[SerializeField] private GameObject toAddressSection;
	[SerializeField] private GameObject transactionSection;
	[SerializeField] private GameObject feeSection;

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		dim.AnimateGraphic(1f, 0.15f);
        form.AnimateGraphicAndScale(1f, 1f, 0.15f,
            () => tokenIcon.AnimateGraphicAndScale(1f, 1f, 0.15f,
            () => toAddressSection.AnimateScaleX(1f, 0.15f,
            () => feeSection.AnimateScaleX(1f, 0.15f,
            () => cancelButton.AnimateGraphicAndScale(1f, 1f, 0.15f, StartTimerAnimation)))));

		title.AnimateGraphicAndScale(1f, 0.85f, 0.15f,
			() => fromAddressSection.AnimateScaleX(1f, 0.15f,
			() => transactionSection.AnimateScaleX(1f, 0.15f,
			() => confirmButton.AnimateGraphicAndScale(1f, 1f, 0.15f, FinishedAnimating))));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
        timerText.AnimateGraphicAndScale(0f, 0f, 0.15f);
		confirmButton.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => transactionSection.AnimateScaleX(0f, 0.15f,
			() => fromAddressSection.AnimateScaleX(0f, 0.15f,
			() => title.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.15f)))));

		cancelButton.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => feeSection.AnimateScaleX(0f, 0.15f,
			() => toAddressSection.AnimateScaleX(0f, 0.15f,
			() => tokenIcon.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => dim.AnimateGraphic(0f, 0.15f, FinishedAnimating)))));
	}
}
