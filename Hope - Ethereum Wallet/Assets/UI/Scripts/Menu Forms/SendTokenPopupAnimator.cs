using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Hope.Utils.EthereumUtils;
using System.Linq;

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

	private bool advancedMode;
	private bool validAddress;
	private bool validAmount;

	public bool ValidAddress
	{
		get { return validAddress; }
		set
		{
			validAddress = value;
			addressSection.transform.GetChild(addressSection.transform.childCount - 1).gameObject
					.AnimateGraphicAndScale(validAddress ? 0f : 1f, validAddress ? 0f : 1f, 0.2f);
		}
	}

	public bool ValidAmount
	{
		get { return validAmount; }
		set
		{
			validAmount = value;
			amountSection.transform.GetChild(amountSection.transform.childCount - 1).gameObject
				.AnimateGraphicAndScale(validAddress ? 0f : 1f, validAddress ? 0f : 1f, 0.2f);
		}
	}

	//DEMO VARIABLE TO BE REPLACED
	private readonly decimal DEMO_TOKEN_AMOUNT = 355.5994235643m;

	private void Awake()
	{
		addressInputField.onValueChanged.AddListener(AddressChanged);
		amountInputField.onValueChanged.AddListener(AmountChanged);
		gasLimitInputField.onValueChanged.AddListener(GasLimitChanged);
		gasPriceInputField.onValueChanged.AddListener(GasPriceChanged);

		advancedModeToggle.transform.GetComponent<ToggleAnimator>().ToggleClick = AdvancedModeClicked;
		maxToggle.transform.GetComponent<ToggleAnimator>().ToggleClick = MaxClicked;

		tokenSection.transform.GetChild(2).GetComponent<TMP_Dropdown>().onValueChanged.AddListener(TokenChanged);

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

	private void TokenChanged(int value)
	{
		//Set token icon
		//Set token amount
	}

	private void AddressChanged(string address)
	{
		if (address.Length > 42)
			addressInputField.text = address.LimitEnd(42);

		if (AddressUtils.IsValidEthereumAddress(address))
			ValidAddress = true;
		else
			ValidAddress = false;

		SetSendButtonInteractable();
	}

	private void AmountChanged(string amount)
	{
		amountInputField.text = NumbersAndDotOnly(amountInputField.text);
		SetSendButtonInteractable();
	}

	private void GasLimitChanged(string gasLimit)
	{
		gasLimitInputField.text = GetNumbersOnly(gasLimitInputField.text);
		SetSendButtonInteractable();
	}

	private void GasPriceChanged(string gasPrice)
	{
		gasPriceInputField.text = GetNumbersOnly(gasPriceInputField.text);
		SetSendButtonInteractable();
	}

	private void SetSendButtonInteractable()
	{
		bool interactable = !IsEmptyString(addressInputField.text) && !IsEmptyString(amountInputField.text);

		if (advancedMode && IsEmptyString(gasLimitInputField.text) || advancedMode && IsEmptyString(gasPriceInputField.text))
			interactable = false;

		sendButton.GetComponent<Button>().interactable = interactable;
	}

	private string GetNumbersOnly(string oldString) => new string(oldString.Where(c => char.IsDigit(c)).ToArray());

	private string NumbersAndDotOnly(string oldString)
	{
		if (oldString.Count(c => c == '.') == 2)
			oldString = oldString.Substring(0, oldString.Length - 1);

		return new string(oldString.Where(c => char.IsDigit(c) || c == '.').ToArray());
	}

	private bool IsEmptyString(string str) => string.IsNullOrEmpty(str);

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
