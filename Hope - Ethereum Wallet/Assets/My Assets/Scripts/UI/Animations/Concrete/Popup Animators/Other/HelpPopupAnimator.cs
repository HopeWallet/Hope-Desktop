using UnityEngine;

public class HelpPopupAnimator : PopupAnimator
{
	[SerializeField] private GameObject comingSoonText;

	/// <summary>
	/// Animates the unique elements of this popup into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		comingSoonText.AnimateGraphicAndScale(1f, 1f, 0.2f, FinishedAnimating);
	}
}
