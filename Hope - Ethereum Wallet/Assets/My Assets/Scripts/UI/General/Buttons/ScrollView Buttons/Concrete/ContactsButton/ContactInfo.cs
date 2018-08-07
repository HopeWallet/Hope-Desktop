
public sealed class ContactInfo
{

	public ContactsPopup ContactsPopup { get; }

	public string ContactAddress { get; }

	public string ContactName { get; }

	
	public ContactInfo(ContactsPopup contactsPopup, string contactAddress, string contactName)
	{
		ContactsPopup = contactsPopup;
		ContactAddress = contactAddress;
		ContactName = contactName;
	}
}