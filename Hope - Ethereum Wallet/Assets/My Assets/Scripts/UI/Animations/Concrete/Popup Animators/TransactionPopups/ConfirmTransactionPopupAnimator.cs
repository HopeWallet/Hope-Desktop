using UnityEngine;
using UnityEngine.UI;

public sealed class ConfirmTransactionPopupAnimator : CountdownTimerAnimator
{
	[SerializeField] private GameObject tokenIcon;
	[SerializeField] private GameObject fromAddressSection;
	[SerializeField] private GameObject toAddressSection;
	[SerializeField] private GameObject transactionSection;
	[SerializeField] private GameObject feeSection;

	protected override void AnimateUniqueElementsIn()
	{
		throw new System.NotImplementedException();
	}

	protected override void AnimateUniqueElementsOut()
	{
		throw new System.NotImplementedException();
	}
}
