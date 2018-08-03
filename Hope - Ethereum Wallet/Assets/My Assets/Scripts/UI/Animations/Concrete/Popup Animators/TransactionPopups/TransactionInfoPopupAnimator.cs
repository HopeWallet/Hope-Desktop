using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TransactionInfoPopupAnimator : UIAnimator
{

	[SerializeField] private Image blur;
	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject transactionHashSection;
	[SerializeField] private GameObject fromAddressSection;
	[SerializeField] private GameObject toAddressSection;
	[SerializeField] private GameObject valueAndTimeSection;
	[SerializeField] private GameObject setGasSection;
	[SerializeField] private GameObject actualGasSection;

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		blur.AnimateMaterialBlur(1f, 0.15f,
			() => form.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => transactionHashSection.AnimateScaleX(1f, 0.15f,
			() => toAddressSection.AnimateScaleX(1f, 0.15f,
			() => setGasSection.AnimateScaleX(1f, 0.15f)))));

		dim.AnimateGraphic(1f, 0.15f,
			() => title.AnimateGraphicAndScale(0.85f, 1f, 0.15f,
			() => fromAddressSection.AnimateScaleX(1f, 0.15f,
			() => valueAndTimeSection.AnimateScaleX(1f, 0.15f,
			() => actualGasSection.AnimateScaleX(1f, 0.15f, FinishedAnimating)))));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		actualGasSection.AnimateScaleX(0f, 0.15f,
			() => valueAndTimeSection.AnimateScaleX(0f, 0.15f,
			() => fromAddressSection.AnimateScaleX(0f, 0.15f,
			() => title.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => blur.AnimateMaterialBlur(-1f, 0.15f)))));

		setGasSection.AnimateScaleX(0f, 0.15f,
			() => toAddressSection.AnimateScaleX(0f, 0.15f,
			() => transactionHashSection.AnimateScaleX(0f, 0.15f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => dim.AnimateGraphic(0f, 0.15f, FinishedAnimating)))));
	}
}