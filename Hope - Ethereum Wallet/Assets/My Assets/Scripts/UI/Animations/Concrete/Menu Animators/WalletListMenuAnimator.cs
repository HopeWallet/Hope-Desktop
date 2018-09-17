﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The animator class of the WalletListMenu
/// </summary>
public class WalletListMenuAnimator : MenuAnimator
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
	protected override void AnimateIn()
	{
		base.AnimateIn();

		AnimateWallets(0);
		line2.AnimateScaleX(1f, 0.25f);
		newWalletButton.AnimateGraphicAndScale(1f, 1f, 0.3f);
	}

	/// <summary>
	/// Animates the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		base.AnimateOut();

		foreach (GameObject wallet in Wallets)
			wallet.AnimateGraphicAndScale(0f, 0f, 0.3f);

		line2.AnimateScaleX(0f, 0.3f);
		newWalletButton.AnimateGraphicAndScale(0f, 0f, 0.3f, FinishedAnimating);
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
			{
				Wallets[i].transform.localScale = new Vector2(1f, 1f);
				Wallets[i].GetComponent<TextMeshProUGUI>().color = UIColors.White;
			}

			FinishedAnimating();
		}
		if (index == (Wallets.Length - 1))
		{
			Wallets[index].AnimateGraphicAndScale(1f, 1f, 0.2f, FinishedAnimating);
		}
		else
		{
			Wallets[index].AnimateGraphicAndScale(1f, 1f, 0.2f, () => AnimateWallets(++index));
		}
	}
}
