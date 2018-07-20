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

	private bool ValidName
	{
		set
		{
			validName = value;
			nameSection.transform.GetChild(nameSection.transform.childCount - 1).gameObject
					.AnimateGraphicAndScale(validName ? 0f : 1f, validName ? 0f : 1f, 0.2f);
		}
	}

	private bool ValidAddress
	{
		set
		{
			validAddress = value;
			addressSection.transform.GetChild(addressSection.transform.childCount - 1).gameObject
					.AnimateGraphicAndScale(validAddress ? 0f : 1f, validAddress ? 0f : 1f, 0.2f);
		}
	}

	private void Awake()
	{
		SetAddingContact(true);

		nameInputField = nameSection.transform.GetChild(2).GetComponent<TMP_InputField>();
		addressInputField = addressSection.transform.GetChild(2).GetComponent<TMP_InputField>();

		nameInputField.onValueChanged.AddListener(ContactNameChanged);
		addressInputField.onValueChanged.AddListener(AddressChanged);

		confirmButton.GetComponent<Button>().onClick.AddListener(ConfirmButtonClicked);
	}

	public void SetAddingContact(bool addingContact, string name = "", string address = "")
	{
		this.addingContact = addingContact;
		nameInputField.text = name;
		addressInputField.text = address;
		title.GetComponent<TextMeshProUGUI>().text = addingContact ? "A D D  C O N T A C T" : "E D I T  C O N T A C T";
	}

	protected override void AnimateIn()
	{
		dim.AnimateGraphic(1f, 0.2f);
		form.AnimateGraphicAndScale(1f, 1f, 0.2f,
		() => title.AnimateScaleX(1f, 0.15f,
		() => nameSection.AnimateScaleX(1f, 0.15f,
		() => addressSection.AnimateScaleX(1f, 0.15f,
		() => AnimateMainButton(true)))));
	}

	protected override void AnimateOut()
	{
	}

	private void AnimateMainButton(bool animatingIn)
	{
		if (addingContact)
			addContactButton.AnimateScaleX(animatingIn ? 1f : 0f, 0.15f);
		else
			confirmButton.AnimateScaleX(animatingIn ? 1f : 0f, 0.15f);

		if (animatingIn) FinishedAnimating();
	}

	private void ContactNameChanged(string name)
	{
		try
		{
			SecurePlayerPrefs.GetString(name);
			ValidName = addingContact ? false : true;
		}

		catch { ValidName = true; }

		SetMainButtonInteractable();
	}

	private void AddressChanged(string address)
	{
		if (!AddressUtils.CorrectAddressLength(address))
			addressInputField.text = address.LimitEnd(42);

		string updatedAddress = addressInputField.text;

		ValidAddress = string.IsNullOrEmpty(updatedAddress) ? true : AddressUtils.IsValidEthereumAddress(updatedAddress);

		SetMainButtonInteractable();
	}

	private void SetMainButtonInteractable()
	{
		bool validInputs = !string.IsNullOrEmpty(nameInputField.text) && !string.IsNullOrEmpty(addressInputField.text) && validAddress;

		if (addingContact)
			addContactButton.GetComponent<Button>().interactable = validInputs;
		else
			confirmButton.GetComponent<Button>().interactable = validInputs;
	}

	private void ConfirmButtonClicked() => AnimateDisable();
}
