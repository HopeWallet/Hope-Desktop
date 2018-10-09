using UnityEngine;

/// <summary>
/// The animator class of the InfoPopup
/// </summary>
public sealed class InfoPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject triangle;
	[SerializeField] private GameObject box;
	[SerializeField] private GameObject infoTitle;
	[SerializeField] private GameObject bodyText;

	/// <summary>
	/// Animates the elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		//gameObject.AnimateTransformX(10f, 0.2f);

		triangle.AnimateGraphic(1f, 0.2f);
		box.AnimateGraphic(1f, 0.2f);
		infoTitle.AnimateGraphic(1f, 0.2f);
		bodyText.AnimateGraphic(1f, 0.2f, FinishedAnimating);
	}

	/// <summary>
	/// Animates the elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		triangle.AnimateGraphic(0f, 0.2f);
		box.AnimateGraphic(0f, 0.2f);
		infoTitle.AnimateGraphic(0f, 0.2f);
		bodyText.AnimateGraphic(0f, 0.2f, FinishedAnimating);
	}

}
