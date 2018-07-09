using UnityEngine;
using UnityEngine.UI;

public class TokenListForm : MenuAnimator
{

	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject tokenList;
	[SerializeField] private GameObject customTokenButton;
	[SerializeField] private GameObject addTokenForm;

	/// <summary>
	/// Initializes the button listeners
	/// </summary>
	private void Awake() => customTokenButton.GetComponent<Button>().onClick.AddListener(CustomTokenClicked);

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		dim.AnimateGraphic(1f, 0.2f);
		form.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => title.AnimateGraphicAndScale(0.85f, 1f, 0.2f,
			() => AnimateTokenList(0)));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
	}

	/// <summary>
	/// Animates the main token list, then starts animating the tokens one by one
	/// </summary>
	/// <param name="index"> The token index being animated </param>
	private void AnimateTokenList(int index)
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

	/// <summary>
	/// customTokenButton is clicked and opens up the addTokenForm
	/// </summary>
	private void CustomTokenClicked() => addTokenForm.SetActive(true);
}
