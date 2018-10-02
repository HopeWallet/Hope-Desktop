using UnityEngine;

/// <summary>
/// The animator class of the ConnectionLostPopup
/// </summary>
public sealed class ConnectionLostPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject blur;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject connectionLostText;
	[SerializeField] private GameObject retryingConnectionText;

	/// <summary>
	/// Animates the elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		dim.AnimateGraphic(1f, 0.2f);
		blur.AnimateScale(1f, 0.2f);
		form.AnimateGraphic(1f, 0.2f);
		form.AnimateScaleX(1f, 0.2f);
		connectionLostText.AnimateGraphicAndScale(1f, 1f, 0.25f);
		retryingConnectionText.AnimateGraphicAndScale(1f, 1f, 0.3f, FinishedAnimating);
	}

	/// <summary>
	/// Animates the elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		dim.AnimateGraphic(0f, 0.2f);
		blur.AnimateScale(0f, 0.2f);
		form.AnimateGraphic(0f, 0.2f);
		connectionLostText.AnimateGraphic(0f, 0.2f);
		retryingConnectionText.AnimateGraphic(0f, 0.2f, FinishedAnimating);
	}
}
