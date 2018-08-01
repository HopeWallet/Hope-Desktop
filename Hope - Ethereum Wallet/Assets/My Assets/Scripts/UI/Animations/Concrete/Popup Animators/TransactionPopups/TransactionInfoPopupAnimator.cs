using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TransactionInfoPopupAnimator : UIAnimator
{

	[SerializeField] private Image blur;
	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject transactionHashSection;
	[SerializeField] private GameObject fromAddressSection;
	[SerializeField] private GameObject toAddressSection;
	[SerializeField] private GameObject valueAndTimeSection;
	[SerializeField] private GameObject setGasSection;
	[SerializeField] private GameObject actualGasSection;

	[SerializeField] private TMP_InputField transactionHash;
	[SerializeField] private TMP_InputField fromAddress;
	[SerializeField] private TMP_InputField toAddress;

	/// <summary>
	/// Initializes the elements
	/// </summary>
	private void Awake()
	{
		transactionHash.text = "0x93f01a7933d74acb10fc95fddc4584534d5f6304782766211610b6b4e707af62";
		fromAddress.text = "0xbF203720DaA26c88114273471cC5f3C83c7A0246";
		toAddress.text = "0x1192cDf96D594083BeCCc264fE58Df6E75c966f6";
	}

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		blur.AnimateMaterialBlur(1f, 0.15f,
			() => form.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => transactionHashSection.AnimateScaleX(1f, 0.15f,
			() => toAddressSection.AnimateScaleX(1f, 0.15f,
			() => setGasSection.AnimateScaleX(1f, 0.15f)))));

		dim.AnimateGraphic(1f, 0.15f,
			() => title.AnimateGraphicAndScale(0.85f, 1f, 0.15f,
			() => fromAddressSection.AnimateScaleX(1f, 0.15f,
			() => valueAndTimeSection.AnimateScaleX(1f, 0.15f,
			() => actualGasSection.AnimateScaleX(1f, 0.15f, FinishedAnimating)))));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		actualGasSection.AnimateScaleX(0f, 0.15f,
			() => valueAndTimeSection.AnimateScaleX(0f, 0.15f,
			() => fromAddressSection.AnimateScaleX(0f, 0.15f,
			() => title.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => blur.AnimateMaterialBlur(0f, 0.15f)))));

		setGasSection.AnimateScaleX(0f, 0.15f,
			() => toAddressSection.AnimateScaleX(0f, 0.15f,
			() => transactionHashSection.AnimateScaleX(0f, 0.15f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => dim.AnimateGraphic(0f, 0.15f, FinishedAnimating)))));
	}
}