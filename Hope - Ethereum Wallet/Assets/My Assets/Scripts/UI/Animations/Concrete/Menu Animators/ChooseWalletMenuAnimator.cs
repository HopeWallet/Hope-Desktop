using UnityEngine;

/// <summary>
/// The animator class of the ChooseWalletMenu
/// </summary>
public sealed class ChooseWalletMenuAnimator : MenuAnimator
{
    [SerializeField] private GameObject ledgerButton;
	[SerializeField] private GameObject trezorButton;
	[SerializeField] private GameObject hopeButton;
	[SerializeField] private GameObject networkTypeText;
	[SerializeField] private GameObject networkTypeDropdown;

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		networkTypeText.AnimateTransformY(377.2f, 0.3f);
		networkTypeDropdown.AnimateTransformY(377f, 0.3f);
		ledgerButton.AnimateGraphicAndScale(1f, 1f, 0.25f);
		trezorButton.AnimateGraphicAndScale(1f, 1f, 0.3f);
		hopeButton.AnimateGraphicAndScale(1f, 1f, 0.35f, FinishedAnimating);
	}

	/// <summary>
	/// Animate the unique elements of the form out of view
	/// </summary>
	protected override void AnimateUniqueElementsOut()
	{
		networkTypeText.AnimateTransformY(418.3f, 0.3f);
		networkTypeDropdown.AnimateTransformY(418.1f, 0.3f);
		ledgerButton.AnimateGraphicAndScale(0f, 0f, 0.3f);
		trezorButton.AnimateGraphicAndScale(0f, 0f, 0.3f);
		hopeButton.AnimateGraphicAndScale(0f, 0f, 0.3f, FinishedAnimating);
	}
}
