using System.Collections.Generic;

public sealed class ContactsManager
{
	public string SelectedName { get; private set; }

	public string SelectedAddress { get; private set; }

	public Dictionary<string, string> Contacts { get; } = new Dictionary<string, string>();

	public ContactsManager()
	{
		SetContacts();
	}

	public void SetSelectedContact(string name, string address)
	{
		SelectedName = name;
		SelectedAddress = address;
	}

	private void SetContacts()
	{
		if (!SecurePlayerPrefs.HasKey("Contacts") || SecurePlayerPrefs.GetInt("Contacts") == 0)
			return;

		for (int i = 1; i <= SecurePlayerPrefs.GetInt("Contacts"); i++)
		{
			string address = SecurePlayerPrefs.GetString("contact_" + i);
			Contacts.Add(address, SecurePlayerPrefs.GetString(address));
		}
	}
}
