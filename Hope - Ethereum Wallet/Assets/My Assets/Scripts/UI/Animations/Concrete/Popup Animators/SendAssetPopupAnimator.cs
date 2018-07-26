using UnityEngine;
using TMPro;

public class SendAssetPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;

	[SerializeField] private GameObject tokenSection;
	[SerializeField] private GameObject advancedModeSection;
	[SerializeField] private GameObject addressSection;
	[SerializeField] private GameObject amountSection;
	[SerializeField] private GameObject gasLimitSection;
	[SerializeField] private GameObject gasPriceSection;
	[SerializeField] private GameObject transactionSpeedSection;

	[SerializeField] private TMP_InputField addressInputField;

	[SerializeField] private GameObject advancedModeToggle;

	[SerializeField] private GameObject sendButton;

    private SendAssetPopup sendAssetPopup;

	private bool advancedMode;

	/// <summary>
	/// Initializes the button and input field listeners
	/// </summary>
	private void Start()
	{
        sendAssetPopup = GetComponent<SendAssetPopup>();

		addressInputField.onValueChanged.AddListener(_ => AnimateFieldError(addressSection, sendAssetPopup.Address.IsValid));
        sendAssetPopup.Amount.AddSendAmountListener(() => AnimateFieldError(amountSection, sendAssetPopup.Amount.IsValid || !sendAssetPopup.Amount.AmountChanged));

		advancedModeToggle.transform.GetComponent<Toggle>().AddToggleListener(AdvancedModeClicked);
	}

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		dim.AnimateGraphic(1f, 0.2f);
		form.AnimateGraphicAndScale(1f, 1f, 0.2f);
		title.AnimateGraphicAndScale(0.85f, 1f, 0.2f);
		tokenSection.AnimateGraphicAndScale(1f, 1f, 0.2f);
		advancedModeSection.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => addressSection.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => amountSection.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => transactionSpeedSection.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => sendButton.AnimateGraphicAndScale(1f, 1f, 0.15f, FinishedAnimating)))));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		sendButton.AnimateGraphicAndScale(0f, 0f, 0.2f,
			() => title.AnimateGraphicAndScale(0f, 0f, 0.2f,
			() => dim.AnimateGraphic(0f, 0.2f, FinishedAnimating)));
		amountSection.AnimateGraphicAndScale(0f, 0f, 0.2f,
			() => tokenSection.AnimateGraphicAndScale(0f, 0f, 0.2f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.2f)));
		addressSection.AnimateGraphicAndScale(0f, 0f, 0.2f,
			() => advancedModeSection.AnimateGraphicAndScale(0f, 0f, 0.2f));

        if (advancedMode)
        {
            gasLimitSection.AnimateGraphicAndScale(0f, 0f, 0.2f);
            gasPriceSection.AnimateGraphicAndScale(0f, 0f, 0.2f);
        }

        else
        {
            transactionSpeedSection.AnimateGraphicAndScale(0f, 0f, 0.2f);
        }
    }

	/// <summary>
	/// Advanced mode is toggled
	/// </summary>
	private void AdvancedModeClicked()
	{
		advancedMode = !advancedMode;
		Animating = true;

		if (advancedMode)
			transactionSpeedSection.AnimateGraphicAndScale(0f, 0f, 0.1f, () => AnimateGasLimitAndPrice(true));

		else
			gasLimitSection.AnimateGraphicAndScale(0f, 0f, 0.1f, () => AnimateGasLimitAndPrice(false));
	}

	/// <summary>
	/// Animates the gas limit section and gas price section together
	/// </summary>
	/// <param name="animatingIn"> Checks to see if animating these fields in or out </param>
	private void AnimateGasLimitAndPrice(bool animatingIn)
	{
		gasLimitSection.AnimateGraphicAndScale(animatingIn ? 1f : 0f, animatingIn ? 1f : 0f, 0.1f);

		if (animatingIn)
			gasPriceSection.AnimateGraphicAndScale(1f, 1f, 0.1f, () => Animating = false);
		else
			gasPriceSection.AnimateGraphicAndScale(0f, 0f, 0.1f,
				() => transactionSpeedSection.AnimateGraphicAndScale(1f, 1f, 0.1f,
				() => Animating = false));
	}

    /// <summary>
    /// Animates the error icon for the given input field.
    /// </summary>
    /// <param name="sectionObj"> The gameobject that holds the input field. </param>
    /// <param name="isValidField"> Checks if valid input or not. </param>
    private void AnimateFieldError(GameObject sectionObj, bool isValidField)
    {
        sectionObj.transform.GetChild(sectionObj.transform.childCount - 1).gameObject.AnimateGraphicAndScale(isValidField ? 0f : 1f, isValidField ? 0f : 1f, 0.2f);
    }
}
