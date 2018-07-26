using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddOrEditContactPopup : ExitablePopupComponent<AddOrEditContactPopup>
{

	private string previousName, previousAddress;
	public Button addContactButton, confirmButton;
	public TMP_InputField nameInputField, addressInputField;
	public TextMeshProUGUI title;

	private AddOrEditContactPopupAnimator addOrEditContactPopupAnimator;

	private Dictionary<string, string> contacts;

	protected override void OnStart()
	{
		addContactButton.onClick.AddListener(AddContactClicked);
		confirmButton.onClick.AddListener(ConfirmClicked);
	}

	public void SetDictionary(Dictionary<string, string> contacts) => this.contacts = contacts;

	private void AddContactClicked()
	{
		contacts.Add(addressInputField.text, nameInputField.text);

		SecurePlayerPrefs.SetString(ContactsPopup.PREF_NAME + contacts.Count, addressInputField.text);
		SecurePlayerPrefs.SetString(addressInputField.text, nameInputField.text);
	}

	private void ConfirmClicked()
	{
		contacts[previousAddress] = nameInputField.text;
		contacts[previousAddress].Replace(previousAddress, addressInputField.text);

		if (SecurePlayerPrefs.HasKey(addressInputField.text))
			SecurePlayerPrefs.SetString(addressInputField.text, nameInputField.text);
		else
		{
			for (int i = 0; i < contacts.Count; i++)
			{
				string prefName = ContactsPopup.PREF_NAME + contacts.Count;

				if (SecurePlayerPrefs.GetString(prefName) == previousAddress)
				{
					SecurePlayerPrefs.DeleteKey(previousAddress);

					SecurePlayerPrefs.SetString(prefName, addressInputField.text);
					SecurePlayerPrefs.SetString(addressInputField.text, nameInputField.text);
				}
			}
		}
	}
	
	/// <summary>
	/// Sets all the necessary popup text elements
	/// </summary>
	/// <param name="addingContact"> Checks if adding a contact or editing an existing one </param>
	/// <param name="name"> The current name of the contact </param>
	/// <param name="address"> The current address of the contact </param>
	public void SetPopupLayout(bool addingContact, string name = "", string address = "")
	{
		addOrEditContactPopupAnimator = transform.GetComponent<AddOrEditContactPopupAnimator>();
		addOrEditContactPopupAnimator.addingContact = addingContact;

		if (!addingContact)
		{
			previousName = name;
			previousAddress = address;
		}

		title.GetComponent<TextMeshProUGUI>().text = addingContact ? "A D D  C O N T A C T" : "E D I T  C O N T A C T";
		nameInputField.text = name;
		addressInputField.text = address;
	}
}
