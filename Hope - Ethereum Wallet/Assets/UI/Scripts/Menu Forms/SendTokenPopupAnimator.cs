using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

	[SerializeField] private GameObject addressInputField;
	[SerializeField] private GameObject amountInputField;
	[SerializeField] private GameObject gasLimitInputField;
	[SerializeField] private GameObject gasPriceInputField;

	[SerializeField] private GameObject advancedModeToggle;
	[SerializeField] private GameObject maxToggle;

	[SerializeField] private GameObject sendButton;

	private bool advancedMode;

	//DEMO VARIABLE TO BE REPLACED
	private readonly decimal DEMO_TOKEN_AMOUNT = 355.5994235643m;

	private void Awake()
	{
		advancedModeToggle.transform.GetComponent<ToggleAnimator>().ToggleClick = AdvancedModeClicked;
		maxToggle.transform.GetComponent<ToggleAnimator>().ToggleClick = MaxClicked;

		SetTokenAmount(DEMO_TOKEN_AMOUNT);
	}

	protected override void AnimateIn()
	{
		dim.AnimateGraphic(1f, 0.15f);
		form.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => title.AnimateGraphicAndScale(1f, 0.85f, 0.12f,
			() => tokenSection.AnimateGraphicAndScale(1f, 1f, 0.12f,
			() => advancedModeSection.AnimateGraphicAndScale(1f, 1f, 0.12f,
			() => addressSection.AnimateGraphicAndScale(1f, 1f, 0.12f,
			() => amountSection.AnimateGraphicAndScale(1f, 1f, 0.12f,
			() => transactionSpeedSection.AnimateGraphicAndScale(1f, 1f, 0.12f,
			() => sendButton.AnimateGraphicAndScale(1f, 1f, 0.12f, FinishedAnimating))))))));
	}

	protected override void AnimateOut()
	{
	}

	public void SetTokenAmount(decimal tokenAmount) => tokenSection.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = tokenAmount.ToString();

	private void SetSendButtonInteractable()
	{
		bool interactable = !IsEmptyString(addressInputField) && !IsEmptyString(amountInputField);

		if (advancedMode && !IsEmptyString(gasLimitInputField) && !IsEmptyString(gasPriceInputField))
			interactable = false;

		sendButton.GetComponent<Button>().interactable = interactable;
	}

	private string GetInputFieldString(GameObject inputField) => gameObject.transform.GetComponent<TMP_InputField>().text;

	private bool IsEmptyString(GameObject inputField) => string.IsNullOrEmpty(GetInputFieldString(inputField));

	private void AdvancedModeClicked()
	{
		advancedMode = !advancedMode;
		Animating = true;

		if (advancedMode)
		{
			transactionSpeedSection.AnimateGraphicAndScale(0f, 0f, 0.1f,
				() => gasLimitSection.AnimateGraphicAndScale(1f, 1f, 0.1f,
				() => gasPriceSection.AnimateGraphicAndScale(1f, 1f, 0.1f,
				() => Animating = false)));
		}

		else
		{
			gasLimitSection.AnimateGraphicAndScale(0f, 0f, 0.1f,
				() => gasPriceSection.AnimateGraphicAndScale(0f, 0f, 0.1f,
				() => transactionSpeedSection.AnimateGraphicAndScale(1f, 1f, 0.1f,
				() => Animating = false)));
		}
	}

	private void MaxClicked()
	{
		TMP_InputField inputField = amountInputField.transform.GetComponent<TMP_InputField>();
		string tokenAmountText = DEMO_TOKEN_AMOUNT.ToString();
		bool maxToggledOn = maxToggle.transform.GetComponent<ToggleAnimator>().IsToggledOn;

		inputField.interactable = maxToggledOn ? false : true;

		inputField.placeholder.GetComponent<TextMeshProUGUI>().text = maxToggledOn ? tokenAmountText + " (Max)" : "Enter amount...";
		inputField.text = maxToggledOn ? "" : tokenAmountText;
	}
}
