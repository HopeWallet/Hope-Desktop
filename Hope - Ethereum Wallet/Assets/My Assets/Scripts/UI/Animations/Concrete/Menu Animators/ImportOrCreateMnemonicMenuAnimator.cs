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

	/// <summary>
	/// Resets the unique elements of the form back to the starting positions
	/// </summary>
	protected override void ResetElementValues()
	{
		FinishedAnimating();

		importWalletButton.SetGraphicAndScale(Vector2.zero);
		importWalletDesc.SetScale(new Vector2(0f, 1f));
		createWalletButton.SetGraphicAndScale(Vector2.zero);
		createWalletDesc.SetScale(new Vector2(0f, 1f));
	}
}
