using System;
using UnityEngine;
using UnityEngine.UI;

public class LockedPRPSPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject topText;
	[SerializeField] private GameObject lockedPRPSList;
	[SerializeField] private GameObject lockPRPSButton;

	[SerializeField] private GameObject pendingIcon;

	private Transform listTransform;

	private bool pending;

    /// <summary>
    /// Array of initial locked purpose items.
    /// </summary>
    public GameObject[] LockedPurposeItems { get; set; }

	public bool Pending
	{
		set
		{
			pending = value;
			SetPendingIcon(pending);
		}
	}

	/// <summary>
	/// Initializes the listTransform
	/// </summary>
	private void Awake() => listTransform = lockedPRPSList.transform.GetChild(0).GetChild(0);

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		topText.AnimateScaleX(1f, 0.2f);
		lockedPRPSList.AnimateScaleX(1f, 0.25f, () => AnimateList(0));
		lockPRPSButton.AnimateGraphicAndScale(1f, 1f, 0.3f);
	}

	public void AnimateWalletIn(GameObject walletToAnimate)
    {
        walletToAnimate.AnimateScaleX(1f, 0.15f);
    }

    public void AnimateWalletOut(GameObject walletToAnimate, Action onAnimateFinished)
    {
        walletToAnimate.AnimateScaleX(0f, 0.15f, () => onAnimateFinished?.Invoke());
    }

	/// <summary>
	/// Animates the items in the main list one by one
	/// </summary>
	/// <param name="index"> The index of the item being animated </param>
	private void AnimateList(int index)
	{
		if (listTransform.childCount == 0)
		{
			FinishedAnimating();
			return;
		}

		if (index == 6)
		{
			for (int i = index; i < listTransform.childCount; i++)
				listTransform.GetChild(i).gameObject.transform.localScale = new Vector2(1f, 1f);

			FinishedAnimating();
		}
		else if (index == listTransform.childCount - 1)
			listTransform.GetChild(index).gameObject.AnimateScaleX(1f, 0.1f, FinishedAnimating);
		else
			listTransform.GetChild(index).gameObject.AnimateScaleX(1f, 0.1f, () => AnimateList(++index));
	}

	/// <summary>
	/// Animates the pending icon in or out
	/// </summary>
	/// <param name="active"> Checks if animating icon in or out </param>
	private void SetPendingIcon(bool active)
	{
		if (active)
			pendingIcon.SetActive(active);

		pendingIcon.AnimateGraphicAndScale(active ? 1f : 0f, active ? 1f : 0f, 0.2f);

		if (!active)
			pendingIcon.SetActive(false);
	}
}
