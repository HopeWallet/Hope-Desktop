using UnityEngine;

/// <summary>
/// The animator class of the ChooseWalletMenu
/// </summary>
public sealed class ChooseWalletMenuAnimator : MenuAnimator
{
    [SerializeField] private GameObject ledgerButton;
	[SerializeField] private GameObject trezorButton;
	[SerializeField] private GameObject hopeButton;

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateIn()
	{
		base.AnimateIn();

		ledgerButton.AnimateGraphicAndScale(1f, 1f, 0.25f);
		trezorButton.AnimateGraphicAndScale(1f, 1f, 0.3f);
		hopeButton.AnimateGraphicAndScale(1f, 1f, 0.35f, FinishedAnimating);
	}

	/// <summary>
	/// Animates the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		base.AnimateOut();

		ledgerButton.AnimateGraphicAndScale(0f, 0f, 0.3f);
		trezorButton.AnimateGraphicAndScale(0f, 0f, 0.3f);
		hopeButton.AnimateGraphicAndScale(0f, 0f, 0.3f, FinishedAnimating);
	}
}
