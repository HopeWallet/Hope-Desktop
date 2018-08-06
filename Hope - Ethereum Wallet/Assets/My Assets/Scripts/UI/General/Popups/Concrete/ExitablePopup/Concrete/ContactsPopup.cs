using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Linq;
using System.Collections.Generic;

public sealed class ContactsPopup : ExitablePopupComponent<ContactsPopup>
{
	public Button addContactButton, confirmButton;
	public Transform contactsListTransform;
	public TMP_Dropdown sortByDropdown;

	private SendAssetPopup sendAssetPopup;
	private ContactsPopupAnimator contactsPopupAnimator;
	private ContactButton.Factory contactButtonFactory;
	private ContactsManager contactsManager;

	private string selectedContactAddress;

	public ContactButton ActiveContactButton { get; set; }

	/// <summary>
	/// Adds the required dependencies to the ContactsPopup
	/// </summary>
	/// <param name="contactButtonFactory"> The active ContactButtonFactory </param>
	/// <param name="contactsManager"> The active ContactsManager </param>
	[Inject]
	public void Construct(ContactButton.Factory contactButtonFactory, ContactsManager contactsManager)
	{
		this.contactButtonFactory = contactButtonFactory;
		this.contactsManager = contactsManager;
	}

	/// <summary>
	/// Sets the active SendActivePopup
	/// </summary>
	/// <param name="sendAssetPopup"> The active SendAssetPopup instance </param>
	public void SetSendAssetPopup(SendAssetPopup sendAssetPopup) => this.sendAssetPopup = sendAssetPopup;

	/// <summary>
	/// Adds button listeners and also adds the contact objects for the number of saved contacts
	/// </summary>
	protected override void Awake()
	{
		base.Awake();

		contactsPopupAnimator = transform.GetComponent<ContactsPopupAnimator>();

		addContactButton.onClick.AddListener(AddContact);
		confirmButton.onClick.AddListener(ConfirmButtonClicked);
		sortByDropdown.onValueChanged.AddListener(ListOrderChanged);

		AddContactButtons();
	}

	/// <summary>
	/// Sets the active contact button if the inputted address is from a saved contact
	/// </summary>
	protected override void OnStart()
	{
		if (!string.IsNullOrEmpty(sendAssetPopup.Address.contactName.text))
		{
			string inputtedAddress = sendAssetPopup.Address.addressField.text;

			for (int i = 0; i < contactsListTransform.childCount; i++)
			{
				ContactButton contactButton = contactsListTransform.GetChild(i).GetChild(0).GetComponent<ContactButton>();

				if (contactButton.RealContactAddress == inputtedAddress)
					EnableNewContactButton(contactButton);
			}
		}
	}

	/// <summary>
	/// Adds the contact buttons accourding to all the saved contacts in the SecurePlayerPrefs
	/// </summary>
	private void AddContactButtons()
	{
		if (contactsManager.Contacts.Count == 0)
			return;

		for (int i = 1; i <= contactsManager.Contacts.Count; i++)
		{
			var button = contactButtonFactory.Create();
			var address = SecurePlayerPrefs.GetString("contact_" + i);
			button.SetButtonInfo(new ContactInfo(this, SecurePlayerPrefs.GetString(address), address));

			Transform buttonParent = button.transform.parent;
			buttonParent.parent = contactsListTransform;
			buttonParent.localScale = new Vector3(0f, 1f, 1f);
		}
	}

	/// <summary>
	/// Gets The currently selected contact details and closes the popup
	/// </summary>
	private void ConfirmButtonClicked()
	{
		sendAssetPopup.Address.addressField.text = ActiveContactButton.RealContactAddress;
		popupManager.CloseActivePopup();
	}

	/// <summary>
	/// Opens up the AddOrEditContactPopup
	/// </summary>
	private void AddContact()
	{
		popupManager.GetPopup<AddOrEditContactPopup>(true).SetPopupLayout(true);
		popupManager.GetPopup<AddOrEditContactPopup>(true).SetContactsPopup(this);
	}

	/// <summary>
	/// List order has been changed
	/// </summary>
	/// <param name="value"> The value of the sorting type in the dropdown </param>
	private void ListOrderChanged(int value)
	{
		if (contactsManager.Contacts.Count == 0)
			return;

		var orderedList = new List<KeyValuePair<string, int>>();
		if (ActiveContactButton != null) selectedContactAddress = ActiveContactButton.RealContactAddress;

		if (value == 0) //Sort by oldest
			orderedList = contactsManager.ContactOrders.OrderBy(p => p.Value).ToList();

		else if (value == 1) //Sort by newest
			orderedList = contactsManager.ContactOrders.OrderByDescending(p => p.Value).ToList();

		else //Sort by alphabetical
		{
			//orderedList = contactsManager.Contacts.OrderBy(p => p.Value).ToList();
		}

		//orderedList.ForEach(p => Debug.Log(p.Key + " => " + p.Value));
		SetOrderedList(orderedList);
	}

	private void DoStuff<T, V>()
	{
		var orderedList = new List<KeyValuePair<T, V>>();
	}

	/// <summary>
	/// Manages the interactables of the newly clicked and old contact button that was pressed
	/// </summary>
	/// <param name="contactButton"> The ContactButton that was just clicked </param>
	public void EnableNewContactButton(ContactButton contactButton)
	{
		if (ActiveContactButton != null)
			ActiveContactButton.Button.interactable = true;

		contactButton.Button.interactable = false;
		ActiveContactButton = contactButton;
		confirmButton.interactable = true;
	}

	private void SetOrderedList(List<KeyValuePair<string, int>> orderedList)
	{
		for (int i = 0; i < contactsManager.Contacts.Count; i++)
		{
			ContactButton currentContactButton = contactsListTransform.GetChild(i).GetChild(0).GetComponent<ContactButton>();
			string address = orderedList[i].Key;

			currentContactButton.UpdateContactDetails(address, contactsManager.Contacts[address]);

			if (address == selectedContactAddress)
				EnableNewContactButton(currentContactButton);
		}
	}
}
