using UnityEngine;

public class WalletsMenu : MenuAnimation
{

	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject walletList;
	[SerializeField] private GameObject[] wallets;
	[SerializeField] private GameObject newWalletButton;

	private void Start()
	{
		Transform walletListTransform = walletList.transform.GetChild(0).GetChild(0);
		wallets = new GameObject[walletListTransform.childCount];

		for (int i = 0; i < wallets.Length; i++)
			wallets[i] = walletListTransform.GetChild(i).GetChild(0).gameObject;
	}

	#region Animating

	/// <summary>
	/// Animates the form's UI Elements into display
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
	/// Animates the form's UI Elements out of display
	/// </summary>
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
	/// <param name="i"></param>
	private void AnimateWallets(int i)
	{
		if (i == (wallets.Length - 1))
			wallets[i].AnimateScaleX(1f, 0.15f,
				() => FinishedAnimatingIn());

		else
			wallets[i].AnimateScaleX(1f, 0.15f, () => AnimateWallets(++i));
	}

	#endregion

	#region ButtonClicks

	/// <summary>
	/// Disables menu and opens up the wallet number that has been clicked
	/// </summary>
	/// <param name="walletNum">The wallet number in the list of wallets</param>
	public void SavedWalletClicked(int walletNum)
	{
		DisableMenu();
		//Open up the saved wallet by the designated number in the list
	}

	/// <summary>
	/// Disables menu and opens up the create wallet menu
	/// </summary>
	public void NewWalletClicked()
	{
		DisableMenu();
		//Open up create wallet menu
	}

	/// <summary>
	/// Disables menu and opens up the previous ChooseWallet menu
	/// </summary>
	public void BackButtonClicked()
	{
		DisableMenu();
		//Add the ChooseWallet menu to the canvas
	}

	#endregion

}
