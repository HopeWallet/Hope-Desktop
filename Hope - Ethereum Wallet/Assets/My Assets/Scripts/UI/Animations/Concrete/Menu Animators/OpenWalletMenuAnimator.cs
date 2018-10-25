using UnityEngine;
using Zenject;

/// <summary>
/// The animator class of the OpenWalletMenu
/// </summary>
public sealed class OpenWalletMenuAnimator : MenuAnimator
{
	[SerializeField] private GameObject sideBar1;
	[SerializeField] private GameObject sideBar2;
	[SerializeField] private GameObject bottomBar;
	[SerializeField] private GameObject topBar;

	[SerializeField] private GameObject walletNameText;
	[SerializeField] private GameObject walletAccountText;
	[SerializeField] private GameObject walletLine;
	[SerializeField] private GameObject assetImage;
	[SerializeField] private GameObject currentAssetName;
	[SerializeField] private GameObject currentAssetBalance;
	[SerializeField] private GameObject currentTokenNetWorth;

	[SerializeField] private Transform assetListTransform;
	[SerializeField] private Transform transactionListTransform;
	[SerializeField] private Transform transactionTabsTransform;

	[Inject]
	public void Construct(EthereumTransactionButtonManager ethereumTransactionButtonManager)
	{
		ethereumTransactionButtonManager.transactionButtonsAdded += () => AnimateList(transactionListTransform, 0, 4, true);
	}

	/// <summary>
	/// Waits a small amount of time and then animates the elements in
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		CoroutineUtils.ExecuteAfterWait(0.2f, Animate);
	}

	/// <summary>
	/// Animates all the unique elements of this menu into view
	/// </summary>
	private void Animate()
	{
		topBar.AnimateGraphic(1f, 0.2f, () => AnimateList(topBar.transform, 0, 6, true));
		sideBar1.AnimateGraphic(1f, 0.2f, () => sideBar1.transform.GetChild(0).gameObject.AnimateScale(1f, 0.2f));
		sideBar2.AnimateGraphic(1f, 0.2f, () => AnimateList(assetListTransform, 0, 7, true));
		bottomBar.AnimateGraphic(1f, 0.2f, () => AnimateList(transactionTabsTransform, 0, 3, true));

		walletNameText.AnimateGraphicAndScale(1f, 1f, 0.2f);
		walletAccountText.AnimateGraphicAndScale(1f, 1f, 0.25f);
		walletLine.AnimateScaleX(1f, 0.3f);
		assetImage.AnimateGraphicAndScale(1f, 1f, 0.35f);
		currentAssetName.AnimateGraphicAndScale(1f, 1f, 0.4f);
		currentAssetBalance.AnimateGraphicAndScale(1f, 1f, 0.45f);
		currentTokenNetWorth.AnimateGraphicAndScale(1f, 1f, 0.5f, FinishedAnimating);
	}

	/// <summary>
	/// Animates a given list in the open wallet menu
	/// </summary>
	/// <param name="listTransform"> The list parent transform </param>
	/// <param name="index"> The current index of the child being animated in the hiearchy </param>
	/// <param name="maxAnimationLimit"> The max index to animate to before setting all of the rest of the child object scales </param>
	/// <param name="animateIn"> Whether the child objects should be animated in or out </param>
	private void AnimateList(Transform listTransform, int index, int maxAnimationLimit, bool animateIn)
	{
		if (index == maxAnimationLimit)
		{
			for (int i = index; i < listTransform.childCount; i++)
				listTransform.GetChild(i).transform.localScale = new Vector2(1f, 1f);
		}
		else
		{
			listTransform.GetChild(index)?.gameObject.AnimateScale(1f, 0.15f, () => AnimateList(listTransform, ++index, maxAnimationLimit, animateIn));
		}
	}

	/// <summary>
	/// Animate the unique elements of the form out of view
	/// </summary>
	protected override void AnimateUniqueElementsOut()
	{
		sideBar1.transform.GetChild(0).gameObject.AnimateScale(0f, 0.15f);
		AnimateList(topBar.transform, 0, 6, false);
		AnimateList(assetListTransform, 0, 7, false);
		AnimateList(transactionListTransform, 0, 4, false);
		AnimateList(transactionTabsTransform, 0, 3, false);

		topBar.AnimateGraphic(0f, 0.5f);
		sideBar1.AnimateGraphic(0f, 0.5f);
		sideBar2.AnimateGraphic(0f, 0.5f);
		bottomBar.AnimateGraphic(0f, 0.5f);

		walletNameText.AnimateGraphicAndScale(0f, 0f, 0.5f);
		walletAccountText.AnimateGraphicAndScale(0f, 0f, 0.5f);
		walletLine.AnimateScaleX(0f, 0.5f);
		assetImage.AnimateGraphicAndScale(0f, 0f, 0.5f);
		currentAssetName.AnimateGraphicAndScale(0f, 0f, 0.5f);
		currentAssetBalance.AnimateGraphicAndScale(0f, 0f, 0.5f);
		currentTokenNetWorth.AnimateGraphicAndScale(0f, 0f, 0.5f, FinishedAnimating);
	}
}
