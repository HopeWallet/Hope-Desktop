using System;
using UnityEngine;
using Zenject;

/// <summary>
/// The animator class of the OpenWalletMenu
/// </summary>
public sealed class OpenWalletMenuAnimator : MenuAnimator
{
	public event Action animateOut;

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

	[SerializeField] private GameObject loadingStatusText;
	[SerializeField] private GameObject loadingIcon;

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
		loadingStatusText.AnimateGraphic(1f, 0.2f);
		loadingIcon.AnimateGraphicAndScale(1f, 1f, 0.2f);

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
			listTransform.GetChild(index)?.gameObject.AnimateScale(animateIn ? 1f : 0f, animateIn ? 0.15f : 0.05f, () => AnimateList(listTransform, ++index, maxAnimationLimit, animateIn));
		}
	}

	/// <summary>
	/// Animate the unique elements of the form out of view
	/// </summary>
	protected override void AnimateUniqueElementsOut()
	{
		float duration = 1f;

		animateOut.Invoke();

		sideBar1.transform.GetChild(0).gameObject.AnimateScale(0f, 0.05f);
		AnimateList(topBar.transform, 0, 6, false);
		AnimateList(assetListTransform, 0, 7, false);
		AnimateList(transactionListTransform, 0, 4, false);
		AnimateList(transactionTabsTransform, 0, 3, false);

		topBar.AnimateGraphic(0f, duration);
		sideBar1.AnimateGraphic(0f, duration);
		sideBar2.AnimateGraphic(0f, duration);
		bottomBar.AnimateGraphic(0f, duration);

		loadingStatusText.AnimateGraphic(0f, duration);
		loadingIcon.AnimateGraphicAndScale(0f, 0f, duration);

		walletNameText.AnimateGraphicAndScale(0f, 0f, duration);
		walletAccountText.AnimateGraphicAndScale(0f, 0f, duration);
		walletLine.AnimateScaleX(0f, duration);
		assetImage.AnimateGraphicAndScale(0f, 0f, duration);
		currentAssetName.AnimateGraphicAndScale(0f, 0f, duration);
		currentAssetBalance.AnimateGraphicAndScale(0f, 0f, duration);
		currentTokenNetWorth.AnimateGraphicAndScale(0f, 0f, duration, FinishedAnimating);
	}
}
