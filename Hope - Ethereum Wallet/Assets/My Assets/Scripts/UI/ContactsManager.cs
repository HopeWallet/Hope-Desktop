
public sealed class ContactsManager
{

	public SecurePlayerPrefList<ContactInfoJson> ContactList { get; } = new SecurePlayerPrefList<ContactInfoJson>("Contacts");

	public void AddContact(string contactAddress, string contactName) => ContactList.Add(new ContactInfoJson(contactAddress, contactName));

	public void RemoveContact(string contactAddress) => ContactList.Remove(contactAddress);

	public void EditContact(string previousAddress, string newContactAddress, string newContactName) => ContactList[previousAddress] = new ContactInfoJson(newContactAddress, newContactName);

}
