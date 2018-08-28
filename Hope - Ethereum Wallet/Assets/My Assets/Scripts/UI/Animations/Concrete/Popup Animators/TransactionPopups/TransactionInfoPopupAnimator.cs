using UnityEngine;

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
}