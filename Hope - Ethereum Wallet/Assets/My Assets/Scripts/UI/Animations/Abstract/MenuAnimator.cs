using UnityEngine;

/// <summary>
/// The class that manages the general menu animations
/// </summary>
public class MenuAnimator : UIAnimator
{
	[SerializeField] private GameObject line;

	/// <summary>
	/// Animates the elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		if (formTitle != null)
		{
			formTitle.AnimateGraphicAndScale(1f, 1f, 0.2f);
			line.AnimateScaleX(1f, 0.25f);
		}
	}

	/// <summary>
	/// Animates the elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		if (formTitle != null)
		{
			formTitle.AnimateGraphicAndScale(0f, 0f, 0.25f);
			line.AnimateScaleX(0f, 0.25f);
		}
	}
}
