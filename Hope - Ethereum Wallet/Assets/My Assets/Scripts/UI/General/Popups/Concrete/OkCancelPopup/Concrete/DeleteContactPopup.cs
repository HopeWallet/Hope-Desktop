using TMPro;
using UnityEngine;
using Zenject;

public sealed class DeleteContactPopup : OkCancelPopupComponent<DeleteContactPopup>
{

	public TextMeshProUGUI contactName;
	public TextMeshProUGUI contactAddress;

	private GameObject contactObject;
	private ContactButton contactButton;
	private ContactsPopup contactsPopup;
	private ContactsManager contactsManager;

	/// <summary>
	/// Sets the required dependency for the DeleteContactPopup
	/// </summary>
	/// <param name="contactsManager"> The active ContactsManager </param>
	[Inject]
	public void Construct(ContactsManager contactsManager) => this.contactsManager = contactsManager;

	/// <summary>
	/// Deletes the selected contact from the dictionary, SecurePlayerPrefs and also removes the contactButton from the scene
	/// </summary>
	protected override void OnOkClicked()
	{
		string address = contactAddress.text;
		int index = contactsManager.ContactOrders[address];

		SecurePlayerPrefs.DeleteKey(address);
		contactsManager.RemoveContact(address);
		MoveContacts(index);

		if (contactButton == contactsPopup.ActiveContactButton)
		{
			contactsPopup.ActiveContactButton = null;
			contactsPopup.confirmButton.interactable = false;
		}

		contactObject.AnimateScaleX(0f, 0.1f, () => DestroyImmediate(contactObject));
	}

	/// <summary>
	/// Moves all of the contacts above the current one being deleted
	/// </summary>
	/// <param name="index"> The index of the contact being moved </param>
	private void MoveContacts(int index)
	{
		if (SecurePlayerPrefs.HasKey("contact_" + (index + 1)))
		{
			string nextAddress = SecurePlayerPrefs.GetString("contact_" + (index + 1));

			contactsManager.ContactOrders.Remove(nextAddress);
			contactsManager.ContactOrders.Add(nextAddress, index);
			SecurePlayerPrefs.SetString("contact_" + index, nextAddress);
			MoveContacts(++index);
		}

		else
			SecurePlayerPrefs.DeleteKey("contact_" + index);
	}

	/// <summary>
	/// Sets the details of the popup to fit the contact being deleted
	/// </summary>
	/// <param name="name"> The contact name </param>
	/// <param name="address"> The contact address </param>
	/// <param name="contactButton"> The contactButton </param>
	/// <param name="contactsPopup"> The ContactsPopup </param>
	public void SetContactDetails(string name, string address, ContactButton contactButton, ContactsPopup contactsPopup)
	{
		contactName.text = name;
		contactAddress.text = address;
		this.contactButton = contactButton;
		contactObject = contactButton.transform.parent.gameObject.gameObject;
		this.contactsPopup = contactsPopup;
	}
}
