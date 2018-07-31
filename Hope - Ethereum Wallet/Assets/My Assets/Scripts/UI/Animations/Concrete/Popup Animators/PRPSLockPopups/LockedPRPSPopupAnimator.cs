using System;
using UnityEngine;
using UnityEngine.UI;

public class LockedPRPSPopupAnimator : UIAnimator
{
	[SerializeField] private Image blur;
	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
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
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		blur.AnimateMaterialBlur(0.75f, 0.15f);
		dim.AnimateGraphic(1f, 0.2f);
		form.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => title.AnimateScaleX(1f, 0.15f,
			() => topText.AnimateScaleX(1f, 0.15f,
			() => { lockedPRPSList.AnimateScaleX(1f, 0.15f, () => AnimateList(0)); lockPRPSButton.AnimateGraphicAndScale(1f, 1f, 0.15f); })));

		if (pending) SetPendingIcon(true);
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		for (int i = 0; i < listTransform.childCount; i++)
			listTransform.GetChild(i).gameObject.AnimateScaleX(0f, 0.15f);

		lockPRPSButton.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => lockedPRPSList.AnimateScaleX(0f, 0.15f,
			() => topText.AnimateScaleX(0f, 0.15f,
			() => title.AnimateScaleX(0f, 0.15f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => { blur.AnimateMaterialBlur(-0.75f, 0.2f); dim.AnimateGraphic(0f, 0.2f, FinishedAnimating); })))));

		if (pending) SetPendingIcon(false);
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
		if (index == 6)
		{
			for (int i = index; i < listTransform.childCount; i++)
				listTransform.GetChild(i).gameObject.transform.localScale = new Vector2(1f, 1f);

			FinishedAnimating();
		}
		else if (index == listTransform.childCount - 1)
			listTransform.GetChild(index).gameObject.AnimateScaleX(1f, 0.15f, FinishedAnimating);
		else
			listTransform.GetChild(index).gameObject.AnimateScaleX(1f, 0.15f, () => AnimateList(++index));
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
