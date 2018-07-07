using UnityEngine;
using UnityEngine.UI;

public class WalletListForm : MenuAnimator
{

	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject walletList;
	[SerializeField] private GameObject[] wallets;
	[SerializeField] private GameObject newWalletButton;
	[SerializeField] private GameObject signInForm;

	/// <summary>
	/// Initializes the necessary variables that haven't already been initialized in the inspector
	/// </summary>
	private void Awake()
	{
		Transform walletListTransform = walletList.transform.GetChild(0).GetChild(0);
		wallets = new GameObject[walletListTransform.childCount];

		for (int i = 0; i < wallets.Length; i++)
		{
			wallets[i] = walletListTransform.GetChild(i).GetChild(0).gameObject;
			walletListTransform.GetChild(i).GetComponent<Button>().onClick.AddListener(OpenExistingWallet);
		}
	}

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

		for (int i = 0; i < wallets.Length; i++)
			wallets[i].AnimateScaleX(0, 0.2f);

		title.AnimateGraphicAndScale(0f, 0f, 0.2f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.2f, FinishedAnimating));
	}

	/// <summary>
	/// Loops through the amount of saved wallets and animates them one by one
	/// </summary>
	/// <param name="index"> The wallet number in the array </param>
	private void AnimateWallets(int index)
	{
		if (index == (wallets.Length - 1))
			wallets[index].AnimateScaleX(1f, 0.15f, FinishedAnimating);
		else
			wallets[index].AnimateScaleX(1f, 0.15f, () => AnimateWallets(++index));
	}

	/// <summary>
	/// Opens up the sign in form to enter the password for the saved wallet
	/// </summary>
	/// <param name="walletNum"> The number of the wallet being opened in the hierarchy </param>
	private void OpenExistingWallet() => signInForm.SetActive(true);

}
