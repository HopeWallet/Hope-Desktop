using UnityEngine;

public sealed class ModifyTokenListPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject tokenList;
	[SerializeField] private GameObject customTokenButton;

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		dim.AnimateGraphic(1f, 0.2f);
		form.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => title.AnimateGraphicAndScale(0.85f, 1f, 0.2f,
			() => AnimateTokenListIn(0)));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		AnimateTokenListOut(tokenList.transform.GetChild(0).GetChild(0).childCount - 1);

		customTokenButton.AnimateScaleX(0f, 0.15f,
			() => title.AnimateGraphicAndScale(0f, 0f, 0.15f));
	}

	/// <summary>
	/// Animates the individual tokens out from last to first and then finishes with the tokenList object
	/// </summary>
	/// <param name="index"> The index of the token being animated </param>
	private void AnimateTokenListOut(int index)
	{
		Transform tokenTransform = tokenList.transform.GetChild(0).GetChild(0);

		if (index == 0)
			tokenTransform.GetChild(index).gameObject.AnimateScaleX(0f, 0.1f,
				() => tokenList.AnimateGraphicAndScale(0f, 0f, 0.15f, FinishAnimatingOut));
		else
			tokenTransform.GetChild(index).gameObject.AnimateScaleX(0f, 0.1f, () => AnimateTokenListOut(--index));
	}

	/// <summary>
	/// Finishes the AnimateOut with the form and dim fading out
	/// </summary>
	private void FinishAnimatingOut()
	{
		form.AnimateGraphicAndScale(0f, 0f, 0.15f);
		dim.AnimateGraphic(0f, 0.15f, FinishedAnimating);
	}

	/// <summary>
	/// Animates the main token list, then starts animating the tokens one by one
	/// </summary>
	/// <param name="index"> The token index being animated </param>
	private void AnimateTokenListIn(int index)
	{
		tokenList.AnimateGraphicAndScale(1f, 1f, 0.2f, () => AnimateTokens(index));
		customTokenButton.AnimateScaleX(1f, 0.2f);
	}

	/// <summary>
	/// Animates the tokens one by one
	/// </summary>
	/// <param name="index"> The token index being animated </param>
	private void AnimateTokens(int index)
	{
		Transform tokenTransform = tokenList.transform.GetChild(0).GetChild(0);

		if (index == tokenTransform.childCount - 1)
			tokenTransform.GetChild(index).gameObject.AnimateScaleX(1.183325f, 0.15f, FinishedAnimating);
		else
			tokenTransform.GetChild(index).gameObject.AnimateScaleX(1.183325f, 0.15f, () => AnimateTokens(++index));
	}
}