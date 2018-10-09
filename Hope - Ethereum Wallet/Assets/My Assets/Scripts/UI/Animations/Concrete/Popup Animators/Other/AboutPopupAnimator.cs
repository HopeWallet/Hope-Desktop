using UnityEngine;

/// <summary>
/// Animator class of the AboutPopup
/// </summary>
public sealed class AboutPopupAnimator : PopupAnimator
{
	[SerializeField] private GameObject hopeVersionSection;
	[SerializeField] private GameObject websiteButton;
	[SerializeField] private GameObject githubButton;
	[SerializeField] private GameObject redditButton;
	[SerializeField] private GameObject discordButton;

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		hopeVersionSection.AnimateScale(1f, 0.2f);
		websiteButton.AnimateScale(1f, 0.225f);
		githubButton.AnimateScale(1f, 0.25f);
		redditButton.AnimateScale(1f, 0.275f);
		discordButton.AnimateScale(1f, 0.3f, FinishedAnimating);
	}
}
