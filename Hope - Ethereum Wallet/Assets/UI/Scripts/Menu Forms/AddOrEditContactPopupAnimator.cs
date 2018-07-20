using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Hope.Utils.EthereumUtils;
using System;

public class AddOrEditContactPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject nameSection;
	[SerializeField] private GameObject addressSection;
	[SerializeField] private GameObject addContactButton;
	[SerializeField] private GameObject confirmButton;

	private TMP_InputField nameInputField;
	private TMP_InputField addressInputField;

	private bool addingContact;
	private bool validName;
	private bool validAddress;

	/// <summary>
	/// Animates the error icon beside the name input field if it is valid or not
	/// </summary>
	private bool ValidName
	{
		set
		{
			validName = value;
			nameSection.transform.GetChild(nameSection.transform.childCount - 1).gameObject
					.AnimateGraphicAndScale(validName ? 0f : 1f, validName ? 0f : 1f, 0.2f);
		}
	}

	/// <summary>
	/// Animates the error icon beside the address input field if it is valid or not
	/// </summary>
	private bool ValidAddress
	{
		set
		{
			validAddress = value;
			addressSection.transform.GetChild(addressSection.transform.childCount - 1).gameObject
					.AnimateGraphicAndScale(validAddress ? 0f : 1f, validAddress ? 0f : 1f, 0.2f);
		}
	}

	/// <summary>
	/// Initializes the elements
	/// </summary>
	private void Awake()
	{
		SetAddingContact(true);

		nameInputField = nameSection.transform.GetChild(2).GetComponent<TMP_InputField>();
		addressInputField = addressSection.transform.GetChild(2).GetComponent<TMP_InputField>();

		nameInputField.onValueChanged.AddListener(ContactNameChanged);
		addressInputField.onValueChanged.AddListener(AddressChanged);

		confirmButton.GetComponent<Button>().onClick.AddListener(ConfirmButtonClicked);
	}

	/// <summary>
	/// Sets all the necessary values
	/// </summary>
	/// <param name="addingContact"> Checks if adding a new contact or editing an existing one </param>
	/// <param name="name"> The current contact name being edited, if any </param>
	/// <param name="address"> The current contact address being edited, if any </param>
	public void SetAddingContact(bool addingContact, string name = "", string address = "")
	{
		this.addingContact = addingContact;
		nameInputField.text = name;
		addressInputField.text = address;
		title.GetComponent<TextMeshProUGUI>().text = addingContact ? "A D D  C O N T A C T" : "E D I T  C O N T A C T";
	}

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		dim.AnimateGraphic(1f, 0.2f);
		form.AnimateGraphicAndScale(1f, 1f, 0.2f,
		() => title.AnimateScaleX(1f, 0.15f,
		() => nameSection.AnimateScaleX(1f, 0.15f,
		() => addressSection.AnimateScaleX(1f, 0.15f,
		() => AnimateMainButton(true)))));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	[ContextMenu("Animate Out")]
	protected override void AnimateOut()
	{
		AnimateMainButton(false);

		nameSection.AnimateScaleX(0f, 0.15f,
			() => title.AnimateScaleX(0f, 0.15f,
			() => dim.AnimateGraphic(0f, 0.15f)));
		addressSection.AnimateScaleX(0f, 0.15f,
			() => addressSection.AnimateScaleX(0f, 0.15f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.15f, FinishedAnimating)));
	}

	/// <summary>
	/// Animates the main button, depending on the boolean addingContact
	/// </summary>
	/// <param name="animatingIn"> Checks if animating the button in or out </param>
	private void AnimateMainButton(bool animatingIn)
	{
		if (addingContact)
			addContactButton.AnimateScaleX(animatingIn ? 1f : 0f, 0.15f);
		else
			confirmButton.AnimateScaleX(animatingIn ? 1f : 0f, 0.15f);

		if (animatingIn) FinishedAnimating();
	}

	/// <summary>
	/// Checks if the name hasn't already been taken up by another contact
	/// </summary>
	/// <param name="name"> The current string in the name input field </param>
	private void ContactNameChanged(string name)
	{
		ValidName = SecurePlayerPrefs.HasKey(name) ? (addingContact ? false : true) : true;

		SetMainButtonInteractable();
	}

	/// <summary>
	/// Checks to see if the text is a valid ethereum address
	/// </summary>
	/// <param name="address"> The current string in the address input field </param>
	private void AddressChanged(string address)
	{
		if (!AddressUtils.CorrectAddressLength(address))
			addressInputField.text = address.LimitEnd(42);

		string updatedAddress = addressInputField.text;

		ValidAddress = string.IsNullOrEmpty(updatedAddress) ? true : AddressUtils.IsValidEthereumAddress(updatedAddress);

		SetMainButtonInteractable();
	}

	/// <summary>
	/// Sets the main button to interactable depending on if the inputs are not empty, and are valid
	/// </summary>
	private void SetMainButtonInteractable()
	{
		bool validInputs = !string.IsNullOrEmpty(nameInputField.text) && validName && !string.IsNullOrEmpty(addressInputField.text) && validAddress;

		if (addingContact)
			addContactButton.GetComponent<Button>().interactable = validInputs;
		else
			confirmButton.GetComponent<Button>().interactable = validInputs;
	}

	/// <summary>
	/// Disables the menu
	/// </summary>
	private void ConfirmButtonClicked() => AnimateDisable();
}
