using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContactsPopup : ExitablePopupComponent<ContactsPopup>
{

	public Button addContactButton,
				  editContactButton,
				  deleteContactButton;

	private ContactsPopupAnimator contactsPopupAnimator;

	public const string PREF_NAME = "contact_";

	private Dictionary<string, string> contacts { get; } = new Dictionary<string, string>();

	protected override void OnStart()
	{
		PopulateContacts();

		contactsPopupAnimator = transform.GetComponent<ContactsPopupAnimator>();

		addContactButton.onClick.AddListener(AddContact);
		editContactButton.onClick.AddListener(EditContact);
		deleteContactButton.onClick.AddListener(DeleteContact);
	}

	private void PopulateContacts()
	{
		for (int i = 0; ; i++)
		{
			if (SecurePlayerPrefs.HasKey(PREF_NAME + i))
				return;

			string name = SecurePlayerPrefs.GetString(PREF_NAME + i);
			contacts.Add(name, SecurePlayerPrefs.GetString(name));
		}
	}

	private void AddContact()
	{
		var addOrCreateContactPopup = popupManager.GetPopup<AddOrCreateContactPopup>();

		addOrCreateContactPopup.SetPopupLayout(true);
		addOrCreateContactPopup.SetDictionary(contacts);
	}

	private void EditContact()
	{
		var addOrCreateContactPopup = popupManager.GetPopup<AddOrCreateContactPopup>();

		addOrCreateContactPopup.SetPopupLayout(false, contactsPopupAnimator.selectedContactName, contactsPopupAnimator.selectedContactAddress);
		addOrCreateContactPopup.SetDictionary(contacts);
	}

	private void DeleteContact()
	{
	}
}
