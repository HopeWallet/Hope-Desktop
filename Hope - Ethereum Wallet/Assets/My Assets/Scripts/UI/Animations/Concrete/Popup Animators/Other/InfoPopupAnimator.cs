using UnityEngine;

/// <summary>
/// The info popup animator
/// </summary>
public sealed class InfoPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject background;
	[SerializeField] private GameObject infoTitle;
	[SerializeField] private GameObject bodyText;
	[SerializeField] private GameObject infoIcon;
	[SerializeField] private GameObject errorIcon;

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		background.AnimateGraphicAndScale(1f, 1f, 0.05f);
		infoTitle.AnimateGraphicAndScale(0.84f, 1f, 0.1f);
		bodyText.AnimateGraphicAndScale(0.64f, 1f, 0.1f, FinishedAnimating);

		infoIcon.AnimateGraphicAndScale(1f, 1f, 0.1f);
		errorIcon.AnimateGraphicAndScale(1f, 1f, 0.1f);
	}
}
