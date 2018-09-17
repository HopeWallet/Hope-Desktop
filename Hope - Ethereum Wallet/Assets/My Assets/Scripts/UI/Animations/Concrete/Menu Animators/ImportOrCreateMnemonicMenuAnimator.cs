using UnityEngine;

/// <summary>
/// The animator class of the ImportOrCreateMnemonicMenu
/// </summary>
public sealed class ImportOrCreateMnemonicMenuAnimator : MenuAnimator
{
	[SerializeField] private GameObject importWalletButton;
	[SerializeField] private GameObject importWalletDesc;
	[SerializeField] private GameObject createWalletButton;
	[SerializeField] private GameObject createWalletDesc;

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
