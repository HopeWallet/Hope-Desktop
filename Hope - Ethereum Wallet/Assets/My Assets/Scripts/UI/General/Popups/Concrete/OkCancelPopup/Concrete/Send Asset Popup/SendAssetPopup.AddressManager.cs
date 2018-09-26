using Hope.Utils.Ethereum;
using TMPro;

/// <summary>
/// Class which displays the popup for sending a TradableAsset.
/// </summary>
public sealed partial class SendAssetPopup : OkCancelPopupComponent<SendAssetPopup>
{
	/// <summary>
	/// Class which manages the send address of the <see cref="SendAssetPopup"/>.
	/// </summary>
	public sealed class AddressManager
	{
		public readonly HopeInputField addressField;
		public readonly TMP_Text contactName;

		private ContactsManager contactsManager;
		private RestrictedAddressManager restrictedAddressManager;

		/// <summary>
		/// Whether the address input field is empty or not.
		/// </summary>
		public bool IsEmpty { get { return string.IsNullOrEmpty(addressField.Text); } }

		/// <summary>
		/// The address the asset should be sent to.
		/// </summary>
		public string SendAddress { get { return addressField.Text; } }

		/// <summary>
		/// Initializes the <see cref="AddressManager"/> by assigning the send address input field.
		/// </summary>
		/// <param name="addressField"> The input field for the address. </param>
		/// <param name="contactName"> The contactName text component. </param>
		/// <param name="contactsManager"> The active ContactsManager. </param>
		public AddressManager(
			HopeInputField addressField,
			TMP_Text contactName,
			ContactsManager contactsManager,
			RestrictedAddressManager restrictedAddressManager)
		{
			this.addressField = addressField;
			this.contactName = contactName;
			this.contactsManager = contactsManager;
			this.restrictedAddressManager = restrictedAddressManager;

			addressField.OnInputUpdated += _ => CheckAddress();
		}

		/// <summary>
		/// Checks if the address is valid once the text is changed.
		/// </summary>
		private void CheckAddress()
		{
			string address = addressField.Text.ToLower();

			bool invalidEthereumAddress = !AddressUtils.IsValidEthereumAddress(address);
			bool isRestrictedAddress = restrictedAddressManager.IsRestrictedAddress(address);

			addressField.Error = invalidEthereumAddress || isRestrictedAddress;

			if (invalidEthereumAddress)
				addressField.errorMessage.text = "Invalid address";
			else if (isRestrictedAddress)
				addressField.errorMessage.text = "Restricted address";

			if (!addressField.Error)
				CheckIfSavedContact(address);
			else
				contactName.text = string.Empty;
		}

		/// <summary>
		/// Checks if the inputted address is from a saved contact.
		/// </summary>
		/// <param name="address"> The address in the input field. </param>
		public void CheckIfSavedContact(string address)
		{
			address = address.ToLower();
			contactName.text = contactsManager.ContactList.Contains(address) ? "<style=Contact>" + contactsManager.ContactList[address].ContactName + "</style>" : string.Empty;
			contactName.gameObject.AnimateGraphic(contactName.text == string.Empty ? 0f : 1f, 0.15f);
		}
	}
}