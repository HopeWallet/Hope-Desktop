using UnityEngine;

public class OpenWalletMenuAnimator : UIAnimator
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

	protected override void AnimateUniqueElementsIn()
	{
		FinishedAnimating();
	}

	protected override void ResetElementValues()
	{
		FinishedAnimating();
	}

	/// <summary>
	/// Animates a given list of objects one by one
	/// </summary>
	/// <param name="objectTransform"> The parent object of the entire list of objects </param>
	/// <param name="index"> The index of object in the list </param>
	private void AnimateList(Transform objectTransform, int index)
	{
		if (index == objectTransform.childCount - 1)
			objectTransform.GetChild(index).gameObject.AnimateScaleX(1f, 0.15f);
		else
			objectTransform.GetChild(index).gameObject.AnimateScaleX(1f, 0.15f, () => AnimateList(objectTransform, ++index));
	}
}
