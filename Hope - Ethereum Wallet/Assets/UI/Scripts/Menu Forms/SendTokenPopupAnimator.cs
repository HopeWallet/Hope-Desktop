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
		addressInputField.GetComponent<TMP_InputField>().onValueChanged.AddListener(AddressChanged);
		amountInputField.GetComponent<TMP_InputField>().onValueChanged.AddListener(AmountChanged);
		gasLimitInputField.GetComponent<TMP_InputField>().onValueChanged.AddListener(GasLimitChanged);
		gasPriceInputField.GetComponent<TMP_InputField>().onValueChanged.AddListener(GasPriceChanged);

		advancedModeToggle.transform.GetComponent<ToggleAnimator>().ToggleClick = AdvancedModeClicked;
		maxToggle.transform.GetComponent<ToggleAnimator>().ToggleClick = MaxClicked;

		SetTokenAmount(DEMO_TOKEN_AMOUNT);
	}

	public void SetTokenAmount(decimal tokenAmount) => tokenSection.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = tokenAmount.ToString();

	protected override void AnimateIn()
	{
		dim.AnimateGraphic(1f, 0.2f);
		form.AnimateGraphicAndScale(1f, 1f, 0.2f);
		title.AnimateGraphicAndScale(1f, 1f, 0.2f);
		tokenSection.AnimateGraphicAndScale(1f, 1f, 0.2f);
		advancedModeSection.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => addressSection.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => amountSection.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => transactionSpeedSection.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => sendButton.AnimateGraphicAndScale(1f, 1f, 0.15f, FinishedAnimating)))));
	}

	protected override void AnimateOut()
	{

	}

	private void AddressChanged(string address)
	{
		SetSendButtonInteractable();
	}

	private void AmountChanged(string amount)
	{
		SetSendButtonInteractable();
	}

	private void GasLimitChanged(string gasLimit)
	{
		SetSendButtonInteractable();
	}

	private void GasPriceChanged(string gasPrice)
	{
		SetSendButtonInteractable();
	}

	private void SetSendButtonInteractable()
	{
		bool interactable = !IsEmptyField(addressInputField) && !IsEmptyField(amountInputField);

		if (advancedMode && IsEmptyField(gasLimitInputField) || advancedMode && IsEmptyField(gasPriceInputField))
			interactable = false;

		sendButton.GetComponent<Button>().interactable = interactable;
	}

	private string GetInputFieldString(GameObject inputField) => inputField.transform.GetComponent<TMP_InputField>().text;

	private bool IsEmptyField(GameObject inputField) => string.IsNullOrEmpty(GetInputFieldString(inputField));

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

			SetSendButtonInteractable();
		}

		else
		{
			gasLimitSection.AnimateGraphicAndScale(0f, 0f, 0.1f,
				() => gasPriceSection.AnimateGraphicAndScale(0f, 0f, 0.1f,
				() => transactionSpeedSection.AnimateGraphicAndScale(1f, 1f, 0.1f,
				() => Animating = false)));
		}

		SetSendButtonInteractable();
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
