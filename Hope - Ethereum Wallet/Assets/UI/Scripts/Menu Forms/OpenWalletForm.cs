using UnityEngine;
using UnityEngine.UI;

public class OpenWalletForm : MenuAnimator
{

	[SerializeField] private GameObject background;
	[SerializeField] private GameObject tokenList;
	[SerializeField] private GameObject taskBarButtons;
	[SerializeField] private GameObject transactionList;
	[SerializeField] private GameObject tokenIcon;
	[SerializeField] private GameObject tokenName;
	[SerializeField] private GameObject tokenAmount;
	[SerializeField] private GameObject buyButton;
	[SerializeField] private GameObject sellButton;

	protected override void AnimateIn()
	{
		background.AnimateGraphic(1f, 0.15f,
			() => tokenIcon.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => tokenName.AnimateGraphicAndScale(0.85f, 1f, 0.15f,
			() => tokenAmount.AnimateGraphicAndScale(0.65f, 1f, 0.15f,
			() => buyButton.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => sellButton.AnimateGraphicAndScale(1f, 1f, 0.15f, FinishedAnimating))))));

		AnimateList(tokenList.transform.GetChild(0).GetChild(0), 0);
		AnimateList(transactionList.transform.GetChild(0).GetChild(0), 0);
		AnimateTaskbarButtons(0);
	}

	protected override void AnimateOut()
	{
		FinishedAnimating();
	}

	private void AnimateList(Transform listTransform, int index)
	{
		if (index == listTransform.childCount - 1)
			listTransform.GetChild(index).gameObject.AnimateScaleX(1.183325f, 0.15f);
		else
			listTransform.GetChild(index).gameObject.AnimateScaleX(1.183325f, 0.15f, () => AnimateList(listTransform, ++index));
	}

	private void AnimateTaskbarButtons(int index)
	{
		taskBarButtons.transform.GetChild(index).gameObject.AnimateScaleX(1f, 0.15f);
	}
}
