using UnityEngine;

/// <summary>
/// Class which animates the transitions for the ImportOrCreateMnemonicMenu.
/// </summary>
public class ImportOrCreateMnemonicMenuAnimator : UIAnimator
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
		importWalletButton.AnimateGraphicAndScale(1f, 1f, 0.15f);
		importWalletDesc.AnimateScaleX(1f, 0.2f);
		createWalletButton.AnimateGraphicAndScale(1f, 1f, 0.25f);
		createWalletDesc.AnimateScaleX(1f, 0.3f, FinishedAnimating);
	}
}
