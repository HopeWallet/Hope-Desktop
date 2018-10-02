using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// InfoButton class that manages the details of the contact
/// </summary>
public sealed class ContactButton : InfoButton<ContactButton, ContactInfo>
{
	[SerializeField] private TextMeshProUGUI contactName, contactAddress;
	[SerializeField] private Button editButton, deleteButton;

	private PopupManager popupManager;
	private ContactsManager contactsManager;
	private ContactsPopup contactsPopup;

	public string RealContactName { get; private set; }

	public string RealContactAddress { get; private set; }

	/// <summary>
	/// Adds the required dependencies to the ContactButton
	/// </summary>
	/// <param name="popupManager"> The PopupManager </param>
	/// <param name="contactsManager"> The active ContactsManager </param>
	[Inject]
	public void Construct(PopupManager popupManager, ContactsManager contactsManager)
	{
		this.popupManager = popupManager;
		this.contactsManager = contactsManager;
	}

	/// <summary>
	/// Sets the button listeners
	/// </summary>
	protected override void OnAwake()
	{
		Button.onClick.AddListener(() => ButtonInfo.ContactsPopup.EnableNewContactButton(this));
		editButton.onClick.AddListener(() => popupManager.GetPopup<AddOrEditContactPopup>(true).SetPopupLayout(false, RealContactName, RealContactAddress, this));
		deleteButton.onClick.AddListener(() => popupManager.GetPopup<GeneralOkCancelPopup>(true).SetSubText("Are you sure you want to delete the " + RealContactName + " contact?").OnOkClicked(DeleteThisContact));
	}

	/// <summary>
	/// Sets the button variables to the corresponding ones in the ContactInfo
	/// </summary>
	/// <param name="info"> The ContactInfo of this contact </param>
	protected override void OnValueUpdated(ContactInfo info)
	{
		RealContactName = info.ContactName;
		RealContactAddress = info.ContactAddress;
		contactsPopup = info.ContactsPopup;

		contactName.text = RealContactName?.LimitEnd(20, "...");
		contactAddress.text = RealContactAddress?.Substring(0, 8) + "...." + RealContactAddress.Substring(RealContactAddress.Length - 8, 8);
	}

	/// <summary>
	/// Updates the current contact details to what the user has entered
	/// </summary>
	/// <param name="address"> The new contact address </param>
	/// <param name="name"> The new contact name </param>
	public void UpdateContactDetails(string address, string name) => SetButtonInfo(new ContactInfo(ButtonInfo.ContactsPopup, address.ToLower(), name));

	/// <summary>
	/// Deletes this contact from the contact list
	/// </summary>
	private void DeleteThisContact()
	{
		GameObject contactObject = transform.parent.gameObject;

		contactsManager.RemoveContact(RealContactAddress);

		if (contactsPopup.ActiveContactButton == this)
		{
			contactsPopup.ActiveContactButton = null;
			contactsPopup.confirmButton.interactable = false;
		}

		contactObject.AnimateScaleX(0f, 0.1f, () => DestroyImmediate(contactObject));
	}
}
