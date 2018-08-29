using UnityEngine;
using TMPro;

public class SendAssetPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject tokenSection;
	[SerializeField] private GameObject advancedModeSection;
	[SerializeField] private GameObject addressSection;
	[SerializeField] private GameObject amountSection;
	[SerializeField] private GameObject gasLimitInputField;
	[SerializeField] private GameObject gasPriceInputField;
	[SerializeField] private GameObject transactionSpeedSection;

	[SerializeField] private HopeInputField addressField;
	[SerializeField] private GameObject contactNameObject;

	[SerializeField] private GameObject sendButton;

	/// <summary>
	/// Initializes the button and input field listeners
	/// </summary>
	private void Start()
	{
		transform.GetComponent<SendAssetPopup>().AnimateAdvancedMode += AdvancedModeClicked;
		addressField.OnInputUpdated += () => AnimateContactName(!string.IsNullOrEmpty(contactNameObject.GetComponent<TextMeshProUGUI>().text));
	}

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		addressField.inputFieldBase.ActivateInputField();
		tokenSection.AnimateScale(1f, 0.1f);
		advancedModeSection.AnimateScale(1f, 0.1f);
		addressSection.AnimateScaleX(1f, 0.15f);
		amountSection.AnimateScaleX(1f, 0.2f);
		transactionSpeedSection.AnimateScaleX(1f, 0.25f);
		sendButton.AnimateGraphicAndScale(1f, 1f, 0.3f, FinishedAnimating);
	}

	/// <summary>
	/// Advanced mode is toggled
	/// </summary>
	/// <param name="advancedMode"> Whether the popup needs to show the advanced mode or not </param>
	private void AdvancedModeClicked(bool advancedMode)
	{
		Animating = true;

		if (advancedMode)
			transactionSpeedSection.AnimateScale(0f, 0.1f, () => AnimateGasLimitAndPrice(true));
		else
			gasLimitInputField.AnimateScale(0f, 0.1f, () => AnimateGasLimitAndPrice(false));
	}

	/// <summary>
	/// Animates the gas limit section and gas price section together
	/// </summary>
	/// <param name="animatingIn"> Checks to see if animating these fields in or out </param>
	private void AnimateGasLimitAndPrice(bool animatingIn)
	{
		gasLimitInputField.AnimateScale(animatingIn ? 1f : 0f, 0.1f);

		if (animatingIn)
			gasPriceInputField.AnimateScale(1f, 0.1f, () => Animating = false);
		else
			gasPriceInputField.AnimateScale(0f, 0.1f,
				() => transactionSpeedSection.AnimateScale(1f, 0.1f,
				() => Animating = false));
	}

	/// <summary>
	/// Animates the contact name in or out of sight
	/// </summary>
	/// <param name="animateIn"> Checks if animating in or out </param>
	private void AnimateContactName(bool animateIn) => contactNameObject.AnimateGraphic(animateIn ? 1f : 0f, 0.15f);
}
