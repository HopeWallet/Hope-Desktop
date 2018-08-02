using System.Collections.Generic;

public sealed class ContactsManager
{

	public Dictionary<string, string> contacts = new Dictionary<string, string>();

	public string SelectedName { get; private set; }
	public string SelectedAddress { get; private set; }

	public void SetSelectedContact(string name, string address)
	{
		SelectedName = name;
		SelectedAddress = address;
	}
}
