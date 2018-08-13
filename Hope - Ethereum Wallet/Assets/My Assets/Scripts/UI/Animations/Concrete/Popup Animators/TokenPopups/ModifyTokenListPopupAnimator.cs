using UnityEngine;
using UnityEngine.UI;

public class ModifyTokenListPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject tokenList;
	[SerializeField] private GameObject customTokenButton;
	[SerializeField] private GameObject searchSection;
	[SerializeField] private GameObject confirmButton;

	private Transform tokenTransform;

	private ModifyTokensPopup modifyTokensPopup;

	/// <summary>
	/// Initializes the button listeners
	/// </summary>
	private void Awake()
	{
		modifyTokensPopup = transform.GetComponent<ModifyTokensPopup>();
		modifyTokensPopup.OnAddableTokenAdded += AnimateTokenIntoList;

		tokenTransform = tokenList.transform.GetChild(0).GetChild(0);
	}

	protected override void AnimateUniqueElementsIn()
	{
		customTokenButton.AnimateScaleX(1f, 0.2f);
		searchSection.AnimateScaleX(1f, 0.2f, () => AnimateTokens(0));
		tokenList.AnimateGraphicAndScale(1f, 1f, 0.25f);
		confirmButton.AnimateGraphicAndScale(1f, 1f, 0.3f, FinishedAnimating);
	}

	protected override void AnimateUniqueElementsOut()
	{
		confirmButton.AnimateGraphicAndScale(0f, 0f, 0.2f, () => AnimateBasicElements(false));

		for (int i = 0; i < tokenTransform.childCount; i++)
			tokenTransform.GetChild(i).gameObject.AnimateScaleX(0f, 0.2f);

		tokenList.AnimateGraphicAndScale(0f, 0f, 0.3f);
		customTokenButton.AnimateScaleX(0f, 0.3f);
		searchSection.AnimateScaleX(0f, 0.3f);
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
        {
            tokenTransform.GetChild(index).gameObject.AnimateScaleX(1f, 0.1f, FinishedAnimating);
        }
        else
        {
            tokenTransform.GetChild(index).gameObject.AnimateScaleX(1f, 0.1f, () => AnimateTokens(++index));
        }
    }

	private void AnimateTokenIntoList(AddableTokenButton addableTokenButton) => addableTokenButton.transform.parent.gameObject.AnimateScaleX(1f, 0.1f);
}
