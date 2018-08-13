using UnityEngine;
using UnityEngine.UI;

public class DeleteContactPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject mainText;
	[SerializeField] private GameObject contactName;
	[SerializeField] private GameObject contactAddress;
	[SerializeField] private GameObject yesButton;
	[SerializeField] private GameObject noButton;

	protected override void AnimateUniqueElementsIn()
	{
		throw new System.NotImplementedException();
	}

	protected override void AnimateUniqueElementsOut()
	{
		throw new System.NotImplementedException();
	}
}
