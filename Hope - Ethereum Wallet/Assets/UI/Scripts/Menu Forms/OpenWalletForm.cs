using UnityEngine;
using UnityEngine.UI;

public class OpenWalletForm : UIAnimator
{

	[SerializeField] private GameObject background;
	[SerializeField] private GameObject assetList;
	[SerializeField] private GameObject taskBarButtons;
	[SerializeField] private GameObject transactionList;
	[SerializeField] private GameObject assetIcon;
	[SerializeField] private GameObject assetName;
	[SerializeField] private GameObject assetAmount;
	[SerializeField] private GameObject buyButton;
	[SerializeField] private GameObject sellButton;

	protected override void AnimateIn()
	{
		background.AnimateGraphic(1f, 0.15f,
			() => assetIcon.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => assetName.AnimateGraphicAndScale(0.85f, 1f, 0.15f,
			() => assetAmount.AnimateGraphicAndScale(0.65f, 1f, 0.15f,
			() => buyButton.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => sellButton.AnimateGraphicAndScale(1f, 1f, 0.15f, FinishedAnimating))))));

		AnimateList(assetList.transform.GetChild(0).GetChild(0), 0, true);
		AnimateList(transactionList.transform.GetChild(0).GetChild(0), 0, true);
		AnimateList(taskBarButtons.transform, 0, false);
	}

	protected override void AnimateOut()
	{
		FinishedAnimating();
	}

	private void AnimateList(Transform objectTransform, int index, bool isList)
	{
		if (index == objectTransform.childCount - 1)
			objectTransform.GetChild(index).gameObject.AnimateScaleX(isList ? 1.183325f : 1f, 0.15f);
		else
			objectTransform.GetChild(index).gameObject.AnimateScaleX(isList ? 1.183325f : 1f, 0.15f, () => AnimateList(objectTransform, ++index, isList));
	}
}
