using System.Collections.Generic;
using System.Linq;
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
		popupManager.GetPopup<AddOrEditContactPopup>(true).SetPopupLayout(true);
		popupManager.GetPopup<AddOrEditContactPopup>(true).SetDictionary(contacts);
	}

	private void EditContact()
	{
		popupManager.GetPopup<AddOrEditContactPopup>(true).SetPopupLayout(false, contactsPopupAnimator.SelectedContactName, contactsPopupAnimator.SelectedContactAddress);
		popupManager.GetPopup<AddOrEditContactPopup>(true).SetDictionary(contacts);
	}

	private void DeleteContact()
	{
		//Open up the DeleteContactPopup
	}

	//private void SearchBarChanged(string str)
	//{
	//	var contactsContainingSearch = contacts.Where(s => s.Key.StartsWith(str) || s.Value.Contains(str)).ToDictionary(k => k.Key, v => v.Value);
	//}
}
