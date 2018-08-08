
public sealed class ContactInfo
{

	public ContactsPopup ContactsPopup { get; }

	public string ContactAddress { get; }

	public string ContactName { get; }

	/// <summary>
	/// The contact info
	/// </summary>
	/// <param name="contactsPopup"> The active ContactsPopup </param>
	/// <param name="contactAddress"> The contactAddress string </param>
	/// <param name="contactName"> The contact name string </param>
	public ContactInfo(ContactsPopup contactsPopup, string contactAddress, string contactName)
	{
		ContactsPopup = contactsPopup;
		ContactAddress = contactAddress;
		ContactName = contactName;
	}
}