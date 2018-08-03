using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AddOrEditContactPopup : ExitablePopupComponent<AddOrEditContactPopup>
{

	private string previousName, previousAddress;
	public Button addContactButton, confirmButton;
	public TMP_InputField nameInputField, addressInputField;
	public TextMeshProUGUI title;

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
	}

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
		contactsManager.Contacts.Add(addressInputField.text, nameInputField.text);

		SecurePlayerPrefs.SetInt("Contacts", contactsManager.Contacts.Count);
		SecurePlayerPrefs.SetString("contact_" + contactsManager.Contacts.Count, addressInputField.text);
		SecurePlayerPrefs.SetString(addressInputField.text, nameInputField.text);

		CreateNewContactObjectInList();

		popupManager.CloseActivePopup();
	}

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
		contactsManager.Contacts.Remove(previousAddress);
		contactsManager.Contacts.Add(addressInputField.text, nameInputField.text);

		if (SecurePlayerPrefs.HasKey(addressInputField.text))
			SecurePlayerPrefs.SetString(addressInputField.text, nameInputField.text);
		else
		{
			for (int i = 0; i < contactsManager.Contacts.Count; i++)
			{
				string prefName = "contact_" + i;

				if (SecurePlayerPrefs.GetString(prefName) == previousAddress)
				{
					SecurePlayerPrefs.DeleteKey(previousAddress);

					SecurePlayerPrefs.SetString(prefName, addressInputField.text);
					SecurePlayerPrefs.SetString(addressInputField.text, nameInputField.text);
				}
			}
		}

		popupManager.CloseActivePopup();
	}

	/// <summary>
	/// Sets all the necessary popup text elements
	/// </summary>
	/// <param name="addingContact"> Checks if adding a contact or editing an existing one </param>
	/// <param name="name"> The current name of the contact </param>
	/// <param name="address"> The current address of the contact </param>
	public void SetPopupLayout(bool addingContact, string name = "", string address = "")
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
		title.GetComponent<TextMeshProUGUI>().text = addingContact ? "A D D  C O N T A C T" : "E D I T  C O N T A C T";
	}
}
