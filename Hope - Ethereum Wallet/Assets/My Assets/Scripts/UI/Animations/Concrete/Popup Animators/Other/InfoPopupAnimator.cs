using UnityEngine;

public class InfoPopupAnimator : UIAnimator
{

	[SerializeField] private GameObject background;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject bodyText;
	[SerializeField] private GameObject infoIcon;
	[SerializeField] private GameObject errorIcon;


	private void Awake() => InfoMessage.OnHoverChanged += AnimateInfoPopup;

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		infoIcon.AnimateGraphicAndScale(1f, 1f, 0.1f);
		errorIcon.AnimateGraphicAndScale(1f, 1f, 0.1f);

		background.AnimateGraphicAndScale(1f, 1f, 0.1f);
		title.AnimateGraphicAndScale(0.85f, 1f, 0.1f);
		bodyText.AnimateGraphicAndScale(0.65f, 1f, 0.1f, FinishedAnimating);
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		infoIcon.AnimateGraphicAndScale(0f, 0f, 0.15f);
		errorIcon.AnimateGraphicAndScale(0f, 0f, 0.15f);
		bodyText.AnimateGraphicAndScale(0f, 0f, 0.15f);
		title.AnimateGraphicAndScale(0f, 0f, 0.15f);
		background.AnimateGraphicAndScale(0f, 0f, 0.15f, FinishedAnimating);
	}

	private void AnimateInfoPopup(bool animateIn)
	{
		if (animateIn)
			AnimateEnable();
		else
			AnimateDisable();
	}
}
