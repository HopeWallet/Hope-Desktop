using System.Collections.Generic;
using UnityEngine.UI;

public class ContactsPopup : ExitablePopupComponent<ContactsPopup>
{

	public Button addContactButton,
				  editContactButton,
				  deleteContactButton;

	private ContactsPopupAnimator contactsPopupAnimator;

    private readonly Dictionary<string, string> contacts = new Dictionary<string, string>();

    public const string PREF_NAME = "contact_";

	protected override void OnStart()
	{
		PopulateContacts();

		contactsPopupAnimator = transform.GetComponent<ContactsPopupAnimator>();

		addContactButton.onClick.AddListener(AddContact);
		//editContactButton.onClick.AddListener(EditContact);
		//deleteContactButton.onClick.AddListener(DeleteContact);
	}

	private void PopulateContacts()
	{
		for (int i = 0; ; i++)
		{
			if (!SecurePlayerPrefs.HasKey(PREF_NAME + i))
				return;

			string name = SecurePlayerPrefs.GetString(PREF_NAME + i);
			contacts.Add(name, SecurePlayerPrefs.GetString(name));
		}
	}

	private void AddContact()
	{
		var addOrCreateContactPopup = popupManager.GetPopup<AddOrEditContactPopup>(true);

		addOrCreateContactPopup.SetPopupLayout(true);
		addOrCreateContactPopup.SetDictionary(contacts);
	}

	private void EditContact()
	{
		var addOrCreateContactPopup = popupManager.GetPopup<AddOrEditContactPopup>(true);

		addOrCreateContactPopup.SetPopupLayout(false, contactsPopupAnimator.SelectedContactName, contactsPopupAnimator.SelectedContactAddress);
		addOrCreateContactPopup.SetDictionary(contacts);
	}

	private void DeleteContact()
	{
	}
}
