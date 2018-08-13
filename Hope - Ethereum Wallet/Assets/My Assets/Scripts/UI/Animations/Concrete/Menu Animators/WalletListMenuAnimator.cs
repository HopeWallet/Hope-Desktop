using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class which animates the WalletListMenu.
/// </summary>
public class WalletListMenuAnimator : UIAnimator
{
	[SerializeField] private GameObject walletList;
	[SerializeField] private GameObject newWalletButton;
    [SerializeField] private Scrollbar scrollbar;

    /// <summary>
    /// The wallet gameobjects to animate.
    /// </summary>
    public GameObject[] Wallets { get; set; }

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		walletList.AnimateGraphicAndScale(1f, 1f, 0.2f, () => AnimateWallets(0));
		newWalletButton.AnimateGraphicAndScale(1f, 1f, 0.25f);
	}

	/// <summary>
	/// Animates the unique elements of this form out of view
	/// </summary>
	protected override void AnimateUniqueElementsOut()
	{
		newWalletButton.AnimateGraphicAndScale(0f, 0f, 0.2f, () => AnimateBasicElements(false));

		for (int i = 0; i < Wallets.Length; i++)
			Wallets[i].AnimateScaleX(0, 0.25f);

		walletList.AnimateGraphicAndScale(0f, 0f, 0.3f);
	}

	/// <summary>
	/// Loops through the amount of saved wallets and animates them one by one
	/// </summary>
	/// <param name="index"> The wallet number in the array </param>
	private void AnimateWallets(int index)
	{
        scrollbar.value = 1f;

		if (index == 4)
		{
			for (int i = index; i < Wallets.Length; i++)
				Wallets[i].transform.localScale = new Vector2(1f, 1f);

			FinishedAnimating();
		}
		if (index == (Wallets.Length - 1))
            Wallets[index].AnimateScaleX(1f, 0.1f, FinishedAnimating);
		else
            Wallets[index].AnimateScaleX(1f, 0.1f, () => AnimateWallets(++index));
	}
}
