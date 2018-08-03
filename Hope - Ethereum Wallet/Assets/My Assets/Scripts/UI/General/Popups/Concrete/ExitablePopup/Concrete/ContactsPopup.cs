using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;

public class ContactsPopup : ExitablePopupComponent<ContactsPopup>
{
	public Button addContactButton, confirmButton;
	public Transform contactsListTransform;

	private ContactsPopupAnimator contactsPopupAnimator;
	private ContactButton.Factory contactButtonFactory;
	private ContactsManager contactsManager;
	private ContactButton activeContactButton;

	[Inject]
	public void Construct(ContactButton.Factory contactButtonFactory, ContactsManager contactsManager)
	{
		this.contactButtonFactory = contactButtonFactory;
		this.contactsManager = contactsManager;
	}

	protected override void Awake()
	{
		base.Awake();

		contactsPopupAnimator = transform.GetComponent<ContactsPopupAnimator>();

		addContactButton.onClick.AddListener(AddContact);
		confirmButton.onClick.AddListener(ConfirmButtonClicked);

		if (!SecurePlayerPrefs.HasKey("Contacts"))
			return;

		for (int i = 1; i <= SecurePlayerPrefs.GetInt("Contacts"); i++)
		{
			var button = contactButtonFactory.Create();
			var address = SecurePlayerPrefs.GetString("contact_" + i);
			button.SetButtonInfo(new ContactInfo(this, SecurePlayerPrefs.GetString(address), address));

			Transform buttonParent = button.transform.parent;
			buttonParent.parent = contactsListTransform;
			buttonParent.localScale = new Vector3(0f, 1f, 1f);
		}
	}

	private void ConfirmButtonClicked()
	{
		contactsManager.SetSelectedContact(activeContactButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text,
										   activeContactButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text);

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

	private void AddContact()
	{
		popupManager.GetPopup<AddOrEditContactPopup>(true).SetPopupLayout(true);
		popupManager.GetPopup<AddOrEditContactPopup>(true).SetContactsPopup(this);
	}
}
