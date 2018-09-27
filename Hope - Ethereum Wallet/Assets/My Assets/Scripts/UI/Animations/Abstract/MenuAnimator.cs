using UnityEngine;

/// <summary>
/// The class that manages the general menu animations
/// </summary>
public class MenuAnimator : UIAnimator
{
	[SerializeField] private GameObject line;
	[SerializeField] private GameObject menuTooltip;

	/// <summary>
	/// Animates the elements of the form into view
	/// </summary>
	protected override void AnimateIn() => AnimateBasicMenuElements(true);

	/// <summary>
	/// Animates the elements of the form out of view
	/// </summary>
	protected override void AnimateOut() => AnimateBasicMenuElements(false);

	/// <summary>
	/// Animates the form title, line, and menuTip if they are not equal to null
	/// </summary>
	/// <param name="animateIn"> Whether animating the elements into or out of view </param>
	private void AnimateBasicMenuElements(bool animateIn)
	{
		if (formTitle != null)
		{
			formTitle.AnimateGraphicAndScale(animateIn ? 1f : 0f, animateIn ? 1f : 0f, 0.2f);
			line.AnimateScaleX(animateIn ? 1f : 0f, 0.25f);
		}

		if (menuTooltip != null)
		{
			for (int i = 0; i < 3; i++)
				menuTooltip.transform.GetChild(i).gameObject.AnimateGraphic(animateIn ? 1f : 0f, 0.3f);

			if (animateIn)
				menuTooltip.AnimateTransformX(50.2f, 0.3f);
			else
				CoroutineUtils.ExecuteAfterWait(0.25f, () => menuTooltip.transform.localPosition = new Vector2(100f, 0f));
		}
	}
}
