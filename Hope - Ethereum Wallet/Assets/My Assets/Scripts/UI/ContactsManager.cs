using System;

public sealed class ContactsManager
{
    /// <summary>
    /// The list of contacts
    /// </summary>
    public SecurePlayerPrefList<ContactInfo> ContactList { get; }

    /// <summary>
    /// Adds contact under the newly created wallet name and address
    /// </summary>
    /// <param name="userWalletManager"> The active UserWalletManager </param>
    /// <param name="userWalletInfoManager"> The active UserWalletInfoManager </param>
    /// <param name="settings"> The settings for the ContactsManager. </param>
    /// <param name="networkSettings"> The settings for the EthereumNetworkManager. </param>
    public ContactsManager(
        UserWalletManager userWalletManager,
        HopeWalletInfoManager userWalletInfoManager,
        Settings settings,
        EthereumNetworkManager.Settings networkSettings)
    {
        ContactList = new SecurePlayerPrefList<ContactInfo>(settings.contactsPrefName, (int)networkSettings.networkType);
        UserWalletManager.OnWalletLoadSuccessful += () =>
        {
			var walletAddress = userWalletManager.GetWalletAddress().ToLower();

			if (!ContactList.Contains(walletAddress))
			{
				var info = userWalletInfoManager.GetWalletInfo(walletAddress);

				if (!string.IsNullOrEmpty(info?.WalletName))
					AddContact(walletAddress, info.WalletName);
			}
        };
    }

    /// <summary>
    /// Adds a contact to the SecurePlayerPrefList
    /// </summary>
    /// <param name="contactAddress"> The address being added </param>
    /// <param name="contactName"> The name being added </param>
    public void AddContact(string contactAddress, string contactName) => ContactList.Add(new ContactInfo(contactAddress.ToLower(), contactName));

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
        ContactList[previousAddress] = new ContactInfo(newContactAddress.ToLower(), newContactName);
    }

    /// <summary>
    /// Class which contains the pref settings for the ContactManager.
    /// </summary>
    [Serializable]
    public sealed class Settings
    {
        [RandomizeText] public string contactsPrefName;
    }
}
