using UnityEngine;

public class WalletListForm : FormAnimation
{

	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject walletList;
	[SerializeField] private GameObject[] wallets;
	[SerializeField] private GameObject newWalletButton;

	protected override void InitializeElements()
	{
		Transform walletListTransform = walletList.transform.GetChild(0).GetChild(0);
		wallets = new GameObject[walletListTransform.childCount];

		for (int i = 0; i < wallets.Length; i++)
			wallets[i] = walletListTransform.GetChild(i).GetChild(0).gameObject;
	}

	#region Animating

	protected override void AnimateIn()
	{
		form.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => title.AnimateGraphicAndScale(0.85f, 1f, 0.2f,
			() => newWalletButton.AnimateGraphicAndScale(1f, 1f, 0.2f)));

		walletList.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => AnimateWallets(0));
	}

	protected override void AnimateOut()
	{
		newWalletButton.AnimateGraphicAndScale(0f, 0f, 0.2f);
		walletList.AnimateGraphicAndScale(0f, 0f, 0.2f);

		for (int i = 0; i < wallets.Length; i++)
		{
			wallets[i].AnimateScaleX(0, 0.2f);
		}

		title.AnimateGraphicAndScale(0f, 0f, 0.2f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.2f,
			() => FinishedAnimatingOut()));
	}

	/// <summary>
	/// Loops through the amount of saved wallets and animates them one by one
	/// </summary>
	/// <param name="i">The wallet number in the array</param>
	private void AnimateWallets(int i)
	{
		if (i == (wallets.Length - 1))
			wallets[i].AnimateScaleX(1f, 0.15f,
				() => FinishedAnimatingIn());

		else
			wallets[i].AnimateScaleX(1f, 0.15f, () => AnimateWallets(++i));
	}

	#endregion

}
