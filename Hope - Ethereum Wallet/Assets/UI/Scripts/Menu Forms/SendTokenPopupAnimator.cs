﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SendTokenPopupAnimator : UIAnimator
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
	[SerializeField] private TMP_InputField amountInputField;
	[SerializeField] private TMP_InputField gasLimitInputField;
	[SerializeField] private TMP_InputField gasPriceInputField;

	[SerializeField] private GameObject advancedModeToggle;
	[SerializeField] private GameObject maxToggle;

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
		amountInputField.onValueChanged.AddListener(_ => AnimateFieldError(amountSection, sendAssetPopup.Amount.IsValid));

		advancedModeToggle.transform.GetComponent<Toggle>().AddToggleListener(AdvancedModeClicked);
		maxToggle.transform.GetComponent<Toggle>().AddToggleListener(MaxClicked);
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
	/// Sets the send button to interactable if all the input fields are filled in and valid
	/// </summary>
	private void SetSendButtonInteractable()
	{
		bool interactable = !string.IsNullOrEmpty(addressInputField.text) && !string.IsNullOrEmpty(amountInputField.text)
            && sendAssetPopup.Address.IsValid && sendAssetPopup.Amount.IsValid;

		if (advancedMode && (string.IsNullOrEmpty(gasLimitInputField.text) || string.IsNullOrEmpty(gasPriceInputField.text)))
			interactable = false;

		sendButton.GetComponent<Button>().interactable = interactable;
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

		SetSendButtonInteractable();
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

    private void AnimateFieldError(GameObject sectionObj, bool isValidField)
    {
        sectionObj.transform.GetChild(sectionObj.transform.childCount - 1).gameObject.AnimateGraphicAndScale(isValidField ? 0f : 1f, isValidField ? 0f : 1f, 0.2f);
    }

	/// <summary>
	/// Maximum amount is toggled
	/// </summary>
	private void MaxClicked()
	{
		TMP_InputField inputField = amountInputField.transform.GetComponent<TMP_InputField>();
		//string tokenAmountText = DEMO_TOKEN_AMOUNT.ToString();
		//bool maxToggledOn = maxToggle.transform.GetComponent<Toggle>().IsToggledOn;

		inputField.interactable = !maxToggle.transform.GetComponent<Toggle>().IsToggledOn;

		//inputField.placeholder.GetComponent<TextMeshProUGUI>().text = maxToggledOn ? tokenAmountText + " (Max)" : "Enter amount...";
		//inputField.text = maxToggledOn ? "" : tokenAmountText;
	}
}
