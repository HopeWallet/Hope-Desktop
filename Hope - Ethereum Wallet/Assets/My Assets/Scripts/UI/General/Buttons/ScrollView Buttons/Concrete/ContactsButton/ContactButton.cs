using TMPro;
using UnityEngine.UI;
using Zenject;

public sealed class ContactButton : InfoButton<ContactButton, ContactInfo>
{

	public TextMeshProUGUI contactName, contactAddress;
	public Button editButton, deleteButton;

	private PopupManager popupManager;
	private ContactsManager contactsManager;

	private string realContactName, realContactAddress;

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
		editButton.onClick.AddListener(() => popupManager.GetPopup<AddOrEditContactPopup>(true).SetPopupLayout(false, realContactName, realContactAddress, this));
		deleteButton.onClick.AddListener(() => popupManager.GetPopup<DeleteContactPopup>(true).SetContactDetails(realContactName, realContactAddress, transform.parent.gameObject));
	}

	/// <summary>
	/// Sets the button variables to the corresponding ones in the ContactInfo
	/// </summary>
	/// <param name="info"></param>
	protected override void OnValueUpdated(ContactInfo info)
	{
		string name = info.ContactName;
		string address = info.ContactAddress;

		realContactName = name;
		realContactAddress = address;

		contactName.text = name?.LimitEnd(18, "...");
		contactAddress.text = address?.Substring(0, 8) + "...." + address.Substring(address.Length - 8, 8);
	}

	/// <summary>
	/// Updates the current contact details to what the user has entered
	/// </summary>
	/// <param name="name"> The new contact name </param>
	/// <param name="address"> The new contact address </param>
	public void UpdateContactDetails(string name, string address) => SetButtonInfo(new ContactInfo(ButtonInfo.ContactsPopup, name, address));
}
