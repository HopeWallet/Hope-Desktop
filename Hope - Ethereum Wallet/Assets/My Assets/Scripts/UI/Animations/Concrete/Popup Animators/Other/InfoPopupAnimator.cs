using UnityEngine;

/// <summary>
/// The animator class of the InfoPopup
/// </summary>
public sealed class InfoPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject infoTitle;
	[SerializeField] private GameObject bodyText;
	[SerializeField] private GameObject icon;
	[SerializeField] private GameObject errorIcon;
	[SerializeField] private GameObject exitButton;

	/// <summary>
	/// Animates the elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		gameObject.AnimateTransformX(501.5f, 0.2f);

		form.AnimateGraphic(1f, 0.2f);
		icon.AnimateGraphic(1f, 0.2f);
		infoTitle.AnimateGraphic(1f, 0.2f);
		bodyText.AnimateGraphic(1f, 0.2f);
		exitButton.AnimateGraphic(1f, 0.2f, FinishedAnimating);
	}

	/// <summary>
	/// Animates the elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		//gameObject.AnimateTransformX(778.3f, 0.15f);

		form.AnimateGraphic(0f, 0.2f);
		icon.AnimateGraphic(0f, 0.2f);
		infoTitle.AnimateGraphic(0f, 0.2f);
		bodyText.AnimateGraphic(0f, 0.2f);
		exitButton.AnimateGraphic(0f, 0.2f, FinishedAnimating);
	}

}
