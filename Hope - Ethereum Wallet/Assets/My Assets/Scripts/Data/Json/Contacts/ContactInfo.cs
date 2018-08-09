using Newtonsoft.Json;
using System;

/// <summary>
/// Class which contains info about a saved contact.
/// </summary>
[Serializable]
public sealed class ContactInfo
{
    /// <summary>
    /// The currently active ContactsPopup.
    /// </summary>
	public ContactsPopup ContactsPopup { get; }

    /// <summary>
    /// The address of the contact.
    /// </summary>
    [JsonProperty]
	public string ContactAddress { get; set; }

    /// <summary>
    /// The name of the contact.
    /// </summary>
    [JsonProperty]
    public string ContactName { get; set; }

    /// <summary>
    /// The contructor to use when contructing the object from serialized text.
    /// </summary>
    /// <param name="contactAddress"> The address of the contact. </param>
    /// <param name="contactName"> The name of the contact. </param>
    [JsonConstructor]
    public ContactInfo(string contactAddress, string contactName)
    {
        ContactAddress = contactAddress;
        ContactName = contactName;
    }

    /// <summary>
    /// The contact info
    /// </summary>
    /// <param name="contactsPopup"> The active ContactsPopup </param>
    /// <param name="contactAddress"> The address of the contact. </param>
    /// <param name="contactName"> The name of the contact. </param>
    public ContactInfo(ContactsPopup contactsPopup, string contactAddress, string contactName) : this(contactAddress, contactName)
	{
		ContactsPopup = contactsPopup;
	}
}