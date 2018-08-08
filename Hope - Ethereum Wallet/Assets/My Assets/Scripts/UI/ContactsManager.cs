
public sealed class ContactsManager
{

	public SecurePlayerPrefList<ContactInfoJson> ContactList { get; } = new SecurePlayerPrefList<ContactInfoJson>("Contacts");

	/// <summary>
	/// Adds contact under the newly created wallet name and address
	/// </summary>
	/// <param name="userWalletManager"> The active UserWalletManager </param>
	/// <param name="userWalletInfoManager"> The active UserWalletInfoManager </param>
	public ContactsManager(UserWalletManager userWalletManager, UserWalletInfoManager userWalletInfoManager)
	{
		UserWallet.OnWalletLoadSuccessful += () =>
		{
			string walletAddress = userWalletManager.WalletAddress.ToLower();
			AddContact(walletAddress, userWalletInfoManager.GetWalletInfo(walletAddress).WalletName);
		};
	}

	/// <summary>
	/// Adds a contact to the SecurePlayerPrefList
	/// </summary>
	/// <param name="contactAddress"> The address being added </param>
	/// <param name="contactName"> The name being added </param>
	public void AddContact(string contactAddress, string contactName) => ContactList.Add(new ContactInfoJson(contactAddress.ToLower(), contactName));

	/// <summary>
	/// Removes a contact from the SecurePlayerPrefList
	/// </summary>
	/// <param name="contactAddress"> The address of the contact being removed </param>
	public void RemoveContact(string contactAddress) => ContactList.Remove(contactAddress.ToLower());

	/// <summary>
	/// Edits a contact from the SecurePlayerPrefList
	/// </summary>
	/// <param name="previousAddress"> The previous address that the contact was saved under </param>
	/// <param name="newContactAddress"> The new address that the contact will be saved under </param>
	/// <param name="newContactName"> The new contact name </param>
	public void EditContact(string previousAddress, string newContactAddress, string newContactName)
	{
		ContactList[previousAddress] = new ContactInfoJson(newContactAddress.ToLower(), newContactName);
	}

}
