using UnityEngine;

/// <summary>
/// The animator class of the WalletCreatedMenu
/// </summary>
public sealed class WalletCreatedMenuAnimator : MenuAnimator
{
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
		FinishedAnimating();
	}
}
