using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class which animates the WalletListMenu.
/// </summary>
public class WalletListMenuAnimator : UIAnimator
{
	[SerializeField] private GameObject line2;
	[SerializeField] private GameObject newWalletButton;
    [SerializeField] private Scrollbar scrollbar;

    /// <summary>
    /// The wallet gameobjects to animate
    /// </summary>
    public GameObject[] Wallets { get; set; }

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		FinishedAnimating();
		//walletList.AnimateGraphicAndScale(1f, 1f, 0.2f, () => AnimateWallets(0));
		//newWalletButton.AnimateGraphicAndScale(1f, 1f, 0.25f);
	}

	/// <summary>
	/// Resets the unique elements of the form back to the starting positions
	/// </summary>
	protected override void ResetElementValues()
	{
		FinishedAnimating();

		//walletList.SetGraphicAndScale(Vector2.zero);
		//newWalletButton.SetGraphicAndScale(Vector2.zero);
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
