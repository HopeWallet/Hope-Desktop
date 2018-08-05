
public sealed class ContactInfo
{

	public ContactsPopup ContactsPopup { get; }

	public string ContactName { get; }

	public string ContactAddress { get; }

	
	public ContactInfo(ContactsPopup contactsPopup, string contactName, string contactAddress)
	{
		ContactsPopup = contactsPopup;
		ContactName = contactName;
		ContactAddress = contactAddress;
	}
}