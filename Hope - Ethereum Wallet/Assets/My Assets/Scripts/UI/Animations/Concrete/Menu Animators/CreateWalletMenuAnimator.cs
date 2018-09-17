using UnityEngine;

/// <summary>
/// The animator class of the CreateWalletMenu
/// </summary>
public sealed class CreateWalletMenuAnimator : MenuAnimator
{
	[SerializeField] private GameObject walletNameField;
	[SerializeField] private GameObject password1Field;
	[SerializeField] private GameObject password2Field;
	[SerializeField] private GameObject nextButton;

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateIn()
	{
		base.AnimateIn();

		FinishedAnimating();
	}

	/// <summary>
	/// Animates the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		base.AnimateOut();

		FinishedAnimating();
	}
}
