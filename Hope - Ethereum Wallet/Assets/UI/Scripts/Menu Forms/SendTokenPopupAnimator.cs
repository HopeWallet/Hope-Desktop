using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Hope.Utils.EthereumUtils;
using System.Linq;
using System.Collections;

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

	private int oldValue;
	private bool advancedMode;
	private bool validAddress;
	private bool validAmount;

	/// <summary>
	/// If the address is valid or not, it animates the error icon in or out
	/// </summary>
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

	/// <summary>
	/// If the transaction amount is valid or not, it animates the error icon in or out
	/// </summary>
	public bool ValidAmount
	{
		get { return validAmount; }
		set
		{
			validAmount = value;
			amountSection.transform.GetChild(amountSection.transform.childCount - 1).gameObject
				.AnimateGraphicAndScale(validAmount ? 0f : 1f, validAmount ? 0f : 1f, 0.2f);
		}
	}

	//DEMO VARIABLE TO BE REPLACED
	private readonly decimal DEMO_TOKEN_AMOUNT = 355.5994235643m;

	/// <summary>
	/// Initializes the button and input field listeners
	/// </summary>
	private void Awake()
	{
		addressInputField.onValueChanged.AddListener(AddressChanged);
		amountInputField.onValueChanged.AddListener(AmountChanged);
		gasLimitInputField.onValueChanged.AddListener(GasLimitChanged);
		gasPriceInputField.onValueChanged.AddListener(GasPriceChanged);

		advancedModeToggle.transform.GetComponent<Toggle>().AddToggleListener(AdvancedModeClicked);
		maxToggle.transform.GetComponent<Toggle>().AddToggleListener(MaxClicked);

		tokenSection.transform.GetChild(2).GetComponent<TMP_Dropdown>().onValueChanged.AddListener(TokenChanged);

		SetTokenAmount(DEMO_TOKEN_AMOUNT);
	}

	/// <summary>
	/// Sets the token amount 
	/// </summary>
	/// <param name="tokenAmount"> The value of the token amount on the wallet </param>
	public void SetTokenAmount(decimal tokenAmount) => tokenSection.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = tokenAmount.ToString();

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
			() => dim.AnimateGraphic(0f, 2f, FinishedAnimating)));
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
			transactionSpeedSection.AnimateGraphicAndScale(0f, 0f, 2f);
	}

	/// <summary>
	/// When a different token is selected to be sent
	/// </summary>
	/// <param name="value"> The value of the token options in the token dropdown </param>
	private void TokenChanged(int value)
	{
		if (value != oldValue)
			oldValue = value;
		else
			return;

		tokenSection.transform.GetChild(0).gameObject.AnimateGraphicAndScale(0f, 0f, 0.1f, SetTokenIcon);
		tokenSection.transform.GetChild(1).gameObject.AnimateScaleY(0f, 0.1f, SetTokenAmount);
	}

	/// <summary>
	/// Sets the proper token icon and animates it back into view
	/// </summary>
	private void SetTokenIcon()
	{
		//Set the proper image icon to the corresponding token
		tokenSection.transform.GetChild(0).gameObject.AnimateGraphicAndScale(1f, 1f, 0.1f);
	}

	/// <summary>
	/// Sets the proper token amount and animates it back into view
	/// </summary>
	private void SetTokenAmount()
	{
		//Set the proper token amount to the right amount in the wallet
		tokenSection.transform.GetChild(1).gameObject.AnimateScaleY(1f, 0.1f);
	}

	/// <summary>
	/// Checks to see if the address is the correct length, and a valid address
	/// </summary>
	/// <param name="address"> The address string in the address input field </param>
	private void AddressChanged(string address)
	{
		if (!AddressUtils.CorrectAddressLength(address))
			addressInputField.text = address.LimitEnd(42);

		string updatedAddress = addressInputField.text;

		ValidAddress = string.IsNullOrEmpty(updatedAddress) ? true : AddressUtils.IsValidEthereumAddress(updatedAddress);

		SetSendButtonInteractable();
	}

	/// <summary>
	/// Limits the user from typing anything other than numbers and/or a dot
	/// </summary>
	/// <param name="amount"> The string in the amount input field </param>
	private void AmountChanged(string amount)
	{
		amountInputField.text = NumbersAndDotsOnly(amountInputField.text);

		if (!string.IsNullOrEmpty(amount))
			ValidAmount = amount.Substring(0, 1) != "." && amount.Count(c => c == '.') <= 1 && amount.Substring(amount.Length - 1, 1) != ".";
		else
			ValidAmount = true;

		SetSendButtonInteractable();
	}

	/// <summary>
	/// Limits the user from typing anything other than numbers
	/// </summary>
	/// <param name="amount"> The string in the gas limit input field </param>
	private void GasLimitChanged(string gasLimit)
	{
		gasLimitInputField.text = GetNumbersOnly(gasLimitInputField.text);
		SetSendButtonInteractable();
	}

	/// <summary>
	/// Limits the user from typing anything other than numbers
	/// </summary>
	/// <param name="amount"> The string in the gas price input field </param>
	private void GasPriceChanged(string gasPrice)
	{
		gasPriceInputField.text = GetNumbersOnly(gasPriceInputField.text);
		SetSendButtonInteractable();
	}

	/// <summary>
	/// Sets the send button to interactable if all the input fields are filled in and valid
	/// </summary>
	private void SetSendButtonInteractable()
	{
		bool interactable = !string.IsNullOrEmpty(addressInputField.text) && !string.IsNullOrEmpty(amountInputField.text) && validAddress && validAmount;

		if (advancedMode && (string.IsNullOrEmpty(gasLimitInputField.text) || string.IsNullOrEmpty(gasPriceInputField.text)))
			interactable = false;

		sendButton.GetComponent<Button>().interactable = interactable;
	}

	/// <summary>
	/// Returns a string with only digits
	/// </summary>
	/// <param name="oldString"> The string being changed </param>
	/// <returns></returns>
	private string GetNumbersOnly(string oldString) => new string(oldString.Where(c => char.IsDigit(c)).ToArray());

	/// <summary>
	/// Returns a string with only digits and periods
	/// </summary>
	/// <param name="oldString"> The string being changed </param>
	/// <returns></returns>
	private string NumbersAndDotsOnly(string oldString) => new string(oldString.Where(c => char.IsDigit(c) || c == '.').ToArray());

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

	/// <summary>
	/// Maximum amount is toggled
	/// </summary>
	private void MaxClicked()
	{
		TMP_InputField inputField = amountInputField.transform.GetComponent<TMP_InputField>();
		string tokenAmountText = DEMO_TOKEN_AMOUNT.ToString();
		bool maxToggledOn = maxToggle.transform.GetComponent<Toggle>().IsToggledOn;

		inputField.interactable = maxToggledOn ? false : true;

		inputField.placeholder.GetComponent<TextMeshProUGUI>().text = maxToggledOn ? tokenAmountText + " (Max)" : "Enter amount...";
		inputField.text = maxToggledOn ? "" : tokenAmountText;
	}
}
