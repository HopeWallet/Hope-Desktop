using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;

public sealed class ContactsPopup : ExitablePopupComponent<ContactsPopup>
{
	public Button addContactButton, confirmButton;
	public Transform contactsListTransform;

	private SendAssetPopup sendAssetPopup;
	private ContactsPopupAnimator contactsPopupAnimator;
	private ContactButton.Factory contactButtonFactory;
	private ContactsManager contactsManager;
	private ContactButton activeContactButton;

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

		if (!SecurePlayerPrefs.HasKey("Contacts") || SecurePlayerPrefs.GetInt("Contacts") == 0)                     //<< PUT THIS STUFF IN A METHOD: AddContactButtons
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

	/// <summary>
	/// Sets the active contact button if the inputted address is from a saved contact
	/// </summary>
	protected override void OnStart()
	{
		if (!string.IsNullOrEmpty(sendAssetPopup.Address.contactName.text))
		{
			string inputtedAddress = sendAssetPopup.Address.addressField.text;
			Transform contactsTransform = contactsPopupAnimator.contactsTransform;

			for (int i = 0; i < contactsTransform.childCount; i++)
			{
				ContactButton contactButton = contactsTransform.GetChild(i).GetChild(0).GetComponent<ContactButton>();

				if (contactButton.RealContactAddress == inputtedAddress)
					EnableNewContactButton(contactButton);
			}
		}
	}

	/// <summary>
	/// Gets The currently selected contact details and closes the popup
	/// </summary>
	private void ConfirmButtonClicked()
	{
		sendAssetPopup.Address.addressField.text = activeContactButton.RealContactAddress;
		popupManager.CloseActivePopup();
	}

	/// <summary>
	/// Manages the interactables of the newly clicked and old contact button that was pressed
	/// </summary>
	/// <param name="contactButton"> The ContactButton that was just clicked </param>
	public void EnableNewContactButton(ContactButton contactButton)
	{
		if (activeContactButton != null)
			activeContactButton.Button.interactable = true;

		contactButton.Button.interactable = false;
		activeContactButton = contactButton;
		confirmButton.interactable = true;
	}

	/// <summary>
	/// Opens up the AddOrEditContactPopup
	/// </summary>
	private void AddContact()
	{
		popupManager.GetPopup<AddOrEditContactPopup>(true).SetPopupLayout(true);
		popupManager.GetPopup<AddOrEditContactPopup>(true).SetContactsPopup(this);
	}
}
