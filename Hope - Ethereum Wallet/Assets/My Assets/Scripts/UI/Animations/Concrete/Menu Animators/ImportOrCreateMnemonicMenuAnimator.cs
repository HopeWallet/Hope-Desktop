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
	protected override void AnimateUniqueElementsIn()
	{
		importWalletButton.AnimateGraphicAndScale(1f, 1f, 0.25f);
		importWalletDesc.AnimateGraphicAndScale(1f, 1f, 0.3f);
		createWalletButton.AnimateGraphicAndScale(1f, 1f, 0.35f);
		createWalletDesc.AnimateGraphicAndScale(1f, 1f, 0.4f, FinishedAnimating);
	}

	/// <summary>
	/// Animate the unique elements of the form out of view
	/// </summary>
	protected override void AnimateUniqueElementsOut()
	{
		importWalletButton.AnimateGraphicAndScale(0f, 0f, 0.3f);
		importWalletDesc.AnimateGraphicAndScale(0f, 0f, 0.3f);
		createWalletButton.AnimateGraphicAndScale(0f, 0f, 0.3f);
		createWalletDesc.AnimateGraphicAndScale(0f, 0f, 0.3f, FinishedAnimating);
	}
}
