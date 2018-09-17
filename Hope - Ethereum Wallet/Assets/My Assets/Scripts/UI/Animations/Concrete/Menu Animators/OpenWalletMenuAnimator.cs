using UnityEngine;

/// <summary>
/// The animator class of the OpenWalletMenu
/// </summary>
public sealed class OpenWalletMenuAnimator : MenuAnimator
{
	[SerializeField] private GameObject background;
	[SerializeField] private GameObject assetList;
	[SerializeField] private GameObject taskBarButtons;
	[SerializeField] private GameObject transactionList;
	[SerializeField] private GameObject assetIcon;
	[SerializeField] private GameObject assetName;
	[SerializeField] private GameObject assetAmount;
	[SerializeField] private GameObject buyButton;
	[SerializeField] private GameObject sellButton;

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
