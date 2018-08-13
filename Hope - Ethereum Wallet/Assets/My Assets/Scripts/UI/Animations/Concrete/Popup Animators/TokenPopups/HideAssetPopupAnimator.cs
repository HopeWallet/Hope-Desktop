using UnityEngine;
using UnityEngine.UI;

public class HideAssetPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject mainText;
	[SerializeField] private GameObject subText;
	[SerializeField] private GameObject tokenIcon;
	[SerializeField] private GameObject tokenSymbol;
	[SerializeField] private GameObject yesButton;
	[SerializeField] private GameObject noButton;

	//I THINK YOU CAN DELETE THIS POPUP!!?!?!?!?!?!??!!?!?!?!?!?!?!?!?!?!?!?!?!?!!?!?!?!?!?!?!?!?

	/// <summary>
	/// Initializes the button listeners
	/// </summary>
	private void Awake()
	{
		yesButton.GetComponent<Button>().onClick.AddListener(YesButtonClicked);
		noButton.GetComponent<Button>().onClick.AddListener(NoButtonClicked);
	}

	/// <summary>
	/// No button is clicked
	/// </summary>
	private void NoButtonClicked() => AnimateDisable();

	protected override void AnimateUniqueElementsIn()
	{
		throw new System.NotImplementedException();
	}

	protected override void AnimateUniqueElementsOut()
	{
		throw new System.NotImplementedException();
	}

	/// <summary>
	/// Yes button is clicked
	/// </summary>
	private void YesButtonClicked()
	{
		//Disable the token in the AssetList
		AnimateDisable();
	}
}
