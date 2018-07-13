using UnityEngine;
using UnityEngine.UI;

public class HideAssetPopupAnimator : UIAnimator
{

	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject mainText;
	[SerializeField] private GameObject subText;
	[SerializeField] private GameObject tokenIcon;
	[SerializeField] private GameObject tokenSymbol;
	[SerializeField] private GameObject yesButton;
	[SerializeField] private GameObject noButton;

	/// <summary>
	/// Initializes the button listeners
	/// </summary>
	private void Awake()
	{
		yesButton.GetComponent<Button>().onClick.AddListener(YesButtonClicked);
		noButton.GetComponent<Button>().onClick.AddListener(NoButtonClicked);
	}

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		dim.AnimateGraphic(1f, 0.2f,
			() => tokenIcon.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => tokenSymbol.AnimateGraphicAndScale(1f, 0.85f, 0.15f,
			() => yesButton.AnimateGraphicAndScale(1f, 1f, 0.1f,
			() => noButton.AnimateGraphicAndScale(1f, 1f, 0.1f, FinishedAnimating)))));

		form.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => mainText.AnimateScaleX(1f, 0.15f,
			() => subText.AnimateScaleX(1f, 0.15f)));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		subText.AnimateScaleX(0f, 0.1f,
			() => mainText.AnimateScaleX(0f, 0.1f));

		yesButton.AnimateGraphicAndScale(0f, 0f, 0.1f,
			() => tokenIcon.AnimateGraphicAndScale(0f, 0f, 0.1f,
			() => dim.AnimateGraphic(0f, 0.2f)));

		noButton.AnimateGraphicAndScale(0f, 0f, 0.1f,
			() => tokenSymbol.AnimateGraphicAndScale(0f, 0f, 0.1f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.2f, FinishedAnimating)));
	}


	/// <summary>
	/// Yes button is clicked
	/// </summary>
	private void YesButtonClicked()
	{
		//Disable the token in the AssetList
		AnimateDisable();
	}

	/// <summary>
	/// No button is clicked
	/// </summary>
	private void NoButtonClicked() => AnimateDisable();
}
