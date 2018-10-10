using UnityEngine;

/// <summary>
/// The animator class of the TooltipPopup
/// </summary>
public sealed class TooltipPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject[] popupElements;

	/// <summary>
	/// Animates the elements of the form into view
	/// </summary>
	protected override void AnimateIn() => AnimateVisuals(true);

	/// <summary>
	/// Animates the elements of the form out of view
	/// </summary>
	protected override void AnimateOut() => AnimateVisuals(false);

	/// <summary>
	/// Animates the visuals of the info popup in or out
	/// </summary>
	/// <param name="animateIn"> Whether being animated in or out </param>
	private void AnimateVisuals(bool animateIn)
	{
		float graphicValue = animateIn ? 1f : 0f;
		float timeValue = animateIn ? 0.4f : 0.2f;

		foreach (GameObject obj in popupElements)
			obj.AnimateGraphic(graphicValue, timeValue);

		CoroutineUtils.ExecuteAfterWait(timeValue, FinishedAnimating);
	}

}
