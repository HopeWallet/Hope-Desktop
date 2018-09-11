using UnityEngine;

public class AccountsPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject topSection;
	[SerializeField] private GameObject line;
	[SerializeField] private Transform addressSection;
	[SerializeField] private Transform pageSection;
	[SerializeField] private GameObject unlockButton;

	protected override void AnimateUniqueElementsIn()
	{
		topSection.AnimateScaleX(1f, 0.175f);
		line.AnimateScaleX(1f, 0.2f);

		float duration = 0.2f;
		for (int i = 0; i < addressSection.childCount; i++)
		{
			addressSection.GetChild(i).gameObject.AnimateScaleX(1f, duration);
			duration += 0.15f;
		}

		duration = 0.24f;
		for (int i = 0; i < pageSection.childCount; i++)
		{
			pageSection.GetChild(i).gameObject.AnimateGraphicAndScale(1f, 1f, duration);
			duration += 0.15f;
		}

		unlockButton.AnimateGraphicAndScale(1f, 1f, 0.3f, FinishedAnimating);
	}
}
