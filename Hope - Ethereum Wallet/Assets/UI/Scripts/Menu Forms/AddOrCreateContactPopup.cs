using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddOrCreateContactPopup : ExitablePopupComponent<AddOrCreateContactPopup>
{

	private string previousName, previousAddress;
	public Button addContactButton, confirmButton;
	public TMP_InputField nameInputField, addressInputField;

	private AddOrEditContactPopupAnimator addOrEditContactPopupAnimator;

	private Dictionary<string, string> contacts;

	protected override void OnStart()
	{
		addOrEditContactPopupAnimator = transform.GetComponent<AddOrEditContactPopupAnimator>();

		addContactButton.onClick.AddListener(AddContactClicked);
		confirmButton.onClick.AddListener(ConfirmClicked);
	}

	public void SetDictionary(Dictionary<string, string> contacts) => this.contacts = contacts;

	private void AddContactClicked()
	{
		contacts.Add(nameInputField.text, addressInputField.text);

		SecurePlayerPrefs.SetString(ContactsPopup.PREF_NAME + contacts.Count, nameInputField.text);
		SecurePlayerPrefs.SetString(nameInputField.text, addressInputField.text);
	}

	private void ConfirmClicked()
	{
		contacts[previousName] = addressInputField.text;
		contacts[previousName].Replace(previousName, nameInputField.text);

		if (SecurePlayerPrefs.HasKey(nameInputField.text))
			SecurePlayerPrefs.SetString(nameInputField.text, addressInputField.text);
		else
		{
			for (int i = 0; i < contacts.Count; i++)
			{
				string prefName = ContactsPopup.PREF_NAME + contacts.Count;

				if (SecurePlayerPrefs.GetString(prefName) == previousName)
				{
					SecurePlayerPrefs.DeleteKey(previousName);

					SecurePlayerPrefs.SetString(prefName, nameInputField.text);
					SecurePlayerPrefs.SetString(nameInputField.text, addressInputField.text);
				}
			}
		}
	}

	public void SetPopupLayout(bool addingContact, string name = "", string address = "")
	{
		if (!addingContact)
		{
			previousName = name;
			previousAddress = address;
		}

		addOrEditContactPopupAnimator.SetAddingContact(addingContact, name, address);
	}
}
