
using UnityEngine;
/// <summary>
/// The animator class of the LoadingPopup
/// </summary>
public sealed class LoadingPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject loadingText;
	[SerializeField] private GameObject loadingIcon;

	/// <summary>
	/// Animates the elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		loadingText.AnimateGraphicAndScale(1f, 1f, 0.15f);
		loadingIcon.AnimateGraphicAndScale(1f, 1f, 0.2f, FinishedAnimating);
	}

	/// <summary>
	/// Animates the elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		loadingText.AnimateGraphic(0f, 0.2f);
		loadingIcon.AnimateGraphic(0f, 0.2f, FinishedAnimating);
	}
}
