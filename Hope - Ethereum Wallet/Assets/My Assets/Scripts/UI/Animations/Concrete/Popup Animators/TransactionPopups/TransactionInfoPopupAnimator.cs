using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TransactionInfoPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject transactionHashSection;
	[SerializeField] private GameObject fromAddressSection;
	[SerializeField] private GameObject toAddressSection;
	[SerializeField] private GameObject valueAndTimeSection;
	[SerializeField] private GameObject setGasSection;
	[SerializeField] private GameObject actualGasSection;

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		transactionHashSection.AnimateScaleX(1f, 0.1f);
		fromAddressSection.AnimateScaleX(1f, 0.14f);
		toAddressSection.AnimateScaleX(1f, 0.18f);
		valueAndTimeSection.AnimateScaleX(1f, 0.22f);
		setGasSection.AnimateScaleX(1f, 0.26f);
		actualGasSection.AnimateScaleX(1f, 0.3f, FinishedAnimating);
	}

	/// <summary>
	/// Animates the unique elements of this form out of view
	/// </summary>
	protected override void AnimateUniqueElementsOut()
	{
		actualGasSection.AnimateScaleX(0f, 0.1f);
		setGasSection.AnimateScaleX(0f, 0.14f);
		valueAndTimeSection.AnimateScaleX(0f, 0.18f);
		toAddressSection.AnimateScaleX(0f, 0.22f, () => AnimateBasicElements(false));
		fromAddressSection.AnimateScaleX(0f, 0.26f);
		transactionHashSection.AnimateScaleX(0f, 0.3f);
	}
}