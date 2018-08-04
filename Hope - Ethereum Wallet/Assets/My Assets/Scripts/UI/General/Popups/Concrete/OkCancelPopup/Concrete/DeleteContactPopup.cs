using TMPro;
using UnityEngine;
using Zenject;

public sealed class DeleteContactPopup : OkCancelPopupComponent<DeleteContactPopup>
{

	public TextMeshProUGUI contactName;
	public TextMeshProUGUI contactAddress;

	private GameObject contactObject;

	private ContactsManager contactsManager;

	[Inject]
	public void Construct(ContactsManager contactsManager) => this.contactsManager = contactsManager;

	protected override void OnOkClicked()
	{
		string addressInObject = contactAddress.text;

		for (int i = 1; i <= SecurePlayerPrefs.GetInt("Contacts"); i++)
		{
			string currentAddressInLoop = SecurePlayerPrefs.GetString("contact_" + i);

			if (currentAddressInLoop == addressInObject)
			{
				SecurePlayerPrefs.DeleteKey(currentAddressInLoop);
				SecurePlayerPrefs.DeleteKey("contact_" + i);
				MoveContacts(i);
				break;
			}
		}

		contactsManager.Contacts.Remove(addressInObject);
		SecurePlayerPrefs.SetInt("Contacts", contactsManager.Contacts.Count);

		contactObject.AnimateScaleX(0f, 0.1f, () => DestroyImmediate(contactObject));
	}

	private void MoveContacts(int index)
	{
		if (SecurePlayerPrefs.HasKey("contact_" + (index + 1)))
		{
			SecurePlayerPrefs.SetString("contact_" + index, SecurePlayerPrefs.GetString("contact_" + (index + 1)));
			MoveContacts(index + 1);
		}
	}

	public void SetContactDetails(string name, string address, GameObject contactObject)
	{
		contactName.text = name;
		contactAddress.text = address;
		this.contactObject = contactObject;
	}
}
