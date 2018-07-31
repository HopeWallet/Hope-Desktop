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
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
	}
}
