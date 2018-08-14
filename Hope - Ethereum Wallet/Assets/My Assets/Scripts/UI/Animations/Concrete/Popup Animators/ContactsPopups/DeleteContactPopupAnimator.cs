using UnityEngine;
using UnityEngine.UI;

public class DeleteContactPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject mainText;
	[SerializeField] private GameObject contactName;
	[SerializeField] private GameObject contactAddress;
	[SerializeField] private GameObject yesButton;
	[SerializeField] private GameObject noButton;

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		mainText.AnimateScaleX(1f, 0.15f);
		contactName.AnimateScaleX(1f, 0.2f);
		contactAddress.AnimateScaleX(1f, 0.25f);
		yesButton.AnimateGraphicAndScale(1f, 1f, 0.3f);
		noButton.AnimateGraphicAndScale(1f, 1f, 0.3f, FinishedAnimating);
	}

	/// <summary>
	/// Animates the unique elements of this form out of view
	/// </summary>
	protected override void AnimateUniqueElementsOut()
	{
		noButton.AnimateGraphicAndScale(0f, 0f, 0.15f);
		yesButton.AnimateGraphicAndScale(0f, 0f, 0.15f);
		contactAddress.AnimateScaleX(0f, 0.2f);
		contactName.AnimateScaleX(0f, 0.25f);
		mainText.AnimateScaleX(0f, 0.3f);
	}
}
