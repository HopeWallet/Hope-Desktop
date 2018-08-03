using TMPro;
using UnityEngine.UI;
using Zenject;

public class ContactButton : InfoButton<ContactButton, ContactInfo>
{

	public TextMeshProUGUI contactName, contactAddress;
	public Button editButton, deleteButton;

	private PopupManager popupManager;
	private ContactsManager contactsManager;

	private string realContactName, realContactAddress;

	[Inject]
	public void Construct(PopupManager popupManager, ContactsManager contactsManager)
	{
		this.popupManager = popupManager;
		this.contactsManager = contactsManager;
	}

	protected override void OnAwake()
	{
		Button.onClick.AddListener(() => ButtonInfo.ContactsPopup.EnableNewContactButton(this));
		editButton.onClick.AddListener(() => popupManager.GetPopup<AddOrEditContactPopup>(true).SetPopupLayout(false, realContactName, realContactAddress));
		deleteButton.onClick.AddListener(() => popupManager.GetPopup<DeleteContactPopup>(true).SetContactDetails(realContactName, realContactAddress, transform.parent.gameObject));
	}

	protected override void OnValueUpdated(ContactInfo info)
	{
		realContactName = info.ContactName;
		realContactAddress = info.ContactAddress;

		contactName.text = realContactName?.LimitEnd(18, "...");
		contactAddress.text = realContactAddress?.Substring(0, 8) + "...." + info.ContactAddress.Substring(info.ContactAddress.Length - 8, 8);
	}
}
