using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SendAssetPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject tokenSection;
	[SerializeField] private GameObject advancedModeSection;
	[SerializeField] private GameObject addressSection;
	[SerializeField] private GameObject amountSection;
	[SerializeField] private GameObject gasLimitSection;
	[SerializeField] private GameObject gasPriceSection;
	[SerializeField] private GameObject transactionSpeedSection;

	[SerializeField] private TMP_InputField addressInputField;
	[SerializeField] private GameObject contactNameObject;

	[SerializeField] private GameObject advancedModeToggle;

	[SerializeField] private GameObject sendButton;

	private bool advancedMode;

	/// <summary>
	/// Initializes the button and input field listeners
	/// </summary>
	private void Start()
	{
        SendAssetPopup sendAssetPopup = GetComponent<SendAssetPopup>();

		addressInputField.onValueChanged.AddListener(_ => AnimateFieldError(addressSection, sendAssetPopup.Address.IsValid || sendAssetPopup.Address.IsEmpty));
		addressInputField.onValueChanged.AddListener(_ => AnimateContactName(!string.IsNullOrEmpty(sendAssetPopup.Address.contactName.text)));
        sendAssetPopup.Amount.OnAmountChanged += () => AnimateFieldError(amountSection, sendAssetPopup.Amount.IsValid || sendAssetPopup.Amount.IsEmpty);

        advancedModeToggle.transform.GetComponent<Toggle>().AddToggleListener(AdvancedModeClicked);
	}

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
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
	private void AdvancedModeClicked()
	{
		advancedMode = !advancedMode;
		Animating = true;

		if (advancedMode)
			transactionSpeedSection.AnimateScale(0f, 0.1f, () => AnimateGasLimitAndPrice(true));
		else
			gasLimitSection.AnimateScale(0f, 0.1f, () => AnimateGasLimitAndPrice(false));
	}

	/// <summary>
	/// Animates the gas limit section and gas price section together
	/// </summary>
	/// <param name="animatingIn"> Checks to see if animating these fields in or out </param>
	private void AnimateGasLimitAndPrice(bool animatingIn)
	{
		gasLimitSection.AnimateScale(animatingIn ? 1f : 0f, 0.1f);

		if (animatingIn)
			gasPriceSection.AnimateScale(1f, 0.1f, () => Animating = false);
		else
			gasPriceSection.AnimateScale(0f, 0.1f,
				() => transactionSpeedSection.AnimateScale(1f, 0.1f,
				() => Animating = false));
	}

    /// <summary>
    /// Animates the error icon for the given input field.
    /// </summary>
    /// <param name="sectionObj"> The gameobject that holds the input field. </param>
    /// <param name="isValidField"> Checks if valid input or not. </param>
    private void AnimateFieldError(GameObject sectionObj, bool isValidField)
    {
        sectionObj.transform.GetChild(sectionObj.transform.childCount - 1).GetComponent<InteractableIcon>().AnimateIcon(isValidField ? 0f : 1f);
    }

	/// <summary>
	/// Animates the contact name in or out of sight
	/// </summary>
	/// <param name="animateIn"> Checks if animating in or out </param>
	private void AnimateContactName(bool animateIn) => contactNameObject.AnimateGraphic(animateIn ? 0.647f : 0f, 0.15f);
}
