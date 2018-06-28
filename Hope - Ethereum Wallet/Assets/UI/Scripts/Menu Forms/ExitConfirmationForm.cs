using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitConfirmationForm : FormAnimation
{

	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject yesButton;
	[SerializeField] private GameObject noButton;

	#region Animating

	protected override void AnimateIn()
	{
		dim.AnimateGraphic(1f, 0.2f);
		form.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => title.AnimateGraphicAndScale(0.85f, 1f, 0.2f));

		noButton.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => yesButton.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => FinishedAnimatingIn()));
	}

	protected override void AnimateOut()
	{
		dim.AnimateGraphic(0, 0.2f);
		title.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.15f));

		noButton.AnimateGraphicAndScale(0f, 0f, 0.15f);
		yesButton.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => FinishedAnimatingOut());
	}

	#endregion

	#region Button Clicks

	public void YesButtonClicked() => Application.Quit();

	public void NoButtonClicked() => DisableMenu();

	#endregion

}
