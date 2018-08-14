using UnityEngine;

/// <summary>
/// Class which animates the ChooseWalletMenu.
/// </summary>
public sealed class ChooseWalletMenuAnimator : UIAnimator
{
    [SerializeField] private GameObject ledgerButton;
    [SerializeField] private GameObject hopeButton;

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		ledgerButton.AnimateGraphicAndScale(1f, 1f, 0.2f);
		hopeButton.AnimateGraphicAndScale(1f, 1f, 0.2f, FinishedAnimating);
	}

	/// <summary>
	/// Resets the unique elements of the form back to the starting positions
	/// </summary>
	protected override void ResetElementValues()
	{
		FinishedAnimating();

		hopeButton.SetScale(Vector2.zero);
		ledgerButton.SetScale(Vector2.zero);
	}
}
