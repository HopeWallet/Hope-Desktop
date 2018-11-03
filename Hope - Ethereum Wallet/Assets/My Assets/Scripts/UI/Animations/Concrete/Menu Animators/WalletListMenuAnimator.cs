using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The animator class of the WalletListMenu
/// </summary>
public class WalletListMenuAnimator : MenuAnimator
{
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject hopeLogo;
    [SerializeField] private GameObject line2;
    [SerializeField] private GameObject newWalletButton;
    [SerializeField] private Scrollbar scrollbar;

    private WalletListMenu walletListMenu;

    private void Awake()
    {
        walletListMenu = GetComponent<WalletListMenu>();
        backButton.GetComponent<Button>().onClick.AddListener(() => { backButton.AnimateGraphicAndScale(0f, 0f, 0.3f); hopeLogo.AnimateGraphicAndScale(0f, 0f, 0.3f); });
    }

    /// <summary>
    /// Animates the unique elements of this form into view
    /// </summary>
    protected override void AnimateUniqueElementsIn()
    {
		backButton.AnimateGraphicAndScale(1f, 1f, 0.2f);
		hopeLogo.AnimateGraphicAndScale(1f, 1f, 0.2f);

		CoroutineUtils.ExecuteAfterWait(0.1f, () => { scrollbar.value = 1f; AnimateWallets(0); });

		line2.AnimateScaleX(1f, 0.35f);
        newWalletButton.AnimateGraphicAndScale(1f, 1f, 0.4f);
    }

	/// <summary>
	/// Animate the unique elements of the form out of view
	/// </summary>
	protected override void AnimateUniqueElementsOut()
    {
		foreach (GameObject wallet in walletListMenu.Wallets)
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
        if (index == 5)
        {
            for (int i = index; i < walletListMenu.Wallets.Count; i++)
            {
                walletListMenu.Wallets[i].transform.localScale = new Vector2(1f, 1f);
				walletListMenu.Wallets[i].GetComponent<TextMeshProUGUI>().color = new Color(0.85f, 0.85f, 0.85f, 1f);
            }

            FinishedAnimating();
        }
        if (index == (walletListMenu.Wallets.Count - 1))
        {
            walletListMenu.Wallets[index].AnimateGraphicAndScale(1f, 1f, 0.15f, FinishedAnimating);
        }
        else
        {
            walletListMenu.Wallets[index].AnimateGraphicAndScale(1f, 1f, 0.15f, () => AnimateWallets(++index));
        }
    }
}
