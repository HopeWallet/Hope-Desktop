using UnityEngine;
using UnityEngine.UI;

public class ModifyTokenListPopupAnimator : UIAnimator
{

	[SerializeField] private Image blur;
	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject tokenList;
	[SerializeField] private GameObject customTokenButton;
	[SerializeField] private GameObject searchSection;

	private Transform tokenTransform;

	/// <summary>
	/// Initializes the button listeners
	/// </summary>
	private void Awake() => tokenTransform = tokenList.transform.GetChild(0).GetChild(0);

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		blur.AnimateMaterialBlur(1f, 0.15f);
		dim.AnimateGraphic(1f, 0.15f);
		form.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => title.AnimateGraphicAndScale(0.85f, 1f, 0.15f,
			() => searchSection.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => tokenList.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => { AnimateTokens(0); customTokenButton.AnimateScaleX(1f, 0.15f); }))));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		for (int i = 0; i < tokenTransform.childCount; i++)
			tokenTransform.GetChild(i).gameObject.AnimateScaleX(0f, 0.15f);

		customTokenButton.AnimateScaleX(0f, 0.15f,
			() => tokenList.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => searchSection.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => title.AnimateGraphicAndScale(0f, 0f, 0.15f, 
			() => { form.AnimateGraphicAndScale(0f, 0f, 0.15f); blur.AnimateMaterialBlur(-1f, 0.15f); dim.AnimateGraphic(0f, 0.15f, FinishedAnimating); }))));
	}

	/// <summary>
	/// Animates the tokens one by one
	/// </summary>
	/// <param name="index"> The token index being animated </param>
	private void AnimateTokens(int index)
	{
		if (tokenTransform.childCount == 0)
		{
			FinishedAnimating();
			return;
		}

		if (index == 6)
		{
			for (int i = index; i < tokenTransform.childCount; i++)
				tokenTransform.GetChild(i).gameObject.transform.localScale = new Vector2(1f, 1f);

			FinishedAnimating();
		}
		else if (index == tokenTransform.childCount - 1)
			tokenTransform.GetChild(index).gameObject.AnimateScaleX(1f, 0.15f, FinishedAnimating);
		else
			tokenTransform.GetChild(index).gameObject.AnimateScaleX(1f, 0.15f, () => AnimateTokens(++index));
	}
}
