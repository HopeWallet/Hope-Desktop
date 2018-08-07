using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Linq;
using System.Collections.Generic;
using System;

public sealed class ContactsPopup : ExitablePopupComponent<ContactsPopup>
{
	public Button addContactButton, confirmButton;
	public Transform contactsListTransform;
	public TMP_Dropdown sortByDropdown;
	public TMP_InputField searchField;

	private SendAssetPopup sendAssetPopup;
	private ContactsPopupAnimator contactsPopupAnimator;
	private ContactButton.Factory contactButtonFactory;
	private ContactsManager contactsManager;

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
		searchField.onValueChanged.AddListener(SearchBarChanged);

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
	/// Deactivates the contact objects that do not contain information compatible with what is being searched for
	/// </summary>
	/// <param name="search"></param>
	private void SearchBarChanged(string search)
	{
		search = search.ToLower();

		var buttons = contactsListTransform.GetChildrenTransformList().Select(b => b.GetChild(0).GetComponent<ContactButton>()).ToList();
		buttons.Where(button => !SearchName(button, search) || !SearchAddress(button, search)).ForEach(button => SetButtonObjectActive(button, false));
		buttons.Where(button => SearchName(button, search) || SearchAddress(button, search)).ForEach(button => SetButtonObjectActive(button, true));
	}

	/// <summary>
	/// Returns a boolean that checks if the name containes the searched text
	/// </summary>
	/// <param name="contactButton"> The ContactButton being checked </param>
	/// <param name="search"> The inputted search text </param>
	/// <returns></returns>
	private bool SearchName(ContactButton contactButton, string search) => contactButton.ButtonInfo.ContactName.ToLower().Contains(search);

	/// <summary>
	/// Returns a boolean that checks if the address starts with the searched text
	/// </summary>
	/// <param name="contactButton"> The ContactButton being checked </param>
	/// <param name="search"> The inputted search text </param>
	/// <returns></returns>
	private bool SearchAddress(ContactButton contactButton, string search) => contactButton.ButtonInfo.ContactAddress.ToLower().StartsWith(search);

	/// <summary>
	/// Sets the contactButton's parent object to active or not
	/// </summary>
	/// <param name="contactButton"> The ContactButton being checked </param>
	/// <param name="isActive"> Checks if setting active or not </param>
	private void SetButtonObjectActive(ContactButton contactButton, bool isActive) => contactButton.transform.parent.gameObject.SetActive(isActive);

	/// <summary>
	/// List order has been changed
	/// </summary>
	/// <param name="value"> The value of the sorting type in the dropdown </param>
	private void ListOrderChanged(int value)
	{
		if (value == 0)
			ChangeListOrder((b1, b2) => contactsManager.ContactOrders[b1.ButtonInfo.ContactAddress].CompareTo(contactsManager.ContactOrders[b2.ButtonInfo.ContactAddress]));

		else if (value == 1)
			ChangeListOrder((b1, b2) => contactsManager.ContactOrders[b2.ButtonInfo.ContactAddress].CompareTo(contactsManager.ContactOrders[b1.ButtonInfo.ContactAddress]));

		else
			ChangeListOrder((b1, b2) => contactsManager.Contacts[b1.ButtonInfo.ContactAddress].CompareTo(contactsManager.Contacts[b2.ButtonInfo.ContactAddress]));
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

	/// <summary>
	/// Changes the contact list order based on a comparison.
	/// </summary>
	/// <param name="comparison"> The comparison to use to sort the button list. </param>
	private void ChangeListOrder(Comparison<ContactButton> comparison)
	{
		var buttons = contactsListTransform.GetChildrenTransformList().Select(b => b.GetChild(0).GetComponent<ContactButton>()).ToList();
		buttons.Sort(comparison);
		buttons.Select(b => b.transform.parent).ForEach(b => b.SetAsLastSibling());
	}
}
