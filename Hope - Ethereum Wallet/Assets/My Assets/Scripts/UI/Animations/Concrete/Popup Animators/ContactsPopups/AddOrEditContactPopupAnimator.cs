using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Hope.Utils.Ethereum;

public class AddOrEditContactPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject nameSection;
	[SerializeField] private GameObject addressSection;
	[SerializeField] private GameObject addContactButton;
	[SerializeField] private GameObject confirmButton;

	private AddOrEditContactPopup addOrEditContactPopup;
	public ContactsManager contactsManager;

	private TMP_InputField nameInputField;
	private TMP_InputField addressInputField;

	private bool validName;
	private bool validAddress;

    public string PreviousAddress { get; set; }

    public bool AddingContact { get; set; }

	/// <summary>
	/// Initializes the elements
	/// </summary>
	private void Awake()
	{
		addOrEditContactPopup = transform.GetComponent<AddOrEditContactPopup>();

		nameInputField = nameSection.transform.GetChild(2).GetComponent<TMP_InputField>();
		addressInputField = addressSection.transform.GetChild(2).GetComponent<TMP_InputField>();

		nameInputField.onValueChanged.AddListener(ContactNameChanged);
		addressInputField.onValueChanged.AddListener(AddressChanged);
	}

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		nameSection.AnimateScaleX(1f, 0.2f);
		addressSection.AnimateScaleX(1f, 0.25f);
		AnimateMainButton(true);
	}

	/// <summary>
	/// Animates the main button, depending on the boolean addingContact
	/// </summary>
	/// <param name="animateIn"> Checks if animating the button in or out </param>
	private void AnimateMainButton(bool animateIn)
	{
		float endValue = animateIn ? 1f : 0f;

		if (AddingContact)
			addContactButton.AnimateGraphicAndScale(endValue, endValue, animateIn ? 0.3f : 0.2f);
		else
			confirmButton.AnimateGraphicAndScale(endValue, endValue, animateIn ? 0.3f : 0.2f);

		if (animateIn) FinishedAnimating();
	}

	/// <summary>
	/// Checks if the name hasn't already been taken up by another contact
	/// </summary>
	/// <param name="name"> The current string in the name input field </param>
	private void ContactNameChanged(string name)
	{
		if (name.Length > 30)
			nameInputField.text = name.LimitEnd(30);

		string updatedName = nameInputField.text;

		validName = !string.IsNullOrEmpty(updatedName.Trim());
		AnimateErrorIcon(addOrEditContactPopup.nameErrorIcon, validName);

		SetMainButtonInteractable();
	}

	/// <summary>
	/// Checks to see if the text is a valid ethereum address
	/// </summary>
	/// <param name="address"> The current string in the address input field </param>
	private void AddressChanged(string address)
	{
		if (!AddressUtils.CorrectAddressLength(address, AddressUtils.ADDRESS_LENGTH))
			addressInputField.text = address.LimitEnd(AddressUtils.ADDRESS_LENGTH);

		string updatedAddress = addressInputField.text.ToLower();

		bool realEthereumAddress = string.IsNullOrEmpty(updatedAddress) || AddressUtils.IsValidEthereumAddress(updatedAddress);
		bool overridingOtherContactAddresses = contactsManager.ContactList.Contains(updatedAddress) && (!AddingContact ? updatedAddress != PreviousAddress : true);

		validAddress = realEthereumAddress && !overridingOtherContactAddresses;
		AnimateErrorIcon(addOrEditContactPopup.addressErrorIcon, validAddress);

		if (!realEthereumAddress)
			addOrEditContactPopup.SetAddressErrorBodyText("This is not a valid Ethereum address.");
		else if (overridingOtherContactAddresses)
			addOrEditContactPopup.SetAddressErrorBodyText("This address has already been saved under the contact name: " + contactsManager.ContactList[updatedAddress].ContactName + ".");

		SetMainButtonInteractable();
	}

	/// <summary>
	/// Sets the main button to interactable depending on if the inputs are not empty, and are valid
	/// </summary>
	private void SetMainButtonInteractable()
	{
		bool validInputs = !string.IsNullOrEmpty(nameInputField.text) && validName && !string.IsNullOrEmpty(addressInputField.text) && validAddress;

		if (AddingContact)
			addContactButton.GetComponent<Button>().interactable = validInputs;
		else
			confirmButton.GetComponent<Button>().interactable = validInputs;
	}

	/// <summary>
	/// Animates the given icon
	/// </summary>
	/// <param name="icon"> The icon being animated </param>
	/// <param name="isValid"> Whether being animated in or out </param>
	private void AnimateErrorIcon(InteractableIcon icon, bool isValid) => icon.AnimateIcon(isValid ? 0f : 1f);
}
