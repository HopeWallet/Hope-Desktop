using UnityEngine;
using UnityEngine.UI;

public class ExitConfirmationPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject noteText;
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
