using UnityEngine;

/// <summary>
/// The animator class of the WalletCreatedMenu
/// </summary>
public sealed class WalletCreatedMenuAnimator : MenuAnimator
{
	[SerializeField] private GameObject hopeLogo;
	[SerializeField] private GameObject noteText;
	[SerializeField] private GameObject openWalletButton;

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateIn()
	{
		base.AnimateIn();

		hopeLogo.AnimateTransformY(31f, 0.2f);
		hopeLogo.AnimateScale(2f, 0.2f);
		noteText.AnimateGraphicAndScale(1f, 1f, 0.3f);
		openWalletButton.AnimateGraphicAndScale(1f, 1f, 0.4f, FinishedAnimating);
	}

	/// <summary>
	/// Animates the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		base.AnimateOut();

		hopeLogo.AnimateGraphicAndScale(0f, 0f, 0.2f);
		noteText.AnimateGraphicAndScale(0f, 0f, 0.3f);
		openWalletButton.AnimateGraphicAndScale(0f, 0f, 0.4f, FinishedAnimating);
	}
}
