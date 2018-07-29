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

	private Transform listTransform;

	private void Awake()
	{
		listTransform = lockedPRPSList.transform.GetChild(0).GetChild(0);
	}

	protected override void AnimateIn()
	{
		blur.AnimateMaterialBlur(1f, 0.15f);
		dim.AnimateGraphic(1f, 0.2f);
		form.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => title.AnimateScaleX(1f, 0.15f,
			() => topText.AnimateScaleX(1f, 0.15f,
			() => { lockedPRPSList.AnimateScaleX(1f, 0.15f, () => AnimateList(0)); lockPRPSButton.AnimateGraphicAndScale(1f, 1f, 0.15f); })));
	}

	protected override void AnimateOut()
	{
		throw new System.NotImplementedException();
	}

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

}
