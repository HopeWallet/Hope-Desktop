using UnityEngine;

/// <summary>
/// The animator class of the HopeUpdatePopup
/// </summary>
public sealed class HopeUpdatePopupAnimator : UIAnimator
{
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject updateAvailableText;
	[SerializeField] private GameObject downloadButton;
	[SerializeField] private GameObject laterButton;

	/// <summary>
	/// Animates the elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		gameObject.AnimateTransformY(-452f, 0.2f);

		form.AnimateGraphic(1f, 0.2f);
		updateAvailableText.AnimateGraphic(1f, 0.3f);
		downloadButton.AnimateGraphic(1f, 0.3f);
		laterButton.AnimateGraphic(1f, 0.3f, FinishedAnimating);
	}

	/// <summary>
	/// Animates the elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		form.AnimateGraphic(0f, 0.2f);
		updateAvailableText.AnimateGraphic(0f, 0.2f);
		downloadButton.AnimateGraphic(0f, 0.2f);
		laterButton.AnimateGraphic(0f, 0.2f, FinishedAnimating);
	}
}
