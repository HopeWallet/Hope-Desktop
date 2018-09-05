using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public sealed class AddOrEditContactPopup : ExitablePopupComponent<AddOrEditContactPopup>, IEnterButtonObservable, ITabButtonObservable
{
	private Action popupClosed;

	[SerializeField] private Button addContactButton, confirmButton;
	[SerializeField] private HopeInputField nameInputField, addressInputField;
	[SerializeField] private TextMeshProUGUI title;

    private readonly List<Selectable> inputFields = new List<Selectable>();

	private string previousName, previousAddress;
	private ContactButton contactButton;

	private AddOrEditContactPopupAnimator addOrEditContactPopupAnimator;
	private ContactButton.Factory contactButtonFactory;
	private ContactsManager contactsManager;
    private ButtonClickObserver buttonClickObserver;

	private ContactsPopup contactsPopup;

	/// <summary>
	/// Adds the button listeners
	/// </summary>
	protected override void OnStart()
	{
		addContactButton.onClick.AddListener(AddContactClicked);
		confirmButton.onClick.AddListener(ConfirmClicked);

        inputFields.Add(nameInputField.InputFieldBase);
        inputFields.Add(addressInputField.InputFieldBase);
	}

    private void OnEnable()
    {
        buttonClickObserver.SubscribeObservable(this);
    }

    private void OnDisable()
    {
        buttonClickObserver.UnsubscribeObservable(this);
		popupClosed?.Invoke();
    }

    /// <summary>
    /// Adds the required dependencies to the AddOrEditContatPopup
    /// </summary>
    /// <param name="contactButtonFactory"> The active ContactButtonFactory</param>
    /// <param name="contactsManager"> The active ContactsManager</param>
    /// <param name="buttonClickObserver"> The active ButtonClickObserver. </param>
    [Inject]
	public void Construct(ContactButton.Factory contactButtonFactory, ContactsManager contactsManager, ButtonClickObserver buttonClickObserver)
	{
		this.contactButtonFactory = contactButtonFactory;
		this.contactsManager = contactsManager;
        this.buttonClickObserver = buttonClickObserver;
	}

	/// <summary>
	/// Sets this ContactsPopup to the original instance and initializes te closing action
	/// </summary>
	/// <param name="contactsPopup"> The ContactsPopup </param>
	/// <param name="popupClosed"> Action to run when this popup has closed </param>
	public void SetContactsPopup(ContactsPopup contactsPopup, Action popupClosed = null)
	{
		this.contactsPopup = contactsPopup;
		this.popupClosed = popupClosed;
	}

	/// <summary>
	/// Adds a new contact according to the given inputs
	/// </summary>
	private void AddContactClicked()
	{
		contactsManager.AddContact(addressInputField.Text, nameInputField.Text);
		CreateNewContactObjectInList();

		popupManager.CloseActivePopup();
	}

	/// <summary>
	/// Creates a new ContactButton in the current contact list
	/// </summary>
	private void CreateNewContactObjectInList()
	{
		var button = contactButtonFactory.Create();

		button.SetButtonInfo(new ContactInfo(contactsPopup, addressInputField.Text, nameInputField.Text));

		Transform contactObject = button.transform.parent;
		contactObject.parent = contactsPopup.contactsListTransform;
		contactObject.localScale = new Vector3(0f, 1f, 1f);
		contactObject.gameObject.AnimateScaleX(1f, 0.1f);
	}

	/// <summary>
	/// Edits the existing contact and replaces it with the new values
	/// </summary>
	private void ConfirmClicked()
	{
		string newName = nameInputField.Text;
		string newAddress = addressInputField.Text;

		contactsManager.EditContact(previousAddress, newAddress, newName);

		contactButton.UpdateContactDetails(newAddress, newName);

		popupManager.CloseActivePopup();
	}

    /// <summary>
    /// Sets all the necessary popup text elements
    /// </summary>
    /// <param name="addingContact"> Checks if adding a contact or editing an existing one </param>
    /// <param name="name"> The current name of the contact </param>
    /// <param name="address"> The current address of the contact </param>
    /// <param name="contactButton"> The ContactButton being edited. </param>
    public void SetPopupLayout(bool addingContact, string name = "", string address = "", ContactButton contactButton = null)
	{
		addOrEditContactPopupAnimator = transform.GetComponent<AddOrEditContactPopupAnimator>();
		addOrEditContactPopupAnimator.AddingContact = addingContact;
		addOrEditContactPopupAnimator.PreviousAddress = address;
		addOrEditContactPopupAnimator.contactsManager = contactsManager;

		title.GetComponent<TextMeshProUGUI>().text = addingContact ? "Add Contact" : "Edit Contact";

        confirmButton.gameObject.SetActive(!addingContact);
        addContactButton.gameObject.SetActive(addingContact);

		if (addingContact)
            return;

        this.contactButton = contactButton;

        nameInputField.Text = name;
		addressInputField.Text = address;
		previousName = name;
		previousAddress = address;
	}

    public void TabButtonPressed(ClickType clickType)
    {
        if (clickType != ClickType.Down)
            return;

        inputFields.MoveToNextSelectable();
    }

    public void EnterButtonPressed(ClickType clickType)
    {
        if (clickType != ClickType.Down)
            return;

        Button button = confirmButton.gameObject.activeInHierarchy ? confirmButton : addContactButton;

        if (InputFieldUtils.GetActiveInputField() == inputFields[1] && button.interactable)
            button.Press();
        else
            inputFields.MoveToNextSelectable();
    }
}
