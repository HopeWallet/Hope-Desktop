using UnityEngine;
using UnityEngine.UI;

public class ModifyTokenListPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject addTokenButton;
	[SerializeField] private GameObject searchInputField;
	[SerializeField] private GameObject line;
	[SerializeField] private GameObject confirmButton;
	[SerializeField] private Transform tokenTransform;

	private ModifyTokensPopup modifyTokensPopup;

	/// <summary>
	/// Initializes the button listeners
	/// </summary>
	private void Awake()
	{
		modifyTokensPopup = transform.GetComponent<ModifyTokensPopup>();
		modifyTokensPopup.OnAddableTokenAdded += AnimateTokenIntoList;
	}

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		addTokenButton.AnimateScaleX(1f, 0.2f);
		searchInputField.AnimateScaleX(1f, 0.2f, () => AnimateTokens(0));
		line.AnimateScaleX(1f, 0.25f);
		confirmButton.AnimateGraphicAndScale(1f, 1f, 0.3f, FinishedAnimating);
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
