using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Hope.Utils.EthereumUtils;

public class AddOrEditContactPopupAnimator : UIAnimator
{

	[SerializeField] private Image blur;
	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
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

    private bool ValidName
	{
		set
		{
			validName = value;
			AnimateErrorIcon(nameSection, validName);
		}
	}

	private bool ValidAddress
	{
		set
		{
			validAddress = value;
			AnimateErrorIcon(addressSection, validAddress);
		}
	}

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
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		blur.AnimateMaterialBlur(0.25f, 0.2f);
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
	protected override void AnimateOut()
	{
		AnimateMainButton(false);

		nameSection.AnimateScaleX(0f, 0.15f,
			() => title.AnimateScaleX(0f, 0.15f,
			() => { blur.AnimateMaterialBlur(-0.25f, 0.2f); dim.AnimateGraphic(0f, 0.15f); }));
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
		if (AddingContact)
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
		if (name.Length > 30)
			nameInputField.text = name.LimitEnd(30);

		string updatedName = nameInputField.text;

		ValidName = !string.IsNullOrEmpty(updatedName.Trim());

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

		string updatedAddress = addressInputField.text.ToLower();

		bool realEthereumAddress = string.IsNullOrEmpty(updatedAddress) || AddressUtils.IsValidEthereumAddress(updatedAddress);
		bool overridingOtherContactAddresses = contactsManager.ContactList.Contains(updatedAddress) && (!AddingContact ? updatedAddress != PreviousAddress : true);

		ValidAddress = realEthereumAddress && !overridingOtherContactAddresses;

		if (!realEthereumAddress)
			addOrEditContactPopup.SetAddressErrorBodyText("The inputted text is not a valid Ethereum address.");

		else if (overridingOtherContactAddresses)
			addOrEditContactPopup.SetAddressErrorBodyText("This address has already been saved under the contact name: " + contactsManager.ContactList[updatedAddress].name + ".");

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
	/// Animates the error icon in a given section
	/// </summary>
	/// <param name="objectSection"> THe GameObject parent of the section </param>
	/// <param name="isValid"> Checks if the input is valid or not </param>
	private void AnimateErrorIcon(GameObject objectSection, bool isValid)
	{
		objectSection.transform.GetChild(objectSection.transform.childCount - 1).gameObject.AnimateGraphicAndScale(isValid ? 0f : 1f, isValid ? 0f : 1f, 0.2f);
	}
}
