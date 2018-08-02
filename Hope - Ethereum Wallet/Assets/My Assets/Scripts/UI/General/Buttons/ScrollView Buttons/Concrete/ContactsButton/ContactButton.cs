using TMPro;
using UnityEngine.UI;
using Zenject;
using System.Collections.Generic;

public class ContactButton : InfoButton<ContactButton, ContactInfo>
{

	public TextMeshProUGUI contactName, contactAddress;
	public Button editButton, deleteButton;

	private PopupManager popupManager;

	private Dictionary<string, string> contacts;

	[Inject]
	public void Construct(PopupManager popupManager)
	{
		this.popupManager = popupManager;
	}

	protected override void OnAwake()
	{
		Button.onClick.AddListener(ContactClicked);
		editButton.onClick.AddListener(EditContact);
	}

	/// <summary>
	/// Sets the contacts dictionary
	/// </summary>
	/// <param name="contacts"> The contacts dictionary </param>
	public void SetDictionary(Dictionary<string, string> contacts) => this.contacts = contacts;

	protected override void OnValueUpdated(ContactInfo info)
	{
		contactName.text = info.ContactName?.LimitEnd(18, "...");
		contactAddress.text = info.ContactAddress?.Substring(0, 8) + "...." + info.ContactAddress.Substring(info.ContactAddress.Length - 8, 8);
	}

	private void ContactClicked() => ButtonInfo.ContactsPopup.EnableNewContactButton(this);

	private void EditContact()
	{
		popupManager.GetPopup<AddOrEditContactPopup>(true).SetPopupLayout(false, contactName.text, contactAddress.text);
		//popupManager.GetPopup<AddOrEditContactPopup>(true).SetDictionary(contacts);
	}

	private void DeleteContact()
	{

	}
}
