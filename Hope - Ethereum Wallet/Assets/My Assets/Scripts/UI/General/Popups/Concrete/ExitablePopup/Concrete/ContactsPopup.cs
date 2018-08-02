using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ContactsPopup : ExitablePopupComponent<ContactsPopup>
{
	public Button addContactButton, confirmButton;
	public Transform contactsListTransform;

	private ContactsPopupAnimator contactsPopupAnimator;
	private ContactButton.Factory contactButtonFactory;

	private readonly Dictionary<string, string> contacts = new Dictionary<string, string>();

	private ContactButton activeContactButton;

	public const string PREF_NAME = "contact_";

	[Inject]
	public void Construct(ContactButton.Factory contactButtonFactory)
	{
		this.contactButtonFactory = contactButtonFactory;
	}

	protected override void Awake()
	{
		base.Awake();

		PopulateContacts();

		contactsPopupAnimator = transform.GetComponent<ContactsPopupAnimator>();

		addContactButton.onClick.AddListener(AddContact);
		confirmButton.onClick.AddListener(ConfirmButtonClicked);

		if (!SecurePlayerPrefs.HasKey("Contacts"))
			return;

		UnityEngine.Debug.Log(SecurePlayerPrefs.GetInt("Contacts"));
		for (int i = 0; i < SecurePlayerPrefs.GetInt("Contacts"); i++)
		{
			var button = contactButtonFactory.Create();
			var address = SecurePlayerPrefs.GetString(PREF_NAME + i);
			button.SetButtonInfo(new ContactInfo(this, SecurePlayerPrefs.GetString(address), address));
			button.transform.parent.parent = contactsListTransform;
		}
	}

	private void ConfirmButtonClicked()
	{


		popupManager.CloseActivePopup();
	}

	public void EnableNewContactButton(ContactButton contactButton)
	{
		if (activeContactButton != null)
			activeContactButton.Button.interactable = true;

		contactButton.Button.interactable = false;
		activeContactButton = contactButton;
		confirmButton.interactable = true;
	}

	private void PopulateContacts()
	{
		for (int i = 0; ; i++)
		{
			if (!SecurePlayerPrefs.HasKey(PREF_NAME + i))
				return;

			string address = SecurePlayerPrefs.GetString(PREF_NAME + i);
			contacts.Add(address, SecurePlayerPrefs.GetString(address));
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
}
