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

	protected override void AnimateUniqueElementsIn()
	{
		throw new System.NotImplementedException();
	}

	protected override void AnimateUniqueElementsOut()
	{
		throw new System.NotImplementedException();
	}
}