using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TransactionInfoPopupAnimator : UIAnimator
{

	[SerializeField] private Image blur;
	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form1;
	[SerializeField] private GameObject form2;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject tokenIcon;
	[SerializeField] private GameObject transactionHashSection;
	[SerializeField] private GameObject fromAddressSection;
	[SerializeField] private GameObject toAddressSection;
	[SerializeField] private GameObject valueAndTimeSection;
	[SerializeField] private GameObject adjustViewButton;

	[SerializeField] private TMP_InputField transactionHash;
	[SerializeField] private TMP_InputField fromAddress;
	[SerializeField] private TMP_InputField toAddress;

	private bool showAdvancedDetails;

	/// <summary>
	/// Initializes the elements
	/// </summary>
	private void Awake()
	{
		transactionHash.text = "0x93f01a7933d74acb10fc95fddc4584534d5f6304782766211610b6b4e707af62";
		fromAddress.text = "0xbF203720DaA26c88114273471cC5f3C83c7A0246";
		toAddress.text = "0x1192cDf96D594083BeCCc264fE58Df6E75c966f6";

		adjustViewButton.GetComponent<Button>().onClick.AddListener(AdjustViewClicked);
	}

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		blur.AnimateMaterialBlur(1f, 0.15f);
		dim.AnimateGraphic(1f, 0.15f,
			() => tokenIcon.AnimateGraphicAndScale(1f, 1f, 0.15f));
		form1.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => title.AnimateGraphicAndScale(0.85f, 1f, 0.15f,
			() => transactionHashSection.AnimateScaleX(1f, 0.15f,
			() => fromAddressSection.AnimateScaleX(1f, 0.15f,
			() => toAddressSection.AnimateScaleX(1f, 0.15f,
			() => { valueAndTimeSection.AnimateScaleX(1f, 0.15f, FinishedAnimating); adjustViewButton.AnimateGraphicAndScale(1f, 1f, 0.15f); })))));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	[ContextMenu("Animate out")]
	protected override void AnimateOut()
	{
	}

	/// <summary>
	/// adjustViewButton is clicked and modifies what the popup shows
	/// </summary>
	private void AdjustViewClicked()
	{
		showAdvancedDetails = !showAdvancedDetails;

		AnimateAdjustViewButton();
		AnimatePopupView();
	}

	/// <summary>
	/// Animates the adjustViewButton up or down depending on the showAdvancedDetails boolean
	/// </summary>
	private void AnimateAdjustViewButton()
	{
		Button arrowButton = adjustViewButton.GetComponent<Button>();

		arrowButton.interactable = false;
		Animating = true;

		adjustViewButton.AnimateTransformY(showAdvancedDetails ? -328.6f : -228.6f, 0.15f,
			() => adjustViewButton.AnimateRotateZ(showAdvancedDetails ? 180f : 0f, 0.15f,
			() => { arrowButton.interactable = true; Animating = false; }));
	}

	/// <summary>
	/// Enlarges or shrinks the popup view depending on the showAdvancedDetails boolean
	/// </summary>
	private void AnimatePopupView()
	{

	}
}
