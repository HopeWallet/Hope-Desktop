using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public sealed class AddOrEditContactPopup : ExitablePopupComponent<AddOrEditContactPopup>
{

	public Button addContactButton, confirmButton;
	public TMP_InputField nameInputField, addressInputField;
	public TextMeshProUGUI title;

	public InfoMessage nameError, addressError;

	private string previousName, previousAddress;
	private ContactButton contactButton;

	private AddOrEditContactPopupAnimator addOrEditContactPopupAnimator;
	private ContactButton.Factory contactButtonFactory;
	private ContactsManager contactsManager;
	private ContactsPopup contactsPopup;

	/// <summary>
	/// Adds the button listeners
	/// </summary>
	protected override void OnStart()
	{
		addContactButton.onClick.AddListener(AddContactClicked);
		confirmButton.onClick.AddListener(ConfirmClicked);

		nameError.PopupManager = popupManager;
		addressError.PopupManager = popupManager;
	}

	/// <summary>
	/// Adds the required dependencies to the AddOrEditContatPopup
	/// </summary>
	/// <param name="contactButtonFactory"> The active ContactButtonFactory</param>
	/// <param name="contactsManager"> The active ContactsManager</param>
	[Inject]
	public void Construct(ContactButton.Factory contactButtonFactory, ContactsManager contactsManager)
	{
		this.contactButtonFactory = contactButtonFactory;
		this.contactsManager = contactsManager;
	}

	/// <summary>
	/// Sets this ContactsPopup to the original instance
	/// </summary>
	/// <param name="contactsPopup"> The ContactsPopup </param>
	public void SetContactsPopup(ContactsPopup contactsPopup) => this.contactsPopup = contactsPopup;

	/// <summary>
	/// Adds a new contact according to the given inputs
	/// </summary>
	private void AddContactClicked()
	{
		string name = nameInputField.text;
		string address = addressInputField.text;
		int contactsCount = contactsManager.Contacts.Count + 1;

		contactsManager.AddContact(address, name, contactsCount);
		SecurePlayerPrefs.SetString("contact_" + contactsCount, address);
		SecurePlayerPrefs.SetString(address, name);

		CreateNewContactObjectInList();

		popupManager.CloseActivePopup();
	}

	/// <summary>
	/// Creates a new ContactButton in the current contact list
	/// </summary>
	private void CreateNewContactObjectInList()
	{
		var button = contactButtonFactory.Create();

		button.SetButtonInfo(new ContactInfo(contactsPopup, nameInputField.text, addressInputField.text));

		Transform contactObject = button.transform.parent;
		contactObject.parent = contactsPopup.contactsListTransform;
		contactObject.localScale = new Vector3(0f, 1f, 1f);
		contactObject.gameObject.AnimateScaleX(1f, 0.1f);
	}

	/// <summary>
	/// Edits the existing contact and replaces it with the new values
	/// </summary>
	private void ConfirmClicked()
	{
		string newName = nameInputField.text;
		string newAddress = addressInputField.text;
		int index = contactsManager.ContactOrders[previousAddress];

		contactsManager.EditContact(newAddress, previousAddress, newName, index);

		if (newAddress == previousAddress)
			SecurePlayerPrefs.SetString(newAddress, newName);
		else
		{
			SecurePlayerPrefs.DeleteKey(previousAddress);

			SecurePlayerPrefs.SetString("contact_" + index, newAddress);
			SecurePlayerPrefs.SetString(newAddress, newName);
		}

		contactButton.UpdateContactDetails(newAddress, newName);

		popupManager.CloseActivePopup();
	}

	/// <summary>
	/// Sets all the necessary popup text elements
	/// </summary>
	/// <param name="addingContact"> Checks if adding a contact or editing an existing one </param>
	/// <param name="name"> The current name of the contact </param>
	/// <param name="address"> The current address of the contact </param>
	public void SetPopupLayout(bool addingContact, string name = "", string address = "", ContactButton contactButton = null)
	{
		addOrEditContactPopupAnimator = transform.GetComponent<AddOrEditContactPopupAnimator>();
		addOrEditContactPopupAnimator.AddingContact = addingContact;
		addOrEditContactPopupAnimator.PreviousAddress = address;

		if (!addingContact)
		{
			previousName = name;
			previousAddress = address;
		}

		nameInputField.text = name;
		addressInputField.text = address;
		this.contactButton = contactButton;
		title.GetComponent<TextMeshProUGUI>().text = addingContact ? "A D D  C O N T A C T" : "E D I T  C O N T A C T";
	}

	/// <summary>
	/// Sets the addressError body message 
	/// </summary>
	/// <param name="bodyText"> The custom body message </param>
	public void SetAddressErrorBodyText(string bodyText) => addressError.bodyText = bodyText;
}
