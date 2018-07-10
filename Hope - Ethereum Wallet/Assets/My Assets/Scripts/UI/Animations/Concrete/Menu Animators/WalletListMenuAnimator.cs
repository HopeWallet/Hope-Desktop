using UnityEngine;

/// <summary>
/// Class which animates the WalletListMenu.
/// </summary>
public class WalletListMenuAnimator : UIAnimator
{

	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject walletList;
	[SerializeField] private GameObject newWalletButton;

    /// <summary>
    /// The wallet gameobjects to animate.
    /// </summary>
    public GameObject[] Wallets { get; set; }

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		form.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => title.AnimateGraphicAndScale(0.85f, 1f, 0.2f,
			() => newWalletButton.AnimateGraphicAndScale(1f, 1f, 0.2f)));

		walletList.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => AnimateWallets(0));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		newWalletButton.AnimateGraphicAndScale(0f, 0f, 0.2f);
		walletList.AnimateGraphicAndScale(0f, 0f, 0.2f);

		for (int i = 0; i < Wallets.Length; i++)
            Wallets[i].AnimateScaleX(0, 0.2f);

		title.AnimateGraphicAndScale(0f, 0f, 0.2f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.2f, FinishedAnimating));
	}

	/// <summary>
	/// Loops through the amount of saved wallets and animates them one by one
	/// </summary>
	/// <param name="index"> The wallet number in the array </param>
	private void AnimateWallets(int index)
	{
		if (index == (Wallets.Length - 1))
            Wallets[index].AnimateScaleX(1f, 0.15f, FinishedAnimating);
		else
            Wallets[index].AnimateScaleX(1f, 0.15f, () => AnimateWallets(++index));
	}

}
