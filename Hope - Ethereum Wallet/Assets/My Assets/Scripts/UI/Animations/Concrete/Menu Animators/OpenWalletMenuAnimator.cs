using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// The animator class of the OpenWalletMenu
/// </summary>
public sealed class OpenWalletMenuAnimator : MenuAnimator
{
	public event Action ResetGameObjects;

	[SerializeField] private GameObject addTokenButton;

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
	[SerializeField] private GameObject transactionPagesSection;

	[SerializeField] private GameObject[] walletLogos;

	/// <summary>
	/// Assigns the necessary event to the animation
	/// </summary>
	/// <param name="ethereumTransactionButtonManager"> The active EthereumTransactionButtonManager </param>
	[Inject]
	public void Construct(EthereumTransactionButtonManager ethereumTransactionButtonManager)
	{
		ethereumTransactionButtonManager.OnTransactionListCreated += () => AnimateListIn(transactionListTransform, 0, 4);
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
		transactionPagesSection.AnimateScale(1f, 0.2f);

		addTokenButton.AnimateScale(1f, 0.2f);

		topBar.AnimateGraphic(1f, 0.2f, () => AnimateListIn(topBar.transform, 0, 6));
		sideBar1.AnimateGraphic(1f, 0.2f, () => sideBar1.transform.GetChild(0).gameObject.AnimateScale(1f, 0.2f));
		sideBar2.AnimateGraphic(1f, 0.2f, () => AnimateListIn(assetListTransform, 0, 7));
		bottomBar.AnimateGraphic(1f, 0.2f, () => AnimateListIn(transactionTabsTransform, 0, 3));

		walletNameText.AnimateGraphicAndScale(1f, 1f, 0.2f);
		walletAccountText.AnimateGraphicAndScale(1f, 1f, 0.25f);
		walletLine.AnimateScaleX(1f, 0.3f);
		assetImage.AnimateGraphicAndScale(1f, 1f, 0.35f);
		currentAssetName.AnimateGraphicAndScale(1f, 1f, 0.4f);
		currentAssetBalance.AnimateGraphicAndScale(1f, 1f, 0.45f);
		currentTokenNetWorth.AnimateGraphicAndScale(1f, 1f, 0.5f, FinishedAnimating);
	}

	/// <summary>
	/// Animate the unique elements of the form out of view
	/// </summary>
	protected override void AnimateUniqueElementsOut()
	{
		ResetGameObjects.Invoke();

		addTokenButton.transform.localScale = new Vector2(0f, 0f);

		sideBar1.transform.GetChild(0).gameObject.AnimateScale(0f, 0.05f);
		AnimateListOut(topBar.transform);
		AnimateListOut(assetListTransform);
		AnimateListOut(transactionListTransform);
		AnimateListOut(transactionTabsTransform);

		foreach (GameObject logo in walletLogos)
		{
			if (logo.activeInHierarchy)
			{
				logo.GetComponent<LoadingIconAnimator>().enabled = false;
				logo.AnimateColor(new Color(1f, 1f, 1f, 1f), 1.25f);
			}
		}

		topBar.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
		sideBar1.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
		sideBar2.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
		bottomBar.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);

		loadingStatusText.GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, 0f);
		ResetObjectValues(loadingIcon, false);
		transactionPagesSection.transform.localScale = new Vector2(0f, 0f);

		ResetObjectValues(walletNameText, true);
		ResetObjectValues(walletAccountText, true);
		ResetObjectValues(walletLine, true);
		ResetObjectValues(assetImage, true);
		ResetObjectValues(currentAssetName, true);
		ResetObjectValues(currentAssetBalance, true);
		ResetObjectValues(currentTokenNetWorth, true);

		FinishedAnimating();
	}

	/// <summary>
	/// Animates a given list in the open wallet menu
	/// </summary>
	/// <param name="listTransform"> The list parent transform </param>
	/// <param name="index"> The current index of the child being animated in the hiearchy </param>
	/// <param name="maxAnimationLimit"> The max index to animate to before setting all of the rest of the child object scales </param>
	private void AnimateListIn(Transform listTransform, int index, int maxAnimationLimit)
	{
		if (listTransform.childCount == 0)
			return;

		if (index == maxAnimationLimit)
		{
			for (int i = index; i < listTransform.childCount; i++)
				listTransform.GetChild(i).transform.localScale = new Vector2(1f, 1f);
		}
		else
		{
			listTransform.GetChild(index)?.gameObject.AnimateScale(1f, 0.15f, () => AnimateListIn(listTransform, ++index, maxAnimationLimit));
		}
	}

	/// <summary>
	/// Animates a list out of view
	/// </summary>
	/// <param name="transform"> The parent transform of the list </param>
	private void AnimateListOut(Transform transform)
	{
		for (int i = 0; i < transform.childCount; i++)
			transform.GetChild(i).transform.localScale = new Vector2(0f, 0f);
	}

	/// <summary>
	/// Resets the object's values to the original state
	/// </summary>
	/// <param name="gameObject"> The GameObject being reset </param>
	/// <param name="resetColor"> If the color should be reset as well </param>
	private void ResetObjectValues(GameObject gameObject, bool resetColor)
	{
		gameObject.transform.localScale = new Vector2(0f, 0f);

		if (resetColor)
		{
			Graphic graphicComponent = gameObject.GetComponent<Graphic>();
			graphicComponent.color = new Color(graphicComponent.color.r, graphicComponent.color.g, graphicComponent.color.g, 0f);
		}
	}
}
